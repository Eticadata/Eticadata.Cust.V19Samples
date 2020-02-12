namespace Eticadata.Cust.DeskTop.Views
{
    partial class usrCloseCashSession
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
            this.calculadora1 = new Eticadata.ControlsPOS.calculadora();
            this.btnOk = new Eticadata.ControlsPOS.POSv2Button();
            this.dgCashCount = new System.Windows.Forms.DataGridView();
            this.poSv2Button1 = new Eticadata.ControlsPOS.POSv2Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgCashCount)).BeginInit();
            this.SuspendLayout();
            // 
            // calculadora1
            // 
            this.calculadora1.ApagaValorExistente = true;
            this.calculadora1.CasasDecimais = 0;
            this.calculadora1.CurrencySymbolType = Eticadata.ERP.EtiEnums.CurrencySymbolType.Euro;
            this.calculadora1.Location = new System.Drawing.Point(567, 150);
            this.calculadora1.Margin = new System.Windows.Forms.Padding(0);
            this.calculadora1.MaxLength = ((long)(2147483647));
            this.calculadora1.MaxTamanho = 0;
            this.calculadora1.MaxValue = 0;
            this.calculadora1.Name = "calculadora1";
            this.calculadora1.Password = false;
            this.calculadora1.ScreenFontSize = Eticadata.ERP.EtiEnums.FontSizes.pt10;
            this.calculadora1.ScreenFontStyle = Eticadata.ERP.EtiEnums.FontStyles.Regular;
            this.calculadora1.ScreenSize = Eticadata.ControlsPOS.Enums.CalScreenSize.Big;
            this.calculadora1.ShowCorners = false;
            this.calculadora1.Size = new System.Drawing.Size(246, 319);
            this.calculadora1.TabIndex = 0;
            this.calculadora1.TemAsterisco = false;
            this.calculadora1.TemCancelar = true;
            this.calculadora1.TemConfirmar = true;
            this.calculadora1.TemLimpar = true;
            this.calculadora1.TemMais = false;
            this.calculadora1.TemMenos = false;
            this.calculadora1.TemPonto = true;
            this.calculadora1.TemTeclaPreco = false;
            this.calculadora1.TemVisor = true;
            this.calculadora1.ValoresLixo = 0;
            // 
            // btnOk
            // 
            this.btnOk.Focusable = true;
            this.btnOk.Location = new System.Drawing.Point(713, 0);
            this.btnOk.Margin = new System.Windows.Forms.Padding(0);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(100, 75);
            this.btnOk.TabIndex = 1;
            this.btnOk.TextAlignment = System.Drawing.StringAlignment.Center;
            this.btnOk.TextOrientation = Eticadata.ERP.EtiEnums.TextOrientation.Horizontal;
            this.btnOk.TextPosition = ERP.EtiEnums.TextPosition.Center;
            this.btnOk.ThemeVisualObjectType = Eticadata.ControlsPOS.Enums.VisualButtonTypes.Confirm;
            this.btnOk.Click += new Eticadata.ControlsPOS.POSv2Button.ClickEventHandler(this.btnOk_Click);
            // 
            // dgCashCount
            // 
            this.dgCashCount.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgCashCount.Location = new System.Drawing.Point(3, 78);
            this.dgCashCount.Name = "dgCashCount";
            this.dgCashCount.Size = new System.Drawing.Size(561, 391);
            this.dgCashCount.TabIndex = 2;
            // 
            // poSv2Button1
            // 
            this.poSv2Button1.Focusable = true;
            this.poSv2Button1.Location = new System.Drawing.Point(0, 0);
            this.poSv2Button1.Margin = new System.Windows.Forms.Padding(0);
            this.poSv2Button1.Name = "poSv2Button1";
            this.poSv2Button1.Size = new System.Drawing.Size(100, 75);
            this.poSv2Button1.TabIndex = 3;
            this.poSv2Button1.TextAlignment = System.Drawing.StringAlignment.Center;
            this.poSv2Button1.TextOrientation = ERP.EtiEnums.TextOrientation.Horizontal;
            this.poSv2Button1.TextPosition = ERP.EtiEnums.TextPosition.Center;
            this.poSv2Button1.ThemeVisualObjectType = Eticadata.ControlsPOS.Enums.VisualButtonTypes.Back;
            // 
            // usrCloseCashSession
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.poSv2Button1);
            this.Controls.Add(this.dgCashCount);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.calculadora1);
            this.Name = "usrCloseCashSession";
            this.Size = new System.Drawing.Size(813, 472);
            ((System.ComponentModel.ISupportInitialize)(this.dgCashCount)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ControlsPOS.calculadora calculadora1;
        private ControlsPOS.POSv2Button btnOk;
        private System.Windows.Forms.DataGridView dgCashCount;
        private ControlsPOS.POSv2Button poSv2Button1;
    }
}
