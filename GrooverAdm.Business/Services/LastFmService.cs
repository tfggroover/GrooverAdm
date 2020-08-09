using GrooverAdm.Entities.LastFm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GrooverAdm.Business.Services
{
    public class LastFmService
    {


        public async Task<TopTagsResponse> GetTrackTags(HttpClient client, string name, string artist)
        {
            var nameUrlEncoded = name.Replace(' ', '+');
            var artistUrlEncoded = artist.Replace(' ', '+');

            var songsDataRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri($"http://ws.audioscrobbler.com/2.0/?method=track.gettoptags&artist={ artistUrlEncoded }&track={ nameUrlEncoded }&api_key=4b47fd9962d0b97e7da9a5f5e7393bd1&format=json"),
                Method = HttpMethod.Get
            };

            var songsDataResponse = await client.SendAsync(songsDataRequest);
            if (songsDataResponse.IsSuccessStatusCode)
            {
                var songsData = JsonConvert.DeserializeObject<TopTagsResponse>(await songsDataResponse.Content.ReadAsStringAsync());
                return songsData;
            }

            return null;
        }
    }
}
