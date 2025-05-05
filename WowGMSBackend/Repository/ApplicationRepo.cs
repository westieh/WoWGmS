using System.Collections.Generic;
using WoW.MockData;
using WoW.Model;

namespace WoWGMS.Repository
{
    public class ApplicationRepo
    {
        private static int _currentApplicationId = 1;
        public static List<Application> Applications { get; set; } = MockApplications.Get();

        public ApplicationRepo()
        {
        }

        // opret en application
        public void AddApplication(Application application)
        {
            if (!Applications.Contains(application))
            {
                application.ApplicationId = _currentApplicationId++;
                Applications.Add(application);
            }
        }


        // fjern med Id
        public Application RemoveApplication(int id)
        {
            Application application = GetApplication(id);
            if (application != null)
            {
                Applications.Remove(application);
                return application;
            }
            return null;
        }

        // find med Id
        public Application GetApplication(int id)
        {
            foreach (Application application in Applications)
            {
                if (application.ApplicationId == id)
                    return application;
            }
            return null;
        }

        // se alle applications
        public List<Application> GetAllApplications()
        {
            return Applications;
        }

        // bruger search by på discordname
        public List<Application> SearchApplicationByDiscordName(string discordName)
        {
            List<Application> applicationsResult = new List<Application>();
            foreach (Application application in Applications)
            {
                if (application.DiscordName.Equals(discordName, StringComparison.OrdinalIgnoreCase))
                {
                    applicationsResult.Add(application);
                }
            }
            return applicationsResult;
        }

        // Opdater application
        public Application UpdateApplication(Application newApplication)
        {
            Application gmApplication = GetApplication(newApplication.ApplicationId);
            if (gmApplication != null)
            {
                gmApplication.CharacterName = newApplication.CharacterName;
                gmApplication.DiscordName = newApplication.DiscordName;
                gmApplication.Class = newApplication.Class;
                gmApplication.Role = newApplication.Role;
                gmApplication.SubmissionDate = newApplication.SubmissionDate;
                gmApplication.ProcessedBy = newApplication.ProcessedBy;
                gmApplication.Note = newApplication.Note;
                gmApplication.Approved = newApplication.Approved;

                return gmApplication;
            }
            return null;
        }
    }
}
