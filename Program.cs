using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSendClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("전송할 ip ?");
            string ipString = Console.ReadLine();
            int port = 13000;
            FileSendClient fileCopyClient = new FileSendClient(ipString, port);

            Console.WriteLine("전송할 파일명 ?");
            string fileName = Console.ReadLine();
            fileCopyClient.sendAsync(fileName);

            fileCopyClient.sendFileEventHandler += FileCopyClient_sendFileEventHandler;
        }

        private static void FileCopyClient_sendFileEventHandler(object sender, SendFileEventArgs e)
        {
            
        }
    }
}
