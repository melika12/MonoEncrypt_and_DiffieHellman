using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
                Console.WriteLine("Write your message: ");
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

            for (int i = chars.Length-1; i > chars.Length - 8; i--)
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
            for (int i = 7; i < msg.Length - 8; i++)
            {
                if (msg[i] == '!')
                {
                    chars[count] = ' ';
                    count++;
                }
                else if (msg[i] == '\0')
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

            byte[] buffer = Encoding.UTF8.GetBytes(privateKey + encryption.key);
            stream.Write(buffer, 0, buffer.Length);

            buffer = new byte[256];
            int read = stream.Read(buffer, 0, buffer.Length);
            string msg = Encoding.UTF8.GetString(buffer, 0, read);
            key = privateKey + msg;
        }
    }
}
