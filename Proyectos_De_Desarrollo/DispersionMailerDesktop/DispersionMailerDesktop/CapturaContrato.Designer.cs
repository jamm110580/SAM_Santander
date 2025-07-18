namespace DispersionMailerDesktop
{
    partial class CapturaContrato
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
            lblTituloDispersion = new Label( );
            panel1 = new Panel( );
            pnlActualizaContratos = new Panel( );
            txtFechaSolicitud = new DateTimePicker( );
            txtFechaAplicacion = new DateTimePicker( );
            lblFechaSolicitud = new Label( );
            lblArchivoCargado = new Label( );
            btnSubirArchivo = new Button( );
            lblMontoLetra = new Label( );
            btnGuardar = new Button( );
            txtMontoEnLetra = new TextBox( );
            lblMonto = new Label( );
            txtMontoDelContrato = new TextBox( );
            lblNombreCliente = new Label( );
            txtNombreCliente = new TextBox( );
            lblFechaAplicacion = new Label( );
            lblContrato = new Label( );
            txtNumContrato = new TextBox( );
            lblTituloActualizar = new Label( );
            panel3 = new Panel( );
            dgvContratos = new DataGridView( );
            panel2 = new Panel( );
            pbFlechaDos = new PictureBox( );
            pbFlechaUno = new PictureBox( );
            imgUno = new PictureBox( );
            imgTres = new PictureBox( );
            imgDos = new PictureBox( );
            ofdContratoLayout = new OpenFileDialog( );
            btnCerrar = new Button( );
            label1 = new Label( );
            lblUsuario = new Label( );
            panel1.SuspendLayout( );
            pnlActualizaContratos.SuspendLayout( );
            panel3.SuspendLayout( );
            ((System.ComponentModel.ISupportInitialize) dgvContratos).BeginInit( );
            panel2.SuspendLayout( );
            ((System.ComponentModel.ISupportInitialize) pbFlechaDos).BeginInit( );
            ((System.ComponentModel.ISupportInitialize) pbFlechaUno).BeginInit( );
            ((System.ComponentModel.ISupportInitialize) imgUno).BeginInit( );
            ((System.ComponentModel.ISupportInitialize) imgTres).BeginInit( );
            ((System.ComponentModel.ISupportInitialize) imgDos).BeginInit( );
            SuspendLayout( );
            // 
            // lblTituloDispersion
            // 
            lblTituloDispersion.AutoSize = true;
            lblTituloDispersion.Font = new Font( "Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            lblTituloDispersion.Location = new Point( 6, 9 );
            lblTituloDispersion.Name = "lblTituloDispersion";
            lblTituloDispersion.Size = new Size( 397, 37 );
            lblTituloDispersion.TabIndex = 3;
            lblTituloDispersion.Text = "Flujo de Liberación de Capital";
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ActiveCaption;
            panel1.Controls.Add( pnlActualizaContratos );
            panel1.Location = new Point( 6, 182 );
            panel1.Name = "panel1";
            panel1.Size = new Size( 563, 665 );
            panel1.TabIndex = 11;
            // 
            // pnlActualizaContratos
            // 
            pnlActualizaContratos.BackColor = SystemColors.ButtonFace;
            pnlActualizaContratos.Controls.Add( txtFechaSolicitud );
            pnlActualizaContratos.Controls.Add( txtFechaAplicacion );
            pnlActualizaContratos.Controls.Add( lblFechaSolicitud );
            pnlActualizaContratos.Controls.Add( lblArchivoCargado );
            pnlActualizaContratos.Controls.Add( btnSubirArchivo );
            pnlActualizaContratos.Controls.Add( lblMontoLetra );
            pnlActualizaContratos.Controls.Add( btnGuardar );
            pnlActualizaContratos.Controls.Add( txtMontoEnLetra );
            pnlActualizaContratos.Controls.Add( lblMonto );
            pnlActualizaContratos.Controls.Add( txtMontoDelContrato );
            pnlActualizaContratos.Controls.Add( lblNombreCliente );
            pnlActualizaContratos.Controls.Add( txtNombreCliente );
            pnlActualizaContratos.Controls.Add( lblFechaAplicacion );
            pnlActualizaContratos.Controls.Add( lblContrato );
            pnlActualizaContratos.Controls.Add( txtNumContrato );
            pnlActualizaContratos.Controls.Add( lblTituloActualizar );
            pnlActualizaContratos.Location = new Point( 27, 26 );
            pnlActualizaContratos.Name = "pnlActualizaContratos";
            pnlActualizaContratos.Size = new Size( 513, 618 );
            pnlActualizaContratos.TabIndex = 1;
            // 
            // txtFechaSolicitud
            // 
            txtFechaSolicitud.Location = new Point( 281, 156 );
            txtFechaSolicitud.Name = "txtFechaSolicitud";
            txtFechaSolicitud.Size = new Size( 200, 27 );
            txtFechaSolicitud.TabIndex = 20;
            // 
            // txtFechaAplicacion
            // 
            txtFechaAplicacion.Location = new Point( 32, 156 );
            txtFechaAplicacion.Name = "txtFechaAplicacion";
            txtFechaAplicacion.Size = new Size( 206, 27 );
            txtFechaAplicacion.TabIndex = 19;
            // 
            // lblFechaSolicitud
            // 
            lblFechaSolicitud.AutoSize = true;
            lblFechaSolicitud.Font = new Font( "Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            lblFechaSolicitud.Location = new Point( 281, 119 );
            lblFechaSolicitud.Name = "lblFechaSolicitud";
            lblFechaSolicitud.Size = new Size( 182, 28 );
            lblFechaSolicitud.TabIndex = 18;
            lblFechaSolicitud.Text = "Fecha de solicitud";
            // 
            // lblArchivoCargado
            // 
            lblArchivoCargado.AutoSize = true;
            lblArchivoCargado.Font = new Font( "Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            lblArchivoCargado.Location = new Point( 30, 535 );
            lblArchivoCargado.Name = "lblArchivoCargado";
            lblArchivoCargado.Size = new Size( 332, 28 );
            lblArchivoCargado.TabIndex = 16;
            lblArchivoCargado.Text = "Archivo cargado correctamente ...";
            lblArchivoCargado.Visible = false;
            // 
            // btnSubirArchivo
            // 
            btnSubirArchivo.Enabled = false;
            btnSubirArchivo.Location = new Point( 30, 498 );
            btnSubirArchivo.Name = "btnSubirArchivo";
            btnSubirArchivo.Size = new Size( 208, 29 );
            btnSubirArchivo.TabIndex = 15;
            btnSubirArchivo.Text = "Subir Archivo Layout";
            btnSubirArchivo.UseVisualStyleBackColor = true;
            btnSubirArchivo.Click += btnSubirArchivo_Click;
            // 
            // lblMontoLetra
            // 
            lblMontoLetra.AutoSize = true;
            lblMontoLetra.Font = new Font( "Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            lblMontoLetra.Location = new Point( 25, 330 );
            lblMontoLetra.Name = "lblMontoLetra";
            lblMontoLetra.Size = new Size( 154, 28 );
            lblMontoLetra.TabIndex = 14;
            lblMontoLetra.Text = "Monto en letra";
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = SystemColors.Highlight;
            btnGuardar.Location = new Point( 25, 410 );
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size( 456, 68 );
            btnGuardar.TabIndex = 2;
            btnGuardar.Text = "Actualizar Lista de Contactos";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // txtMontoEnLetra
            // 
            txtMontoEnLetra.BackColor = SystemColors.ActiveCaption;
            txtMontoEnLetra.BorderStyle = BorderStyle.FixedSingle;
            txtMontoEnLetra.ForeColor = SystemColors.HotTrack;
            txtMontoEnLetra.Location = new Point( 31, 361 );
            txtMontoEnLetra.Name = "txtMontoEnLetra";
            txtMontoEnLetra.Size = new Size( 456, 27 );
            txtMontoEnLetra.TabIndex = 13;
            // 
            // lblMonto
            // 
            lblMonto.AutoSize = true;
            lblMonto.Font = new Font( "Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            lblMonto.Location = new Point( 28, 262 );
            lblMonto.Name = "lblMonto";
            lblMonto.Size = new Size( 197, 28 );
            lblMonto.TabIndex = 12;
            lblMonto.Text = "Monto del contrato";
            // 
            // txtMontoDelContrato
            // 
            txtMontoDelContrato.BackColor = SystemColors.ActiveCaption;
            txtMontoDelContrato.BorderStyle = BorderStyle.FixedSingle;
            txtMontoDelContrato.ForeColor = SystemColors.HotTrack;
            txtMontoDelContrato.Location = new Point( 31, 292 );
            txtMontoDelContrato.Name = "txtMontoDelContrato";
            txtMontoDelContrato.Size = new Size( 456, 27 );
            txtMontoDelContrato.TabIndex = 11;
            txtMontoDelContrato.Enter += txtMontoDelContrato_Enter;
            txtMontoDelContrato.Leave += txtMontoDelContrato_Leave;
            // 
            // lblNombreCliente
            // 
            lblNombreCliente.AutoSize = true;
            lblNombreCliente.Font = new Font( "Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            lblNombreCliente.Location = new Point( 28, 195 );
            lblNombreCliente.Name = "lblNombreCliente";
            lblNombreCliente.Size = new Size( 194, 28 );
            lblNombreCliente.TabIndex = 10;
            lblNombreCliente.Text = "Nombre del cliente";
            // 
            // txtNombreCliente
            // 
            txtNombreCliente.BackColor = SystemColors.ActiveCaption;
            txtNombreCliente.BorderStyle = BorderStyle.FixedSingle;
            txtNombreCliente.ForeColor = SystemColors.HotTrack;
            txtNombreCliente.Location = new Point( 31, 226 );
            txtNombreCliente.Name = "txtNombreCliente";
            txtNombreCliente.Size = new Size( 456, 27 );
            txtNombreCliente.TabIndex = 9;
            // 
            // lblFechaAplicacion
            // 
            lblFechaAplicacion.AutoSize = true;
            lblFechaAplicacion.Font = new Font( "Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            lblFechaAplicacion.Location = new Point( 28, 119 );
            lblFechaAplicacion.Name = "lblFechaAplicacion";
            lblFechaAplicacion.Size = new Size( 197, 28 );
            lblFechaAplicacion.TabIndex = 8;
            lblFechaAplicacion.Text = "Fecha de aplicación";
            // 
            // lblContrato
            // 
            lblContrato.AutoSize = true;
            lblContrato.Font = new Font( "Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            lblContrato.Location = new Point( 27, 52 );
            lblContrato.Name = "lblContrato";
            lblContrato.Size = new Size( 205, 28 );
            lblContrato.TabIndex = 6;
            lblContrato.Text = "Numero de contrato";
            // 
            // txtNumContrato
            // 
            txtNumContrato.BackColor = SystemColors.ActiveCaption;
            txtNumContrato.BorderStyle = BorderStyle.FixedSingle;
            txtNumContrato.ForeColor = SystemColors.HotTrack;
            txtNumContrato.Location = new Point( 30, 83 );
            txtNumContrato.Name = "txtNumContrato";
            txtNumContrato.Size = new Size( 456, 27 );
            txtNumContrato.TabIndex = 5;
            txtNumContrato.Leave += txtNumContrato_Leave;
            // 
            // lblTituloActualizar
            // 
            lblTituloActualizar.AutoSize = true;
            lblTituloActualizar.Font = new Font( "Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            lblTituloActualizar.Location = new Point( 135, 7 );
            lblTituloActualizar.Name = "lblTituloActualizar";
            lblTituloActualizar.Size = new Size( 266, 35 );
            lblTituloActualizar.TabIndex = 1;
            lblTituloActualizar.Text = "Registro de contratos";
            // 
            // panel3
            // 
            panel3.BackColor = SystemColors.ActiveCaption;
            panel3.Controls.Add( dgvContratos );
            panel3.Location = new Point( 600, 57 );
            panel3.Name = "panel3";
            panel3.Size = new Size( 746, 790 );
            panel3.TabIndex = 12;
            // 
            // dgvContratos
            // 
            dgvContratos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvContratos.Location = new Point( 25, 26 );
            dgvContratos.Name = "dgvContratos";
            dgvContratos.RowHeadersWidth = 51;
            dgvContratos.Size = new Size( 696, 743 );
            dgvContratos.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.ActiveCaption;
            panel2.Controls.Add( pbFlechaDos );
            panel2.Controls.Add( pbFlechaUno );
            panel2.Controls.Add( imgUno );
            panel2.Controls.Add( imgTres );
            panel2.Controls.Add( imgDos );
            panel2.Location = new Point( 6, 57 );
            panel2.Name = "panel2";
            panel2.Size = new Size( 563, 119 );
            panel2.TabIndex = 13;
            // 
            // pbFlechaDos
            // 
            pbFlechaDos.Location = new Point( 329, 33 );
            pbFlechaDos.Name = "pbFlechaDos";
            pbFlechaDos.Size = new Size( 100, 62 );
            pbFlechaDos.TabIndex = 18;
            pbFlechaDos.TabStop = false;
            // 
            // pbFlechaUno
            // 
            pbFlechaUno.Location = new Point( 103, 33 );
            pbFlechaUno.Name = "pbFlechaUno";
            pbFlechaUno.Size = new Size( 100, 62 );
            pbFlechaUno.TabIndex = 17;
            pbFlechaUno.TabStop = false;
            // 
            // imgUno
            // 
            imgUno.InitialImage = Properties.Resources.UnoRojo;
            imgUno.Location = new Point( 15, 33 );
            imgUno.Name = "imgUno";
            imgUno.Size = new Size( 82, 60 );
            imgUno.TabIndex = 16;
            imgUno.TabStop = false;
            // 
            // imgTres
            // 
            imgTres.InitialImage = Properties.Resources.UnoRojo;
            imgTres.Location = new Point( 463, 33 );
            imgTres.Name = "imgTres";
            imgTres.Size = new Size( 88, 60 );
            imgTres.TabIndex = 15;
            imgTres.TabStop = false;
            // 
            // imgDos
            // 
            imgDos.InitialImage = Properties.Resources.UnoRojo;
            imgDos.Location = new Point( 232, 33 );
            imgDos.Name = "imgDos";
            imgDos.Size = new Size( 91, 60 );
            imgDos.TabIndex = 14;
            imgDos.TabStop = false;
            // 
            // ofdContratoLayout
            // 
            ofdContratoLayout.FileName = "LayOutContrato";
            // 
            // btnCerrar
            // 
            btnCerrar.Location = new Point( 1236, 862 );
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size( 110, 29 );
            btnCerrar.TabIndex = 14;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = true;
            btnCerrar.Click += btnCerrar_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font( "Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            label1.Location = new Point( 1133, 16 );
            label1.Name = "label1";
            label1.Size = new Size( 89, 28 );
            label1.TabIndex = 19;
            label1.Text = "Usuario:";
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font( "Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point,  0 );
            lblUsuario.Location = new Point( 1241, 16 );
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size( 52, 28 );
            lblUsuario.TabIndex = 20;
            lblUsuario.Text = "user";
            // 
            // CapturaContrato
            // 
            AutoScaleDimensions = new SizeF( 8F, 20F );
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientActiveCaption;
            ClientSize = new Size( 1358, 910 );
            Controls.Add( lblUsuario );
            Controls.Add( label1 );
            Controls.Add( btnCerrar );
            Controls.Add( panel2 );
            Controls.Add( panel3 );
            Controls.Add( panel1 );
            Controls.Add( lblTituloDispersion );
            FormBorderStyle = FormBorderStyle.None;
            Name = "CapturaContrato";
            Text = "CapturaContrato";
            Load += CapturaContrato_Load;
            panel1.ResumeLayout( false );
            pnlActualizaContratos.ResumeLayout( false );
            pnlActualizaContratos.PerformLayout( );
            panel3.ResumeLayout( false );
            ((System.ComponentModel.ISupportInitialize) dgvContratos).EndInit( );
            panel2.ResumeLayout( false );
            ((System.ComponentModel.ISupportInitialize) pbFlechaDos).EndInit( );
            ((System.ComponentModel.ISupportInitialize) pbFlechaUno).EndInit( );
            ((System.ComponentModel.ISupportInitialize) imgUno).EndInit( );
            ((System.ComponentModel.ISupportInitialize) imgTres).EndInit( );
            ((System.ComponentModel.ISupportInitialize) imgDos).EndInit( );
            ResumeLayout( false );
            PerformLayout( );
        }

        #endregion
        private Label lblTituloDispersion;
        private PictureBox imgDos;
        private Panel panel1;
        private Panel pnlActualizaContratos;
        private TextBox txtNumContrato;
        private Button btnGuardar;
        private Label lblTituloActualizar;
        private Panel panel3;
        private DataGridView dgvContratos;
        private Label lblContrato;
        private Label lblFechaAplicacion;
        private Label lblNombreCliente;
        private TextBox txtNombreCliente;
        private Label lblMonto;
        private TextBox txtMontoDelContrato;
        private Label lblMontoLetra;
        private TextBox txtMontoEnLetra;
        private Panel panel2;
        private PictureBox imgTres;
        private Button btnSubirArchivo;
        private OpenFileDialog ofdContratoLayout;
        private Label lblArchivoCargado;
        private Label lblFechaSolicitud;
        private PictureBox imgUno;
        private Button btnCerrar;
        private Label label1;
        private Label lblUsuario;
        private DateTimePicker txtFechaAplicacion;
        private DateTimePicker txtFechaSolicitud;
        private PictureBox pbFlechaDos;
        private PictureBox pbFlechaUno;
    }
}