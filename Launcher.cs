using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Launcher
{
    public static async Task<bool> CanLaunchProfile(string profileId)
    {
        var sleepingProfiles = new List<SleepingProfiles>();
        bool fileExists = File.Exists("sleepingProfiles.json");

        if (fileExists)
        {
            string jsonString = await File.ReadAllTextAsync("sleepingProfiles.json");
            sleepingProfiles = JsonConvert.DeserializeObject<List<SleepingProfiles>>(jsonString);
        }
        else
        {
            return true;
        }

        var profile = sleepingProfiles?.FirstOrDefault(p => p.ProfilesId == profileId);

        if (profile == null)
        {
            return true;
        }

        if (profile.LimitProfile == true)
        {
            return false;
        }

        if (profile.FellAsleepProfile.HasValue && (DateTime.Now - profile.FellAsleepProfile.Value).TotalDays < 2)
        {
            return false;
        }

        return true;
    }
}

