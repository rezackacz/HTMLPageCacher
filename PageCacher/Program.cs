using System;
using System.Net;
using System.Net.Http;
namespace PageCacher {
    public class Program {
        public static void Main(string[] args){
            if (args.Length == 0) Console.WriteLine("Usage: PageCacher SERVER_IP STORAGE_URL");

            Console.WriteLine(args[0]);
            Console.WriteLine(args[1]);

            IPAddress server_ip = IPAddress.Parse(args[0]);
        }
    }
}