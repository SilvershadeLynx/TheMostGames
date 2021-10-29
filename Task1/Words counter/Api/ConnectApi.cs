using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Words_counter.Api
{
    public class ConnectApi
    {
        /// <summary>
        /// Returns result of GET request to textstrings API
        /// </summary>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        public async static Task<TextStrings> GetInfo(string requestParams)
        {
            //initialization
            TextStrings responseObj = new TextStrings();
            //HTTP GET
            using (HttpClient client = new HttpClient())
            {
                //setting base address
                client.BaseAddress = new Uri("https://tmgwebtest.azurewebsites.net/");

                //setting content type
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //setting authorization header
                client.DefaultRequestHeaders.Add("TMG-Api-Key", "0J/RgNC40LLQtdGC0LjQutC4IQ==");

                //initialization
                HttpResponseMessage response = new HttpResponseMessage();

                //HTTP GET
                response = await client.GetAsync("api/textstrings/" + requestParams).ConfigureAwait(false);

                //verification
                if (response.IsSuccessStatusCode)
                {
                    //reading response
                    string result = response.Content.ReadAsStringAsync().Result;
                    responseObj = JsonConvert.DeserializeObject<TextStrings>(result);
                }
                else
                {
                    //there is an 418 "i'm a teapot" status code arise time to time. Might be annoying for user, so potentially could use retry counter in case of 418 code, because it's rather redirection issue
                    throw new HttpRequestException("Ошибка " + response.StatusCode + " - " + response.ReasonPhrase + ".\nПовторите попытку или свяжитесь с администратором системы.");
                }
            }
            return responseObj;
        }
    }
    //class for deserialization
    public class TextStrings
    {
        public string Text;
    }

}
