using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SLightParameterManager.NumericIncrementsConfig;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace SLightParameterManager {
    public partial class MainForm : Form {

        public static MainForm Instance { get; set; }

        private const string SETTINGS_KEY = @"SOFTWARE\RustParametersSettings";
        private static RegistryKey registry = getOrCreateKey(SETTINGS_KEY);
        private string Address;
        private Dictionary<long, TabPage> idsToJsonTabs = new Dictionary<long, TabPage>();

        public TagDialogManager tagDialogManager = new TagDialogManager();

        public FTPConnectionData ConnectionData = new FTPConnectionData();

        public MainForm() {
            Instance = this;
            tagDialogManager.MainForm = this;
            ClientConnection.Instance.MainForm = this;
            InitializeComponent();
            MinimumSize = Size;
            new Thread(new ThreadStart(ClientConnection.Instance.continuousRead)).Start();

            jsonTableDifferenceSize = new Size(Size.Width - jsonDataTab.Size.Width, Size.Height - jsonDataTab.Size.Height);
            toolboxDifferenceSize = new Size(Size.Width - toolbox.Size.Width, Size.Height - toolbox.Size.Height);
            toolboxDifferenceLocation = new Point(Size.Width - toolbox.Location.X, Size.Height - toolbox.Location.Y);
            toolboxTabDifferenceSize = new Size(Size.Width - toolboxTab.Size.Width, Size.Height - toolboxTab.Size.Height);

            jsonDataTab.TabIndexChanged += (sender, ev) => JsonForm.focusedTextBox = null;

            ipTextBox.TextChanged += ipBoxCheck;
            ipTextBox.TextChanged += OnAsksNewConnection;
            ipTextBox.TextChanged += (sender, ev) => ipTextBox.Focus();
            portUpDown.ValueChanged += ipBoxCheck;
            portUpDown.ValueChanged += OnAsksNewConnection;
            portUpDown.ValueChanged += (sender, ev) => portUpDown.Focus();
            FormClosing += (sender, e) => {
                List<IncrementRangeGroup> incrementRangeGroups = this.AllControls().OfClass<Control, NumericIncrementsConfig>()
                .Select(numConfig => numConfig.data()).NotNull().ToList();
                Console.WriteLine("Main form closed");
                saveKey(registry, "groupsOfRecognizableTags", tagDialogManager.groupsOfTags);
                saveKey(registry, "preferredIP", ipTextBox.Text);
                saveKey(registry, "preferredPort", portUpDown.Value);
                saveKey(registry, "ftpConnection", ConnectionData);
                // WinForms app
                Application.Exit();
                // Console app
                Environment.Exit(1);
            };
            tagDialogManager.groupsOfTags = getOrInsertKey(registry, "groupsOfRecognizableTags", tagDialogManager.groupsOfTags);
            ipTextBox.Text = getOrInsertKey(registry, "preferredIP", "127.000.000.001");
            portUpDown.Value = getOrInsertKey(registry, "preferredPort", 7878);
            ConnectionData = getOrInsertKey(registry, "ftpConnection", ConnectionData);


            OnAsksNewConnection(null, null);
            tagDialogManager.prepareContextMenu();
            tagDialogManager.generateTagTreeView();

            ftpAddressTextbox.Text = ConnectionData.Address;
            ftpUserTextbox.Text = ConnectionData.User;
            ftpPasswordTextbox.Text = ConnectionData.Password;
            ftpCheckbox.CheckedChanged += ftpCheckbox_CheckedChanged;
            ftpUserTextbox.TextChanged += ftpConnectionDataChanged;
            ftpAddressTextbox.TextChanged += ftpConnectionDataChanged;
            ftpPasswordTextbox.TextChanged += ftpConnectionDataChanged;
            ftpCheckbox.CheckedChanged += ftpConnectionDataChanged;
            ftpCheckbox.Checked = ConnectionData.IsSet;


            toolboxTab.Controls.Remove(tabPage3);

        }

        public void introduceJSONLink(long id, string name, string jsonData) {
            if(idsToJsonTabs.ContainsKey(id)) {
                ((JsonForm)idsToJsonTabs[id].Controls[0]).sendUpdate(jsonData, false);
            } else {
                Invoke(new Action(() => {
                    JsonForm dataAsJson = new JsonForm(jsonData, id, this);
                    TabPage miTab = new TabPage() { AutoScroll = true, Text = name };
                    miTab.Controls.Add(dataAsJson);
                    miTab.Enter += (sender, e) => dataAsJson.onEnter();
                    jsonDataTab.Controls.Add(miTab);
                    idsToJsonTabs.Add(id, miTab);
                }));
            }
        }

        public void removeJSONLink(long id) {
            if(idsToJsonTabs.ContainsKey(id)) {
                Invoke(new Action(() => jsonDataTab.Controls.Remove(idsToJsonTabs[id])));
                idsToJsonTabs.Remove(id);
            }
        }

        public void removeAllJSON() {
            foreach(KeyValuePair<long, TabPage> idAndTab in idsToJsonTabs) {
                Invoke(new Action(() => jsonDataTab.Controls.Remove(idAndTab.Value)));
            }
            idsToJsonTabs.Clear();
        }


        private void ipBoxCheck(object sender, EventArgs e) {
            string ipStr = ipTextBox.Text;
            while(ipStr.Length < 15) {
                ipStr += " ";
            }
            Address = String.Join(".", ipStr.Split('.').Select(ip => ip.Trim().Length == 0 ? "n" : ip.Trim()).ToList());
            try {
                IPAddress.Parse(Address);
                ipTextBox.BackColor = SystemColors.Window;
            } catch(System.FormatException) {
                Address = null;
                ipTextBox.BackColor = Color.FromArgb(255, 184, 173);
            }
        }

        private void OnAsksNewConnection(object sender, EventArgs e) {
            if(Address != null) {
                ClientConnection.Instance.changeConnection(Address, (int)portUpDown.Value);
            }
        }

        private Size jsonTableDifferenceSize;
        private Size toolboxDifferenceSize;
        private Point toolboxDifferenceLocation;
        private Size toolboxTabDifferenceSize;

        private void windowResize(object sender, EventArgs e) {

            jsonDataTab.Size = new Size(Size.Width - jsonTableDifferenceSize.Width, Size.Height - jsonTableDifferenceSize.Height);
            toolbox.Size = new Size(toolbox.Size.Width, Size.Height - toolboxDifferenceSize.Height);
            toolbox.Location = new Point(Size.Width - toolboxDifferenceLocation.X, toolbox.Location.Y);
            toolboxTab.Size = new Size(toolboxTab.Size.Width, Size.Height - toolboxTabDifferenceSize.Height);
            tagTreeView.Size = new Size(tagTreeView.Size.Width, Size.Height - toolboxTabDifferenceSize.Height - 30);

        }

        private static RegistryKey getOrCreateKey(string key) {
            RegistryKey possibleKey = Registry.CurrentUser.OpenSubKey(key, true);
            return possibleKey != null ? possibleKey : Registry.CurrentUser.CreateSubKey(key);
        }

        private T getOrInsertKey<T>(RegistryKey registry, string key, T defaultValue) {
            try {
                return JsonConvert.DeserializeObject<T>(registry.GetValue(key).ToString());
            } catch(Exception) {
                registry.SetValue(key, JsonConvert.SerializeObject(defaultValue, Formatting.Indented));
                return defaultValue;
            }
        }

        private void saveKey<T>(RegistryKey registry, string key, T value) {
            registry.SetValue(key, JsonConvert.SerializeObject(value, Formatting.Indented));
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            if(e.Node.Level > 0 && JsonForm.focusedTextBox != null) {
                try {
                    JsonForm.focusedTextBox.Text = e.Node.Text;
                } catch(Exception) { }
            }
        }

        private void ftpCheckbox_CheckedChanged(object sender, EventArgs e) {
            ftpConnectionDataTableLayout.Enabled = ftpCheckbox.Checked;
        }

        private void ftpConnectionDataChanged(object sender, EventArgs e) {
            ConnectionData.IsSet = ftpCheckbox.Checked;
            ConnectionData.Address = ftpAddressTextbox.Text;
            ConnectionData.User = ftpUserTextbox.Text;
            ConnectionData.Password = ftpPasswordTextbox.Text;
            Console.WriteLine("Credentials are: " + JsonConvert.SerializeObject(ConnectionData, Formatting.Indented));
        }

        private void button1_Click(object sender, EventArgs e) {

            incrementsPanel.Controls.Add(new NumericIncrementsConfig());
        }
    }

}
