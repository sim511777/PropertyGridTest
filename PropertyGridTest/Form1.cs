using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PropertyGridTest {
   public partial class Form1 : Form {
      public Form1() {
         InitializeComponent();
         this.propertyGrid1.SelectedObject = new Config() {
            boolVal     = true,
            intVal      = 1,
            floatVal    = 1.2f,
            stringVal   = "hello",
            ColorVal    = Color.Red,
            DateTimeVal = DateTime.Now,
            PointVal    = new Point(100, 100),
            fbstyle     = FormBorderStyle.Fixed3D | FormBorderStyle.FixedDialog,
            weekday     = Weekday.Sunday | Weekday.Monday,
         };
      }
   }
}
