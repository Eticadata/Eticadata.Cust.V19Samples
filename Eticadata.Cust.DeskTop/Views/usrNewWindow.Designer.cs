namespace Eticadata.Cust.DeskTop.Views
{
    partial class NewWindow
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewWindow));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnExit = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnEntitiesCategory = new System.Windows.Forms.ToolStripButton();
            this.btnPrintLabel = new System.Windows.Forms.ToolStripButton();
            this.btnPrintLabelSalesDoc = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnExit,
            this.btnSave,
            this.btnEntitiesCategory,
            this.btnPrintLabel,
            this.btnPrintLabelSalesDoc});
            this.toolStrip1.Location = new System.Drawing.Point(0, 283);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(487, 43);
            this.toolStrip1.TabIndex = 53;
            this.toolStrip1.Tag = "";
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnExit
            // 
            this.btnExit.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnExit.AutoSize = false;
            this.btnExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(40, 40);
            this.btnExit.Text = "Sair";
            // 
            // btnSave
            // 
            this.btnSave.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnSave.AutoSize = false;
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(40, 40);
            this.btnSave.Text = "Gravar";
            // 
            // btnEntitiesCategory
            // 
            this.btnEntitiesCategory.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnEntitiesCategory.AutoSize = false;
            this.btnEntitiesCategory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEntitiesCategory.Image = ((System.Drawing.Image)(resources.GetObject("btnEntitiesCategory.Image")));
            this.btnEntitiesCategory.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnEntitiesCategory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEntitiesCategory.Name = "btnEntitiesCategory";
            this.btnEntitiesCategory.Size = new System.Drawing.Size(40, 40);
            this.btnEntitiesCategory.Text = "Categorias Entidade";
            this.btnEntitiesCategory.Click += new System.EventHandler(this.btnEntitiesCategory_Click);
            // 
            // btnPrintLabel
            // 
            this.btnPrintLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnPrintLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPrintLabel.Image = ((System.Drawing.Image)(resources.GetObject("btnPrintLabel.Image")));
            this.btnPrintLabel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnPrintLabel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrintLabel.Name = "btnPrintLabel";
            this.btnPrintLabel.Size = new System.Drawing.Size(23, 40);
            this.btnPrintLabel.Text = "Print Label";
            this.btnPrintLabel.Click += new System.EventHandler(this.btnPrintLabel_Click);
            // 
            // btnPrintLabelSalesDoc
            // 
            this.btnPrintLabelSalesDoc.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnPrintLabelSalesDoc.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPrintLabelSalesDoc.Image = ((System.Drawing.Image)(resources.GetObject("btnPrintLabelSalesDoc.Image")));
            this.btnPrintLabelSalesDoc.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnPrintLabelSalesDoc.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrintLabelSalesDoc.Name = "btnPrintLabelSalesDoc";
            this.btnPrintLabelSalesDoc.Size = new System.Drawing.Size(23, 40);
            this.btnPrintLabelSalesDoc.Text = "Print Label";
            this.btnPrintLabelSalesDoc.Click += new System.EventHandler(this.btnPrintFromSalesDocLabel_Click);
            // 
            // NewWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Name = "NewWindow";
            this.Size = new System.Drawing.Size(487, 326);
            this.Tag = "Formação - Janela desktop";
            this.Load += new System.EventHandler(this.NewWindow_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnExit;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnEntitiesCategory;
        private System.Windows.Forms.ToolStripButton btnPrintLabel;
        private System.Windows.Forms.ToolStripButton btnPrintLabelSalesDoc;
    }
}
