using Microsoft.Extensions.Configuration;
using SkillLink.Application.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace SkillLink.Infrastructure.Services
{
    public class LocalEmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpointUrl;

        public LocalEmbeddingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _endpointUrl = configuration["EmbeddingSettings:Url"] ?? "http://python-service:5000/embed";
        } 

        public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            var payload = new { text = text };
            var response = await _httpClient.PostAsJsonAsync(_endpointUrl, payload, cancellationToken);
            
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<EmbeddingResponse>(cancellationToken: cancellationToken);
            
            if (result?.Embedding == null)
            {
                throw new InvalidOperationException("Failed to retrieve embeddings from local service.");
            }

            return result.Embedding;
        }

        private class EmbeddingResponse
        {
            public float[] Embedding { get; set; } = [];
        }
    }
}
