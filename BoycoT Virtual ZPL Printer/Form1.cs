using System.Text.RegularExpressions;

namespace BoycoT_Virtual_ZPL_Printer
{
    public partial class Form1 : Form
    {
        private ZplTcpServer _server;

        private readonly IZplRenderer[] _renderers =
        [
            new BinaryKitsZplRenderer(),
            new LabelaryZplRenderer()
        ];

        // Only call from the UI thread
        private IZplRenderer ActiveRenderer =>
            _renderers[cboRenderer.SelectedIndex < 0 ? 0 : cboRenderer.SelectedIndex];

        public Form1()
        {
            InitializeComponent();

            cboRenderer.Items.AddRange(_renderers.Select(r => r.DisplayName).ToArray<object>());
            cboRenderer.SelectedIndex = 0;
            cboSize.SelectedIndex = 0;

            tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabs.DrawItem += Tabs_DrawItem;
            tabs.MouseDown += Tabs_MouseDown;
            tabs.ItemSize = new Size(0, 24);

            // Wire flip button — only fires on explicit user click
            btnFlip.Click += BtnFlip_Click;

            _server = new ZplTcpServer(9100);
            _server.Start(OnZplReceived);
        }

        // -------------------------------------------------------------------------
        // Flip — explicit user click only
        // -------------------------------------------------------------------------

        private void BtnFlip_Click(object? sender, EventArgs e)
        {
            // btnFlip.Checked already toggled by CheckOnClick before this fires.
            // Walk all open tabs and flip/unflip their images now.
            foreach (TabPage tab in tabs.TabPages)
                foreach (var pb in FindPictureBoxes(tab))
                    UpdatePictureBoxFlip(pb);
        }

        /// <summary>
        /// Stores the original image in Tag on first call, then always derives
        /// the displayed image from Tag so toggle-off restores the original.
        /// </summary>
        private void SetPictureBoxImage(PictureBox pb, Image original)
        {
            // Dispose any previous original stored in Tag
            if (pb.Tag is Image prev) prev.Dispose();

            pb.Tag = original;   // unflipped original — never mutated
            UpdatePictureBoxFlip(pb);
        }

        private void UpdatePictureBoxFlip(PictureBox pb)
        {
            if (pb.Tag is not Image original) return;

            var display = (Image)original.Clone();
            if (btnFlip.Checked)
                display.RotateFlip(RotateFlipType.Rotate180FlipNone);

            var old = pb.Image;
            pb.Image = display;
            if (old != null) old.Dispose();
        }

        private static IEnumerable<PictureBox> FindPictureBoxes(Control root)
        {
            foreach (Control c in root.Controls)
            {
                if (c is PictureBox pb) yield return pb;
                foreach (var child in FindPictureBoxes(c))
                    yield return child;
            }
        }

        // -------------------------------------------------------------------------
        // Tab close button — drawing
        // -------------------------------------------------------------------------

