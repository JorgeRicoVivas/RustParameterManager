using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SLightParameterManager {
    internal class ClientConnection {
        private const string verylong_number_pattern = "{0:000000000000000000000000000}";
        public static ClientConnection Instance = new ClientConnection();
        public MainForm MainForm { get; set; }
        public Queue<string> JsonsToSend { get; }

        private static bool ejemplo = true;
        private string Hostname = "127.0.0.1";
        private int Port = 7878;
        private string messagesAcumulation = "";
        private ulong transactionID = 0;
        private ClientConnection() {
            JsonsToSend = new Queue<string>();
        }

        private long client_id = long.MaxValue;

        internal void changeConnection(string hostname, int port) {
            client = null;
            Hostname = hostname;
            Port = port;
        }

        private bool resetConnection() {
            messagesAcumulation = "";
            try {
                client = new TcpClient(Hostname, Port);
                byte[] connectionText = Encoding.UTF8.GetBytes("");
                client.GetStream().Write(connectionText, 0, connectionText.Length);
                client.Client.ReceiveTimeout = 5000;
                client.Client.SendTimeout = 1000;
                connectionState = ConnectionState.JustConnected;
                return true;
            } catch(Exception) {
                return false;
            }
        }

        public TcpClient client;

        private ConnectionState connectionState = ConnectionState.Disconnected;

        enum ConnectionState {
            Disconnected, JustConnected, Connected, JustDisconnected
        }


        public void continuousRead() {
            while(!MainForm.Created) { }
            if(ejemplo) {
                ejemplo = false;
            }
            while(true) {
                if(client == null || !client.Connected) {
                    MainForm.removeAllJSON();
                    MainForm.Invoke((new Action(() => {
                        MainForm.connectionLabel.Text = "Connection Status: Trying to connect with " + Hostname + ":" + Port;
                        MainForm.connectionLabel.BackColor = System.Drawing.Color.Red;
                    })));

                    if(resetConnection()) {
                        //Console.WriteLine("Reconected");
                        MainForm.Invoke((new Action(() => {
                            MainForm.connectionLabel.Text = "Connection Status: Connected to " + Hostname + ":" + Port;
                            MainForm.connectionLabel.BackColor = System.Drawing.Color.Green;
                        })));
                    } else {
                        //Console.WriteLine("Could not connect");
                    }
                } else {

                    try {
                        //sendInternalMessage("<TCP_MESSAGE><\\/TCP_MESSAGE>");
                        if(connectionState == ConnectionState.Connected) {
                            while(client.Available > 0) {
                                if(!readMessage()) {
                                    continue;
                                }

                                List<string> messages = extractMessages();
                                messages.ForEach(onReceiveMessage);
                            }
                        }
                        updateConnection();
                        //Console.WriteLine("There is connection");
                    } catch(SocketException e) {
                        //Console.WriteLine("Socket connection error: " + e.SocketErrorCode);
                    } catch(IOException e) {
                        //Console.WriteLine("There is no connection: " + e.Message);
                        updateDisconnection();
                        resetConnection();
                    } catch(System.NullReferenceException) {
                        //Console.WriteLine("Connection manually resetted");
                    }
                }
                Thread.Sleep(100);
            }


        }

        private static Regex tcpMessageRegex = new Regex("<TCP_MESSAGE>(?<message>.*?)<\\/TCP_MESSAGE>", RegexOptions.Singleline);

        private List<string> extractMessages() {
            return tcpMessageRegex.Matches(messagesAcumulation).Cast<Match>().ToList().AsEnumerable().Reverse().Select(match => {
                messagesAcumulation = messagesAcumulation.Substring(0, match.Index) + messagesAcumulation.Substring(match.Index + match.Length);
                return match.Groups["message"].Value;
            }).Reverse().ToList();
        }

        private bool readMessage() {
            int available = client.Available;
            byte[] bytes = new byte[available];
            client.GetStream().Read(bytes, 0, bytes.Length);
            messagesAcumulation += Encoding.UTF8.GetString(bytes);
            return available > 0;
        }

        class update_message {
            public long id { get; set; }
            public string newValue { get; set; }
        }

        public void sendJSON(long id, JToken json) {
            var res_message = new update_message() { id = id, newValue = JsonConvert.SerializeObject(json, Formatting.Indented) };
            string serializedData = JsonConvert.SerializeObject(res_message, Formatting.Indented);
            string res_string = "<TCP_MESSAGE>" + serializedData + "</TCP_MESSAGE>";
            sendInternalMessage(res_string);

            if(MainForm.ConnectionData.IsSet) {
                using(var client = new WebClient()) {
                    client.Credentials = new NetworkCredential(MainForm.ConnectionData.User, MainForm.ConnectionData.Password);
                    string objectIdStr = String.Format(verylong_number_pattern, id);
                    string clientIdStr = String.Format(verylong_number_pattern, client_id);
                    string transactionStr = String.Format(verylong_number_pattern, transactionID++);
                    string filepath = MainForm.ConnectionData.Address + "/object-" + objectIdStr + "-client-" + clientIdStr + "-transaction-" + transactionStr; ;
                    try {
                        Console.WriteLine(client.UploadString("ftp://" + filepath, WebRequestMethods.Ftp.UploadFile, serializedData));
                    } catch(System.Net.WebException ex) {
                        List<string> dirTree = filepath.Split(new string[] { "/" }, StringSplitOptions.None).ToList();
                        dirTree.RemoveAt(dirTree.Count - 1);
                        int startFrom = 1;
                        for(int i = dirTree.Count - 1; i >= 0; i--) {
                            string pathToDir = String.Join("/", dirTree.GetRange(0, i + 1));
                            if(actionOnFTPFile(pathToDir, MainForm.ConnectionData.User, MainForm.ConnectionData.Password, WebRequestMethods.Ftp.MakeDirectory)) {
                                startFrom = i + 1;
                                break;
                            }
                        }
                        for(int i = startFrom; i < dirTree.Count; i++) {
                            string pathToDir = String.Join("/", dirTree.GetRange(0, i + 1));
                            actionOnFTPFile(pathToDir, MainForm.ConnectionData.User, MainForm.ConnectionData.Password, WebRequestMethods.Ftp.MakeDirectory);
                        }
                        for(int i = 5; i > 0; i--) {
                            try {
                                client.UploadString("ftp://" + filepath, WebRequestMethods.Ftp.UploadFile, serializedData);
                            } catch(System.Net.WebException) { }
                        }
                    }
                }
            }

        }

        private bool actionOnFTPFile(string path, string user, string password, string method) {
            FtpWebRequest ftpReq = WebRequest.Create("ftp://" + path) as FtpWebRequest;
            //WebRequestMethods.Ftp.DeleteFile
            ftpReq.Method = method;
            ftpReq.KeepAlive = false;
            ftpReq.Credentials = new NetworkCredential(user, password);
            try {
                FtpWebResponse ftpResp = ftpReq.GetResponse() as FtpWebResponse;
                return ftpResp.StatusCode == FtpStatusCode.FileActionOK;
            } catch(Exception) { return false; }
        }

        private void sendInternalMessage(string message) {
            try {
                byte[] sendBytes = Encoding.UTF8.GetBytes(message);
                client.GetStream().Write(sendBytes, 0, sendBytes.Length);
                string trimed = message.Trim();
                if(trimed.Length > 0) {
                    Console.WriteLine("Sent message: " + message);
                }
            } catch(Exception) {
                client = null;
                client_id = long.MaxValue;
            }

        }

        private void onReceiveMessage(string message) {
            Console.WriteLine("Received message: {0}", message);
            var deserialized = JObject.Parse(message);
            Dictionary<string, object> headerAndBody = JSONManagement.dictionaryOfValues(deserialized);
            Dictionary<string, object> body = JSONManagement.dictionaryOfValues(JToken.Parse((string)headerAndBody["body"]));
            switch(headerAndBody["header"]) {
                case "Notify":
                    long id = (long)body["Notify.id"];
                    string name = (string)body["Notify.name"];
                    string jsonValue = (string)body["Notify.value_in_json"];
                    MainForm.introduceJSONLink(id, name, jsonValue);
                    break;
                case "Remove":
                    id = (long)body["Remove.id"];
                    MainForm.removeJSONLink(id);
                    break;
                case "RemoveAll":
                    MainForm.removeAllJSON();
                    break;
                case "GiveClientId":
                    client_id = (long)body["GiveClientId.client_id"];
                    break;

            }
        }

        private void updateConnection() {
            switch(connectionState) {
                case ConnectionState.Disconnected:
                case ConnectionState.JustDisconnected:
                    connectionState = ConnectionState.JustConnected;
                    break;
                case ConnectionState.JustConnected:
                case ConnectionState.Connected:
                    connectionState = ConnectionState.Connected;
                    break;
            }
        }

        private void updateDisconnection() {
            switch(connectionState) {
                case ConnectionState.Disconnected:
                case ConnectionState.JustDisconnected:
                    connectionState = ConnectionState.Disconnected;
                    break;
                case ConnectionState.JustConnected:
                case ConnectionState.Connected:
                    connectionState = ConnectionState.JustConnected;
                    break;
            }
        }
    }
}
