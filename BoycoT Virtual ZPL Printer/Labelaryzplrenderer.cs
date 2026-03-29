namespace BoycoT_Virtual_ZPL_Printer
{
    /// <summary>
    /// ZPL renderer backed by the Labelary web API (labelary.com).
    /// Requires internet access. Produces very accurate output including
    /// 2D barcodes (MaxiCode, PDF417, DataMatrix) that BinaryKits may miss.
    ///
    /// API: POST http://api.labelary.com/v1/printers/{dpmm}dpmm/labels/{w}x{h}/0/
    ///      Body: raw ZPL text (application/x-www-form-urlencoded)
    ///      Returns: PNG image bytes
    /// </summary>
    public class LabelaryZplRenderer : IZplRenderer
    {
        public string DisplayName => "Labelary (online)";

        private static readonly HttpClient _http = new()
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        public Image? RenderZpl(string zpl, double labelWidthInches, double labelHeightInches)
        {
            // Labelary expects density in dpmm and dimensions in inches
            const int dpmm = 8; // 203 dpi

            var url = $"http://api.labelary.com/v1/printers/{dpmm}dpmm/labels/{labelWidthInches}x{labelHeightInches}/0/";

            try
            {
                using var content = new StringContent(zpl, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
                using var response = _http.PostAsync(url, content).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Labelary returned {(int)response.StatusCode}: {response.ReasonPhrase}");

                var bytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                using var ms = new MemoryStream(bytes);
                return Image.FromStream(new MemoryStream(bytes)); // keep stream alive
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException("Labelary API timed out. Check your internet connection or switch to BinaryKits.");
            }
        }
    }
}