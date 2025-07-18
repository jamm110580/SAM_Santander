
namespace SAM_APP
{
    partial class frmCartasConfirmacion
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.gpbParametros = new System.Windows.Forms.GroupBox();
            this.txtPasw = new System.Windows.Forms.TextBox();
            this.lblPasw = new System.Windows.Forms.Label();
            this.btnDir = new System.Windows.Forms.Button();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.btnFile = new System.Windows.Forms.Button();
            this.lbletiqueta = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.cmbLayout = new System.Windows.Forms.ComboBox();
            this.btnAbrirArchivo = new System.Windows.Forms.Button();
            this.gpbVista = new System.Windows.Forms.GroupBox();
            this.btnExportar = new System.Windows.Forms.Button();
            this.dgvPrevio = new System.Windows.Forms.DataGridView();
            this.gpbParametros.SuspendLayout();
            this.gpbVista.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrevio)).BeginInit();
            this.SuspendLayout();
            // 
            // gpbParametros
            // 
            this.gpbParametros.Controls.Add(this.txtPasw);
            this.gpbParametros.Controls.Add(this.lblPasw);
            this.gpbParametros.Controls.Add(this.btnDir);
            this.gpbParametros.Controls.Add(this.txtDir);
            this.gpbParametros.Controls.Add(this.btnFile);
            this.gpbParametros.Controls.Add(this.lbletiqueta);
            this.gpbParametros.Controls.Add(this.label1);
            this.gpbParametros.Controls.Add(this.txtFile);
            this.gpbParametros.Controls.Add(this.cmbLayout);
            this.gpbParametros.Controls.Add(this.btnAbrirArchivo);
            this.gpbParametros.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpbParametros.Location = new System.Drawing.Point(12, 12);
            this.gpbParametros.Name = "gpbParametros";
            this.gpbParametros.Size = new System.Drawing.Size(854, 141);
            this.gpbParametros.TabIndex = 0;
            this.gpbParametros.TabStop = false;
            this.gpbParametros.Text = "Por favor seleccione las opciones correctas:";
            // 
            // txtPasw
            // 
            this.txtPasw.Location = new System.Drawing.Point(643, 37);
            this.txtPasw.Name = "txtPasw";
            this.txtPasw.Size = new System.Drawing.Size(191, 22);
            this.txtPasw.TabIndex = 9;
            // 
            // lblPasw
            // 
            this.lblPasw.AutoSize = true;
            this.lblPasw.Location = new System.Drawing.Point(640, 18);
            this.lblPasw.Name = "lblPasw";
            this.lblPasw.Size = new System.Drawing.Size(118, 16);
            this.lblPasw.TabIndex = 8;
            this.lblPasw.Text = "Ingrese password:";
            // 
            // btnDir
            // 
            this.btnDir.Location = new System.Drawing.Point(595, 97);
            this.btnDir.Name = "btnDir";
            this.btnDir.Size = new System.Drawing.Size(37, 22);
            this.btnDir.TabIndex = 7;
            this.btnDir.Text = "...";
            this.btnDir.UseVisualStyleBackColor = true;
            this.btnDir.Click += new System.EventHandler(this.btnDir_Click);
            // 
            // txtDir
            // 
            this.txtDir.Location = new System.Drawing.Point(22, 97);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(567, 22);
            this.txtDir.TabIndex = 6;
            // 
            // btnFile
            // 
            this.btnFile.Location = new System.Drawing.Point(595, 97);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(37, 22);
            this.btnFile.TabIndex = 5;
            this.btnFile.Text = "...";
            this.btnFile.UseVisualStyleBackColor = true;
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // lbletiqueta
            // 
            this.lbletiqueta.AutoSize = true;
            this.lbletiqueta.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbletiqueta.Location = new System.Drawing.Point(19, 76);
            this.lbletiqueta.Name = "lbletiqueta";
            this.lbletiqueta.Size = new System.Drawing.Size(53, 16);
            this.lbletiqueta.TabIndex = 4;
            this.lbletiqueta.Text = "Archivo";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Layout";
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(22, 97);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(567, 22);
            this.txtFile.TabIndex = 2;
            // 
            // cmbLayout
            // 
            this.cmbLayout.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLayout.FormattingEnabled = true;
            this.cmbLayout.Items.AddRange(new object[] {
            "BBVA",
            "Scotiabank",
            "Bancomext",
            "Nafin",
            "Banobras",
            "Banorte",
            "Banamex",
            "HSBC"});
            this.cmbLayout.Location = new System.Drawing.Point(108, 34);
            this.cmbLayout.Name = "cmbLayout";
            this.cmbLayout.Size = new System.Drawing.Size(481, 24);
            this.cmbLayout.TabIndex = 1;
            this.cmbLayout.SelectedIndexChanged += new System.EventHandler(this.cmbLayout_SelectedIndexChanged);
            // 
            // btnAbrirArchivo
            // 
            this.btnAbrirArchivo.Location = new System.Drawing.Point(719, 76);
            this.btnAbrirArchivo.Name = "btnAbrirArchivo";
            this.btnAbrirArchivo.Size = new System.Drawing.Size(115, 43);
            this.btnAbrirArchivo.TabIndex = 0;
            this.btnAbrirArchivo.Text = "Extraer información";
            this.btnAbrirArchivo.UseVisualStyleBackColor = true;
            this.btnAbrirArchivo.Click += new System.EventHandler(this.btnAbrirArchivo_Click);
            // 
            // gpbVista
            // 
            this.gpbVista.Controls.Add(this.btnExportar);
            this.gpbVista.Controls.Add(this.dgvPrevio);
            this.gpbVista.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic);
            this.gpbVista.Location = new System.Drawing.Point(12, 159);
            this.gpbVista.Name = "gpbVista";
            this.gpbVista.Size = new System.Drawing.Size(854, 301);
            this.gpbVista.TabIndex = 1;
            this.gpbVista.TabStop = false;
            this.gpbVista.Text = "Vista previa";
            // 
            // btnExportar
            // 
            this.btnExportar.Location = new System.Drawing.Point(719, 258);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(115, 23);
            this.btnExportar.TabIndex = 1;
            this.btnExportar.Text = "Exportar a CSV";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            // 
            // dgvPrevio
            // 
            this.dgvPrevio.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPrevio.Location = new System.Drawing.Point(22, 36);
            this.dgvPrevio.Name = "dgvPrevio";
            this.dgvPrevio.Size = new System.Drawing.Size(812, 196);
            this.dgvPrevio.TabIndex = 0;
            // 
            // frmCartasConfirmacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 469);
            this.Controls.Add(this.gpbVista);
            this.Controls.Add(this.gpbParametros);
            this.Name = "frmCartasConfirmacion";
            this.Text = "Lector de Cartas Confirmación en PDF";
            this.Load += new System.EventHandler(this.frmPDFLector_Load);
            this.gpbParametros.ResumeLayout(false);
            this.gpbParametros.PerformLayout();
            this.gpbVista.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPrevio)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gpbParametros;
        private System.Windows.Forms.Label lbletiqueta;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.ComboBox cmbLayout;
        private System.Windows.Forms.Button btnAbrirArchivo;
        private System.Windows.Forms.Button btnFile;
        private System.Windows.Forms.GroupBox gpbVista;
        private System.Windows.Forms.DataGridView dgvPrevio;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Button btnDir;
        private System.Windows.Forms.TextBox txtDir;
        private System.Windows.Forms.TextBox txtPasw;
        private System.Windows.Forms.Label lblPasw;
    }
}

