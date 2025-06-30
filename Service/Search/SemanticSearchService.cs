using Microsoft.Extensions.Configuration;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.Search
{
    public class SemanticSearchService
    {
        private readonly IConfiguration _cfg;
        private readonly IndexNameResolver _indexNameResolver;
        private readonly HttpClient _http;

        public SemanticSearchService(IConfiguration cfg, IndexNameResolver indexNameResolver)
        {
            _cfg = cfg;
            _indexNameResolver = indexNameResolver;
            _http = new HttpClient();
        }

        // 1. הפונקציה ליצירת EMBEDDING
        public async Task<float[]> GetEmbeddingFromOpenAI(string inputText)
        {
            var request = new
            {
                input = inputText,
                model = "text-embedding-3-small"
            };

            var requestJson = JsonSerializer.Serialize(request);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _cfg["OpenAI:ApiKey"]);

            HttpResponseMessage response = null;
            int maxRetries = 3;
            int delayMilliseconds = 2000;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                response = await _http.PostAsync("https://api.openai.com/v1/embeddings", content);

                if (response.IsSuccessStatusCode)
                    break;

                if ((int)response.StatusCode == 429 && attempt < maxRetries)
                {
                    Console.WriteLine($"[OpenAI] קיבלת 429 (יותר מדי בקשות). ממתינה {delayMilliseconds} מילישניות ומנסה שוב...");
                    await Task.Delay(delayMilliseconds);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"OpenAI Error: {response.StatusCode} - {error}");
                }
            }

            Console.WriteLine("OpenAI Key used: " + _cfg["OpenAI:ApiKey"]);

            var responseContent = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseContent);
            var embedding = document.RootElement
                .GetProperty("data")[0]
                .GetProperty("embedding")
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();

            return embedding;
        }


        // 2. הפונקציה ששולחת את הווקטור ל-PINECONE ומחזירה תוצאות
        public async Task<List<string>> SearchSimilarItems(float[] vector, int topK = 5)
        {
            var pineconeKey = _cfg["Pinecone:ApiKey"];
            var environment = _cfg["Pinecone:Environment"];
            var indexName = _indexNameResolver.GetIndexName();

            var uri = $"https://{indexName}-{environment}.svc.pinecone.io/query";

            var body = new
            {
                vector = vector,
                topK = topK,
                includeMetadata = true
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(uri),
                Headers = {
                    Authorization = new AuthenticationHeaderValue("Bearer", pineconeKey)
                },
                Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            };

            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var matches = new List<string>();
            foreach (var match in doc.RootElement.GetProperty("matches").EnumerateArray())
            {
                matches.Add(match.GetProperty("id").GetString());
            }

            return matches;
        }
    }
}
