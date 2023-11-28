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
            var config = new Config();
            config.PropertyChanged += (s, e) => {
                if (e.PropertyName == "Age") {
                    Text = $"Age changed to {config.Age}";
                } else if (e.PropertyName == "Weight") {
                    Text = $"Weight changed to {config.Weight}";
                    propertyGrid1.Refresh();
                }
            };
            propertyGrid1.SelectedObject = config;
        }
    }
}
