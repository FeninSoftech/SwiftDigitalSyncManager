using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDigitalSyncManager.Entities
{
    public class PersonCsv
    {
        public string PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Country { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string EmailCopy { get; set; }
        public Int64 SYNID { get; set; }
    }

    public sealed class CSVMap : ClassMap<PersonCsv>
    {
        public CSVMap()
        {
            Map(m => m.PersonId).Name("PersonId");
            Map(m => m.FirstName).Name("FirstName");
            Map(m => m.LastName).Name("LastName");
            Map(m => m.Email).Name("Email");
            Map(m => m.Country).Name("Country");
            Map(m => m.Country).Name("Country");
            Map(m => m.Title).Name("Title");
            Map(m => m.Company).Name("Company");
            Map(m => m.SYNID).Name("SYNID");
        }
    }
}
