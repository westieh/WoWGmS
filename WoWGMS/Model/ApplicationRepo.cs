using System.Collections.Generic;
using WoW.MockData;

namespace WoW.Model
{
    public class ApplicationRepo
    {
        public static List<Application> Applications { get; set; } = MockApplications.Get();

    }
}
