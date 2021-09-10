using SwiftDigitalSyncManager.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDigitalSyncManager.Helpers
{
    public class EmailHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void SendSyncLogEmail(string recipients, string subject, string body, List<SyncLog> syncLog, MailPriority mailPriority = MailPriority.Normal)
        {
            if (syncLog != null && syncLog.Count > 0)
            {
                var mailMessage = new MailMessage()
                {
                    From = new MailAddress(AppConfiguration.SMTPFrom),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                    Priority = mailPriority
                };

                var to = recipients;

                to.Split(new char[] { ';' }).Select(t => new MailAddress(t))
                                            .ToList()
                                            .ForEach(ma => mailMessage.To.Add(ma));

                IEnumerable<string> personTexts = syncLog.Select(p => String.Join(",", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"), p.MailGroupId, p.SINID, p.PersonId, "\"" + p.FirstName.Replace("\"", "\"\"") + "\"", "\"" + p.Email.Replace("\"", "\"\"") + "\"", "\"" + p.Action.Replace("\"", "\"\"") + "\""));
                string syncLogCsv = "SyncDate,MailGroupId,SINID,PersonId,FirstName,Email,Action";
                syncLogCsv +=    String.Join(Environment.NewLine, personTexts);

                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(stream))
                    {
                        writer.Write(personTexts);
                        writer.Flush();
                        stream.Position = 0;

                        System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage();
                        System.Net.Mail.Attachment att = new System.Net.Mail.Attachment(stream, "SyncLog.csv");
                        m.Attachments.Add(att);
                    }
                }


                var smtpClient = new SmtpClient(AppConfiguration.SMTPServer)
                {
                    Credentials = new NetworkCredential(AppConfiguration.SMTPUsername, AppConfiguration.SMTPPassword)
                };

                try
                {
                    if (AppConfiguration.SMTPPort > 0)
                        smtpClient.Port = AppConfiguration.SMTPPort;

                    smtpClient.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }               
            }
        }


        public static void SendEmail(string recipients, string subject, string body, string[] attachments, MailPriority mailPriority = MailPriority.Normal)
        {
            var mailMessage = new MailMessage()
            {
                From = new MailAddress(AppConfiguration.SMTPFrom),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                Priority = mailPriority
            };

            var to = recipients;

            to.Split(new char[] { ';' }).Select(t => new MailAddress(t))
                                        .ToList()
                                        .ForEach(ma => mailMessage.To.Add(ma));

            if (attachments != null && attachments.Length > 0)
            {
                attachments.Select(t => new Attachment(t))
                                           .ToList()
                                           .ForEach(ma => mailMessage.Attachments.Add(ma));
            }


            var smtpClient = new SmtpClient(AppConfiguration.SMTPServer)
            {
                Credentials = new NetworkCredential(AppConfiguration.SMTPUsername, AppConfiguration.SMTPPassword)
            };

            try
            {
                if (AppConfiguration.SMTPPort > 0)
                    smtpClient.Port = AppConfiguration.SMTPPort;

                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
