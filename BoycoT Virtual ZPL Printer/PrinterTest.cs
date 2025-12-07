using System.Drawing.Printing;
using System.Text;

namespace BoycoT_Virtual_ZPL_Printer
{
    internal class PrinterTest
    {
        private static string zplToPrint;

        public static bool SendTestLabelToPrinter(string printerName = "Virtual ZPL Printer")
        {
            try
            {
                string testZpl = Properties.Settings.Default.TestLabel;

                if (string.IsNullOrEmpty(testZpl))
                {
                    throw new Exception("Test ZPL label not found in settings.");
                }

                zplToPrint = testZpl;

                // Create a PrintDocument
                PrintDocument printDoc = new PrintDocument();
                printDoc.PrinterSettings.PrinterName = printerName;

                // Check if printer exists
                if (!printDoc.PrinterSettings.IsValid)
                {
                    throw new Exception($"Printer '{printerName}' not found or is not available.");
                }

                // Set to print raw data
                printDoc.PrintPage += PrintDoc_PrintPage;

                // Send to printer
                printDoc.Print();

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Failed to send test label to printer: {ex.Message}",
                    "Print Test Failed",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
                return false;
            }
        }

        private static void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Send raw ZPL data to the printer
            byte[] zplBytes = Encoding.UTF8.GetBytes(zplToPrint);

            // Write directly to printer stream as raw data
            IntPtr hPrinter;
            if (RawPrinterHelper.OpenPrinter(((PrintDocument)sender).PrinterSettings.PrinterName, out hPrinter, IntPtr.Zero))
            {
                try
                {
                    RawPrinterHelper.SendBytesToPrinter(hPrinter, zplBytes);
                }
                finally
                {
                    RawPrinterHelper.ClosePrinter(hPrinter);
                }
            }
        }
    }

    // Helper class for sending raw data to printer
    internal class RawPrinterHelper
    {
        [System.Runtime.InteropServices.DllImport("winspool.drv", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

        [System.Runtime.InteropServices.DllImport("winspool.drv")]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [System.Runtime.InteropServices.DllImport("winspool.drv", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOC_INFO_1 pDocInfo);

        [System.Runtime.InteropServices.DllImport("winspool.drv")]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [System.Runtime.InteropServices.DllImport("winspool.drv")]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [System.Runtime.InteropServices.DllImport("winspool.drv")]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [System.Runtime.InteropServices.DllImport("winspool.drv")]
        public static extern bool WritePrinter(IntPtr hPrinter, byte[] pBytes, int dwCount, out int dwWritten);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public struct DOC_INFO_1
        {
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)]
            public string pDocName;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)]
            public string pOutputFile;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPTStr)]
            public string pDataType;
        }

        public static bool SendBytesToPrinter(IntPtr hPrinter, byte[] pBytes)
        {
            DOC_INFO_1 docInfo = new DOC_INFO_1
            {
                pDocName = "ZPL Test Label",
                pOutputFile = null,
                pDataType = "RAW"
            };

            if (!StartDocPrinter(hPrinter, 1, ref docInfo))
                return false;

            if (!StartPagePrinter(hPrinter))
            {
                EndDocPrinter(hPrinter);
                return false;
            }

            int dwWritten;
            bool success = WritePrinter(hPrinter, pBytes, pBytes.Length, out dwWritten);

            EndPagePrinter(hPrinter);
            EndDocPrinter(hPrinter);

            return success && (dwWritten == pBytes.Length);
        }
    }
}