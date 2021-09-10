using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDigitalSyncManager.Entities
{

    public class AddGroupMember
    {
        public Status[] status { get; set; }
    }

    public class Status
    {
        public string personId { get; set; }
        public string mailGroupId { get; set; }
        public int status { get; set; }
    }

    public class Groups
    {
        public List<MailGroup> groups { get; set; }
    }

    public class MailGroup
    {
        public string mailGroupId { get; set; }
        public string mailGroupName { get; set; }
        public object folderId { get; set; }
        public string folderName { get; set; }
        public string isTest { get; set; }
        public int noOfMembers { get; set; }
        public string mappedGroupId { get; set; }
        public string timestamp { get; set; }
    }

    public class MailGroups
    {
        public Member[] members { get; set; }
    }

    public class Member
    {
        public string mailGroupId { get; set; }
        public MemberGroup[] members { get; set; }
    }

    public class MemberGroup
    {
        public string personId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public object mobile { get; set; }
        public string subscribeStamp { get; set; }
        public object unsubscribeStamp { get; set; }
        public string timestamp { get; set; }
        public string mappedPersonId { get; set; }
    }
}
