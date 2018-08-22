using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAT = System.ComponentModel.CategoryAttribute;
using DSP = System.ComponentModel.DisplayNameAttribute;
using DSC = System.ComponentModel.DescriptionAttribute;
using BRW = System.ComponentModel.BrowsableAttribute;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

// todo:
// 1. CategoryAttribute : 카테고리 구분
// 2. DisplayNameAttribute : 프로퍼티 이름
// 3. DescriptionAttribute : 프로퍼티 설명
// 4. string 변환을 바꾸려면 : TypeConverter 를 상속받아 새로운 컨버터 클래스 생성
// 5. 다른 에디터를 사용 하려면 : UITypeEditor 를 상속받아 새로운 에디터 클래스 작성
// - bool 타입 체크박스로
// - Enum 타입 표시 문자 바꾸기
// - Flags Enum 체크박스로 바꿈
// - 문자열 리스트 선택기 (확장하여 오브젝트 리스트 선택기)

namespace PropertyGridTest {
   class Config {
      [BRW(false)]                                                                                                 public int             hidden      { get; set; }
      
      [CAT("1. Basic")]                        [DSP("bool")]               [DSC("Input bool value")]               public bool            boolVal     { get; set; }
      [CAT("1. Basic")]                        [DSP("int")]                [DSC("Input int value")]                public int             intVal      { get; set; }
      [CAT("1. Basic")]                        [DSP("float")]              [DSC("Input float value")]              public float           floatVal    { get; set; }
      [CAT("1. Basic")]                        [DSP("string")]             [DSC("Input string value")]             public string          stringVal   { get; set; }
      
      [CAT("2. Struct")]                       [DSP("Color")]              [DSC("Input Color value")]              public Color           ColorVal    { get; set; }
      [CAT("2. Struct")]                       [DSP("DateTime")]           [DSC("Input DateTime value")]           public DateTime        DateTimeVal { get; set; }
      [CAT("2. Struct")]                       [DSP("Point")]              [DSC("Input Point value")]              public Point           PointVal    { get; set; }
      
      [CAT("3. Enum")]                         [DSP("FormBorderStyle")]    [DSC("Input FormBorderStyle value")]    public FormBorderStyle fbstyle     { get; set; }
      [CAT("3. Enum")]                         [DSP("Weekday Flag")]       [DSC("Input Weekday value")]            public Weekday         weekday     { get; set; }

      [CAT("4. Array")]                        [DSP("int[]")]              [DSC("Input int[] value")]              public int[]           intArrayVal { get; set; }
      
      [TypeConverter(typeof(PointConverter))]
      [CAT("5. Custom Type Converter")]        [DSP("Point")]              [DSC("Point x,y displayed by '/")]      public Point           point2      { get; set; }

      [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
      [CAT("6. UITypeEditor")] [DSP("File")]               [DSC("Select File")]                    public string          fileName    { get; set; }
      [Editor(typeof(TextInputEditor), typeof(UITypeEditor))]
      [CAT("6. UITypeEditor")] [DSP("String")]               [DSC("Input Text")]                    public string          stringVal2    { get; set; }
   }

   [Flags]
   enum Weekday {
      Sunday    = 1,
      Monday    = 2,
      Tuesday   = 4,
      Wednesday = 8,
      Thursday  = 16,
      Friday    = 32,
      Saturday  = 64,
   }

   public class PointConverter : TypeConverter {
      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
         if (sourceType == typeof(string)) {
            return true;
         }
         return base.CanConvertFrom(context, sourceType);
      }

      public override object ConvertFrom(ITypeDescriptorContext context,
         CultureInfo culture, object value) {
         if (value is string) {
            string[] v = ((string)value).Split(new char[] { '/' });
            return new Point(int.Parse(v[0]), int.Parse(v[1]));
         }
         return base.ConvertFrom(context, culture, value);
      }

      public override object ConvertTo(ITypeDescriptorContext context,
         CultureInfo culture, object value, Type destinationType) {
         if (destinationType == typeof(string)) {
            return ((Point)value).X + "/" + ((Point)value).Y;
         }
         return base.ConvertTo(context, culture, value, destinationType);
      }
   }

    class TextInputEditor : UITypeEditor {
      public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
         return UITypeEditorEditStyle.Modal;
      }
      public override bool IsDropDownResizable { get { return true; } }
      public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value) {
         FormTextInput form = new FormTextInput();
         form.tbxText.Text = value as string;

         IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
         if(svc.ShowDialog(form) == DialogResult.OK) {
            value = form.tbxText.Text;
         }

         return value;
      }
   }

}
