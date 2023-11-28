using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing;

namespace PropertyGridTest {
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
                var prms = context.PropertyDescriptor.Attributes.OfType<ParamsAttribute>().FirstOrDefault();
                if (prms != null) {
                    trc.Minimum = prms.Min;
                    trc.Maximum = prms.Max;
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

    class TrackbarFormEditor : UITypeEditor {
        public TrackbarFormEditor() { }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.Modal;
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
                trc.Dock = DockStyle.Fill;
                trc.ValueChanged += (sender, e) => {
                    context.PropertyDescriptor.SetValue(context.Instance, trc.Value);
                };
                var prms = context.PropertyDescriptor.Attributes.OfType<ParamsAttribute>().FirstOrDefault();
                if (prms != null) {
                    trc.Minimum = prms.Min;
                    trc.Maximum = prms.Max;
                    trc.Value = Math.Max(Math.Min(iVal, trc.Maximum), trc.Minimum);
                } else {
                    trc.Minimum = iVal - 100;
                    trc.Maximum = iVal + 100;
                    trc.Value = iVal;
                }
                var form = new Form();
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.Controls.Add(trc);
                form.StartPosition = FormStartPosition.Manual;
                form.Location = Cursor.Position;
                form.Size = new Size(400, 100);
                edSvc.ShowDialog(form);

                return trc.Value;
            }
            return Convert.ChangeType(value, typeof(int));
        }
    }

}