        private void Tabs_DrawItem(object? sender, DrawItemEventArgs e)
        {
            var page = tabs.TabPages[e.Index];
            var tabRect = tabs.GetTabRect(e.Index);

            bool isSelected = e.Index == tabs.SelectedIndex;
            using var bgBrush = new SolidBrush(isSelected ? SystemColors.Window : SystemColors.Control);
            e.Graphics.FillRectangle(bgBrush, tabRect);

            var closeRect = GetCloseRect(tabRect);
            var cursorPos = tabs.PointToClient(Cursor.Position);

            if (closeRect.Contains(cursorPos))
            {
                using var hoverBrush = new SolidBrush(Color.FromArgb(196, 43, 28));
                e.Graphics.FillRectangle(hoverBrush, closeRect);
            }

            using var xFont = new Font("Arial", 8f, FontStyle.Bold);
            var xColor = closeRect.Contains(cursorPos) ? Color.White : Color.DimGray;
            TextRenderer.DrawText(e.Graphics, "×", xFont, closeRect, xColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            var labelRect = new Rectangle(
                tabRect.Left + 4, tabRect.Top,
                tabRect.Width - closeRect.Width - 8, tabRect.Height);

            TextRenderer.DrawText(e.Graphics, page.Text, tabs.Font, labelRect,
                isSelected ? SystemColors.ControlText : SystemColors.ControlDarkDark,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private void Tabs_MouseDown(object? sender, MouseEventArgs e)
        {
            for (int i = 0; i < tabs.TabCount; i++)
            {
                if (GetCloseRect(tabs.GetTabRect(i)).Contains(e.Location))
                {
                    tabs.TabPages.RemoveAt(i);
                    tabs.Invalidate();
                    return;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            tabs.Invalidate();
        }

        private static Rectangle GetCloseRect(Rectangle tabRect) =>
            new Rectangle(tabRect.Right - 18, tabRect.Top + (tabRect.Height - 14) / 2, 14, 14);

        // -------------------------------------------------------------------------
        // Toolbar — open file
        // -------------------------------------------------------------------------

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Open ZPL File",
                Filter = "ZPL files (*.zpl)|*.zpl|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Multiselect = true
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            foreach (var path in dlg.FileNames)
            {
                try
                {
                    var zpl = File.ReadAllText(path);
                    RenderAndDisplay(zpl, label: Path.GetFileName(path));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not read {path}:\n\n{ex.Message}", "File Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // -------------------------------------------------------------------------
        // Toolbar — new paste tab
        // -------------------------------------------------------------------------

        private void btnNewTab_Click(object sender, EventArgs e) => CreatePasteTab();

        private void CreatePasteTab(string initialZpl = "")
        {
            int tabIndex = tabs.TabCount + 1;
            var tabPage = new TabPage($"Paste-{tabIndex}");

            var panel = new Panel { Dock = DockStyle.Fill };

            var renderBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                Padding = new Padding(4, 3, 4, 3)
            };

            var btnRender = new Button
            {
                Text = "▶  Render  (Ctrl+Enter)",
                Dock = DockStyle.Left,
                Width = 180,
                FlatStyle = FlatStyle.Flat
            };

            var lblHint = new Label
            {
                Text = "Paste ZPL below, then click Render or press Ctrl+Enter",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                ForeColor = SystemColors.GrayText,
                Font = new Font(Font, FontStyle.Italic)
            };

            renderBar.Controls.Add(lblHint);
            renderBar.Controls.Add(btnRender);

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 400
            };

            var textBox = new RichTextBox
            {
                Multiline = true,
                ReadOnly = false,
                ScrollBars = RichTextBoxScrollBars.Both,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                Text = initialZpl,
                BorderStyle = BorderStyle.FixedSingle,
                WordWrap = false
            };
            if (!string.IsNullOrWhiteSpace(initialZpl))
                ZplSyntaxHighlighter.ApplyHighlighting(textBox);

            var pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };

            splitContainer.Panel1.Controls.Add(textBox);
            splitContainer.Panel2.Controls.Add(pictureBox);
            panel.Controls.Add(splitContainer);
            panel.Controls.Add(renderBar);
            tabPage.Controls.Add(panel);
            tabs.TabPages.Add(tabPage);
            tabs.SelectedTab = tabPage;

            async void DoRender()
            {
                var zpl = textBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(zpl)) return;

                var formatted = Regex.Replace(zpl, @"(?<!^)\^", "\r\n^");
                textBox.Text = formatted;
                ZplSyntaxHighlighter.ApplyHighlighting(textBox);

                var renderer = ActiveRenderer;
                var parts = cboSize.Text.Split('x');
                double w = double.Parse(parts[0]);
                double h = double.Parse(parts[1]);

                tabPage.Text = $"{tabPage.Text.Split('[')[0].TrimEnd()} [{renderer.DisplayName}]";
                tabs.Invalidate();

                try
                {
                    btnRender.Enabled = false;
                    Image labelImage = await Task.Run(() => renderer.RenderZpl(zpl, w, h))
                        ?? throw new InvalidOperationException("Renderer returned no image.");

                    SetPictureBoxImage(pictureBox, labelImage);

                    int containerWidth = splitContainer.Width;
                    splitContainer.SplitterDistance = labelImage.Width < containerWidth
                        ? containerWidth - labelImage.Width
                        : (int)(containerWidth * 0.7);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to render ZPL using {renderer.DisplayName}:\n\n{ex.Message}",
                        "Rendering Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnRender.Enabled = true;
                }
            }

            btnRender.Click += (_, _) => DoRender();
            textBox.KeyDown += (_, e) =>
            {
                if (e.Control && e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    DoRender();
                }
            };

            if (!string.IsNullOrWhiteSpace(initialZpl))
                DoRender();

            textBox.Focus();
        }

        // -------------------------------------------------------------------------
        // ZPL received from TCP server — background thread
        // -------------------------------------------------------------------------

        private void OnZplReceived(string zpl)
        {
            if (string.IsNullOrWhiteSpace(zpl)) return;
            if (!zpl.Contains("^XA") || !zpl.Contains("^XZ")) return;
            tabs.Invoke(() => RenderAndDisplay(zpl));
        }

        // -------------------------------------------------------------------------
        // Core render → display (UI thread)
        // -------------------------------------------------------------------------

        private async void RenderAndDisplay(string zpl, string? label = null)
        {
            var renderer = ActiveRenderer;
            var sizeText = cboSize.Text;
            string formattedZpl = Regex.Replace(zpl, @"(?<!^)\^", "\r\n^");

            int tabIndex = tabs.TabCount + 1;
            string tabTitle = label != null
                ? $"{label} [{renderer.DisplayName}]"
                : $"{sizeText}-{tabIndex} [{renderer.DisplayName}]";

            var tabPage = new TabPage(tabTitle);

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 300
            };

            var textBox = new RichTextBox
            {
                Multiline = true,
                ReadOnly = false,
                ScrollBars = RichTextBoxScrollBars.Both,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                Text = formattedZpl,
                BorderStyle = BorderStyle.FixedSingle,
                WordWrap = false
            };
            ZplSyntaxHighlighter.ApplyHighlighting(textBox);
            splitContainer.Panel1.Controls.Add(textBox);

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
                var parts = sizeText.Split('x');
                double w = double.Parse(parts[0]);
                double h = double.Parse(parts[1]);

                Image labelImage = await Task.Run(() => renderer.RenderZpl(zpl, w, h))
                    ?? throw new InvalidOperationException("Renderer returned no image.");

                SetPictureBoxImage(pictureBox, labelImage);

                int containerWidth = splitContainer.Width;
                splitContainer.SplitterDistance = labelImage.Width < containerWidth
                    ? containerWidth - labelImage.Width
                    : (int)(containerWidth * 0.7);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to render ZPL using {renderer.DisplayName}:\n\n{ex.Message}",
                    "Rendering Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // -------------------------------------------------------------------------
        // Toolbar events
        // -------------------------------------------------------------------------

        private void btnClear_Click(object sender, EventArgs e) =>
            tabs.TabPages.Clear();

        private void btnVirtualZplPrinterTest_Click(object sender, EventArgs e) =>
            PrinterTest.SendTestLabelToPrinter("Virtual ZPL Printer");
    }
}