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
// ok - BrowsableAttribute : 해당 속성을 감출 수 있다.
// ok - CategoryAttribute : 카테고리 구분
// ok - DisplayNameAttribute : 프로퍼티 이름
// ok - DescriptionAttribute : 프로퍼티 설명
// ok - nullable 타입은 생성자에서 null 아닌 값으로 초기화 할 것
// ok - 사용자 정의 클래스 확장 속성 : [TypeConverter(typeof(ExpandableObjectConverter))]
// ok - string 변환을 바꾸려면 : TypeConverter 를 상속받아 새로운 컨버터 클래스 생성
// ok - 다른 에디터를 사용 하려면 : UITypeEditor 를 상속받아 새로운 에디터 클래스 작성
// ok - Enum 타입 표시 문자 바꾸기
// ok - 문자열 리스트 선택기
// ok - Enum 문자열 리스트 선택기
// ok - int 문자열 리스트 선택기
// - bool 타입 체크박스로
// - Flags Enum 체크박스로 바꿈


namespace PropertyGridTest {
   class Config {
      [BRW(false)] public int hidden { get; set; }

      const string cat1 = "1. Basic";
      [CAT(cat1), DSP("bool")] [DSC("Input bool value")] public bool boolVal { get; set; }
      [CAT(cat1), DSP("int")] [DSC("Input int value")] public int intVal { get; set; }
      [CAT(cat1), DSP("float")] [DSC("Input float value")] public float floatVal { get; set; }
      [CAT(cat1), DSP("string")] [DSC("Input string value")] public string stringVal { get; set; } = string.Empty;
      [CAT(cat1), DSP("Color")] [DSC("Input Color value")] public Color ColorVal { get; set; }
      [CAT(cat1), DSP("DateTime")] [DSC("Input DateTime value")] public DateTime DateTimeVal { get; set; }
      [CAT(cat1), DSP("Point")] [DSC("Input Point value")] public Point PointVal { get; set; }
      [CAT(cat1), DSP("Font")] [DSC("Input Font value")] public Font FontVal { get; set; } = SystemFonts.DefaultFont;

      const string cat2 = "2. Enum";
      [CAT(cat2), DSP("FormBorderStyle")] [DSC("Select FormBorderStyle value")] public FormBorderStyle fbstyle { get; set; }
      [CAT(cat2), DSP("Weekday")] [DSC("Custom Enum")] public Weekday weekday { get; set; }

      const string cat3 = "3. Array";
      [CAT("cat3"), DSP("int[]")] [DSC("Int array")] public int[] intArrayVal { get; set; } = new int[0];

      const string cat4 = "4. Custom Class";
      [CAT(cat4), DSP("Person")] [DSC("Custom class")] public Person Personval { get; set; } = new Person();
      [CAT(cat4), DSP("People")] [DSC("Custom class array")] public Person[] PersonArrayVal { get; set; } = new Person[0];

      const string cat5 = "5. Custom Type Converter";
      [TypeConverter(typeof(PointConverter))]
      [CAT(cat5), DSP("Point")] [DSC("Point x,y displayed by '/")] public Point point2 { get; set; }
      [TypeConverter(typeof(YesNoConverter))]
      [CAT(cat5), DSP("bool")] [DSC("True/False -> Yes/No")] public bool boolVal2 { get; set; }
      [TypeConverter(typeof(EnumDisplayNameConverter))]
      [CAT(cat5), DSP("Direction")] [DSC("Enum Text Converter")] public Direction dirVal { get; set; }
      [TypeConverter(typeof(StringSelectConverter))] [StringList("Please|Select|Just|One", "|")]
      [CAT(cat5), DSP("string")] [DSC("You can select 1 string")] public string selText { get; set; } = "choose one";
      [TypeConverter(typeof(Int32SelectConverter))] [StringList("Please|Select|Just|One", "|")]
      [CAT(cat5), DSP("int")] [DSC("You can select 1 Int32")] public int selInt { get; set; } = 0;
      [TypeConverter(typeof(EnumSelectConverter))] [StringList("없음|고정싱글|고정3D|고정대화상자|리사이즈|고정도구창", "|")]
      [CAT(cat5), DSP("FormBorderStyle")] [DSC("Select FormBorderStyle value")] public FormBorderStyle fbstyle2 { get; set; }


