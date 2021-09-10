using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDigitalSyncManager.Entities
{

    public class Membersdetail
    {
        public Membersdetails membersdetails { get; set; }
    }

    public class Membersdetails
    {
        public Membership[] memberships { get; set; }
        public Person[] persons { get; set; }
    }

    public class Membership
    {
        public string mailGroupId { get; set; }
        public Member[] members { get; set; }
    }
}
