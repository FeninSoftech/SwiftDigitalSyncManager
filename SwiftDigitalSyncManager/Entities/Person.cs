using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDigitalSyncManager.Entities
{
    public class Persons
    {
        public string mailGroupId { get; set; }
        public List<Person> persons { get; set; }
    }

    public class Person
    {
        public string personId { get; set; }
        public string mappedPersonId { get; set; }
        public string emailUnsubscribeStamp { get; set; }
        public string smsUnsubscribeStamp { get; set; }
        public string bounceStamp { get; set; }
        public object country { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public object mobile { get; set; }
        public string title { get; set; }
        public Field[] fields { get; set; }
    }

    public class PersonField
    {
        public string apiCallSource { get; set; }
        public string consumer_key { get; set; }
        public string mappedPersonId { get; set; }
        public string personId { get; set; }
        public Field[] fields { get; set; }
    }

    public class PersonFieldMultiple
    {
        public string apiCallSource { get; set; }
        public string consumer_key { get; set; }
        public Person[] persons { get; set; }
    }

    public class Field
    {
        public string fieldId { get; set; }
        public string fieldType { get; set; }
        public string fieldCode { get; set; }
        public string value { get; set; }
    }


    public class AddPerson
    {
        public string personId { get; set; }
    }


    public class Unbounce
    {
        public PersonStatus[] status { get; set; }
    }

    public class PersonStatus
    {
        public string personId { get; set; }
        public int status { get; set; }
        public string error { get; set; }
    }

    public class Rootobject
    {
        public PersonMembership[] memberships { get; set; }
    }

    public class PersonMembership
    {
        public string personId { get; set; }
        public Membership1[] memberships { get; set; }
    }

    public class Membership1
    {
        public string mailGroupId { get; set; }
        public string mailGroupName { get; set; }
        public string isTest { get; set; }
        public string SubscribeStamp { get; set; }
        public string folderId { get; set; }
        public string folderName { get; set; }
    }


    public class EditPerson
    {
        public PersonStatus[] status { get; set; }
    }    
}
