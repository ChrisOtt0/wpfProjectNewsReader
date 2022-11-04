using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using wpfProjectNewsReader.Tools;

namespace wpfProjectNewsReader.Model
{
    public class NntpClientSingleton : Bindable
    {
        #region Fields
        private static NntpClientSingleton? instance = null;
        private string serverName = "";
        private int serverPort;
        TcpClient? socket = null;
        Stream stream = null;
        NntpStreamReader? reader = null;
        private bool canPost = false;
        private string currentGroup = "";


        public string ServerName
        {
            get => this.serverName;
            set
            {
                this.serverName = value;
                OnPropertyChanged();
            }
        }

        public int ServerPort
        {
            get => this.serverPort;
            set
            {
                this.serverPort = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        private NntpClientSingleton()
        {
            serverPort = 119;
        }
        #endregion

        #region GetInstance
        public static NntpClientSingleton GetInstance()
        {
            if (instance == null)
                instance = new NntpClientSingleton();

            return instance;
        }
        #endregion

        public async Task<InternalResponse> OpenConnectionAsync()
        {
            try
            {
                Trace.Write("Connecting to the server... ");
                if (serverName == "") return new InternalResponse(false, "No servername given.");
                if (serverPort == 0) return new InternalResponse(false, "Invalid server port.");
                socket = new TcpClient();
                await socket.ConnectAsync(serverName, serverPort);
                
                stream = socket.GetStream();
                reader = new NntpStreamReader(stream);
                ServerResponse sr = await ReceiveAsync();

                switch (sr.Code)
                {
                    case 200:
                        canPost = true;
                        break;

                    case 201:
                        canPost = false;
                        MessageBox.Show(sr.Message);
                        break;

                    case 400:
                    case 502:
                        socket.Close();
                        stream.Close();
                        reader.Close();
                        Trace.WriteLine("\nService unavailable.");
                        return new InternalResponse(false, "Service unavailable.");

                    default:
                        Trace.WriteLine($"\n Error {sr.Code}: {sr.Message}");
                        return new InternalResponse(false, $"Error {sr.Code}: {sr.Message}");

                }

                Console.WriteLine("Done.");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error:");
                Trace.WriteLine(ex.Message);
                return new InternalResponse(false, ex.Message);
            }

            return new InternalResponse(true, "");
        }

        public async Task<InternalResponse> LoginAsync(string username, string password)
        {
            await SendAsync($"AUTHINFO USER {username}\r\n");
            ServerResponse sr = await ReceiveAsync();

            switch (sr.Code)
            {
                case 281: // No password required, auth accepted
                    break;

                case 381: // Password required
                    await SendAsync($"AUTHINFO PASS {password}\r\n");
                    ServerResponse srPass = await ReceiveAsync();

                    switch (srPass.Code)
                    {
                        case 281:
                            break;

                        case 481:
                            return new InternalResponse(false, srPass.Message);

                        default:
                            return new InternalResponse(false, $"Error {srPass.Code}: {srPass.Message}");
                    }
                    break;

                case 481:
                case 482:
                case 502:
                    return new InternalResponse(false, sr.Message);
                default:
                    return new InternalResponse(false, $"Error {sr.Code}: {sr.Message}");
            }

            return new InternalResponse(true, "");
        }

        public async Task SendAsync(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            stream.Write(messageBytes, 0, messageBytes.Length);
            stream.Flush();
        }

        public async Task<ServerResponse> ReceiveAsync(bool multiline = false)
        {
            var line = await this.reader.ReadLineAsync();

            if (line == null)
            {
                Trace.WriteLine("\nNo response from server.");
                return new ServerResponse(591, "No response from server.");
            }

            int code;
            if (line.Length < 5 || !int.TryParse(line.Substring(0, 3), out code))
            {
                Trace.WriteLine("\nInvalid response from server.");
                return new ServerResponse(592, "Invalid response from server.");
            }

            string message = line.Substring(4);

            return multiline
                ? new ServerResponse(code, message, new ReadOnlyCollection<string>(reader.ReadAllLines().ToList())) : new ServerResponse(code, message);
        }

        public async Task<InternalResponse> GetGroupListAsync()
        {
            await SendAsync("LIST\r\n");
            ServerResponse sr = await ReceiveAsync(true);


            if (sr.Code != 215) return new InternalResponse(false, sr.Message);
            ReadOnlyCollection<string> list = sr.Lines.Select(line => line.Split(' ')).Select(values => values[0]).ToList().AsReadOnly();

            return new InternalResponse(true, "", list);
        }

        public async Task<InternalResponse> SelectGroup(string group)
        {
            currentGroup = group;
            await SendAsync("GROUP " + group + "\r\n");
            ServerResponse sr = await ReceiveAsync();
            if (sr.Code != 211)
                return new InternalResponse(false, sr.Message);

            return new InternalResponse(true, sr.Message);
        }

        public async Task<InternalResponse> GetHeadlinesAsync(string group)
        {
            currentGroup = group;
            await SendAsync("GROUP " + group + "\r\n");
            ServerResponse sr = await ReceiveAsync();
            if (sr.Code != 211)
                return new InternalResponse(false, sr.Message);

            GroupResponse gr = new GroupResponse(sr.Message);
            List<int> articleNumbers = new List<int>();
            
            for (int i = gr.First; i < gr.Last; i++)
            {
                articleNumbers.Add(i);
            }
            articleNumbers.Sort();

            return new InternalResponse(true, "", articleNumbers);
        }

        public async Task<InternalResponse> GetBodyAsync(int? articleNumber)
        {
            await SendAsync("BODY " + articleNumber + "\r\n");
            ServerResponse sr = await ReceiveAsync(true);
            if (sr.Code == 222) return new InternalResponse(true, sr.Message, sr.Lines);
            return new InternalResponse(false, sr.Message);
        }

        public async Task<InternalResponse> GetArticleAsync(int? articleNumber)
        {
            await SendAsync("ARTICLE " + articleNumber + "\r\n");
            ServerResponse sr = await ReceiveAsync(true);
            if (sr.Code == 220) return new InternalResponse(true, sr.Message, sr.Lines);
            return new InternalResponse(false, sr.Message);
        }

        public async Task<InternalResponse> PostAsync(string message)
        {
            await SendAsync("POST\r\n");
            ServerResponse sr = await ReceiveAsync();
            if (sr.Code != 340)
                return new InternalResponse(false, sr.Message);

            await SendAsync(message);
            sr = await ReceiveAsync();

            return sr.Code == 240 ?
                new InternalResponse(true, sr.Message) :
                new InternalResponse(false, sr.Message);
        }
    }
}
