using Newtonsoft.Json;
using SwiftDigitalSyncManager.Entities;
using SwiftDigitalSyncManager.Helpers;
using SwiftDigitalSyncManager.OAuth;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDigitalSyncManager.SwiftDigitalApi
{
    public class SyncManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void SyncPersons(Apitoken apiToken, string groupId, IList<PersonCsv> csvPersons, List<Person> masterMembers, List<SyncLog> syncLog)
        {

            foreach (PersonCsv person in csvPersons)
            {
                PersonField personField = new PersonField();
                personField.apiCallSource = AppConfiguration.APICallSource;
                personField.consumer_key = AppConfiguration.ConsumerKey;

                List<Field> fields = new List<Field>();

                if (!string.IsNullOrWhiteSpace(person.FirstName))
                    fields.Add(new Field() { fieldId = "first_name", fieldType = "standard", value = person.FirstName });

                if (!string.IsNullOrWhiteSpace(person.LastName))
                    fields.Add(new Field() { fieldId = "last_name", fieldType = "standard", value = person.LastName });

                if (!string.IsNullOrWhiteSpace(person.Email))
                    fields.Add(new Field() { fieldId = "email", fieldType = "standard", value = person.Email });

                if (!string.IsNullOrWhiteSpace(person.Mobile))
                    fields.Add(new Field() { fieldId = "mobile", fieldType = "standard", value = person.Mobile });

                if (!string.IsNullOrWhiteSpace(person.Country))
                    fields.Add(new Field() { fieldId = "country", fieldType = "standard", value = person.Country });

                if (!string.IsNullOrWhiteSpace(person.Title))
                    fields.Add(new Field() { fieldId = "title", fieldType = "standard", value = person.Title });

                if (!string.IsNullOrWhiteSpace(person.Company))
                    fields.Add(new Field() { fieldId = AppConfiguration.SwiftCompanyFieldID, fieldType = "custom", value = person.Company });

                fields.Add(new Field() { fieldId = AppConfiguration.SwiftSYNIDFieldID, fieldType = "custom", value = person.SYNID.ToString() });

                personField.fields = fields.ToArray();

                var groupPersons = masterMembers.Where(Y => Y.fields.Any(T => T.fieldCode == "SYNID" && T.value == person.SYNID.ToString()));

                //Check if user exists on Swift
                if (groupPersons.Count() > 0)
                {
                    var swiftPerson = groupPersons.ElementAt(0);

                    personField.personId = swiftPerson.personId;
                    personField.mappedPersonId = swiftPerson.mappedPersonId;

                    PersonFieldMultiple personMultiple = new PersonFieldMultiple();
                    personMultiple.apiCallSource = AppConfiguration.APICallSource;
                    personMultiple.consumer_key = AppConfiguration.ConsumerKey;
                    personMultiple.persons = new Person[1];

                    personMultiple.persons[0] = new Person() { personId = swiftPerson.personId, fields = fields.ToArray() };

                    PersonManager.EditPerson(apiToken, personMultiple);
                    syncLog.Add(new SyncLog() { SINID = person.SYNID.ToString(), MailGroupId = groupId, Email = person.Email, FirstName = person.FirstName, PersonId = swiftPerson.personId, Action = "Update Person" });

                    //Unbounce if bounced 
                    if (!string.IsNullOrWhiteSpace(swiftPerson.bounceStamp))
                    {
                        PersonManager.UnbouncePerson(apiToken, swiftPerson.personId);
                        syncLog.Add(new SyncLog() { SINID = person.SYNID.ToString(), MailGroupId = groupId, Email = person.Email, FirstName = person.FirstName, PersonId = swiftPerson.personId, Action = "Unbounce Person" });
                    }

                    //Check if csv person have this mailgroup
                    var personMembership = PersonManager.GetPersonMembership(apiToken, swiftPerson.personId);
                    if (personMembership != null)
                    {
                        //Add person to file group
                        var apiGroup = personMembership.memberships.Where(X => X.memberships.Any(Y => Y.mailGroupId == groupId));
                        if (apiGroup == null || apiGroup.Count() == 0)
                        {
                            GroupManager.AddMemeberToGroup(apiToken, groupId, swiftPerson.personId);
                        }

                        //Add person to Master Group
                        var apiMasterGroup = personMembership.memberships.Where(X => X.memberships.Any(Y => Y.mailGroupId == AppConfiguration.MasterGroupID));
                        if (apiMasterGroup == null || apiMasterGroup.Count() == 0)
                        {
                            GroupManager.AddMemeberToGroup(apiToken, AppConfiguration.MasterGroupID, swiftPerson.personId);
                        }
                    }
                }
                else
                {
                    personField.mappedPersonId = person.SYNID.ToString();
                    AddPerson addPerson = PersonManager.AddPerson(apiToken, personField);
                    if (addPerson != null && !string.IsNullOrWhiteSpace(addPerson.personId))
                    {
                        syncLog.Add(new SyncLog() { SINID = person.SYNID.ToString(), MailGroupId = groupId, Email = person.Email, FirstName = person.FirstName, PersonId = addPerson.personId, Action = "Add Person" });
                        GroupManager.AddMemeberToGroup(apiToken, groupId, addPerson.personId);

                        //Add this person to Master group also
                        GroupManager.AddMemeberToGroup(apiToken, AppConfiguration.MasterGroupID, addPerson.personId);
                    }
                }
            }
        }
    }
}
