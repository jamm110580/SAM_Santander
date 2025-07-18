namespace DispersionMailerDesktop
{
    partial class Menu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( Menu ) );
            menuStrip1 = new MenuStrip( );
            inicioToolStripMenuItem = new ToolStripMenuItem( );
            dispersiónToolStripMenuItem = new ToolStripMenuItem( );
            configuraciónToolStripMenuItem = new ToolStripMenuItem( );
            liberaciónDeCapitalToolStripMenuItem = new ToolStripMenuItem( );
            capturaDeContratoToolStripMenuItem = new ToolStripMenuItem( );
            configuraciónToolStripMenuItem1 = new ToolStripMenuItem( );
            panelContenedor = new Panel( );
            menuStrip1.SuspendLayout( );
            SuspendLayout( );
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size( 20, 20 );
            menuStrip1.Items.AddRange( new ToolStripItem[ ] { inicioToolStripMenuItem, dispersiónToolStripMenuItem, configuraciónToolStripMenuItem, liberaciónDeCapitalToolStripMenuItem } );
            menuStrip1.Location = new Point( 0, 0 );
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size( 1382, 28 );
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // inicioToolStripMenuItem
            // 
            inicioToolStripMenuItem.Name = "inicioToolStripMenuItem";
            inicioToolStripMenuItem.Size = new Size( 59, 24 );
            inicioToolStripMenuItem.Text = "Inicio";
            inicioToolStripMenuItem.Click += inicioToolStripMenuItem_Click;
            // 
            // dispersiónToolStripMenuItem
            // 
            dispersiónToolStripMenuItem.Name = "dispersiónToolStripMenuItem";
            dispersiónToolStripMenuItem.Size = new Size( 93, 24 );
            dispersiónToolStripMenuItem.Text = "Dispersión";
            dispersiónToolStripMenuItem.Click += dispersiónToolStripMenuItem_Click;
            // 
            // configuraciónToolStripMenuItem
            // 
            configuraciónToolStripMenuItem.Name = "configuraciónToolStripMenuItem";
            configuraciónToolStripMenuItem.Size = new Size( 116, 24 );
            configuraciónToolStripMenuItem.Text = "Configuración";
            configuraciónToolStripMenuItem.Click += configuraciónToolStripMenuItem_Click;
            // 
            // liberaciónDeCapitalToolStripMenuItem
            // 
            liberaciónDeCapitalToolStripMenuItem.DropDownItems.AddRange( new ToolStripItem[ ] { capturaDeContratoToolStripMenuItem, configuraciónToolStripMenuItem1 } );
            liberaciónDeCapitalToolStripMenuItem.Name = "liberaciónDeCapitalToolStripMenuItem";
            liberaciónDeCapitalToolStripMenuItem.Size = new Size( 164, 24 );
            liberaciónDeCapitalToolStripMenuItem.Text = "Liberación de Capital";
            // 
            // capturaDeContratoToolStripMenuItem
            // 
            capturaDeContratoToolStripMenuItem.Name = "capturaDeContratoToolStripMenuItem";
            capturaDeContratoToolStripMenuItem.Size = new Size( 225, 26 );
            capturaDeContratoToolStripMenuItem.Text = "Captura de contrato";
            capturaDeContratoToolStripMenuItem.Click += capturaDeContratoToolStripMenuItem_Click;
            // 
            // configuraciónToolStripMenuItem1
            // 
            configuraciónToolStripMenuItem1.Name = "configuraciónToolStripMenuItem1";
            configuraciónToolStripMenuItem1.Size = new Size( 225, 26 );
            configuraciónToolStripMenuItem1.Text = "Configuración";
            configuraciónToolStripMenuItem1.Click += configuraciónToolStripMenuItem1_Click;
            // 
            // panelContenedor
            // 
            panelContenedor.BackgroundImageLayout = ImageLayout.Stretch;
            panelContenedor.Dock = DockStyle.Fill;
            panelContenedor.Location = new Point( 0, 28 );
            panelContenedor.Name = "panelContenedor";
            panelContenedor.Size = new Size( 1382, 1025 );
            panelContenedor.TabIndex = 2;
            // 
            // Menu
            // 
            AutoScaleDimensions = new SizeF( 8F, 20F );
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size( 1382, 1053 );
            Controls.Add( panelContenedor );
            Controls.Add( menuStrip1 );
            Icon = (Icon) resources.GetObject( "$this.Icon" );
            MainMenuStrip = menuStrip1;
            Name = "Menu";
            Text = "Dispersión Mail";
            WindowState = FormWindowState.Maximized;
            Load += Menu_Load;
            menuStrip1.ResumeLayout( false );
            menuStrip1.PerformLayout( );
            ResumeLayout( false );
            PerformLayout( );
        }

        #endregion
        private MenuStrip menuStrip1;
        private ToolStripMenuItem dispersiónToolStripMenuItem;
        private ToolStripMenuItem configuraciónToolStripMenuItem;
        private Panel panelContenedor;
        private ToolStripMenuItem inicioToolStripMenuItem;
        private ToolStripMenuItem liberaciónDeCapitalToolStripMenuItem;
        private ToolStripMenuItem capturaDeContratoToolStripMenuItem;
        private ToolStripMenuItem configuraciónToolStripMenuItem1;
    }
}