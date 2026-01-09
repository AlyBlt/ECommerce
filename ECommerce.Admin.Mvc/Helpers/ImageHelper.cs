namespace ECommerce.Admin.Mvc.Helpers
{
    public static class ImageHelper
    {
        private static string? _fileApiUrl;

        // Program.cs başladığında bu metodu çağırıp URL'i sisteme yükleyeceğiz
        public static void Initialize(IConfiguration configuration)
        {
            _fileApiUrl = configuration["FileApiUrl"];
        }

        public static string GetUrl(string? fileName)
        {
            // Eğer fileName null veya boş gelirse, profesyonelce 'default' görseli dön
            string finalFileName = string.IsNullOrEmpty(fileName) ? StaticFiles.DefaultProduct : fileName;

            // Eğer zaten tam bir link gelmişse dokunma
            if (finalFileName.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return finalFileName;

            // Dosya yolu temizleme (URL birleştirme hatalarını önlemek için)
            var baseUrl = _fileApiUrl?.TrimEnd('/');
            return $"{baseUrl}/api/File/download?fileName={finalFileName}";
        }
    }
}
