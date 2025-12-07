using System.Text;

namespace BoycoT_Virtual_ZPL_Printer
{
    public class ZplRenderer
    {
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Renders ZPL to an image using Labelary's API.
        /// </summary>
        /// <param name="zpl">Raw ZPL string.</param>
        /// <param name="dpi">Dots per mm (default 8 for 203dpi).</param>
        /// <param name="widthInInches">Label width (e.g. 4 inches).</param>
        /// <param name="heightInInches">Label height (e.g. 6 inches).</param>
        /// <returns>Image object from rendered label.</returns>
        public async Task<Image> RenderZplAsync(string zpl, string documentDimensions, int dpi = 8)
        {
            var url = $"http://api.labelary.com/v1/printers/{dpi}dpmm/labels/{documentDimensions}/0/";

            using var content = new StringContent(zpl, Encoding.UTF8, "application/x-www-form-urlencoded");
            using var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Labelary API error: {response.StatusCode} - {error}");
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            return Image.FromStream(stream);
        }
    }
}
