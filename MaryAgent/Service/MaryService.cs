using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using MaryAgent.Service.Models;
using Newtonsoft.Json.Linq;


using System.Diagnostics;
using System.Net.Http.Headers;

namespace MaryAgent.Service
{
    internal class MaryService
    {
        static string baseUrl = "https://192.168.0.11:5000";
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


        public static async IAsyncEnumerable<MaryResponse> Chat(string thread_id, string userInput , List<string> fileIds)
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
                string requestUri = $"{baseUrl}/assesment/{thread_id}/run";

                using var form = new MultipartFormDataContent();

                form.Add(new StringContent(userInput), "user_input");
                string fileIdsJson = JsonSerializer.Serialize(fileIds);
                form.Add(new StringContent(fileIdsJson, System.Text.Encoding.UTF8, "application/json"), "file_ids");

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
                            MaryResponse maryResponse = new MaryResponse()
                            {
                                balance = 0,
                                response = line + "\n",
                                cost = 0
                            };

                            yield return maryResponse;
                        }
                    }
                }
            }
        }

        public static async Task<string> CreateAssesment()
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
                HttpResponseMessage response = await client.GetAsync($"{baseUrl}/assesment");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);
                return json["thread_id"].ToString();
            }

        }

        public static async Task<bool> DeleteAssesment(string thread_id, List<string> fileIds)
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

                string requestUri = $"{baseUrl}/assesment/{thread_id}";

                using var form = new MultipartFormDataContent();

                string fileIdsJson = JsonSerializer.Serialize(fileIds);
                form.Add(new StringContent(fileIdsJson, System.Text.Encoding.UTF8, "application/json"), "file_ids");

                var request = new HttpRequestMessage(HttpMethod.Delete, requestUri)
                {
                    Content = form
                };

                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseBody);
                    if ((bool)json["deleted"])
                        return true;
                    return false;
                }

                //HttpResponseMessage response = await client.DeleteAsync($"{baseUrl}/assesment/{thread_id}");

                //if (response.IsSuccessStatusCode)
                //{
                //    string responseBody = await response.Content.ReadAsStringAsync();
                //    var json = JObject.Parse(responseBody);
                //    if ((bool)json["deleted"])
                //        return true;
                //    return false;
                //}
                //else
                //{
                //    return false;
                //}
            }

        }

        public static async Task<string> Uploadfile(string filePath)
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
                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                    content.Add(fileContent, "file", System.IO.Path.GetFileName(filePath));

                    var response = await client.PostAsync($"{baseUrl}/assesment/file", content);
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseString);
                    return json["file_id"].ToString();
                }

            }

        }

        public static async Task<bool> DeleteFile(string file_id)
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
                HttpResponseMessage response = await client.DeleteAsync($"{baseUrl}/assesment/file/{file_id}");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseBody);
                    if ((bool)json["deleted"])
                        return true;
                    return false;
                }
                else
                {
                    return false;
                }
            }

        }

    }
}
