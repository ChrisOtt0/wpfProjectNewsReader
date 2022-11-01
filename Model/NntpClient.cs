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
using wpfProjectNewsReader.Tools;

namespace wpfProjectNewsReader.Model
{
    public class NntpClientSingleton : Bindable
    {
        #region Fields
        private static NntpClientSingleton? instance = null;
        private string serverName = "";
        private int serverPort = 119;
        TcpClient? socket = null;
        Stream stream = null;
        NntpStreamReader? reader = null;
        private bool canPost = false;

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
        private NntpClientSingleton() { }
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
            ReadOnlyCollection<string> list = sr.Lines;

            return new InternalResponse(true, "", list);
        }
    }
}
