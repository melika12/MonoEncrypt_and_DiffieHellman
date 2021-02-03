using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoEncrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("What do you want to be? ");
            Console.WriteLine("1. Client");
            Console.WriteLine("2. Server");
            switch (Console.ReadLine())
            {
                case "Client":
                    new Client();
                    break;
                case "client":
                    new Client();
                    break;
                case "1":
                    new Client();
                    break;
                case "Server":
                    new Server();
                    break;
                case "server":
                    new Server();
                    break;
                case "2":
                    new Server();
                    break;
                default:
                    Console.WriteLine("I dont know what that should mean.. Buh bye!");
                    break;
            }
        }
    }
}
