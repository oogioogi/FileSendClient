using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSendClient
{
    public delegate void SendFileEventHandler(object sender, SendFileEventArgs e);

    public class SendFileEventArgs: EventArgs
    {
        public string FileName { get; private set; }
        public long remain { get; private set; }

        public SendFileEventArgs(string fileName, long remain)
        {
            FileName = fileName;
            this.remain = remain;
        }
    }
}