      const string cat6 = "6. UITypeEditor";
      [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
      [CAT(cat6), DSP("File")] [DSC("Select File")] public string fileName { get; set; } = string.Empty;
      [Editor(typeof(TextInputEditor), typeof(UITypeEditor))]
      [CAT(cat6), DSP("String")] [DSC("Input Text")] public string stringVal2 { get; set; } = string.Empty;
   }

   enum Weekday {
      Sunday,
      Monday,
      Tuesday,
      Wednesday,
      Thursday,
      Friday,
      Saturday,
   }

   enum Direction {
      [DSC("동쪽")] East,
      [DSC("서쪽")] West,
      [DSC("남쪽")] South,
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

   // 문자열 선택기
   public class StringSelectConverter : StringConverter {
      public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
         var strListAttr = context.PropertyDescriptor.Attributes.Cast<Attribute>().ToList().Find(attr => attr is StringListAttribute) as StringListAttribute;
         return (strListAttr != null);
      }

      public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
         var strListAttr = context.PropertyDescriptor.Attributes.Cast<Attribute>().ToList().Find(attr => attr is StringListAttribute) as StringListAttribute;
         return new StandardValuesCollection(strListAttr.Items);
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
         return false;
      }
   }

   // 정수 선택기
   public class Int32SelectConverter : Int32Converter {
      public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
         var strListAttr = context.PropertyDescriptor.Attributes.Cast<Attribute>().ToList().Find(attr => attr is StringListAttribute) as StringListAttribute;
         return (strListAttr != null);
      }

