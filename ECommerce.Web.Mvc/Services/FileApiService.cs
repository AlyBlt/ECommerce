using ECommerce.Application.DTOs.FileApi;
using System.Net.Http.Headers;

namespace ECommerce.Web.Mvc.Services
{
    public class FileApiService
    {
        private readonly HttpClient _httpClient;

        public FileApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // File API'nin URL'ini buraya yazmalısın (Örn: localhost:5002)
            // Not: Bunu ileride Program.cs'den de yönetebiliriz.
        }

        public async Task<string?> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            // 1. Çok parçalı (multipart) bir form içeriği oluşturuyoruz
            using var content = new MultipartFormDataContent();

            // 2. Dosyayı akış (stream) olarak içeriğe ekliyoruz
            var fileStream = file.OpenReadStream();
            var fileContent = new StreamContent(fileStream);

            // 3. Dosyanın tipini (image/jpeg vb.) belirtiyoruz
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            // 4. API'deki "file" parametre ismiyle eşleşecek şekilde ekliyoruz
            content.Add(fileContent, "file", file.FileName);

            // 5. File API'deki "upload" endpoint'ine POST atıyoruz
            var response = await _httpClient.PostAsync("api/File/upload", content);

            if (response.IsSuccessStatusCode)
            {
                // API'den dönen JSON'ı oku (Örn: { "fileName": "abc.jpg" })
                var result = await response.Content.ReadFromJsonAsync<FileUploadResponseDTO>();
                return result?.FileName?.Replace("\"", "").Trim();
            }

            return null;
        }
    }

}
