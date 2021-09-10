using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDigitalSyncManager.Entities
{
    public class SyncLog
    {
        public string SINID { get; set; }
        public string MailGroupId { get; set; }
        public string MailGroupName { get; set; }
        public string PersonId { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Action { get; set; }
    }
}
