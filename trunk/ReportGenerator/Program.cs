using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
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
        static Waiter waiter;
        static void Main(string[] args)
        {
            try
            {

                Waiter w = new Waiter("ajhfjsdhf sajdfhsajfh sdfhsdhf [SendAt:18:20-19:40] ashda asdjhas");
                w.Wait();
                MailMessage msg = new MailMessage();
                msg.Body = GetBody();

                //Getting Pause

                Match m = Regex.Match(msg.Body, @"\[Wait:((\d)*)\]");
                if (m.Success && m.Groups.Count > 1 && m.Groups[1].Value != null)
                {
                    int.TryParse(m.Groups[1].Value, out Pause);
                }

                msg.Subject = GetSubject();
                FillEmails(msg.To, ConfigurationManager.AppSettings["SendTo"].Split(','));
                if(!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CcSendTo"]))
                    FillEmails(msg.CC, ConfigurationManager.AppSettings["CcSendTo"].Split(','));
                if(!string.IsNullOrEmpty(ConfigurationManager.AppSettings["BccSendTo"]))
                    FillEmails(msg.Bcc, ConfigurationManager.AppSettings["BccSendTo"].Split(','));
                string host = ConfigurationManager.AppSettings["host"];
                SmtpClientEx client = new SmtpClientEx(host,"aboimov.softwarium.net");
                 
                


                msg.From = new MailAddress(ConfigurationManager.AppSettings["From"]);
                msg.IsBodyHtml = true;
                if (waiter != null) waiter.Wait();
                if (Pause > 0)
                {
                    Console.WriteLine(string.Format("Sleepping for {0} minutes", Pause));
                    Thread.Sleep(Pause * 60000);
                }
                client.Send(msg);
                
                Console.WriteLine("Done");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
            
        }
        private static void FillEmails(MailAddressCollection to, string[] addr)
        {
            foreach(string a in addr)
            {
                to.Add(new MailAddress(a.Trim()));
            }
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
        private static int Pause = 0;
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
