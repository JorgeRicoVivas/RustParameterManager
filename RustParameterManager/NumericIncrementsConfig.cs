using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLightParameterManager {
    public partial class NumericIncrementsConfig : UserControl {
        public NumericIncrementsConfig() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            rangesPanel.Controls.Add(new NumericIncrementRange());
        }

        public class IncrementRangeGroup {
            public Regex regex { get; set; } = new Regex("");
            public List<IncrementRange> ranges { get; set; } = new List<IncrementRange>();
        }
        public class IncrementRange {
            public decimal step { get; set; }
            public decimal from { get; set; }
            public decimal to { get; set; }

            public bool is_in_range(decimal value) {
                return from <= to && value >= from && value <= to;
            }

        }

        public IncrementRangeGroup data() {
            try {
                return new IncrementRangeGroup() {
                    regex = new Regex(regexTextBox.Text),
                    ranges = this.AllControls().OfClass<Control, NumericIncrementRange>().Select(range => new IncrementRange() {
                        from = range.fromUpDown.Value, to = range.toUpDown.Value, step = range.incrementUpDown.Value
                    }).ToList()
                };
            } catch(Exception) {
                return null;
            }
        }
    }
}
