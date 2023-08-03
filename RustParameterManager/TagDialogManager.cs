using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SLightParameterManager {
    public class TagDialogManager {
        public SortedDictionary<string, Group> groupsOfTags { get; set; } = new SortedDictionary<string, Group>() {
            { "C", new Group(){Name= "C", Values=new SortedSet<string>(){"Item C","Item c" }, Regexes=new List<Regex>{ new Regex("C"), new Regex("c") } } },
            { "A", new Group(){Name= "A", Values=new SortedSet<string>(){"Item A","Item a" }, Regexes=new List<Regex>{ new Regex("A"), new Regex("a") } } },
            { "B", new Group(){Name= "B", Values=new SortedSet<string>(){"Item B","Item b" }, Regexes=new List<Regex>{ new Regex("B"), new Regex("b") } } },
            { "Null", new Group(){Name= "Null" } },
            { "Unsorted", new Group(){Name= "Unsorted", Values=new SortedSet<string>(){"Item c","Item a","Item B" }, Regexes=new List<Regex>{ new Regex("D"), new Regex("d") } } },
        };
        public MainForm MainForm { get; set; }

        public ContextMenu contextMenu = new ContextMenu();

        public void prepareContextMenu() {
            contextMenu.MenuItems.Clear();
            groupsOfTags.ToList().Where(keyAndVal => keyAndVal.Value.Values.Count > 0).ToList()
                .ForEach(keyAndVal => {
                    var listName = keyAndVal.Key;
                    var listValues = keyAndVal.Value.Values;
                    contextMenu.MenuItems.Add("Convertir a lista: " + listName, (object sender, EventArgs e) => {

                        MainForm.Invoke(new Action(() => {
                            TabPage selectedTab = MainForm.jsonDataTab.SelectedTab;
                            JsonForm jsonForm = ((JsonForm)selectedTab.Controls[0]);
                            TableLayoutPanel jsonTable = jsonForm.jsonTable;
                            var controlClicked = ((ContextMenu)((MenuItem)sender).Parent).SourceControl;
                            if (controlClicked is RichTextBox richTextBox) {
                                var pos = jsonTable.GetPositionFromControl(controlClicked);
                                jsonTable.Controls.Remove(controlClicked);
                                DomainUpDown valuesUpDown = createDropDown(listValues, controlClicked.Tag);
                                valuesUpDown.GotFocus += (focSender, focEv) => JsonForm.focusedTextBox = valuesUpDown;
                                jsonTable.Controls.Add(valuesUpDown, pos.Column, pos.Row);
                            } else {
                                var upDown = (DomainUpDown)controlClicked.Parent;
                                upDown.Items.Clear();
                                upDown.Items.AddRange(listValues);
                                upDown.Size = new Size(listValues.Select(value => value.Length).Max() * 10, upDown.Size.Height);
                            }
                        }));
                    });

                });

            /*
                Control valueControl = decideControl(found_object, (sender, ev) => {
                    try_send_update();
                });
                valueControl.Tag = path_and_object.Item1;
             */
        }

        private DomainUpDown createDropDown(SortedSet<string> listValues, object jtoken_path) {
            DomainUpDown valuesUpDown = new DomainUpDown() { Tag = jtoken_path, ContextMenu = contextMenu };
            valuesUpDown.Items.AddRange(listValues);
            valuesUpDown.TextChanged += (object selSender, EventArgs selE) => {
                var realSelSender = (DomainUpDown)selSender;
                var selJsonForm = ((JsonForm)MainForm.jsonDataTab.SelectedTab.Controls[0]);
                ((JValue)selJsonForm.jtoken.SelectToken((string)valuesUpDown.Tag)).Value = realSelSender.Text;
                selJsonForm.try_send_update();
            };
            valuesUpDown.Size = new Size(listValues.Select(value => value.Length).Max() * 10, valuesUpDown.Size.Height);
            return valuesUpDown;
        }

        public void ensureVisibilityOfBestMatch(string fieldName, string value) {
            try {
                KeyValuePair<string, Group> bestGroup = groupsOfTags
                    .OrderBy(group => group.Value.Regexes.Any(regex => regex.IsMatch(fieldName)))
                    .Where(group => group.Value.Values.Contains(value))
                    .First();
                MainForm.Invoke(new Action(() => {
                    TreeNode fieldNode = MainForm.tagTreeView.Nodes.Find(bestGroup.Key, false)[0];
                    TreeNode tagNode = fieldNode.Nodes.Find(value, false)[0];
                    MainForm.tagTreeView.SelectedNode = tagNode;
                }));
            } catch(InvalidOperationException) { }
        }

        public void generateTagTreeView() {
            EventHandler validateCreateGroupDialog;
            CreateGroup dialog = new CreateGroup();
            string usingGroup = null;


            validateCreateGroupDialog = new EventHandler(delegate (object s, EventArgs ev) {
                bool isValid = !groupsOfTags.ContainsKey(dialog.nameTextBox.Text) || (usingGroup != null && usingGroup.Equals(dialog.nameTextBox.Text));
                dialog.button1.Enabled = isValid;
                dialog.nameTextBox.BackColor = isValid ? SystemColors.Window : Color.Red;
            });
            dialog.nameTextBox.TextChanged += validateCreateGroupDialog;
            ContextMenu tagMenu = new ContextMenu();
            tagMenu.MenuItems.Add("Create new group", (sender, e) => {
                dialog.Text = "Creating new tag group";
                dialog.nameTextBox.Text = "";
                dialog.valuesTextBox.Text = "";
                dialog.regexesTextBox.Text = "";
                showManageGroupDialog();
            });

            MainForm.tagTreeView.Nodes.Clear();
            groupsOfTags.ToList().ForEach(tag => {
                ContextMenu groupMenu = new ContextMenu();
                TreeNode rootNode = new TreeNode(tag.Key) { ContextMenu = groupMenu, Name = tag.Key };
                var res = new MenuItem() { Text = "Modify" };
                res.Click += (sender, e) => {
                    dialog.Text = "Modifying tag group: " + rootNode.Text;
                    dialog.nameTextBox.Text = rootNode.Text;
                    dialog.valuesTextBox.Text = String.Join("\n", groupsOfTags[rootNode.Text].Values);
                    dialog.regexesTextBox.Text = String.Join("\n", groupsOfTags[rootNode.Text].Regexes);
                    usingGroup = rootNode.Text;
                    showManageGroupDialog();
                };
                groupMenu.MenuItems.Add(res);
                groupMenu.MenuItems.Add("Remove", (sender, e) => {
                    groupsOfTags.Remove(rootNode.Text);
                    generateTagTreeView();
                });

                rootNode.Nodes.AddRange(tag.Value.Values.Select(value => {
                    TreeNode tagNode = new TreeNode(value) { Name = value };
                    return tagNode;
                }).ToArray());
                MainForm.tagTreeView.Nodes.Add(rootNode);
            });
            MainForm.tagTreeView.ContextMenu = tagMenu;

            void showManageGroupDialog() {
                validateCreateGroupDialog.Invoke(null, null);
                if(dialog.ShowDialog().Equals(DialogResult.OK)) {
                    if(usingGroup != null) {
                        groupsOfTags.Remove(usingGroup);
                        usingGroup = null;
                    }
                    string groupName = dialog.nameTextBox.Text;
                    if(!groupsOfTags.ContainsKey(groupName)) {
                        groupsOfTags.Add(groupName, new Group());
                    }
                    groupsOfTags[groupName].Values = new SortedSet<string>(trimAndGetListOfStringIn(dialog.valuesTextBox.Text));
                    groupsOfTags[groupName].Regexes = trimAndGetListOfStringIn(dialog.regexesTextBox.Text)
                        .Where(nstring => { try { new Regex(nstring); return true; } catch(Exception) { return false; } })
                        .Select(nstring => new Regex(nstring)).ToList();
                    Console.WriteLine("Accepted");
                    generateTagTreeView();
                } else {
                    Console.WriteLine("Denied");
                }
            }
        }

        private static string[] trimAndGetListOfStringIn(string text) {
            return text.Split(new string[] { "\n" }, StringSplitOptions.None).Select(nstring => nstring.Trim()).Where(nstring => nstring.Length > 0).ToArray();
        }
    }


}
