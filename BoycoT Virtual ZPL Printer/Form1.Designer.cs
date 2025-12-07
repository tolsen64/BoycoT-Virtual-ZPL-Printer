namespace BoycoT_Virtual_ZPL_Printer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            toolStrip1 = new ToolStrip();
            cboSize = new ToolStripComboBox();
            btnClear = new ToolStripButton();
            btnVirtualZplPrinterTest = new ToolStripButton();
            tabs = new TabControl();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new ToolStripItem[] { cboSize, btnClear, btnVirtualZplPrinterTest });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // cboSize
            // 
            cboSize.AutoToolTip = true;
            cboSize.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSize.FlatStyle = FlatStyle.System;
            cboSize.Items.AddRange(new object[] { "4x6", "4x8", "4x8.25" });
            cboSize.Name = "cboSize";
            cboSize.Size = new Size(75, 25);
            cboSize.ToolTipText = "Document Dimensions";
            // 
            // btnClear
            // 
            btnClear.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnClear.Image = (Image)resources.GetObject("btnClear.Image");
            btnClear.ImageTransparentColor = Color.Magenta;
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(23, 22);
            btnClear.Text = "btnClear";
            btnClear.ToolTipText = "Clear All";
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
            tabs.Size = new Size(800, 425);
            tabs.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
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
        private ToolStripComboBox cboSize;
        private ToolStripButton btnClear;
        private TabControl tabs;
        private ToolStripButton btnVirtualZplPrinterTest;
    }
}
