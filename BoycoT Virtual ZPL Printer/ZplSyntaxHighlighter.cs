using System.Text.RegularExpressions;

namespace BoycoT_Virtual_ZPL_Printer
{
    public class ZplSyntaxHighlighter
    {
        // Color scheme for ZPL syntax
        private static readonly Color DocumentMarkerColor = Color.DarkBlue;
        private static readonly Color CommandColor = Color.Blue;
        private static readonly Color FontBarcodeColor = Color.Green;
        private static readonly Color FieldDataColor = Color.DarkOrange;
        private static readonly Color NumberColor = Color.DarkRed;
        private static readonly Color SeparatorColor = Color.Gray;
        private static readonly Color SetupCommandColor = Color.Teal;

        /// <summary>
        /// Applies syntax highlighting to a RichTextBox containing ZPL code
        /// </summary>
        public static void ApplyHighlighting(RichTextBox rtb)
        {
            if (rtb == null || string.IsNullOrEmpty(rtb.Text))
                return;

            // Save current selection and scroll position
            int originalSelectionStart = rtb.SelectionStart;
            int originalSelectionLength = rtb.SelectionLength;
            int firstVisibleChar = rtb.GetCharIndexFromPosition(new Point(0, 0));

            // Disable repainting for performance
            rtb.SuspendLayout();

            try
            {
                // Reset all formatting first
                rtb.SelectAll();
                rtb.SelectionColor = rtb.ForeColor;
                rtb.SelectionFont = new Font(rtb.Font, FontStyle.Regular);
                rtb.DeselectAll();

                // Apply highlighting in order of precedence
                // 1. Document markers (^XA, ^XZ) - Bold blue
                HighlightPattern(rtb, @"\^XA|\^XZ", DocumentMarkerColor, FontStyle.Bold);

                // 2. Setup commands (^MMT, ^PW, ^LL, ^LS, etc.)
                HighlightPattern(rtb, @"\^(MMT|PW|LL|LS|LH|LT|PR|MD)", SetupCommandColor, FontStyle.Regular);

                // 3. Font and barcode type commands
                HighlightPattern(rtb, @"\^(A[0-9A-Z]|B[A-Z][A-Z0-9]|BY)", FontBarcodeColor, FontStyle.Regular);

                // 4. Field data (^FD...^FS) - Orange for content
                HighlightFieldData(rtb);

                // 5. Position and drawing commands
                HighlightPattern(rtb, @"\^(FO|GB|GC|GD|GE|GF)", CommandColor, FontStyle.Regular);

                // 6. Numbers (coordinates, sizes, etc.)
                HighlightNumbers(rtb);

                // 7. Field separators - Gray
                HighlightPattern(rtb, @"\^FS", SeparatorColor, FontStyle.Regular);
            }
            finally
            {
                // Restore selection and scroll position
                rtb.SelectionStart = originalSelectionStart;
                rtb.SelectionLength = originalSelectionLength;
                rtb.Select(firstVisibleChar, 0);
                rtb.ScrollToCaret();

                // Re-enable repainting
                rtb.ResumeLayout();
            }
        }

        /// <summary>
        /// Highlights text matching a regex pattern
        /// </summary>
        private static void HighlightPattern(RichTextBox rtb, string pattern, Color color, FontStyle style)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            foreach (Match match in regex.Matches(rtb.Text))
            {
                rtb.Select(match.Index, match.Length);
                rtb.SelectionColor = color;
                rtb.SelectionFont = new Font(rtb.Font, style);
            }
        }

        /// <summary>
        /// Highlights field data between ^FD and ^FS
        /// </summary>
        private static void HighlightFieldData(RichTextBox rtb)
        {
            // Match ^FD followed by any characters until ^FS (but don't include ^FS)
            var regex = new Regex(@"\^FD([^\^]*?)(?=\^FS)", RegexOptions.Singleline);
            foreach (Match match in regex.Matches(rtb.Text))
            {
                // Highlight the entire match including ^FD
                rtb.Select(match.Index, match.Length);
                rtb.SelectionColor = FieldDataColor;
                rtb.SelectionFont = new Font(rtb.Font, FontStyle.Regular);
            }
        }

        /// <summary>
        /// Highlights numbers (coordinates, sizes, etc.) but not within ^FD fields
        /// </summary>
        private static void HighlightNumbers(RichTextBox rtb)
        {
            // Find all ^FD...^FS blocks to exclude from number highlighting
            var fieldDataRegex = new Regex(@"\^FD[^\^]*\^FS", RegexOptions.Singleline);
            var fieldDataRanges = fieldDataRegex.Matches(rtb.Text)
                .Cast<Match>()
                .Select(m => new { Start = m.Index, End = m.Index + m.Length })
                .ToList();

            // Highlight numbers that are not inside field data
            var numberRegex = new Regex(@"\b\d+\b");
            foreach (Match match in numberRegex.Matches(rtb.Text))
            {
                // Check if this number is inside a field data block
                bool insideFieldData = fieldDataRanges.Any(range =>
                    match.Index >= range.Start && match.Index < range.End);

                if (!insideFieldData)
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = NumberColor;
                    rtb.SelectionFont = new Font(rtb.Font, FontStyle.Regular);
                }
            }
        }

        /// <summary>
        /// Applies highlighting asynchronously to avoid UI freezing
        /// </summary>
        public static async Task ApplyHighlightingAsync(RichTextBox rtb)
        {
            await Task.Run(() =>
            {
                rtb.Invoke((MethodInvoker)(() => ApplyHighlighting(rtb)));
            });
        }
    }
}