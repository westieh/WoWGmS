using System;
using System.Collections.Generic;
using WoW.Model;

namespace WoW.MockData
{
    public static class MockApplications
    {
        public static List<Application> Get()
        {
            return new List<Application>
            {
                new Application
                {
                    ApplicationId = 1,
                    DiscordName = "GamerDude#1234",
                    CharacterName = "Thrallius",
                    Class = Class.Shaman,
                    Role = Role.Healer,
                    Note = "Ready to heal any raid!",
                    SubmissionDate = DateTime.Now.AddDays(-1),
                    Approved = false
                },
                new Application
                {
                    ApplicationId = 2,
                    DiscordName = "MageQueen#5678",
                    CharacterName = "Frostfire",
                    Class = Class.Mage,
                    Role = Role.RangedDPS,
                    Note = "Raided mythic in SL",
                    SubmissionDate = DateTime.Now,
                    Approved = true
                }
            };
        }
    }
}
