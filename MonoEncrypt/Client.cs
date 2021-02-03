using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MonoEncrypt
{
    class Client
    {
        public Encryption encryption = new Encryption();
        public string key;
        public Client()
        {
            Console.WriteLine("Welcome to the client");
            TcpClient client = new TcpClient();

            //port and ip adress
            int port = 13356;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(ip, port);

            //client
            client.Connect(endPoint);

            NetworkStream stream = client.GetStream();
            Key(stream);
            ReceiveMessage(stream, key);

            bool isRunning = true;
            while (isRunning)
            {
                //sends message to server
                Console.WriteLine("Write your message: ");
                string text = Console.ReadLine();
                string msg = Encrypt(text, key);
                //string msg = encryption.Encrypt(text);
                byte[] buffer = Encoding.UTF8.GetBytes(msg);

                stream.Write(buffer, 0, buffer.Length);
            }

            client.Close();
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
                Console.WriteLine("The server wrote: \n" + msg);
            }
        }

        public string KeyGenerator()
        {
            Random random = new Random();
            string key = "";
            for (int i = 0; i < 49; i++)
            {
                key += random.Next(1, encryption.key.Length).ToString();
            }
            return key;
        }

        public string Encrypt(string msg, string key)
        {
            char[] chars = new char[msg.Length*5];
            int c = 0;
            for (int i = 0; i < msg.Length; i++)
            {
                if (msg[i] == ' ')
                {
                    chars[c] = '!';
                }
                else
                {
                    int k = msg[i] - 97;
                    string bla = key.Substring(k, 4);
                    char[] keys = bla.ToCharArray();
                    for (int j = 0; j < keys.Length; j++)
                    {
                        chars[j+c] = keys[j];
                    }
                    c += 4;
                }
            }
            return new string(chars);
        }
        public string Decrypt(string msg, string key)
        {
            char[] chars = new char[msg.Length];

            for (int i = 0; i < msg.Length; i++)
            {
                if (msg[i] == '!')
                {
                    chars[i] = ' ';
                }
                else
                {
                    int j = key.IndexOf(msg[i]) + 97;
                    chars[i] = (char)j;
                }
            }
            return new string(chars);
        }
        public void Key(NetworkStream stream)
        {
            string privateKey = KeyGenerator();

            byte[] buffer = Encoding.UTF8.GetBytes(privateKey + encryption.key);
            stream.Write(buffer, 0, buffer.Length);
            buffer = new byte[256];
            int read = stream.Read(buffer, 0, buffer.Length);
            string msg = Encoding.UTF8.GetString(buffer, 0, read);
            key = privateKey + msg;
        }
    }
}
