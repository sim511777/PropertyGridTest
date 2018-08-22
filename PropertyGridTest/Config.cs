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
using System.Reflection;

// todo:
// 1. CategoryAttribute : 카테고리 구분
// 2. DisplayNameAttribute : 프로퍼티 이름
// 3. DescriptionAttribute : 프로퍼티 설명
// 4. BrowsableAttribute : 해당 속성을 감출 수 있다.
// 5. string 변환을 바꾸려면 : TypeConverter 를 상속받아 새로운 컨버터 클래스 생성
// 6. 다른 에디터를 사용 하려면 : UITypeEditor 를 상속받아 새로운 에디터 클래스 작성
// 7. 사용자 정의 클래스 확장 속성 : [TypeConverter(typeof(ExpandableObjectConverter))]
// 8. nullable 타입은 생성자에서 null 아닌 값으로 초기화 할 것
// 9. Enum 타입 표시 문자 바꾸기
// - bool 타입 체크박스로
// - 문자열 리스트 선택기 (확장하여 오브젝트 리스트 선택기)
// - Flags Enum 체크박스로 바꿈


namespace PropertyGridTest {
   class Config {
      [BRW(false)] public int hidden { get; set; }

      [CAT("1. Basic")] [DSP("bool")] [DSC("Input bool value")] public bool boolVal { get; set; }
      [CAT("1. Basic")] [DSP("int")] [DSC("Input int value")] public int intVal { get; set; }
      [CAT("1. Basic")] [DSP("float")] [DSC("Input float value")] public float floatVal { get; set; }
      [CAT("1. Basic")] [DSP("string")] [DSC("Input string value")] public string stringVal { get; set; } = string.Empty;
      [CAT("1. Basic")] [DSP("Font")] [DSC("Input Font value")] public Font FontVal { get; set; } = SystemFonts.DefaultFont;

      [CAT("2. Struct")] [DSP("Color")] [DSC("Input Color value")] public Color ColorVal { get; set; }
      [CAT("2. Struct")] [DSP("DateTime")] [DSC("Input DateTime value")] public DateTime DateTimeVal { get; set; }
      [CAT("2. Struct")] [DSP("Point")] [DSC("Input Point value")] public Point PointVal { get; set; }

      [CAT("3. Enum")] [DSP("FormBorderStyle")] [DSC("Input FormBorderStyle value")] public FormBorderStyle fbstyle { get; set; }
      [CAT("3. Enum")] [DSP("Weekday Flag")] [DSC("Input Weekday value")] public Weekday weekday { get; set; }

      [CAT("4. Array")] [DSP("int[]")] [DSC("Input int[] value")] public int[] intArrayVal { get; set; } = new int[0];

      [CAT("5. Custom Class")] [DSP("Person")] [DSC("Input Person value")] public Person Personval { get; set; } = new Person();
      [CAT("5. Custom Class")] [DSP("People")] [DSC("Input Person value")] public Person[] PersonArrayVal { get; set; } = new Person[0];

      [TypeConverter(typeof(PointConverter))]
      [CAT("6. Custom Type Converter")] [DSP("Point")] [DSC("Point x,y displayed by '/")] public Point point2 { get; set; }
      [TypeConverter(typeof(StringSelector))]
      [CAT("6. Custom Type Converter")] [DSP("FileName")] [DSC("Text Select")] public string selText { get; set; }
      [TypeConverter(typeof(YesNoConverter))]
      [CAT("6. Custom Type Converter")] [DSP("YesNo")] [DSC("True/False -> Yes/No")] public bool boolVal2 { get; set; }
      [TypeConverter(typeof(EnumDisplayNameConverter))]
      [CAT("6. Custom Type Converter")] [DSP("방향")] [DSC("동서남북")] public Direction dirVal { get; set; }


      [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
      [CAT("7. UITypeEditor")] [DSP("File")] [DSC("Select File")] public string fileName { get; set; } = string.Empty;
      [Editor(typeof(TextInputEditor), typeof(UITypeEditor))]
      [CAT("7. UITypeEditor")] [DSP("String")] [DSC("Input Text")] public string stringVal2 { get; set; } = string.Empty;


   }

   [Flags]
   enum Weekday {
      Sunday = 1,
      Monday = 2,
      Tuesday = 4,
      Wednesday = 8,
      Thursday = 16,
      Friday = 32,
      Saturday = 64,
   }

   enum Direction {
      [DSC("동쪽")]    East,
      [DSC("서쪽")]    West,
      [DSC("남쪽")]    South,
      [DSC("북극 쪽")] North,
   }

   // TypeConverter
   [TypeConverter(typeof(ExpandableObjectConverter))]
   class Person {
      [DSP("이름")] public string name { get; set; } = string.Empty;
      [DSP("나이")] public int age { get; set; }
      [DSP("성별남자")] public bool sex { get; set; }
      public override string ToString() {
         return $"{name}, {age}, {(sex ? "Male" : "Female")}";
      }
   }

   public class StringSelector : StringConverter {
      public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
         return true;
      }
      public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
         return new StandardValuesCollection(new string[] { "새 파일", "파일1", "문서1" });
      }
      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
         return false;
      }
   }

   class YesNoConverter : BooleanConverter {
      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
         if (value is bool && destinationType == typeof(string)) {
            return (bool)value ? "Yes" : "No";
         }
         return base.ConvertTo(context, culture, value, destinationType);
      }

      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
         if (value is string) {
            return (string)value == "Yes";
         }
         return base.ConvertFrom(context, culture, value);
      }
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

   public class EnumDisplayNameConverter : EnumConverter {
      public EnumDisplayNameConverter(Type type) : base(type) {}

      // description => enum value
      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
         string dsc = value as string;
         var field = this.EnumType.GetFields().First(fi => this.GetDescription(fi) == dsc);
         return field.GetValue(null);
      }

      // enum value to description
      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
         var field = this.EnumType.GetField(value.ToString());
         return this.GetDescription(field);
      }

      // field to description
      private string GetDescription(FieldInfo field) {
         var descAttr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
         return descAttr?.Description;
      }
   }

   // UITypeEditor
   class TextInputEditor : UITypeEditor {
      public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
         return UITypeEditorEditStyle.Modal;
      }
      public override bool IsDropDownResizable { get { return true; } }
      public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value) {
         FormTextInput form = new FormTextInput();
         form.tbxText.Text = value as string;

         IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
         if (svc.ShowDialog(form) == DialogResult.OK) {
            value = form.tbxText.Text;
         }

         return value;
      }
   }

}
