using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoEncrypt
{
    class Encryption
    {
        public string key = "zyxwvutsrqp1029384756JKLMNOPQRSTUVWXYZ";
        public string Encrypt(string msg)
        {
            char[] chars = new char[msg.Length];
            for (int i = 0; i < msg.Length; i++)
            {
                if (msg[i] == ' ')
                {
                    chars[i] = '!';
                }
                else
                {
                    int j = msg[i] - 97;
                    chars[i] = key[j];
                }
            }

            return new string(chars);
        }
        public string Decrypt(string msg)
        {
            char[] chars = new char[msg.Length];

            Console.WriteLine("Encrypted: " + msg);
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
    }
}
