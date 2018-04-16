using FanOutClassLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FanOutDeviceClientClassLibrary.ViewModels
{
    public class ConnectToMothershipsViewModel
    {
        public static readonly ConnectToMothershipsViewModel ViewModel = new ConnectToMothershipsViewModel();

        public string[] MothershipNames { get; private set; } = new string[0];

        public async Task RefreshMothershipsAsync()
        {
            try
            {
                var httpClient = new HttpClient();
                string mothershipsJson = await httpClient.GetStringAsync(WebUrls.BASE_URL + "api/motherships");

                MothershipNames = JsonConvert.DeserializeObject<string[]>(mothershipsJson);
            }
            catch
            {
                MothershipNames = new string[0];
                throw;
            }
        }
    }
}
