namespace BoycoT_Virtual_ZPL_Printer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            toolStrip1 = new ToolStrip();
            lblSize = new ToolStripLabel();
            cboSize = new ToolStripComboBox();
            sep1 = new ToolStripSeparator();
            lblRenderer = new ToolStripLabel();
            cboRenderer = new ToolStripComboBox();
            sep2 = new ToolStripSeparator();
            btnFlip = new ToolStripButton();
            sep3 = new ToolStripSeparator();
            btnOpenFile = new ToolStripButton();
            btnNewTab = new ToolStripButton();
            sep4 = new ToolStripSeparator();
            btnClear = new ToolStripButton();
            btnVirtualZplPrinterTest = new ToolStripButton();
            tabs = new TabControl();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { lblSize, cboSize, sep1, lblRenderer, cboRenderer, sep2, btnFlip, sep3, btnOpenFile, btnNewTab, sep4, btnClear, btnVirtualZplPrinterTest });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1000, 25);
            toolStrip1.TabIndex = 0;
            // 
            // lblSize
            // 
            lblSize.Name = "lblSize";
            lblSize.Size = new Size(30, 22);
            lblSize.Text = "Size:";
            // 
            // cboSize
            // 
            cboSize.AutoToolTip = true;
            cboSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSize.FlatStyle = FlatStyle.System;
            cboSize.Items.AddRange(new object[] { "4x6", "4x8", "4x8.25" });
            cboSize.Name = "cboSize";
            cboSize.Size = new Size(75, 25);
            cboSize.ToolTipText = "Label Dimensions (inches)";
            // 
            // sep1
            // 
            sep1.Name = "sep1";
            sep1.Size = new Size(6, 25);
            // 
            // lblRenderer
            // 
            lblRenderer.Name = "lblRenderer";
            lblRenderer.Size = new Size(57, 22);
            lblRenderer.Text = "Renderer:";
            // 
            // cboRenderer
            // 
            cboRenderer.AutoToolTip = true;
            cboRenderer.DropDownStyle = ComboBoxStyle.DropDownList;
            cboRenderer.FlatStyle = FlatStyle.System;
            cboRenderer.Name = "cboRenderer";
            cboRenderer.Size = new Size(140, 25);
            cboRenderer.ToolTipText = "ZPL rendering backend";
            // 
            // sep2
            // 
            sep2.Name = "sep2";
            sep2.Size = new Size(6, 25);
            // 
            // btnFlip
            // 
            btnFlip.CheckOnClick = true;
            btnFlip.ImageTransparentColor = Color.Magenta;
            btnFlip.Name = "btnFlip";
            btnFlip.Size = new Size(30, 22);
            btnFlip.Text = "Flip";
            btnFlip.ToolTipText = "Rotate output 180° (for bottom-first labels)";
            // 
            // sep3
            // 
            sep3.Name = "sep3";
            sep3.Size = new Size(6, 25);
            // 
            // btnOpenFile
            // 
            btnOpenFile.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnOpenFile.Image = (Image)resources.GetObject("btnOpenFile.Image");
            btnOpenFile.ImageTransparentColor = Color.Magenta;
            btnOpenFile.Name = "btnOpenFile";
            btnOpenFile.Size = new Size(23, 22);
            btnOpenFile.ToolTipText = "Open ZPL File";
            btnOpenFile.Click += btnOpenFile_Click;
            // 
            // btnNewTab
            // 
            btnNewTab.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnNewTab.Image = (Image)resources.GetObject("btnNewTab.Image");
            btnNewTab.ImageTransparentColor = Color.Magenta;
            btnNewTab.Name = "btnNewTab";
            btnNewTab.Size = new Size(23, 22);
            btnNewTab.ToolTipText = "New Paste Tab";
            btnNewTab.Click += btnNewTab_Click;
            // 
            // sep4
            // 
            sep4.Name = "sep4";
            sep4.Size = new Size(6, 25);
            // 
            // btnClear
            // 
            btnClear.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnClear.Image = (Image)resources.GetObject("btnClear.Image");
            btnClear.ImageTransparentColor = Color.Magenta;
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(23, 22);
            btnClear.ToolTipText = "Clear All Tabs";
            btnClear.Click += btnClear_Click;
            // 
            // btnVirtualZplPrinterTest
            // 
            btnVirtualZplPrinterTest.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnVirtualZplPrinterTest.Image = (Image)resources.GetObject("btnVirtualZplPrinterTest.Image");
            btnVirtualZplPrinterTest.ImageTransparentColor = Color.Magenta;
            btnVirtualZplPrinterTest.Name = "btnVirtualZplPrinterTest";
            btnVirtualZplPrinterTest.Size = new Size(23, 22);
            btnVirtualZplPrinterTest.Text = "Virtual ZPL Printer Test";
            btnVirtualZplPrinterTest.Click += btnVirtualZplPrinterTest_Click;
            // 
            // tabs
            // 
            tabs.Dock = DockStyle.Fill;
            tabs.Location = new Point(0, 25);
            tabs.Name = "tabs";
            tabs.SelectedIndex = 0;
            tabs.Size = new Size(1000, 600);
            tabs.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 625);
            Controls.Add(tabs);
            Controls.Add(toolStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Virtual ZPL Printer";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStripLabel lblSize;
        private ToolStripComboBox cboSize;
        private ToolStripSeparator sep1;
        private ToolStripLabel lblRenderer;
        private ToolStripComboBox cboRenderer;
        private ToolStripSeparator sep2;
        private ToolStripButton btnFlip;
        private ToolStripSeparator sep3;
        private ToolStripButton btnOpenFile;
        private ToolStripButton btnNewTab;
        private ToolStripSeparator sep4;
        private ToolStripButton btnClear;
        private TabControl tabs;
        private ToolStripButton btnVirtualZplPrinterTest;
    }
}