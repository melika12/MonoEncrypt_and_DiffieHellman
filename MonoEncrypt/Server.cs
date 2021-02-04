using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MonoEncrypt
{
    class Server
    {
        public List<TcpClient> clients = new List<TcpClient>();
        public Encryption encryption = new Encryption();
        public string key;
        public Server()
        {
            Console.WriteLine("Welcome to the server");
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 13356;
            TcpListener listener = new TcpListener(ip, port);
            listener.Start();

            AcceptClients(listener);

            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("Write your message: ");
                string text = Console.ReadLine();
                string msg = Encrypt(text, key);
                //string msg = encryption.Encrypt(text);
                byte[] buffer = Encoding.UTF8.GetBytes(msg);

                foreach (var client in clients)
                {
                    client.GetStream().Write(buffer, 0, buffer.Length);
                }
            }
        }
        public async void AcceptClients(TcpListener listener)
        {
            bool isRunning = true;
            while (isRunning)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                clients.Add(client);
                NetworkStream stream = client.GetStream();

                Key(stream);

                ReceiveMessage(stream, key);
            }
        }
        public async void ReceiveMessage(NetworkStream stream, string key)
        {
            bool isRunning = true;
            while (isRunning)
            {
                byte[] buffer = new byte[256];
                int read = await stream.ReadAsync(buffer, 0, buffer.Length);

                string msg = Encoding.UTF8.GetString(buffer, 0, read);
                msg = Decrypt(msg, key);
                //msg = encryption.Decrypt(msg);
                Console.WriteLine("The client wrote: \n" + msg);
                Console.WriteLine("Write your message: ");
            }
        }
        public string KeyGenerator()
        {
            Random random = new Random();
            string key = "";
            for (int i = 0; i < 70; i++)
            {
                key += random.Next(1, encryption.key.Length).ToString();
            }
            return key;
        }
        public string Encrypt(string msg, string key)
        {
            Random random = new Random();
            int place = 0;
            string noSpace = msg.Replace(" ", "");
            int charLength = (noSpace.Length * 3) + msg.Length + 14;
            char[] chars = new char[charLength];
            for (int i = 0; i <= 7; i++)
            {
                chars[i] = char.Parse(random.Next(9).ToString());
                place = i;
            }
            int c = place;
            for (int i = 0; i < msg.Length; i++)
            {
                if (msg[i] == ' ')
                {
                    chars[c] = '!';
                    c++;
                }
                else
                {
                    int k = msg[i] - 97;
                    string sub = key.Substring(k, 4);
                    char[] keys = sub.ToCharArray();
                    for (int j = 0; j < keys.Length; j++)
                    {
                        chars[j + c] = keys[j];
                    }
                    c += 4;
                }
            }

            for (int i = chars.Length - 1; i > chars.Length - 8; i--)
            {
                chars[i] = char.Parse(random.Next(9).ToString());
            }
            Console.WriteLine(new string(chars));
            return new string(chars);
        }
    
        public string Decrypt(string msg, string key)
        {
            Console.WriteLine(msg);
            char[] chars = new char[msg.Length];
            
            int count = 0;
            for (int i = 7; i < msg.Length-8; i++)
            {
                if (msg[i] == '!')
                {
                    chars[count] = ' ';
                    count++;
                }
                else if(msg[i] == '\0')
                {
                    continue;
                }
                else
                {
                    string sub = msg.Substring(i, 4);
                    int k = key.IndexOf(sub) + 97;
                    chars[count] = (char)k;
                    i += 3;
                    count++;
                }
            }
            return new string(chars);
        }
        public void Key(NetworkStream stream)
        {
            string privateKey = KeyGenerator();

            byte[] buffer = Encoding.UTF8.GetBytes(encryption.key + privateKey);
            stream.Write(buffer, 0, buffer.Length);

            buffer = new byte[256];
            int read = stream.Read(buffer, 0, buffer.Length);
            string msg = Encoding.UTF8.GetString(buffer, 0, read);
            key = msg + privateKey;
        }

    }
}
