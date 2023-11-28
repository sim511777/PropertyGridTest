using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Reflection;


namespace PropertyGridTest {
    // 문자열 선택기
    public class StringSelectConverter : StringConverter {
        private static StringListAttribute GetStrListAttr(ITypeDescriptorContext context) {
            var attributes = context.PropertyDescriptor.Attributes;
            return attributes.OfType<StringListAttribute>().FirstOrDefault();
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            var strListAttr = GetStrListAttr(context);
            return (strListAttr != null);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            var strListAttr = GetStrListAttr(context);
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

            for (int i = 0; i < enumNames.Length; i++) {
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
        private readonly string[] items;
        public StringListAttribute(params string[] items) {
            this.items = items;
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

    class EnumFlagsEditor : UITypeEditor {
        public EnumFlagsEditor() { }

        // our editor is a Drop-down editor
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            // if value is not an enum than we can not edit it
            if (!(value is Enum))
                throw new Exception("Value doesn't support");

            // try to figure out that is this a Flags enum or not ?
            Type enumType = value.GetType();
            object[] attributes = enumType.GetCustomAttributes(typeof(FlagsAttribute), true);
            if (attributes.Length == 0)
                throw new Exception("Editing enum hasn't got Flags attribute");
            // check the underlying type
            Type type = Enum.GetUnderlyingType(value.GetType());
            if (type != typeof(byte) && type != typeof(sbyte) &&
                  type != typeof(short) && type != typeof(ushort) &&
                 type != typeof(int) && type != typeof(uint))
                return value;
            if (provider != null) {
                // use windows forms editor service to show drop down
                IWindowsFormsEditorService edSvc = provider.GetService(
                     typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                if (edSvc == null)
                    return value;

                var lbx = new CheckedListBox();
                lbx.CheckOnClick = true;
                ListBoxSetEnumFlagsValue(lbx, value);
                edSvc.DropDownControl(lbx);
                return ListBoxGetEnumFlagsValue(lbx);
            }
            return Convert.ChangeType(value, type);
        }

        private static void ListBoxSetEnumFlagsValue(CheckedListBox lbx, object value) {
            // 리스트 색성
            var enumType = value.GetType();
            var enumValues = enumType.GetEnumValues();
            foreach (var enumValue in enumValues) {
                int idx = lbx.Items.Add(enumValue);
                if (((int)value & (int)enumValue) != 0)
                    lbx.SetItemChecked(idx, true);
            }
        }

        private static int ListBoxGetEnumFlagsValue(CheckedListBox lbx) {
            int result = 0;
            for (int i = 0; i < lbx.Items.Count; i++) {
                var item = lbx.Items[i];
                if (lbx.GetItemChecked(i)) {
                    result |= (int)item;
                } else {
                    result &= ~(int)item;
                }
            }
            return result;
        }
    }

    class BoolCheckBoxEditor : UITypeEditor {
        public BoolCheckBoxEditor() { }

        // our editor is a Drop-down editor
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            // if value is not an enum than we can not edit it
            if (!(value is bool))
                throw new Exception("Value doesn't support");

            if (provider != null) {
                // use windows forms editor service to show drop down
                IWindowsFormsEditorService edSvc = provider.GetService(
                     typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                if (edSvc == null)
                    return value;

                var chk = new CheckBox();
                chk.Checked = (bool)value;
                edSvc.DropDownControl(chk);
                return chk.Checked;
            }
            return Convert.ChangeType(value, typeof(bool));
        }
    }

    class TrackbarEditor : UITypeEditor {
        public TrackbarEditor() { }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.DropDown;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            // if value is not an enum than we can not edit it
            if (!(value is int))
                throw new Exception("Value doesn't support");

            if (provider != null) {
                // use windows forms editor service to show drop down
                IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                if (edSvc == null)
                    return value;

                var iVal = (int)value;

                var trc = new TrackBar();
                trc.ValueChanged += (sender, e) => {
                    context.PropertyDescriptor.SetValue(context.Instance, trc.Value);
                };
                var attributes = context.PropertyDescriptor.Attributes.OfType<ParamsAttribute>();
                if (attributes.Count() >= 1) {
                    var trackbarParamsAttribute =  attributes.ElementAt(0);
                    trc.Minimum = trackbarParamsAttribute.Min;
                    trc.Maximum = trackbarParamsAttribute.Max;
                    trc.Value = Math.Max(Math.Min(iVal, trc.Maximum), trc.Minimum);
                } else {
                    trc.Minimum = iVal - 100;
                    trc.Maximum = iVal + 100;
                    trc.Value = iVal;
                }
                edSvc.DropDownControl(trc);
                return trc.Value;
            }
            return Convert.ChangeType(value, typeof(int));
        }

        [AttributeUsage(AttributeTargets.Property)]
        internal class ParamsAttribute : Attribute {
            public int Min { get; }
            public int Max { get; }
            public ParamsAttribute(int min, int max) {
                Min = min;
                Max = max;
            }
        }
    }
}
