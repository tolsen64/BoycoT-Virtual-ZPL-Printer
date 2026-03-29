using BinaryKits.Zpl.Viewer;
using SkiaSharp;
using System.Text;
using System.Text.RegularExpressions;

namespace BoycoT_Virtual_ZPL_Printer
{
    public class BinaryKitsZplRenderer : IZplRenderer
    {
        public string DisplayName => "BinaryKits (local)";

        public Image? RenderZpl(string zpl, double labelWidthInches, double labelHeightInches)
        {
            IPrinterStorage printerStorage = new PrinterStorage();

            zpl = PreloadGrfGraphics(zpl, printerStorage);
            zpl = Regex.Replace(zpl, @"\^ID[^\^]*\^FS", string.Empty, RegexOptions.IgnoreCase);

            var drawer = new ZplElementDrawer(printerStorage);
            var analyzer = new ZplAnalyzer(printerStorage);

            var analyzeInfo = analyzer.Analyze(zpl);
            if (analyzeInfo?.LabelInfos == null || analyzeInfo.LabelInfos.Length == 0)
                return null;

            var imageData = drawer.Draw(analyzeInfo.LabelInfos[0].ZplElements);
            if (imageData == null || imageData.Length == 0)
                return null;

            return Image.FromStream(new MemoryStream(imageData));
        }

        private static string PreloadGrfGraphics(string zpl, IPrinterStorage storage)
        {
            var dgPattern = new Regex(
                @"~DG([A-Z]):([\w]+)\.GRF,(\d+),(\d+),([\s\S]+?)(?=~|\^XA|$)",
                RegexOptions.IgnoreCase);

            return dgPattern.Replace(zpl, m =>
            {
                char storageDevice = char.ToUpper(m.Groups[1].Value[0]);
                string fileName = m.Groups[2].Value;
                int bytesPerRow = int.Parse(m.Groups[4].Value);
                string compressedHex = m.Groups[5].Value.Trim();

                try
                {
                    string rawHex = DecodeGrfCompression(compressedHex);
                    byte[] pngData = GrfHexToPng(rawHex, bytesPerRow);
                    storage.AddFile(storageDevice, fileName, pngData);

                    System.Diagnostics.Debug.WriteLine(
                        $"[GRF] Loaded {storageDevice}:{fileName}.GRF " +
                        $"({bytesPerRow} bytes/row, {rawHex.Length / 2} bytes total)");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[GRF] Failed to preload {storageDevice}:{fileName}.GRF — {ex.Message}");
                }

                return string.Empty;
            });
        }

        private static byte[] GrfHexToPng(string rawHex, int bytesPerRow)
        {
            int totalBytes = rawHex.Length / 2;
            int rows = bytesPerRow > 0 ? totalBytes / bytesPerRow : 1;
            int width = bytesPerRow * 8;

            using var bitmap = new SKBitmap(width, rows, SKColorType.Rgba8888, SKAlphaType.Premul);

            for (int row = 0; row < rows; row++)
            {
                for (int byteIdx = 0; byteIdx < bytesPerRow; byteIdx++)
                {
                    int hexOffset = (row * bytesPerRow + byteIdx) * 2;
                    if (hexOffset + 2 > rawHex.Length) break;

                    byte b = Convert.ToByte(rawHex.Substring(hexOffset, 2), 16);

                    for (int bit = 7; bit >= 0; bit--)
                    {
                        int x = byteIdx * 8 + (7 - bit);
                        bool isBlack = (b & (1 << bit)) != 0;
                        bitmap.SetPixel(x, row, isBlack ? SKColors.Black : SKColors.White);
                    }
                }
            }

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }

        private static string DecodeGrfCompression(string compressed)
        {
            var sb = new StringBuilder();
            int i = 0;

            while (i < compressed.Length)
            {
                char c = compressed[i];

                if (c is '\r' or '\n' or ' ') { i++; continue; }

                if (c == ':') { i++; continue; }

                int repeat = 0;

                if (c >= 'g' && c <= 'z')
                {
                    repeat = 16 + (c - 'g'); i++;
                    if (i < compressed.Length) { sb.Append(compressed[i], repeat); i++; }
                    continue;
                }

                if (c >= 'G' && c <= 'Z')
                {
                    repeat = c - 'F'; i++;
                    if (i < compressed.Length && compressed[i] >= 'g' && compressed[i] <= 'z')
                    {
                        repeat = repeat * 26 + 16 + (compressed[i] - 'g'); i++;
                    }
                    if (i < compressed.Length) { sb.Append(compressed[i], repeat); i++; }
                    continue;
                }

                if (char.IsDigit(c))
                {
                    while (i < compressed.Length && char.IsDigit(compressed[i]))
                    {
                        repeat = repeat * 10 + (compressed[i] - '0'); i++;
                    }
                    if (i < compressed.Length) { sb.Append(compressed[i], repeat); i++; }
                    continue;
                }

                sb.Append(c);
                i++;
            }

            return sb.ToString().ToUpperInvariant();
        }
    }
}