using CsvHelper;
using Newtonsoft.Json;
using SwiftDigitalSyncManager.Entities;
using SwiftDigitalSyncManager.Helpers;
using SwiftDigitalSyncManager.SwiftDigitalApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDigitalSyncManager
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            var auth = SwiftAuthClient.GetAccessToken();

            List<SyncLog> syncLog = new List<SyncLog>();

            log.Info("Starting sync process at: " + DateTime.Now.ToString());
            Console.WriteLine("Starting sync process at: " + DateTime.Now.ToString());
            //Read all files from CSV File Path
            DirectoryInfo csvDir = new DirectoryInfo(AppConfiguration.CsvFilePath);
            FileInfo[] files = csvDir.GetFiles("*.csv");

            //Create list of all csv persons
            List<FilePersons> allFilePersons = new List<FilePersons>();
            foreach (FileInfo file in files)
            {
                string mailGroupId = Path.GetFileNameWithoutExtension(file.Name);

                string delimiter = (file.Extension.ToLower() == ".txt") ? "\t" : ",";

                IList<PersonCsv> csvData = GetTableFromCSV(file.FullName, delimiter);

                if (csvData != null && csvData.Count() > 0)
                {
                    FilePersons newFile = new FilePersons();
                    newFile.FileID = mailGroupId;
                    newFile.FullName = file.FullName;
                    newFile.Name = file.Name;
                    newFile.Extension = file.Extension.ToLower();
                    newFile.Persons = new List<PersonCsv>();

                    foreach (PersonCsv person in csvData)
                    {
                        newFile.Persons.Add(person);
                    }

                    allFilePersons.Add(newFile);
                }
            }

            if (allFilePersons.Count() > 0)
            {
                List<Person> masterMembers = new List<Person>();
                Membersdetail masterGroupMembers = GroupManager.GetGroupMembers(auth.apiToken, AppConfiguration.MasterGroupID);
                if (masterGroupMembers != null && masterGroupMembers.membersdetails != null)
                {
                    //Create a list of all Master Mail Group Persons
                    foreach (Person personParent in masterGroupMembers.membersdetails.persons)
                    {
                        masterMembers.Add(personParent);
                    }

                    PersonFieldMultiple personMultiple = new PersonFieldMultiple();
                    personMultiple.apiCallSource = AppConfiguration.APICallSource;
                    personMultiple.consumer_key = AppConfiguration.ConsumerKey;

                    List<Person> gropuPersons = new List<Person>();

                    //Update all Master Mail Group persons email = <SYNCID>@example.com
                    foreach (Person apiPerson in masterMembers)
                    {
                        personMultiple.persons = new Person[1];
                        List<Field> fields = new List<Field>();

                        var apiSyncField = apiPerson.fields.FirstOrDefault(T => T.fieldCode == "SYNID");
                        //If syncId available set email = <syncID>@example.com
                        if (apiSyncField != null && !string.IsNullOrWhiteSpace(apiSyncField.value))
                        {
                            fields.Add(new Field() { fieldId = "email", fieldType = "standard", value = apiSyncField.value + "@example.com" });
                        }
                        //Else set email = <unique>@example.com
                        else
                        {
                            fields.Add(new Field() { fieldId = "email", fieldType = "standard", value = DateTime.Now.ToString("MMMddyyyyHHMMSSfff") + "@example.com" });
                        }

                        personMultiple.persons[0] = new Person() { personId = apiPerson.personId, fields = fields.ToArray() };

                        PersonManager.EditPerson(auth.apiToken, personMultiple);
                    }

                    //Process all file groups (Remove all persons from each file mail group)
                    foreach (FilePersons filePerson in allFilePersons)
                    {
                        string mailGroupId = Path.GetFileNameWithoutExtension(filePerson.Name);
                        List<string> personIds = null;

                        Membersdetail groupMembers = GroupManager.GetGroupMembers(auth.apiToken, mailGroupId);
                        if (groupMembers != null && groupMembers.membersdetails != null && groupMembers.membersdetails.persons != null && groupMembers.membersdetails.persons.Count() > 0)
                        {
                            personIds = new List<string>();
                            foreach (var person in groupMembers.membersdetails.persons)
                            {
                                personIds.Add(person.personId);
                            }
                        }

                        if (personIds != null && personIds.Count() > 0)
                            GroupManager.RemoveMemeberFromGroup(auth.apiToken, mailGroupId, personIds.ToArray());
                    }

                    //Update each person with email
                    foreach (FilePersons file in allFilePersons)
                    {
                        string mailGroupId = Path.GetFileNameWithoutExtension(file.Name);

                        log.Info("Sync process for MailGroup (" + mailGroupId + ") started.");
                        Console.WriteLine("Sync process for MailGroup (" + mailGroupId + ") started...");

                        try
                        {
                            SyncManager.SyncPersons(auth.apiToken, Path.GetFileNameWithoutExtension(file.Name), file.Persons, masterMembers, syncLog);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }

                        try
                        {
                            string targetFile = Path.Combine(AppConfiguration.ArchiveFilePath, file.Name);

                            if (File.Exists(targetFile))
                            {
                                File.Delete(targetFile);
                            }

                            File.Move(file.FullName, Path.Combine(AppConfiguration.ArchiveFilePath, file.Name));
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }
                    }

                    log.Info("Sync process completed at: " + DateTime.Now.ToString());
                    Console.WriteLine("Sync process completed at: " + DateTime.Now.ToString());

                    if (!string.IsNullOrWhiteSpace(AppConfiguration.EmailRecipients) && !string.IsNullOrWhiteSpace(AppConfiguration.SMTPServer))
                    {
                        EmailHelper.SendSyncLogEmail(AppConfiguration.EmailRecipients, "Sync Log", "Hi, </br></br> Please check attached sync log. </br></br> Thanks", syncLog);
                    }
                }
                else
                {
                    log.Info("Master group not found");
                    Console.WriteLine("Master group not found");
                }
            }
            else
            {
                log.Info("No files found to sync");
                Console.WriteLine("No files found to sync");
            }
        }

        private static IList<PersonCsv> GetTableFromCSV(string csvFileName, string delimiter)
        {
            IList<PersonCsv> csvData = null;
            using (TextReader fileReader = File.OpenText(csvFileName))
            {
                using (var csv = new CsvReader(fileReader))
                {
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.Delimiter = delimiter;
                    csv.Configuration.RegisterClassMap<CSVMap>();

                    csvData = csv.GetRecords<PersonCsv>().ToList();
                }
            }

            return csvData;
        }
    }
}
