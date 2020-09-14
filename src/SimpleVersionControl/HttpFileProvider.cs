// -------------------------------------------------------------------------------------------------
// Simple Version Control - © Copyright 2020 - Jam-Es.com
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleVersionControl
{
    public class HttpFileProvider : IFileProvider
    {
        public HttpFileProvider(string baseUrl, HttpClient httpClient = null)
        {
            BaseUrl = baseUrl;
            HttpClient = httpClient;

            if (HttpClient == null)
            {
                HttpClient = new HttpClient();
            }
        }

        public string BaseUrl { get; set; }

        public HttpClient HttpClient { get; set; }

        public async Task<string> GetFile(string relativePath)
        {
            int attempts = 0;
            string uri = BaseUrl.EndsWith("/") ? $"{BaseUrl}{relativePath}" : $"{BaseUrl}/{relativePath}";
            while (attempts < 3)
            {
                attempts++;

                try
                {
                    HttpResponseMessage response = await HttpClient.GetAsync(uri);
                    if (!response.IsSuccessStatusCode)
                    {
                        continue;
                    }

                    return await response.Content.ReadAsStringAsync();
                }
                catch
                {
                    continue;
                }
            }

            return null;
        }
    }
}
