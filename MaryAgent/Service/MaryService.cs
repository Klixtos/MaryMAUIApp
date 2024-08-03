using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using MaryAgent.Service.Models;

using System.Diagnostics;
using System.Net.Http.Headers;

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


        public async IAsyncEnumerable<MaryResponse> Chat(string userInput)
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
                string requestUri = "https://192.168.0.11:5000/assistant";

                    List<string> filePaths = new List<string>()
                    {
                        @"E:\MaryServer\mary\test_data\c1.txt",
                        @"E:\MaryServer\mary\test_data\c2.txt"
                    };

                    using var form = new MultipartFormDataContent();

                    form.Add(new StringContent(userInput), "user_input");

                    foreach (var filePath in filePaths)
                    {
                        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        var fileContent = new StreamContent(fileStream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        form.Add(fileContent, "files", Path.GetFileName(filePath));
                    }

                var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = form
                };

                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            Debug.WriteLine(line);
                            MaryResponse maryResponse = new MaryResponse()
                            {
                                balance = 0,
                                response = line,
                                cost = 0
                            };

                            yield return maryResponse;
                        }
                    }
                }
            }
        }


    }
}
