using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDigitalSyncManager.Helpers
{
    public class AppConfiguration
    {
        public static string APIBaseUrl { get { return ConfigurationManager.AppSettings["APIBaseUrl"].ToString(); } }
        public static string ConsumerKey { get { return ConfigurationManager.AppSettings["ConsumerKey"].ToString(); } }
        public static string ConsumerSecret { get { return ConfigurationManager.AppSettings["ConsumerSecret"].ToString(); } }        
        public static string UserId { get { return ConfigurationManager.AppSettings["UserId"].ToString(); } }
        public static string APIKey { get { return ConfigurationManager.AppSettings["APIKey"].ToString(); } }
        public static string APICallSource { get { return ConfigurationManager.AppSettings["APICallSource"].ToString(); } }

        public static string CsvFilePath { get { return ConfigurationManager.AppSettings["CsvFilePath"].ToString(); } }
        public static string ArchiveFilePath { get { return ConfigurationManager.AppSettings["ArchiveFilePath"].ToString(); } }
        public static string SwiftSYNIDFieldID { get { return ConfigurationManager.AppSettings["SwiftSYNIDFieldID"].ToString(); } }
        public static string SwiftCompanyFieldID { get { return ConfigurationManager.AppSettings["SwiftCompanyFieldID"].ToString(); } }
        public static string MasterGroupID { get { return ConfigurationManager.AppSettings["MasterGroupID"].ToString(); } }

        #region Email      
        public static string SMTPServer { get { return ConfigurationManager.AppSettings["SMTPServer"].ToString(); } }
        public static string SMTPUsername { get { return ConfigurationManager.AppSettings["SMTPUsername"].ToString(); } }
        public static string SMTPPassword { get { return ConfigurationManager.AppSettings["SMTPPassword"].ToString(); } }
        public static int SMTPPort { get { return Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]); } }
        public static string SMTPFrom { get { return ConfigurationManager.AppSettings["SMTPFrom"].ToString(); } }
        public static string EmailRecipients { get { return ConfigurationManager.AppSettings["EmailRecipients"].ToString(); } }        
        #endregion
    }
}
