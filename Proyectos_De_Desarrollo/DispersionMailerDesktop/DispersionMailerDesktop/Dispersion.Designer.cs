namespace DispersionMailerDesktop
{
    partial class FormDispersion
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if(disposing && (components != null))
            {
                components.Dispose( );
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            panel1 = new Panel( );
            pnlActualizaContratos = new Panel( );
            label3 = new Label( );
            btnListaContactos = new Button( );
            label2 = new Label( );
            label1 = new Label( );
            btnContratosSinContacto = new Button( );
            btnContactosSinContrato = new Button( );
            lblTituloActualizar = new Label( );
            pnlDispersion = new Panel( );
            label4 = new Label( );
            dtpFEchaDeDispersion = new DateTimePicker( );
            btnEnviarCorreos = new Button( );
            lblTituloDispersion = new Label( );
            panel2 = new Panel( );
            dgvContactos = new DataGridView( );
            btnCerrar = new Button( );
            panel1.SuspendLayout( );
            pnlActualizaContratos.SuspendLayout( );
            pnlDispersion.SuspendLayout( );
            panel2.SuspendLayout( );
            ((System.ComponentModel.ISupportInitialize) dgvContactos).BeginInit( );
            SuspendLayout( );
            // 
            // panel1
            // 
            panel1.Controls.Add( pnlActualizaContratos );
            panel1.Controls.Add( pnlDispersion );
            panel1.Location = new Point( 12, 22 );
            panel1.Name = "panel1";
            panel1.Size = new Size( 563, 831 );
            panel1.TabIndex = 1;
            // 
            // pnlActualizaContratos
            // 
            pnlActualizaContratos.BackColor = SystemColors.ButtonFace;
            pnlActualizaContratos.Controls.Add( label3 );
            pnlActualizaContratos.Controls.Add( btnListaContactos );
            pnlActualizaContratos.Controls.Add( label2 );
            pnlActualizaContratos.Controls.Add( label1 );
            pnlActualizaContratos.Controls.Add( btnContratosSinContacto );
            pnlActualizaContratos.Controls.Add( btnContactosSinContrato );
            pnlActualizaContratos.Controls.Add( lblTituloActualizar );
            pnlActualizaContratos.Location = new Point( 25, 326 );
            pnlActualizaContratos.Name = "pnlActualizaContratos";
            pnlActualizaContratos.Size = new Size( 513, 265 );
            pnlActualizaContratos.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = SystemColors.ButtonHighlight;
            label3.BorderStyle = BorderStyle.Fixed3D;
            label3.Font = new Font( "Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            label3.Location = new Point( 46, 200 );
            label3.Name = "label3";
            label3.Size = new Size( 81, 42 );
            label3.TabIndex = 7;
            label3.Text = "Todos los \r\nContactos\r\n";
            label3.TextAlign = ContentAlignment.TopCenter;
            // 
            // btnListaContactos
            // 
            btnListaContactos.BackColor = SystemColors.Highlight;
            btnListaContactos.Location = new Point( 30, 77 );
            btnListaContactos.Name = "btnListaContactos";
            btnListaContactos.Size = new Size( 120, 120 );
            btnListaContactos.TabIndex = 6;
            btnListaContactos.Text = "Contactos sin contrato";
            btnListaContactos.UseVisualStyleBackColor = false;
            btnListaContactos.Click += btnListaContactos_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = SystemColors.ButtonHighlight;
            label2.BorderStyle = BorderStyle.Fixed3D;
            label2.Font = new Font( "Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            label2.Location = new Point( 332, 200 );
            label2.Name = "label2";
            label2.Size = new Size( 108, 42 );
            label2.TabIndex = 5;
            label2.Text = "Contratos sin \r\nContactos";
            label2.TextAlign = ContentAlignment.TopCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.ButtonHighlight;
            label1.BorderStyle = BorderStyle.Fixed3D;
            label1.Font = new Font( "Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            label1.Location = new Point( 173, 200 );
            label1.Name = "label1";
            label1.Size = new Size( 109, 42 );
            label1.TabIndex = 4;
            label1.Text = "Contactos sin \r\nContratos";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // btnContratosSinContacto
            // 
            btnContratosSinContacto.BackColor = SystemColors.Highlight;
            btnContratosSinContacto.Location = new Point( 327, 77 );
            btnContratosSinContacto.Name = "btnContratosSinContacto";
            btnContratosSinContacto.Size = new Size( 120, 120 );
            btnContratosSinContacto.TabIndex = 3;
            btnContratosSinContacto.Text = "Contratos sin contacto";
            btnContratosSinContacto.UseVisualStyleBackColor = false;
            btnContratosSinContacto.Click += btnContratosSinContacto_Click;
            // 
            // btnContactosSinContrato
            // 
            btnContactosSinContrato.BackColor = SystemColors.Highlight;
            btnContactosSinContrato.Location = new Point( 173, 77 );
            btnContactosSinContrato.Name = "btnContactosSinContrato";
            btnContactosSinContrato.Size = new Size( 120, 120 );
            btnContactosSinContrato.TabIndex = 2;
            btnContactosSinContrato.Text = "Contactos sin contrato";
            btnContactosSinContrato.UseVisualStyleBackColor = false;
            btnContactosSinContrato.Click += btnContactosSinContrato_Click;
            // 
            // lblTituloActualizar
            // 
            lblTituloActualizar.AutoSize = true;
            lblTituloActualizar.Font = new Font( "Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            lblTituloActualizar.Location = new Point( 160, 15 );
            lblTituloActualizar.Name = "lblTituloActualizar";
            lblTituloActualizar.Size = new Size( 173, 37 );
            lblTituloActualizar.TabIndex = 1;
            lblTituloActualizar.Text = "Conciliación";
            // 
            // pnlDispersion
            // 
            pnlDispersion.BackColor = SystemColors.Control;
            pnlDispersion.Controls.Add( label4 );
            pnlDispersion.Controls.Add( dtpFEchaDeDispersion );
            pnlDispersion.Controls.Add( btnEnviarCorreos );
            pnlDispersion.Controls.Add( lblTituloDispersion );
            pnlDispersion.Location = new Point( 25, 17 );
            pnlDispersion.Name = "pnlDispersion";
            pnlDispersion.Size = new Size( 514, 292 );
            pnlDispersion.TabIndex = 0;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font( "Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            label4.Location = new Point( 31, 84 );
            label4.Name = "label4";
            label4.Size = new Size( 62, 25 );
            label4.TabIndex = 3;
            label4.Text = "Fecha";
            // 
            // dtpFEchaDeDispersion
            // 
            dtpFEchaDeDispersion.Format = DateTimePickerFormat.Short;
            dtpFEchaDeDispersion.Location = new Point( 107, 84 );
            dtpFEchaDeDispersion.Name = "dtpFEchaDeDispersion";
            dtpFEchaDeDispersion.Size = new Size( 133, 27 );
            dtpFEchaDeDispersion.TabIndex = 2;
            dtpFEchaDeDispersion.ValueChanged += dtpFEchaDeDispersion_ValueChanged;
            // 
            // btnEnviarCorreos
            // 
            btnEnviarCorreos.BackColor = SystemColors.Highlight;
            btnEnviarCorreos.Location = new Point( 30, 133 );
            btnEnviarCorreos.Name = "btnEnviarCorreos";
            btnEnviarCorreos.Size = new Size( 456, 135 );
            btnEnviarCorreos.TabIndex = 1;
            btnEnviarCorreos.Text = "Enviar Correos Masivos";
            btnEnviarCorreos.UseVisualStyleBackColor = false;
            btnEnviarCorreos.Click += btnEnviarCorreos_Click;
            // 
            // lblTituloDispersion
            // 
            lblTituloDispersion.AutoSize = true;
            lblTituloDispersion.Font = new Font( "Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            lblTituloDispersion.Location = new Point( 161, 14 );
            lblTituloDispersion.Name = "lblTituloDispersion";
            lblTituloDispersion.Size = new Size( 153, 37 );
            lblTituloDispersion.TabIndex = 0;
            lblTituloDispersion.Text = "Dispersión";
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.ActiveCaption;
            panel2.Controls.Add( dgvContactos );
            panel2.Location = new Point( 600, 22 );
            panel2.Name = "panel2";
            panel2.Size = new Size( 746, 831 );
            panel2.TabIndex = 2;
            // 
            // dgvContactos
            // 
            dgvContactos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvContactos.Location = new Point( 25, 17 );
            dgvContactos.Name = "dgvContactos";
            dgvContactos.RowHeadersWidth = 51;
            dgvContactos.Size = new Size( 696, 798 );
            dgvContactos.TabIndex = 0;
            // 
            // btnCerrar
            // 
            btnCerrar.Location = new Point( 1236, 861 );
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size( 110, 29 );
            btnCerrar.TabIndex = 3;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = true;
            btnCerrar.Click += btnCerrar_Click;
            // 
            // FormDispersion
            // 
            AutoScaleDimensions = new SizeF( 8F, 20F );
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientActiveCaption;
            ClientSize = new Size( 1358, 918 );
            Controls.Add( btnCerrar );
            Controls.Add( panel2 );
            Controls.Add( panel1 );
            Name = "FormDispersion";
            Text = "Dispersion";
            Load += FormDispersion_Load;
            panel1.ResumeLayout( false );
            pnlActualizaContratos.ResumeLayout( false );
            pnlActualizaContratos.PerformLayout( );
            pnlDispersion.ResumeLayout( false );
            pnlDispersion.PerformLayout( );
            panel2.ResumeLayout( false );
            ((System.ComponentModel.ISupportInitialize) dgvContactos).EndInit( );
            ResumeLayout( false );
        }

        #endregion

        private Panel panel1;
        private Panel pnlActualizaContratos;
        private Button btnSeleccionarArchivosContactos;
        private TextBox tbRutaArchivoContacots;
        private Button btnContactosSinContrato;
        private Label lblTituloActualizar;
        private Panel pnlDispersion;
        private Button btnEnviarCorreos;
        private Label lblTituloDispersion;
        private Panel panel2;
        private DataGridView dgvContactos;
        private Button btnContratosSinContacto;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button btnListaContactos;
        private DateTimePicker dtpFEchaDeDispersion;
        private Label label4;
        private Button btnCerrar;
    }
}