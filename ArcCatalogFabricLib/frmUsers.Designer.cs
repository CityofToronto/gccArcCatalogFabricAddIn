namespace ArcCatalogFabricLib
{
    partial class frmUsers
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnApply = new System.Windows.Forms.Button();
            this.cmdExit = new System.Windows.Forms.Button();
            this.tabCotrol = new System.Windows.Forms.TabControl();
            this.tabPageConnect = new System.Windows.Forms.TabPage();
            this.labelDbStatus = new System.Windows.Forms.Label();
            this.btnTest = new System.Windows.Forms.Button();
            this.lblOwner = new System.Windows.Forms.Label();
            this.txtOwnerPswd = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTNS = new System.Windows.Forms.TextBox();
            this.tabPageUser = new System.Windows.Forms.TabPage();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.chkEnterPswd = new System.Windows.Forms.CheckBox();
            this.lbxUsers = new System.Windows.Forms.ListBox();
            this.tabCotrol.SuspendLayout();
            this.tabPageConnect.SuspendLayout();
            this.tabPageUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(168, 313);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // cmdExit
            // 
            this.cmdExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdExit.Location = new System.Drawing.Point(43, 313);
            this.cmdExit.Name = "cmdExit";
            this.cmdExit.Size = new System.Drawing.Size(75, 23);
            this.cmdExit.TabIndex = 1;
            this.cmdExit.Text = "Exit";
            this.cmdExit.UseVisualStyleBackColor = true;
            this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
            // 
            // tabCotrol
            // 
            this.tabCotrol.Controls.Add(this.tabPageConnect);
            this.tabCotrol.Controls.Add(this.tabPageUser);
            this.tabCotrol.Location = new System.Drawing.Point(12, 12);
            this.tabCotrol.Name = "tabCotrol";
            this.tabCotrol.SelectedIndex = 0;
            this.tabCotrol.Size = new System.Drawing.Size(267, 280);
            this.tabCotrol.TabIndex = 2;
            // 
            // tabPageConnect
            // 
            this.tabPageConnect.Controls.Add(this.labelDbStatus);
            this.tabPageConnect.Controls.Add(this.btnTest);
            this.tabPageConnect.Controls.Add(this.lblOwner);
            this.tabPageConnect.Controls.Add(this.txtOwnerPswd);
            this.tabPageConnect.Controls.Add(this.label1);
            this.tabPageConnect.Controls.Add(this.txtTNS);
            this.tabPageConnect.Location = new System.Drawing.Point(4, 22);
            this.tabPageConnect.Name = "tabPageConnect";
            this.tabPageConnect.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConnect.Size = new System.Drawing.Size(259, 254);
            this.tabPageConnect.TabIndex = 0;
            this.tabPageConnect.Text = "Db connect";
            this.tabPageConnect.UseVisualStyleBackColor = true;
            // 
            // labelDbStatus
            // 
            this.labelDbStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelDbStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelDbStatus.Enabled = false;
            this.labelDbStatus.Location = new System.Drawing.Point(49, 201);
            this.labelDbStatus.Name = "labelDbStatus";
            this.labelDbStatus.Size = new System.Drawing.Size(160, 32);
            this.labelDbStatus.TabIndex = 4;
            this.labelDbStatus.Text = ".....................................";
            this.labelDbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelDbStatus.Visible = false;
            // 
            // btnTest
            // 
            this.btnTest.Enabled = false;
            this.btnTest.Location = new System.Drawing.Point(79, 161);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(100, 28);
            this.btnTest.TabIndex = 3;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // lblOwner
            // 
            this.lblOwner.AutoSize = true;
            this.lblOwner.Location = new System.Drawing.Point(24, 78);
            this.lblOwner.Name = "lblOwner";
            this.lblOwner.Size = new System.Drawing.Size(56, 13);
            this.lblOwner.TabIndex = 3;
            this.lblOwner.Text = "Password:";
            // 
            // txtOwnerPswd
            // 
            this.txtOwnerPswd.Enabled = false;
            this.txtOwnerPswd.Location = new System.Drawing.Point(68, 99);
            this.txtOwnerPswd.MaxLength = 50;
            this.txtOwnerPswd.Name = "txtOwnerPswd";
            this.txtOwnerPswd.PasswordChar = '*';
            this.txtOwnerPswd.Size = new System.Drawing.Size(139, 20);
            this.txtOwnerPswd.TabIndex = 1;
            this.txtOwnerPswd.TextChanged += new System.EventHandler(this.txtOwnerPswd_Testing);
            this.txtOwnerPswd.Enter += new System.EventHandler(this.txtOwnerPswd_Enter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Oracle TNS entry:";
            // 
            // txtTNS
            // 
            this.txtTNS.Enabled = false;
            this.txtTNS.Location = new System.Drawing.Point(68, 37);
            this.txtTNS.MaxLength = 50;
            this.txtTNS.Name = "txtTNS";
            this.txtTNS.Size = new System.Drawing.Size(139, 20);
            this.txtTNS.TabIndex = 0;
            this.txtTNS.TextChanged += new System.EventHandler(this.txtTNS_Testing);
            this.txtTNS.Leave += new System.EventHandler(this.txtTNS_Leave);
            // 
            // tabPageUser
            // 
            this.tabPageUser.Controls.Add(this.txtPassword);
            this.tabPageUser.Controls.Add(this.chkEnterPswd);
            this.tabPageUser.Controls.Add(this.lbxUsers);
            this.tabPageUser.Location = new System.Drawing.Point(4, 22);
            this.tabPageUser.Name = "tabPageUser";
            this.tabPageUser.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageUser.Size = new System.Drawing.Size(259, 254);
            this.tabPageUser.TabIndex = 1;
            this.tabPageUser.Text = "App. User";
            this.tabPageUser.UseVisualStyleBackColor = true;
            // 
            // txtPassword
            // 
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new System.Drawing.Point(29, 221);
            this.txtPassword.MaxLength = 15;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(200, 20);
            this.txtPassword.TabIndex = 7;
            this.txtPassword.TabStop = false;
            // 
            // chkEnterPswd
            // 
            this.chkEnterPswd.AutoSize = true;
            this.chkEnterPswd.Location = new System.Drawing.Point(29, 198);
            this.chkEnterPswd.Name = "chkEnterPswd";
            this.chkEnterPswd.Size = new System.Drawing.Size(100, 17);
            this.chkEnterPswd.TabIndex = 6;
            this.chkEnterPswd.TabStop = false;
            this.chkEnterPswd.Text = "Enter Password";
            this.chkEnterPswd.UseVisualStyleBackColor = true;
            this.chkEnterPswd.CheckedChanged += new System.EventHandler(this.chkEnterPswd_CheckedChanged);
            // 
            // lbxUsers
            // 
            this.lbxUsers.FormattingEnabled = true;
            this.lbxUsers.Items.AddRange(new object[] {
            "111",
            "222",
            "333"});
            this.lbxUsers.Location = new System.Drawing.Point(29, 7);
            this.lbxUsers.Name = "lbxUsers";
            this.lbxUsers.Size = new System.Drawing.Size(200, 173);
            this.lbxUsers.TabIndex = 5;
            this.lbxUsers.SelectedIndexChanged += new System.EventHandler(this.lbxUsers_SelectedIndexChanged);
            // 
            // frmUsers
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdExit;
            this.ClientSize = new System.Drawing.Size(295, 363);
            this.Controls.Add(this.tabCotrol);
            this.Controls.Add(this.cmdExit);
            this.Controls.Add(this.btnApply);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmUsers";
            this.Text = "SDE users";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmUsers_FormClosing);
            this.Load += new System.EventHandler(this.frmUsers_OnLoad);
            this.tabCotrol.ResumeLayout(false);
            this.tabPageConnect.ResumeLayout(false);
            this.tabPageConnect.PerformLayout();
            this.tabPageUser.ResumeLayout(false);
            this.tabPageUser.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button cmdExit;
        private System.Windows.Forms.TabControl tabCotrol;
        private System.Windows.Forms.TabPage tabPageConnect;
        private System.Windows.Forms.TabPage tabPageUser;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkEnterPswd;
        private System.Windows.Forms.ListBox lbxUsers;
        private System.Windows.Forms.Label lblOwner;
        private System.Windows.Forms.TextBox txtOwnerPswd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtTNS;
        private System.Windows.Forms.Label labelDbStatus;
        private System.Windows.Forms.Button btnTest;
    }
}