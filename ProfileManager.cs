using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public static class ProfileManager
{
    private static readonly string _baseUrl = "http://127.0.0.1:35000/profile";
    // private static readonly string _subBaseUrl = "http://127.0.0.1:35000/automation/launch/python/";
    // Получаю объекты "general_profile_information"
    public static async Task<List<ProfileInfo>> GetProfileInfoAsync()
    {
        var httpClient = new HttpClient();
        var profileInfoList = new List<ProfileInfo>();
        var success = false;

        while (!success)
        {
            try
            {
                var response = await httpClient.GetAsync($"{_baseUrl}/all");

                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                Rootobject? rootObject = JsonConvert.DeserializeObject<Rootobject>(jsonString);

                foreach (var profileData in rootObject?.profileData)
                {
                    string? profileName = profileData?.general_profile_information.profile_name;
                    string? profileGroup = profileData?.general_profile_information.profile_group.ToLower();
                    string? profileId = profileData?.general_profile_information.browser_id;

                    if (profileId != null)
                    {
                        if (profileGroup?.ToLower() == "unassigned")
                        {
                            var profileInfo = new ProfileInfo
                            {
                                Name = profileName,
                                ProfileId = profileId
                            };
                            profileInfoList.Add(profileInfo);
                        }
                        else if (profileGroup?.ToLower() == "прут")
                        {
                            var profileInfo = new ProfileInfo
                            {
                                Name = profileName,
                                ProfileId = profileId
                            };
                            profileInfoList.Add(profileInfo);
                        }
                        else if (profileGroup?.ToLower() == "дешевые аки")
                        {
                            var profileInfo = new ProfileInfo
                            {
                                Name = profileName,
                                ProfileId = profileId
                            };
                            profileInfoList.Add(profileInfo);
                        }
                    }
                }

                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving profile info: {ex.Message}");
                Console.WriteLine("Retrying in 10 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        return profileInfoList;
    }

    // Запускаю профиль
    public static async Task<bool> LaunchProfileAsync(string profileId)
    {
        try
        {
            var launchUrl = $"http://127.0.0.1:35000/automation/launch/python/{profileId}/cloud";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(launchUrl);
            string reposnseString = await response.Content.ReadAsStringAsync();

            //Console.WriteLine(response);
            try
            {
                JObject responseDataJson = JObject.Parse(reposnseString);
                string? status = (string)responseDataJson["status"];

                //Console.WriteLine($"Ответ от сервера при попытке загрузить профиль: {status}");
                if (status == "error") { return false; }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения ответа от сервера: {ex.Message}");
            }


            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"Profile {profileId} launched.");
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to launch profile {profileId}. Status code: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error launching profile {profileId}. {ex.Message}");
            return false;
        }
    }

    //Останавливаю профиль
    //public static async Task CloseProfileAsync(string profileId)
    //{
    //    using (var httpClient = new HttpClient())
    //    {
    //        var response = await httpClient.GetAsync($"http://127.0.0.1:35000/profile/stop/{profileId}");
    //        if (!response.IsSuccessStatusCode)
    //        {
    //            Console.WriteLine($"Failed to close profile with id {profileId}. Status code: {response.StatusCode}");
    //        }
    //    }

    //}

    public class Rootobject
    {
        public Profiledata[]? profileData { get; set; }
    }

    public class Profiledata
    {
        public Timezone Timezone { get; set; }
        public General_Profile_Information general_profile_information { get; set; }
        public Proxy Proxy { get; set; }
        public Geolocation Geolocation { get; set; }
        public Navigator Navigator { get; set; }
        public Media_Devices Media_devices { get; set; }
        public Customdns CustomDNS { get; set; }
        public Fonts Fonts { get; set; }
        public Hardware Hardware { get; set; }
        public Extensions Extensions { get; set; }
        public Webrtc WebRTC { get; set; }
        public Other Other { get; set; }
    }

    public class Timezone
    {
        public string fill_timezone_based_on_ip { get; set; }
        public string timezone_offset { get; set; }
        public string timezone_name { get; set; }
    }

    public class General_Profile_Information
    {
        public string profile_browser_version { get; set; }
        public string simulated_operating_system { get; set; }
        public string profile_name { get; set; }
        public string browser_id { get; set; }
        public string profile_notes { get; set; }
        public string profile_group { get; set; }
        public string profile_last_edited { get; set; }
    }

    public class Proxy
    {
        public string connection_type { get; set; }
        public string proxy_rotation_api_url { get; set; }
        public string proxy_username { get; set; }
        public int proxy_rotating { get; set; }
        public string proxy_provider { get; set; }
        public string proxy_url { get; set; }
        public string proxy_password { get; set; }
    }

    public class Geolocation
    {
        public string fill_geolocation_based_on_ip { get; set; }
        public string behavior { get; set; }
        public Location_Information location_information { get; set; }
    }

    public class Location_Information
    {
        public string latitude { get; set; }
        public string accuracy { get; set; }
        public string longitude { get; set; }
    }

    public class Navigator
    {
        public bool navigator_languageIPToggle { get; set; }
        public string hardware_concurrency { get; set; }
        public string languages { get; set; }
        public bool do_not_track { get; set; }
        public string screen_resolution { get; set; }
        public bool navigator_useragent_always_latest { get; set; }
        public string user_agent { get; set; }
        public string platform { get; set; }
        public string navigator_deviceMemory { get; set; }
        public bool navigator_useragent_match_chrome_core { get; set; }
    }

    public class Media_Devices
    {
        public int audio_outputs { get; set; }
        public int video_outputs { get; set; }
        public string enable_media_masking { get; set; }
        public int audio_inputs { get; set; }
    }

    public class Customdns
    {
        public string customDNS_enabled { get; set; }
        public string customDNS_details { get; set; }
    }

    public class Fonts
    {
        public string browser_font_list { get; set; }
        public string enable_font_list_masking { get; set; }
        public string enable_unicode_glyps_domrect { get; set; }
    }

    public class Hardware
    {
        public Webgl WebGL { get; set; }
        public Canvas Canvas { get; set; }
        public Audiocontext AudioContext { get; set; }
    }

    public class Webgl
    {
        public Webgl_Meta WebGL_meta { get; set; }
        public Webgl_Image WebGL_image { get; set; }
    }

    public class Webgl_Meta
    {
        public string WebGL_meta_unmasked_renderer { get; set; }
        public string WebGL_meta_renderer { get; set; }
        public string WebGL_meta_vendor { get; set; }
        public string WebGL_meta_unmasked_vendor { get; set; }
        public string WebGL_meta_behavior { get; set; }
    }

    public class Webgl_Image
    {
        public string WebGL_behavior { get; set; }
        public string WebGL_hash { get; set; }
    }

    public class Canvas
    {
        public string Canvas_hash { get; set; }
        public string Canvas_behavior { get; set; }
    }

    public class Audiocontext
    {
        public string AudioContext_hash { get; set; }
        public string Audio_Context_behavior { get; set; }
    }

    public class Extensions
    {
        public string contains_extensions { get; set; }
    }

    public class Webrtc
    {
        public string local_ip { get; set; }
        public string public_ip { get; set; }
        public string behavior { get; set; }
        public bool set_external_ip { get; set; }
    }

    public class Other
    {
        public string browser_allowRealMediaDevices { get; set; }
        public string other_try_to_pass_iphey { get; set; }
        public string active_session_lock { get; set; }
        public string other_ShowProfileName { get; set; }
        public string custom_browser_args_enabled { get; set; }
        public string browser_language_lock { get; set; }
        public string custom_browser_language { get; set; }
        public string custom_browser_args_string { get; set; }
    }

}
