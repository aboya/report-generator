using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
namespace ReportGenerator
{
    class Program
    {
        private static Encoding enc
        {
            get
            {
                return Encoding.Default;
            }
        }
        static void Main(string[] args)
        {
            try
            {

                MailMessage msg = new MailMessage();
                msg.Body = GetBody();
                msg.Subject = GetSubject();
                msg.To.Add(new MailAddress(ConfigurationManager.AppSettings["SendTo"]));
                string host = ConfigurationManager.AppSettings["host"];
                System.Net.Mail.SmtpClient client = new SmtpClient(host);
                msg.From = new MailAddress("aboimov@softwarium.net");
                msg.IsBodyHtml = true;
                client.Send(msg);
                
                Console.WriteLine("Done");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
            
        }

        private static string GetSubject()
        {
            //FileStream f = File.OpenRead(ConfigurationManager.AppSettings["SubjectTemplate"]);
            string subject = string.Empty;
            using (StreamReader sr = new StreamReader(ConfigurationManager.AppSettings["SubjectTemplate"], enc))
            {
                subject += sr.ReadToEnd();
            }
            return ReplaceTemplates(subject);
            
        }

        private static string GetBody()
        {
            string body = string.Empty;
            using (StreamReader sr = new StreamReader(ConfigurationManager.AppSettings["BodyTemplate"], enc))
            {
                body = sr.ReadToEnd();
            }
            body = body.Replace("[Tasks]", GetTaskTemplate());

            return ReplaceTemplates(body);
        }
        public static string GetTaskTemplate()
        {
            string result = string.Empty;
            string taskTemplate = string.Empty;
            using (StreamReader sr = new StreamReader(ConfigurationManager.AppSettings["TaskTemplate"], enc))
            {
                taskTemplate += sr.ReadToEnd();
            }
            foreach (string task in GetTaskList())
            {
                result += taskTemplate.Replace("[TaskBody]", task);  
            }
            return ReplaceTemplates(result);
        }
        public static List<string> GetTaskList()
        {
            List<string> res = new List<string>();
            using (StreamReader sr = new StreamReader(ConfigurationManager.AppSettings["Tasks"], enc))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    res.Add(line);
                }
            }
            return res;
        }
        public static string ReplaceTemplates(string s)
        {
            //s = Regex.Replace(s, "[Time]", DateTime.Now.ToString("dd.MM.yyyy"), RegexOptions.IgnoreCase);
            s = s.Replace("[Time]", DateTime.Now.ToString("dd.MM.yyyy"));
            return s;
 
        }

    }
}