      public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
         var strListAttr = context.PropertyDescriptor.Attributes.Cast<Attribute>().ToList().Find(attr => attr is StringListAttribute) as StringListAttribute;
         return new StandardValuesCollection(strListAttr.Items);
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
         return true;
      }

      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
         if (value is string) {
            var strListAttr = context.PropertyDescriptor.Attributes.Cast<Attribute>().ToList().Find(attr => attr is StringListAttribute) as StringListAttribute;
            string[] items = strListAttr.Items;
            int idx = items.ToList().IndexOf(value as string);
            if (idx != -1)
               return idx;
         }
         return base.ConvertFrom(context, culture, value);
      }

      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
         if (value is int && destinationType == typeof(string)) {
            var strListAttr = context.PropertyDescriptor.Attributes.Cast<Attribute>().ToList().Find(attr => attr is StringListAttribute) as StringListAttribute;
            string[] items = strListAttr.Items;
            int idx = (int)value;
            if (idx >= 0 && idx < items.Length)
               return items[idx];
         }
         return base.ConvertTo(context, culture, value, destinationType);
      }
   }

   // Enum 선택기
   public class EnumSelectConverter : EnumConverter {
      public EnumSelectConverter(Type type) : base(type) { }

      public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
         var strListAttr = context.PropertyDescriptor.Attributes.Cast<Attribute>().ToList().Find(attr => attr is StringListAttribute) as StringListAttribute;
         return (strListAttr != null);
      }

      private string[] FixNames(string[] replaceNames) {
         var enumNames = Enum.GetNames(base.EnumType);
         // 넘으면 자르고 모자르면 원래이름으로
         string[] strings = new string[enumNames.Length];

         for (int i=0; i<enumNames.Length; i++) {
            if (i < replaceNames.Length)
               strings[i] = replaceNames[i];
            else
               strings[i] = enumNames[i];
         }
         return strings;
      }

      public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
         var strListAttr = context.PropertyDescriptor.Attributes.Cast<Attribute>().ToList().Find(attr => attr is StringListAttribute) as StringListAttribute;
         var strings = FixNames(strListAttr.Items);
         return new StandardValuesCollection(strings);
      }

      public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
         return true;
      }

      // string to enumvalue
      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
         if (value is string) {
            var strListAttr = context.PropertyDescriptor.Attributes.Cast<Attribute>().ToList().Find(attr => attr is StringListAttribute) as StringListAttribute;
            var strings = FixNames(strListAttr.Items);
            int idx = strings.ToList().IndexOf(value as string);
            if (idx != -1)
               return Enum.GetValues(base.EnumType).GetValue(idx);
         }
         return base.ConvertFrom(context, culture, value);
      }

      // enum value to sring
      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
         if (context == null)
            return null;
         
         if (destinationType == typeof(string)) {
            var strListAttr = context.PropertyDescriptor.Attributes.Cast<Attribute>().ToList().Find(attr => attr is StringListAttribute) as StringListAttribute;
            var strings = FixNames(strListAttr.Items);
            var enumValues = Enum.GetValues(base.EnumType);

            if (value is string) {                          // 표시 문자열을 enum 값 문자열로
               int idx = Array.IndexOf(strings, value);
               if (idx >= 0 && idx < enumValues.Length)
                  return enumValues.GetValue(idx).ToString();
               else
                  throw new Exception();
            } else if (value.GetType() == base.EnumType) {  // enum 값을 표시 문자열로
               int idx = Array.IndexOf(enumValues, value);
               if (idx >= 0 && idx < strings.Length)
                  return strings[idx];
            }
         }
         return base.ConvertTo(context, culture, value, destinationType);
      }
   }

   [AttributeUsage(AttributeTargets.Property)]
   public class StringListAttribute : Attribute {
      private string[] items;
      public StringListAttribute(string stringsSeperatedByNewLine, string seperator) {
         this.items = stringsSeperatedByNewLine.Split(new string[] { seperator }, StringSplitOptions.None);
      }
      public string[] Items {
         get { return this.items; }
      }
   }

   // bool 타입변환기
   class YesNoConverter : BooleanConverter {
      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
         if (value is string) {
            return (string)value == "Yes";
         }
         return base.ConvertFrom(context, culture, value);
      }

      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
         if (value is bool && destinationType == typeof(string)) {
            return (bool)value ? "Yes" : "No";
         }
         return base.ConvertTo(context, culture, value, destinationType);
      }
   }

   // 포인트 타입변환기
   public class PointConverter : TypeConverter {
      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
         if (sourceType == typeof(string)) {
            return true;
         }
         return base.CanConvertFrom(context, sourceType);
      }

      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
         if (value is string) {
            string[] v = ((string)value).Split(new char[] { '/' });
            return new Point(int.Parse(v[0]), int.Parse(v[1]));
         }
         return base.ConvertFrom(context, culture, value);
      }

      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
         if (destinationType == typeof(string)) {
            return ((Point)value).X + "/" + ((Point)value).Y;
         }
         return base.ConvertTo(context, culture, value, destinationType);
      }
   }

   // enum 타입변환기
   public class EnumDisplayNameConverter : EnumConverter {
      public EnumDisplayNameConverter(Type type) : base(type) { }

      // description => enum value
      public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
         if (value is string) {
            string dsc = value as string;
            var field = this.EnumType.GetFields().First(fi => this.GetDescription(fi) == dsc);
            return field.GetValue(null);
         }
         return base.ConvertFrom(context, culture, value);
      }

      // enum value to description
      public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
         if (destinationType == typeof(string)) {
            var field = this.EnumType.GetField(value.ToString());
            return this.GetDescription(field);
         }
         return base.ConvertTo(context, culture, value, destinationType);
      }

      // field to description
      private string GetDescription(FieldInfo field) {
         var descAttr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
         return descAttr?.Description;
      }
   }

   // 대화상자 타입에디터
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
