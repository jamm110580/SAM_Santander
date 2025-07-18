namespace DispersionMailerDesktop
{
    partial class FormConfiguracion
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
        private void InitializeComponent( )
        {
            panel1 = new Panel( );
            panel3 = new Panel( );
            tbRutaArchvosDispersionNT = new TextBox( );
            btnSeleccionaCarpetaRutaNT = new Button( );
            btnActualizarRutaContratos = new Button( );
            label2 = new Label( );
            panel4 = new Panel( );
            txtRutaCarpetaSistema = new TextBox( );
            btnSeleccionaCarpetaRutaSistema = new Button( );
            btnRutaCarpetaSistema = new Button( );
            label1 = new Label( );
            pnlRutaDispersion = new Panel( );
            tbRutaArchvosDispersion = new TextBox( );
            btnSeleccionaCarpetaRuta = new Button( );
            btnActualizarRuta = new Button( );
            lblRutaDispersion = new Label( );
            pnlActualizaContratos = new Panel( );
            btnSeleccionarArchivosContactos = new Button( );
            tbRutaArchivoContacots = new TextBox( );
            btnActualizarContratos = new Button( );
            lblTituloActualizar = new Label( );
            panel2 = new Panel( );
            dgvContactos = new DataGridView( );
            OFDArchivoExcel = new OpenFileDialog( );
            fbdRutaCarpetaDispersion = new FolderBrowserDialog( );
            fbdCarpetaSistema = new FolderBrowserDialog( );
            btnCerrar = new Button( );
            fbdCarpetaSistemaNT = new FolderBrowserDialog( );
            panel1.SuspendLayout( );
            panel3.SuspendLayout( );
            panel4.SuspendLayout( );
            pnlRutaDispersion.SuspendLayout( );
            pnlActualizaContratos.SuspendLayout( );
            panel2.SuspendLayout( );
            ((System.ComponentModel.ISupportInitialize) dgvContactos).BeginInit( );
            SuspendLayout( );
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ActiveCaption;
            panel1.Controls.Add( panel3 );
            panel1.Controls.Add( panel4 );
            panel1.Controls.Add( pnlRutaDispersion );
            panel1.Controls.Add( pnlActualizaContratos );
            panel1.Location = new Point( 12, 22 );
            panel1.Name = "panel1";
            panel1.Size = new Size( 563, 831 );
            panel1.TabIndex = 0;
            // 
            // panel3
            // 
            panel3.BackColor = SystemColors.ButtonFace;
            panel3.Controls.Add( tbRutaArchvosDispersionNT );
            panel3.Controls.Add( btnSeleccionaCarpetaRutaNT );
            panel3.Controls.Add( btnActualizarRutaContratos );
            panel3.Controls.Add( label2 );
            panel3.Location = new Point( 24, 427 );
            panel3.Name = "panel3";
            panel3.Size = new Size( 513, 190 );
            panel3.TabIndex = 9;
            // 
            // tbRutaArchvosDispersionNT
            // 
            tbRutaArchvosDispersionNT.BackColor = SystemColors.ActiveCaption;
            tbRutaArchvosDispersionNT.BorderStyle = BorderStyle.FixedSingle;
            tbRutaArchvosDispersionNT.ForeColor = SystemColors.HotTrack;
            tbRutaArchvosDispersionNT.Location = new Point( 30, 152 );
            tbRutaArchvosDispersionNT.Name = "tbRutaArchvosDispersionNT";
            tbRutaArchvosDispersionNT.ReadOnly = true;
            tbRutaArchvosDispersionNT.Size = new Size( 456, 27 );
            tbRutaArchvosDispersionNT.TabIndex = 8;
            // 
            // btnSeleccionaCarpetaRutaNT
            // 
            btnSeleccionaCarpetaRutaNT.BackColor = SystemColors.GradientActiveCaption;
            btnSeleccionaCarpetaRutaNT.FlatAppearance.BorderSize = 0;
            btnSeleccionaCarpetaRutaNT.FlatStyle = FlatStyle.Flat;
            btnSeleccionaCarpetaRutaNT.Location = new Point( 35, 110 );
            btnSeleccionaCarpetaRutaNT.Name = "btnSeleccionaCarpetaRutaNT";
            btnSeleccionaCarpetaRutaNT.Size = new Size( 161, 29 );
            btnSeleccionaCarpetaRutaNT.TabIndex = 7;
            btnSeleccionaCarpetaRutaNT.Text = "Seleccione carpeta";
            btnSeleccionaCarpetaRutaNT.UseVisualStyleBackColor = false;
            btnSeleccionaCarpetaRutaNT.Click += btnSeleccionaCarpetaRutaNT_Click;
            // 
            // btnActualizarRutaContratos
            // 
            btnActualizarRutaContratos.BackColor = SystemColors.Highlight;
            btnActualizarRutaContratos.Location = new Point( 30, 50 );
            btnActualizarRutaContratos.Name = "btnActualizarRutaContratos";
            btnActualizarRutaContratos.Size = new Size( 456, 48 );
            btnActualizarRutaContratos.TabIndex = 3;
            btnActualizarRutaContratos.Text = "Actualizar Ruta de Dispersión";
            btnActualizarRutaContratos.UseVisualStyleBackColor = false;
            btnActualizarRutaContratos.Click += btnActualizarRutaContratos_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font( "Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            label2.Location = new Point( 21, 0 );
            label2.Name = "label2";
            label2.Size = new Size( 404, 35 );
            label2.TabIndex = 2;
            label2.Text = "Archivos de dispersión Mandatos";
            label2.TextAlign = ContentAlignment.TopCenter;
            // 
            // panel4
            // 
            panel4.BackColor = SystemColors.ButtonFace;
            panel4.Controls.Add( txtRutaCarpetaSistema );
            panel4.Controls.Add( btnSeleccionaCarpetaRutaSistema );
            panel4.Controls.Add( btnRutaCarpetaSistema );
            panel4.Controls.Add( label1 );
            panel4.Location = new Point( 26, 629 );
            panel4.Name = "panel4";
            panel4.Size = new Size( 514, 190 );
            panel4.TabIndex = 0;
            // 
            // txtRutaCarpetaSistema
            // 
            txtRutaCarpetaSistema.BackColor = SystemColors.ActiveCaption;
            txtRutaCarpetaSistema.BorderStyle = BorderStyle.FixedSingle;
            txtRutaCarpetaSistema.ForeColor = SystemColors.HotTrack;
            txtRutaCarpetaSistema.Location = new Point( 32, 147 );
            txtRutaCarpetaSistema.Name = "txtRutaCarpetaSistema";
            txtRutaCarpetaSistema.ReadOnly = true;
            txtRutaCarpetaSistema.Size = new Size( 456, 27 );
            txtRutaCarpetaSistema.TabIndex = 9;
            // 
            // btnSeleccionaCarpetaRutaSistema
            // 
            btnSeleccionaCarpetaRutaSistema.BackColor = SystemColors.GradientActiveCaption;
            btnSeleccionaCarpetaRutaSistema.FlatAppearance.BorderSize = 0;
            btnSeleccionaCarpetaRutaSistema.FlatStyle = FlatStyle.Flat;
            btnSeleccionaCarpetaRutaSistema.Location = new Point( 31, 106 );
            btnSeleccionaCarpetaRutaSistema.Name = "btnSeleccionaCarpetaRutaSistema";
            btnSeleccionaCarpetaRutaSistema.Size = new Size( 161, 29 );
            btnSeleccionaCarpetaRutaSistema.TabIndex = 9;
            btnSeleccionaCarpetaRutaSistema.Text = "Seleccione carpeta";
            btnSeleccionaCarpetaRutaSistema.UseVisualStyleBackColor = false;
            btnSeleccionaCarpetaRutaSistema.Click += btnSeleccionaCarpetaRutaSistema_Click;
            // 
            // btnRutaCarpetaSistema
            // 
            btnRutaCarpetaSistema.BackColor = SystemColors.Highlight;
            btnRutaCarpetaSistema.Location = new Point( 31, 48 );
            btnRutaCarpetaSistema.Name = "btnRutaCarpetaSistema";
            btnRutaCarpetaSistema.Size = new Size( 456, 48 );
            btnRutaCarpetaSistema.TabIndex = 9;
            btnRutaCarpetaSistema.Text = "Actualizar Ruta de Respositorio";
            btnRutaCarpetaSistema.UseVisualStyleBackColor = false;
            btnRutaCarpetaSistema.Click += btnRutaCarpetaSistema_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font( "Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            label1.Location = new Point( 84, 5 );
            label1.Name = "label1";
            label1.Size = new Size( 339, 35 );
            label1.TabIndex = 9;
            label1.Text = "Ruta de Carpeta de Sistema";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // pnlRutaDispersion
            // 
            pnlRutaDispersion.BackColor = SystemColors.ButtonFace;
            pnlRutaDispersion.Controls.Add( tbRutaArchvosDispersion );
            pnlRutaDispersion.Controls.Add( btnSeleccionaCarpetaRuta );
            pnlRutaDispersion.Controls.Add( btnActualizarRuta );
            pnlRutaDispersion.Controls.Add( lblRutaDispersion );
            pnlRutaDispersion.Location = new Point( 26, 225 );
            pnlRutaDispersion.Name = "pnlRutaDispersion";
            pnlRutaDispersion.Size = new Size( 513, 190 );
            pnlRutaDispersion.TabIndex = 2;
            // 
            // tbRutaArchvosDispersion
            // 
            tbRutaArchvosDispersion.BackColor = SystemColors.ActiveCaption;
            tbRutaArchvosDispersion.BorderStyle = BorderStyle.FixedSingle;
            tbRutaArchvosDispersion.ForeColor = SystemColors.HotTrack;
            tbRutaArchvosDispersion.Location = new Point( 30, 149 );
            tbRutaArchvosDispersion.Name = "tbRutaArchvosDispersion";
            tbRutaArchvosDispersion.ReadOnly = true;
            tbRutaArchvosDispersion.Size = new Size( 456, 27 );
            tbRutaArchvosDispersion.TabIndex = 8;
            // 
            // btnSeleccionaCarpetaRuta
            // 
            btnSeleccionaCarpetaRuta.BackColor = SystemColors.GradientActiveCaption;
            btnSeleccionaCarpetaRuta.FlatAppearance.BorderSize = 0;
            btnSeleccionaCarpetaRuta.FlatStyle = FlatStyle.Flat;
            btnSeleccionaCarpetaRuta.Location = new Point( 35, 107 );
            btnSeleccionaCarpetaRuta.Name = "btnSeleccionaCarpetaRuta";
            btnSeleccionaCarpetaRuta.Size = new Size( 161, 29 );
            btnSeleccionaCarpetaRuta.TabIndex = 7;
            btnSeleccionaCarpetaRuta.Text = "Seleccione carpeta";
            btnSeleccionaCarpetaRuta.UseVisualStyleBackColor = false;
            btnSeleccionaCarpetaRuta.Click += btnSeleccionaCarpetaRuta_Click;
            // 
            // btnActualizarRuta
            // 
            btnActualizarRuta.BackColor = SystemColors.Highlight;
            btnActualizarRuta.Location = new Point( 30, 49 );
            btnActualizarRuta.Name = "btnActualizarRuta";
            btnActualizarRuta.Size = new Size( 456, 48 );
            btnActualizarRuta.TabIndex = 3;
            btnActualizarRuta.Text = "Actualizar Ruta de Dispersión";
            btnActualizarRuta.UseVisualStyleBackColor = false;
            btnActualizarRuta.Click += btnActualizarRuta_Click;
            // 
            // lblRutaDispersion
            // 
            lblRutaDispersion.AutoSize = true;
            lblRutaDispersion.Font = new Font( "Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            lblRutaDispersion.Location = new Point( 4, 4 );
            lblRutaDispersion.Name = "lblRutaDispersion";
            lblRutaDispersion.Size = new Size( 508, 35 );
            lblRutaDispersion.TabIndex = 2;
            lblRutaDispersion.Text = "Archivos de dispersión Clientes en directo\n";
            lblRutaDispersion.TextAlign = ContentAlignment.TopCenter;
            // 
            // pnlActualizaContratos
            // 
            pnlActualizaContratos.BackColor = SystemColors.ButtonFace;
            pnlActualizaContratos.Controls.Add( btnSeleccionarArchivosContactos );
            pnlActualizaContratos.Controls.Add( tbRutaArchivoContacots );
            pnlActualizaContratos.Controls.Add( btnActualizarContratos );
            pnlActualizaContratos.Controls.Add( lblTituloActualizar );
            pnlActualizaContratos.Location = new Point( 26, 20 );
            pnlActualizaContratos.Name = "pnlActualizaContratos";
            pnlActualizaContratos.Size = new Size( 513, 190 );
            pnlActualizaContratos.TabIndex = 1;
            // 
            // btnSeleccionarArchivosContactos
            // 
            btnSeleccionarArchivosContactos.BackColor = SystemColors.GradientActiveCaption;
            btnSeleccionarArchivosContactos.FlatAppearance.BorderSize = 0;
            btnSeleccionarArchivosContactos.FlatStyle = FlatStyle.Flat;
            btnSeleccionarArchivosContactos.Location = new Point( 30, 104 );
            btnSeleccionarArchivosContactos.Name = "btnSeleccionarArchivosContactos";
            btnSeleccionarArchivosContactos.Size = new Size( 161, 29 );
            btnSeleccionarArchivosContactos.TabIndex = 6;
            btnSeleccionarArchivosContactos.Text = "Seleccione archivo";
            btnSeleccionarArchivosContactos.UseVisualStyleBackColor = false;
            btnSeleccionarArchivosContactos.Click += btnSeleccionarArchivosContactos_Click;
            // 
            // tbRutaArchivoContacots
            // 
            tbRutaArchivoContacots.BackColor = SystemColors.ActiveCaption;
            tbRutaArchivoContacots.BorderStyle = BorderStyle.FixedSingle;
            tbRutaArchivoContacots.ForeColor = SystemColors.HotTrack;
            tbRutaArchivoContacots.Location = new Point( 30, 147 );
            tbRutaArchivoContacots.Name = "tbRutaArchivoContacots";
            tbRutaArchivoContacots.ReadOnly = true;
            tbRutaArchivoContacots.Size = new Size( 456, 27 );
            tbRutaArchivoContacots.TabIndex = 5;
            // 
            // btnActualizarContratos
            // 
            btnActualizarContratos.BackColor = SystemColors.Highlight;
            btnActualizarContratos.Location = new Point( 30, 48 );
            btnActualizarContratos.Name = "btnActualizarContratos";
            btnActualizarContratos.Size = new Size( 456, 48 );
            btnActualizarContratos.TabIndex = 2;
            btnActualizarContratos.Text = "Actualizar Lista de Contactos";
            btnActualizarContratos.UseVisualStyleBackColor = false;
            btnActualizarContratos.Click += btnActualizarContratos_Click;
            // 
            // lblTituloActualizar
            // 
            lblTituloActualizar.AutoSize = true;
            lblTituloActualizar.Font = new Font( "Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            lblTituloActualizar.Location = new Point( 135, -1 );
            lblTituloActualizar.Name = "lblTituloActualizar";
            lblTituloActualizar.Size = new Size( 250, 35 );
            lblTituloActualizar.TabIndex = 1;
            lblTituloActualizar.Text = "Actualizar contratos";
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.ActiveCaption;
            panel2.Controls.Add( dgvContactos );
            panel2.Location = new Point( 600, 22 );
            panel2.Name = "panel2";
            panel2.Size = new Size( 746, 831 );
            panel2.TabIndex = 1;
            // 
            // dgvContactos
            // 
            dgvContactos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvContactos.Location = new Point( 18, 17 );
            dgvContactos.Name = "dgvContactos";
            dgvContactos.RowHeadersWidth = 51;
            dgvContactos.Size = new Size( 709, 790 );
            dgvContactos.TabIndex = 0;
            // 
            // OFDArchivoExcel
            // 
            OFDArchivoExcel.FileName = "openFileDialog1";
            // 
            // btnCerrar
            // 
            btnCerrar.Location = new Point( 1236, 861 );
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size( 110, 29 );
            btnCerrar.TabIndex = 4;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = true;
            btnCerrar.Click += btnCerrar_Click;
            // 
            // FormConfiguracion
            // 
            AutoScaleDimensions = new SizeF( 8F, 20F );
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientActiveCaption;
            ClientSize = new Size( 1358, 918 );
            Controls.Add( btnCerrar );
            Controls.Add( panel2 );
            Controls.Add( panel1 );
            Name = "FormConfiguracion";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Configuración";
            Load += Form1_Load;
            panel1.ResumeLayout( false );
            panel3.ResumeLayout( false );
            panel3.PerformLayout( );
            panel4.ResumeLayout( false );
            panel4.PerformLayout( );
            pnlRutaDispersion.ResumeLayout( false );
            pnlRutaDispersion.PerformLayout( );
            pnlActualizaContratos.ResumeLayout( false );
            pnlActualizaContratos.PerformLayout( );
            panel2.ResumeLayout( false );
            ((System.ComponentModel.ISupportInitialize) dgvContactos).EndInit( );
            ResumeLayout( false );
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Panel pnlRutaDispersion;
        private Panel pnlActualizaContratos;
        private Label lblTituloActualizar;
        private Label lblRutaDispersion;
        private Button btnActualizarRuta;
        private Button btnActualizarContratos;
        private TextBox tbRutaArchivoContacots;
        private DataGridView dgvContactos;
        private Button btnSeleccionarArchivosContactos;
        private Button btnSeleccionaCarpetaRuta;
        private TextBox tbRutaArchvosDispersion;
        private OpenFileDialog OFDArchivoExcel;
        private FolderBrowserDialog fbdRutaCarpetaDispersion;
        private Panel panel4;
        private Label label1;
        private Button btnRutaCarpetaSistema;
        private Button btnSeleccionaCarpetaRutaSistema;
        private TextBox txtRutaCarpetaSistema;
        private FolderBrowserDialog fbdCarpetaSistema;
        private Button btnCerrar;
        private Panel panel3;
        private TextBox tbRutaArchvosDispersionNT;
        private Button btnSeleccionaCarpetaRutaNT;
        private Button btnActualizarRutaContratos;
        private Label label2;
        private FolderBrowserDialog fbdCarpetaSistemaNT;
    }
}
