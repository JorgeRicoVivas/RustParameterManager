using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SLightParameterManager {
    public partial class JsonForm : UserControl {

        public JToken jtoken;
        private bool canSendUpdate = true;
        private string accumulatedJSON = null;
        private string initialJSON;
        private long id;
        private MainForm mainForm;
        private Dictionary<string, Control> pathToControls = new Dictionary<string, Control>();


        public JsonForm(string initialJSON, long id, MainForm mainForm) {
            InitializeComponent();
            restartTableFromJObject(JToken.Parse(initialJSON));
            this.initialJSON = initialJSON;
            this.id = id;
            this.mainForm = mainForm;
        }

        public void onEnter() {
            if(accumulatedJSON != null) {
                sendUpdate(accumulatedJSON, true);
                accumulatedJSON = null;
            }
        }

        public void sendUpdate(string receivedJObject, bool forceUpdate) {
            if(!Visible && !forceUpdate) {
                accumulatedJSON = receivedJObject;
                return;
            } else {
                accumulatedJSON = null;
            }

            canSendUpdate = false;
            JToken newJObject = JToken.Parse(receivedJObject);
            string previousString = JsonConvert.SerializeObject(jtoken, Formatting.None);
            string newString = JsonConvert.SerializeObject(newJObject, Formatting.None);
            if(newString == previousString) {
                //New JSON and previous JSON are the same object
                canSendUpdate = true;
                return;
            }
            List<(string, object)> previousValues = JSONManagement.extractValues(jtoken);
            List<(string, string)> previousStructure = JSONManagement.structureFromValues(previousValues);
            List<(string, object)> newValues = JSONManagement.extractValues(newJObject);
            List<(string, string)> newStructure = JSONManagement.structureFromValues(newValues);

            bool isSameStructure = Enumerable.SequenceEqual(previousStructure, newStructure);
            Invoke(new Action(() => {
                if(!isSameStructure) {
                    restartTableFromJObject(newJObject);
                } else {
                    var prevEnumerator = previousValues.GetEnumerator();
                    var newEnumerator = newValues.GetEnumerator();
                    while(prevEnumerator.MoveNext()) {
                        newEnumerator.MoveNext();
                        object prevValue = prevEnumerator.Current.Item2;
                        object newValue = newEnumerator.Current.Item2;
                        if(prevValue != null && !prevValue.Equals(newValue)) {
                            Control control = pathToControls[newEnumerator.Current.Item1];
                            switch(newValue) {
                                case long num:
                                    ((NumericUpDown)control).Value = num;
                                    break;
                                case double num:
                                    ((NumericUpDown)control).Value = (decimal)num;
                                    break;
                                case bool boolean:
                                    ((CheckBox)control).Checked = boolean;
                                    break;
                                case string str:
                                    ((RichTextBox)control).Text = str;
                                    break;
                                default:
                                    ((Label)control).Text = newValue == null ? "Null" : "Unknown";
                                    break;
                            }
                        }
                    }
                    jtoken = newJObject;
                }
            }));

            canSendUpdate = true;
        }

        private void restartTableFromJObject(JToken jtoken) {
            this.jtoken = jtoken;
            createTableFromValues(JSONManagement.extractValues(this.jtoken));
        }

        private void createTableFromValues(List<(string, object)> values) {
            pathToControls.Clear();
            jsonTable.Visible = false;
            jsonTable.Controls.Clear();
            jsonTable.ColumnCount = 2;
            jsonTable.ColumnCount = 0;
            jsonTable.RowCount++;
            jsonTable.Controls.Add(new Label() { Text = "Keys", AutoSize = true }, 0, 0);
            jsonTable.Controls.Add(new Label() { Text = "Values", AutoSize = true }, 1, 0);

            values.ForEach(path_and_object => {
                string path = path_and_object.Item1;
                object found_object = path_and_object.Item2;
                jsonTable.RowCount++;
                jsonTable.Controls.Add(new Label() { Text = path }, 0, jsonTable.RowCount - 1);
                Control valueControl = decideControl(found_object, (sender, ev) => {
                    try_send_update();
                });
                valueControl.Tag = path_and_object.Item1;
                pathToControls.Add(path, valueControl);
                jsonTable.Controls.Add(valueControl, 1, jsonTable.RowCount - 1);
            });
            jsonTable.Visible = true;

        }

        public void try_send_update() {
            Console.WriteLine(canSendUpdate);
            if(canSendUpdate) {
                Console.WriteLine("Sending new value");
                ClientConnection.Instance.sendJSON(id, jtoken);
            } else {
                Console.WriteLine("Cannot send value");
            }
        }

        public Control decideControl(object found_object, EventHandler onChange) {
            switch(found_object) {
                case long num:
                    NumericUpDown integerNumeric = new NumericUpDown() { Minimum = decimal.MinValue, Maximum = decimal.MaxValue, Value = num };
                    integerNumeric.ValueChanged += (sender, ev) => ((JValue)jtoken.SelectToken((string)integerNumeric.Tag)).Value = (long)integerNumeric.Value;
                    integerNumeric.ValueChanged += onChange;
                    return integerNumeric;
                case double num:
                    NumericUpDown decimalNumeric = new NumericUpDown() { Minimum = decimal.MinValue, Maximum = decimal.MaxValue, DecimalPlaces = decimalPositionsOf((decimal)num), Value = (decimal)num };
                    decimalNumeric.ValueChanged += (sender, ev) => decimalNumeric.DecimalPlaces = decimalPositionsOf(decimalNumeric.Value);
                    decimalNumeric.ValueChanged += (sender, ev) => ((JValue)jtoken.SelectToken((string)decimalNumeric.Tag)).Value = (double)decimalNumeric.Value;
                    decimalNumeric.ValueChanged += onChange;
                    return decimalNumeric;
                case bool boolean:
                    CheckBox check = new CheckBox() { Checked = boolean };
                    check.CheckedChanged += (sender, ev) => ((JValue)jtoken.SelectToken((string)check.Tag)).Value = check.Checked;
                    check.CheckedChanged += onChange;
                    return check;
                case string str:
                    RichTextBox textBox = new RichTextBox() { Text = str, ContextMenu=MainForm.Instance.tagDialogManager.contextMenu };
                    textBox.GotFocus += (sender, ev) => focusedTextBox = textBox;
                    textBox.GotFocus += (sender, ev) => mainForm.tagDialogManager.ensureVisibilityOfBestMatch((string)textBox.Tag, textBox.Text);
                    textBox.GotFocus += (sender, ev) => textBox.Focus();
                    textBox.TextChanged += (sender, ev) => mainForm.tagDialogManager.ensureVisibilityOfBestMatch((string)textBox.Tag, textBox.Text);
                    textBox.LostFocus += (sender, ev) => ((JValue)jtoken.SelectToken((string)textBox.Tag)).Value = textBox.Text;
                    textBox.LostFocus += onChange;
                    return textBox;
                default:
                    return new Label() { Text = found_object == null ? "Null" : "Unknown" };
            }
        }

        public static Control focusedTextBox = null;


        public int decimalPositionsOf(decimal decimalValue) {
            string decimalAsString = decimalValue.ToString();
            int decimalsPositions = !decimalAsString.Contains(".") ? 0 : decimalAsString.Substring(decimalAsString.IndexOf(".") + 1).Length;
            return decimalsPositions;
        }

    }
}
