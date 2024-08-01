using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using MaryAgent.Service.Models;

using System.Diagnostics;

namespace MaryAgent.Service
{
    internal class MaryService
    {
        //public async Task<string> OAuthLogin(string service = "google")
        //{
        //    if (service == "google")
        //    {
        //        //conect to a custom rest server
        //        using( HttpClient client = new HttpClient())
        //        {
        //            string requestUri = "https://192.168.0.17:5000/login";

        //            try
        //            {
        //                var response = await client.GetAsync(requestUri);
        //                response.EnsureSuccessStatusCode();
        //                var content = await response.Content.ReadAsStringAsync();
        //                Debug.WriteLine(content);
        //                return content;
        //            }
        //            catch (HttpRequestException e)
        //            {
        //                Debug.WriteLine("\nException Caught!");
        //                Debug.WriteLine("Message :{0} ", e.Message);
        //                return null;
        //            }
        //        }
        //    }

        //    return null;

        //}


        public async Task<MaryResponse> Chat(string userInput)
        {


            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

            //conect to a custom rest server
#if DEBUG
            using (HttpClient client = new HttpClient(handler))

#else
            using (HttpClient client = new HttpClient())
#endif
            {
                string requestUri = "https://192.168.0.17:5000/chat";

                try
                {
                    //send the user input to the server in a json format {'user_input': 'message'}


                    var jsonContent = new StringContent($"{{\"user_input\": \"{userInput}\"}}", Encoding.UTF8, "application/json");


                    var response = await client.PostAsync(requestUri, jsonContent);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    var maryResponse = JsonSerializer.Deserialize<MaryResponse>(content);

                    await Task.Delay(2000);

                    return maryResponse;
                }
                catch (HttpRequestException e)
                {
                    Debug.WriteLine("\nException Caught!");
                    Debug.WriteLine($"Message :{e.Message}");
                    return null;
                }
            }
        }


    }
}
