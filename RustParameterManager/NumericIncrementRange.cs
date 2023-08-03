using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLightParameterManager {
    public partial class NumericIncrementRange : UserControl {
        public NumericIncrementRange() {
            InitializeComponent();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            var rSender = (NumericUpDown)sender;
            rSender.DecimalPlaces = BitConverter.GetBytes(decimal.GetBits(rSender.Value)[3])[2];
        }
    }
}
