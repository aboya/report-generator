using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Reflection;

namespace ReportGenerator
{
    public class SmtpClientEx : SmtpClient
    {
        private void SetClient(string client)
        {
            typeof(SmtpClient).GetField("clientDomain", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, client);
        }

        public SmtpClientEx()
            : base() { }
        public SmtpClientEx(string client)
            : base()
        {
            SetClient(client);
        }
        public SmtpClientEx(string host, string client)
            : base(host)
        {
            SetClient(client);
        }
        public SmtpClientEx(string host, int port, string client)
            : base(host, port)
        {
            SetClient(client);
        }
    }
}
