using System.Text.RegularExpressions;

namespace BoycoT_Virtual_ZPL_Printer
{
    public partial class Form1 : Form
    {
        private ZplTcpServer server;
        private ZplRenderer renderer;

        public Form1()
        {
            InitializeComponent();
            cboSize.SelectedIndex = 0; // Default to 4x6 inches
            server = new ZplTcpServer(9100); // or any other available port
            server.Start(OnZplReceived);
            renderer = new ZplRenderer();
        }

        private async void OnZplReceived(string zpl)
        {
            // DEBUG: Log what we received
            //System.Diagnostics.Debug.WriteLine("=== ZPL RECEIVED ===");
            //System.Diagnostics.Debug.WriteLine($"Length: {zpl.Length}");
            //System.Diagnostics.Debug.WriteLine($"First 100 chars: {zpl.Substring(0, Math.Min(100, zpl.Length))}");
            //System.Diagnostics.Debug.WriteLine($"Last 100 chars: {zpl.Substring(Math.Max(0, zpl.Length - 100))}");
            //System.Diagnostics.Debug.WriteLine("===================");

            // Filter out empty or invalid ZPL
            if (string.IsNullOrWhiteSpace(zpl))
            {
                System.Diagnostics.Debug.WriteLine("Ignoring empty ZPL data");
                return;
            }

            // Check if ZPL contains the basic start/end markers
            if (!zpl.Contains("^XA") || !zpl.Contains("^XZ"))
            {
                System.Diagnostics.Debug.WriteLine("Ignoring invalid ZPL (missing ^XA or ^XZ)");
                return;
            }

            string formattedZpl = Regex.Replace(zpl, @"(?<!^)\^", "\r\n^");

            _ = await tabs.InvokeAsync(async () =>
            {
                int tabIndex = tabs.TabCount + 1;
                var tabPage = new TabPage($"{cboSize.Text}-{tabIndex}");

                var splitContainer = new SplitContainer
                {
                    Dock = DockStyle.Fill,
                    Orientation = Orientation.Vertical,
                    SplitterDistance = 300
                };

                var textBox = new RichTextBox
                {
                    Multiline = true,
                    ReadOnly = false, // Changed to false so users can edit
                    ScrollBars = RichTextBoxScrollBars.Both,
                    Dock = DockStyle.Fill,
                    Font = new Font("Consolas", 10),
                    Text = formattedZpl,
                    BorderStyle = BorderStyle.FixedSingle,
                    WordWrap = false
                };
                ZplSyntaxHighlighter.ApplyHighlighting(textBox);
                splitContainer.Panel1.Controls.Add(textBox);

                // Right panel: PictureBox for rendered image
                var pictureBox = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BorderStyle = BorderStyle.FixedSingle
                };
                splitContainer.Panel2.Controls.Add(pictureBox);

                tabPage.Controls.Add(splitContainer);
                tabs.TabPages.Add(tabPage);
                tabs.SelectedTab = tabPage;

                try
                {
                    Image labelImage = await renderer.RenderZplAsync(zpl, cboSize.Text);
                    pictureBox.Image = labelImage;

                    // Adjust the SplitterDistance so the right panel fits the image width
                    int imageWidth = labelImage.Width;
                    int containerWidth = splitContainer.Width;

                    // Ensure the SplitterDistance doesn't exceed bounds
                    if (imageWidth < containerWidth)
                    {
                        splitContainer.SplitterDistance = containerWidth - imageWidth;
                    }
                    else
                    {
                        // Fallback: 70%/30% split if image is too wide
                        splitContainer.SplitterDistance = (int)(containerWidth * 0.7);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to render ZPL: " + ex.Message, "Rendering Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            });
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tabs.TabPages.Clear();
        }

        private void btnVirtualZplPrinterTest_Click(object sender, EventArgs e)
        {
            PrinterTest.SendTestLabelToPrinter("Virtual ZPL Printer");
        }
    }
}
