using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDigitalSyncManager.Entities
{
    public class FilePersons
    {
        public string FileID { get; set; }
        public string Extension { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }        

        public List<PersonCsv> Persons { get; set; }

        public List<PersonCsv> DuplicatePersons { get; set; }
    }
}
