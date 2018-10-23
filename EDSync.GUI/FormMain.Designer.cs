namespace EDSMSimpleSync
{
    partial class FormMain
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.rtbLogs = new System.Windows.Forms.RichTextBox();
            this.panelSettings = new System.Windows.Forms.Panel();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.tbDirectory = new System.Windows.Forms.TextBox();
            this.tbApiKey = new System.Windows.Forms.TextBox();
            this.tbCmdr = new System.Windows.Forms.TextBox();
            this.folderBrowserDialogJournal = new System.Windows.Forms.FolderBrowserDialog();
            this.splitContainerSync = new System.Windows.Forms.SplitContainer();
            this.tabControlApp = new System.Windows.Forms.TabControl();
            this.tabSynchro = new System.Windows.Forms.TabPage();
            this.tabCurrentSystem = new System.Windows.Forms.TabPage();
            this.tbCurrentStation = new System.Windows.Forms.TextBox();
            this.tbCurrentSystem = new System.Windows.Forms.TextBox();
            this.labelCurrentSystem = new System.Windows.Forms.Label();
            this.tpSettings = new System.Windows.Forms.TabPage();
            this.gbSettingsInara = new System.Windows.Forms.GroupBox();
            this.tbInaraCmdr = new System.Windows.Forms.TextBox();
            this.tbInaraApiKey = new System.Windows.Forms.TextBox();
            this.gbSettingsEDSM = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.panelSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerSync)).BeginInit();
            this.splitContainerSync.Panel1.SuspendLayout();
            this.splitContainerSync.Panel2.SuspendLayout();
            this.splitContainerSync.SuspendLayout();
            this.tabControlApp.SuspendLayout();
            this.tabSynchro.SuspendLayout();
            this.tabCurrentSystem.SuspendLayout();
            this.tpSettings.SuspendLayout();
            this.gbSettingsInara.SuspendLayout();
            this.gbSettingsEDSM.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbLogs
            // 
            this.rtbLogs.BackColor = System.Drawing.Color.Black;
            this.rtbLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLogs.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLogs.ForeColor = System.Drawing.Color.White;
            this.rtbLogs.Location = new System.Drawing.Point(0, 0);
            this.rtbLogs.Name = "rtbLogs";
            this.rtbLogs.ReadOnly = true;
            this.rtbLogs.Size = new System.Drawing.Size(742, 477);
            this.rtbLogs.TabIndex = 0;
            this.rtbLogs.Text = "EDSM Simple Sync - hello !\n";
            this.rtbLogs.WordWrap = false;
            // 
            // panelSettings
            // 
            this.panelSettings.Controls.Add(this.pictureBoxLogo);
            this.panelSettings.Controls.Add(this.label3);
            this.panelSettings.Controls.Add(this.label2);
            this.panelSettings.Controls.Add(this.label1);
            this.panelSettings.Controls.Add(this.btnStop);
            this.panelSettings.Controls.Add(this.btnStart);
            this.panelSettings.Controls.Add(this.btnSelectFolder);
            this.panelSettings.Controls.Add(this.tbDirectory);
            this.panelSettings.Controls.Add(this.tbApiKey);
            this.panelSettings.Controls.Add(this.tbCmdr);
            this.panelSettings.Location = new System.Drawing.Point(3, 3);
            this.panelSettings.Name = "panelSettings";
            this.panelSettings.Size = new System.Drawing.Size(726, 104);
            this.panelSettings.TabIndex = 1;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = global::EDSMSimpleSync.Properties.Resources.elite_dangerous_minimalistic;
            this.pictureBoxLogo.Location = new System.Drawing.Point(3, 9);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(81, 75);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLogo.TabIndex = 9;
            this.pictureBoxLogo.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(90, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "Journal";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(90, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "Api Key";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(90, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "Name";
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(464, 9);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(380, 8);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Location = new System.Drawing.Point(464, 65);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(75, 23);
            this.btnSelectFolder.TabIndex = 3;
            this.btnSelectFolder.Text = "Select";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // tbDirectory
            // 
            this.tbDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbDirectory.Location = new System.Drawing.Point(145, 65);
            this.tbDirectory.Name = "tbDirectory";
            this.tbDirectory.Size = new System.Drawing.Size(310, 22);
            this.tbDirectory.TabIndex = 2;
            // 
            // tbApiKey
            // 
            this.tbApiKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbApiKey.Location = new System.Drawing.Point(145, 37);
            this.tbApiKey.Name = "tbApiKey";
            this.tbApiKey.Size = new System.Drawing.Size(394, 22);
            this.tbApiKey.TabIndex = 1;
            // 
            // tbCmdr
            // 
            this.tbCmdr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCmdr.Location = new System.Drawing.Point(145, 9);
            this.tbCmdr.Name = "tbCmdr";
            this.tbCmdr.Size = new System.Drawing.Size(229, 22);
            this.tbCmdr.TabIndex = 0;
            // 
            // splitContainerSync
            // 
            this.splitContainerSync.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerSync.Location = new System.Drawing.Point(3, 3);
            this.splitContainerSync.Name = "splitContainerSync";
            this.splitContainerSync.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerSync.Panel1
            // 
            this.splitContainerSync.Panel1.Controls.Add(this.panelSettings);
            this.splitContainerSync.Panel1MinSize = 120;
            // 
            // splitContainerSync.Panel2
            // 
            this.splitContainerSync.Panel2.Controls.Add(this.rtbLogs);
            this.splitContainerSync.Panel2MinSize = 0;
            this.splitContainerSync.Size = new System.Drawing.Size(742, 601);
            this.splitContainerSync.SplitterDistance = 120;
            this.splitContainerSync.TabIndex = 2;
            // 
            // tabControlApp
            // 
            this.tabControlApp.Controls.Add(this.tabSynchro);
            this.tabControlApp.Controls.Add(this.tabCurrentSystem);
            this.tabControlApp.Controls.Add(this.tpSettings);
            this.tabControlApp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlApp.Location = new System.Drawing.Point(0, 0);
            this.tabControlApp.Name = "tabControlApp";
            this.tabControlApp.SelectedIndex = 0;
            this.tabControlApp.Size = new System.Drawing.Size(756, 633);
            this.tabControlApp.TabIndex = 3;
            // 
            // tabSynchro
            // 
            this.tabSynchro.Controls.Add(this.splitContainerSync);
            this.tabSynchro.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabSynchro.Location = new System.Drawing.Point(4, 22);
            this.tabSynchro.Name = "tabSynchro";
            this.tabSynchro.Padding = new System.Windows.Forms.Padding(3);
            this.tabSynchro.Size = new System.Drawing.Size(748, 607);
            this.tabSynchro.TabIndex = 0;
            this.tabSynchro.Text = "Sync";
            this.tabSynchro.UseVisualStyleBackColor = true;
            // 
            // tabCurrentSystem
            // 
            this.tabCurrentSystem.Controls.Add(this.tbCurrentStation);
            this.tabCurrentSystem.Controls.Add(this.tbCurrentSystem);
            this.tabCurrentSystem.Controls.Add(this.labelCurrentSystem);
            this.tabCurrentSystem.Location = new System.Drawing.Point(4, 22);
            this.tabCurrentSystem.Name = "tabCurrentSystem";
            this.tabCurrentSystem.Padding = new System.Windows.Forms.Padding(3);
            this.tabCurrentSystem.Size = new System.Drawing.Size(748, 607);
            this.tabCurrentSystem.TabIndex = 1;
            this.tabCurrentSystem.Text = "System";
            this.tabCurrentSystem.UseVisualStyleBackColor = true;
            // 
            // tbCurrentStation
            // 
            this.tbCurrentStation.Location = new System.Drawing.Point(109, 45);
            this.tbCurrentStation.Name = "tbCurrentStation";
            this.tbCurrentStation.ReadOnly = true;
            this.tbCurrentStation.Size = new System.Drawing.Size(260, 20);
            this.tbCurrentStation.TabIndex = 2;
            // 
            // tbCurrentSystem
            // 
            this.tbCurrentSystem.Location = new System.Drawing.Point(109, 18);
            this.tbCurrentSystem.Name = "tbCurrentSystem";
            this.tbCurrentSystem.ReadOnly = true;
            this.tbCurrentSystem.Size = new System.Drawing.Size(260, 20);
            this.tbCurrentSystem.TabIndex = 1;
            // 
            // labelCurrentSystem
            // 
            this.labelCurrentSystem.AutoSize = true;
            this.labelCurrentSystem.Location = new System.Drawing.Point(24, 26);
            this.labelCurrentSystem.Name = "labelCurrentSystem";
            this.labelCurrentSystem.Size = new System.Drawing.Size(78, 13);
            this.labelCurrentSystem.TabIndex = 0;
            this.labelCurrentSystem.Text = "Current System";
            // 
            // tpSettings
            // 
            this.tpSettings.Controls.Add(this.gbSettingsEDSM);
            this.tpSettings.Controls.Add(this.gbSettingsInara);
            this.tpSettings.Location = new System.Drawing.Point(4, 22);
            this.tpSettings.Name = "tpSettings";
            this.tpSettings.Size = new System.Drawing.Size(748, 607);
            this.tpSettings.TabIndex = 2;
            this.tpSettings.Text = "Settings";
            this.tpSettings.UseVisualStyleBackColor = true;
            // 
            // gbSettingsInara
            // 
            this.gbSettingsInara.Controls.Add(this.tbInaraApiKey);
            this.gbSettingsInara.Controls.Add(this.tbInaraCmdr);
            this.gbSettingsInara.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbSettingsInara.Location = new System.Drawing.Point(8, 134);
            this.gbSettingsInara.Name = "gbSettingsInara";
            this.gbSettingsInara.Size = new System.Drawing.Size(539, 100);
            this.gbSettingsInara.TabIndex = 1;
            this.gbSettingsInara.TabStop = false;
            this.gbSettingsInara.Text = "Inara";
            // 
            // tbInaraCmdr
            // 
            this.tbInaraCmdr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbInaraCmdr.Location = new System.Drawing.Point(98, 19);
            this.tbInaraCmdr.Name = "tbInaraCmdr";
            this.tbInaraCmdr.Size = new System.Drawing.Size(420, 22);
            this.tbInaraCmdr.TabIndex = 0;
            // 
            // tbInaraApiKey
            // 
            this.tbInaraApiKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbInaraApiKey.Location = new System.Drawing.Point(98, 56);
            this.tbInaraApiKey.Name = "tbInaraApiKey";
            this.tbInaraApiKey.Size = new System.Drawing.Size(420, 22);
            this.tbInaraApiKey.TabIndex = 1;
            // 
            // gbSettingsEDSM
            // 
            this.gbSettingsEDSM.Controls.Add(this.textBox1);
            this.gbSettingsEDSM.Controls.Add(this.textBox2);
            this.gbSettingsEDSM.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbSettingsEDSM.Location = new System.Drawing.Point(8, 16);
            this.gbSettingsEDSM.Name = "gbSettingsEDSM";
            this.gbSettingsEDSM.Size = new System.Drawing.Size(539, 100);
            this.gbSettingsEDSM.TabIndex = 2;
            this.gbSettingsEDSM.TabStop = false;
            this.gbSettingsEDSM.Text = "EDSM";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(98, 56);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(420, 22);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(98, 19);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(420, 22);
            this.textBox2.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 633);
            this.Controls.Add(this.tabControlApp);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "EDSM Simple Sync";
            this.panelSettings.ResumeLayout(false);
            this.panelSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.splitContainerSync.Panel1.ResumeLayout(false);
            this.splitContainerSync.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerSync)).EndInit();
            this.splitContainerSync.ResumeLayout(false);
            this.tabControlApp.ResumeLayout(false);
            this.tabSynchro.ResumeLayout(false);
            this.tabCurrentSystem.ResumeLayout(false);
            this.tabCurrentSystem.PerformLayout();
            this.tpSettings.ResumeLayout(false);
            this.gbSettingsInara.ResumeLayout(false);
            this.gbSettingsInara.PerformLayout();
            this.gbSettingsEDSM.ResumeLayout(false);
            this.gbSettingsEDSM.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbLogs;
        private System.Windows.Forms.Panel panelSettings;
        private System.Windows.Forms.TextBox tbDirectory;
        private System.Windows.Forms.TextBox tbApiKey;
        private System.Windows.Forms.TextBox tbCmdr;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogJournal;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.SplitContainer splitContainerSync;
        private System.Windows.Forms.TabControl tabControlApp;
        private System.Windows.Forms.TabPage tabSynchro;
        private System.Windows.Forms.TabPage tabCurrentSystem;
        private System.Windows.Forms.Label labelCurrentSystem;
        private System.Windows.Forms.TextBox tbCurrentSystem;
        private System.Windows.Forms.TextBox tbCurrentStation;
        private System.Windows.Forms.TabPage tpSettings;
        private System.Windows.Forms.GroupBox gbSettingsInara;
        private System.Windows.Forms.TextBox tbInaraApiKey;
        private System.Windows.Forms.TextBox tbInaraCmdr;
        private System.Windows.Forms.GroupBox gbSettingsEDSM;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
    }
}

