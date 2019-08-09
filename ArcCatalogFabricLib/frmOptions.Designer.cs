namespace ArcCatalogFabricLib
{
    partial class frmOptions
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
            this.cmdOk = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.grpLayerCheck = new System.Windows.Forms.GroupBox();
            this.cmdClearAll = new System.Windows.Forms.Button();
            this.cmdCheckAll = new System.Windows.Forms.Button();
            this.chkControlPnts = new System.Windows.Forms.CheckBox();
            this.chkPlans = new System.Windows.Forms.CheckBox();
            this.chkParcel = new System.Windows.Forms.CheckBox();
            this.cmdReadOnly = new System.Windows.Forms.Button();
            this.grpLayerCheck.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdOk
            // 
            this.cmdOk.Location = new System.Drawing.Point(144, 237);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(87, 24);
            this.cmdOk.TabIndex = 0;
            this.cmdOk.Text = "Ok";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.OtptionsFormEvent_Ok);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(257, 237);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(87, 24);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.OptionsFormEvent_Cancel);
            // 
            // grpLayerCheck
            // 
            this.grpLayerCheck.Controls.Add(this.cmdReadOnly);
            this.grpLayerCheck.Controls.Add(this.cmdClearAll);
            this.grpLayerCheck.Controls.Add(this.cmdCheckAll);
            this.grpLayerCheck.Controls.Add(this.chkControlPnts);
            this.grpLayerCheck.Controls.Add(this.chkPlans);
            this.grpLayerCheck.Controls.Add(this.chkParcel);
            this.grpLayerCheck.Location = new System.Drawing.Point(28, 25);
            this.grpLayerCheck.Name = "grpLayerCheck";
            this.grpLayerCheck.Size = new System.Drawing.Size(332, 196);
            this.grpLayerCheck.TabIndex = 2;
            this.grpLayerCheck.TabStop = false;
            this.grpLayerCheck.Text = "Check Layer to change";
            // 
            // cmdClearAll
            // 
            this.cmdClearAll.Location = new System.Drawing.Point(216, 63);
            this.cmdClearAll.Name = "cmdClearAll";
            this.cmdClearAll.Size = new System.Drawing.Size(88, 22);
            this.cmdClearAll.TabIndex = 4;
            this.cmdClearAll.Text = "Clear all";
            this.cmdClearAll.UseVisualStyleBackColor = true;
            this.cmdClearAll.Click += new System.EventHandler(this.cmdClearAllEvent_Click);
            // 
            // cmdCheckAll
            // 
            this.cmdCheckAll.Location = new System.Drawing.Point(216, 33);
            this.cmdCheckAll.Name = "cmdCheckAll";
            this.cmdCheckAll.Size = new System.Drawing.Size(88, 22);
            this.cmdCheckAll.TabIndex = 3;
            this.cmdCheckAll.Text = "Check all";
            this.cmdCheckAll.UseVisualStyleBackColor = true;
            this.cmdCheckAll.Click += new System.EventHandler(this.cmdCheckAllEvent_Click);
            // 
            // chkControlPnts
            // 
            this.chkControlPnts.AutoSize = true;
            this.chkControlPnts.Location = new System.Drawing.Point(51, 95);
            this.chkControlPnts.Name = "chkControlPnts";
            this.chkControlPnts.Size = new System.Drawing.Size(91, 17);
            this.chkControlPnts.TabIndex = 2;
            this.chkControlPnts.Text = "Control Points";
            this.chkControlPnts.UseVisualStyleBackColor = true;
            this.chkControlPnts.Click += new System.EventHandler(this.chkControlPntsEvent_Click);
            // 
            // chkPlans
            // 
            this.chkPlans.AutoSize = true;
            this.chkPlans.Location = new System.Drawing.Point(51, 63);
            this.chkPlans.Name = "chkPlans";
            this.chkPlans.Size = new System.Drawing.Size(52, 17);
            this.chkPlans.TabIndex = 1;
            this.chkPlans.Text = "Plans";
            this.chkPlans.UseVisualStyleBackColor = true;
            this.chkPlans.Click += new System.EventHandler(this.chkPlansEvent_Click);
            // 
            // chkParcel
            // 
            this.chkParcel.AutoSize = true;
            this.chkParcel.Location = new System.Drawing.Point(51, 29);
            this.chkParcel.Name = "chkParcel";
            this.chkParcel.Size = new System.Drawing.Size(93, 17);
            this.chkParcel.TabIndex = 0;
            this.chkParcel.Text = "Fabric Parcels";
            this.chkParcel.UseVisualStyleBackColor = true;
            this.chkParcel.Click += new System.EventHandler(this.chkParcelEvent_click);
            // 
            // cmdReadOnly
            // 
            this.cmdReadOnly.Location = new System.Drawing.Point(216, 137);
            this.cmdReadOnly.Name = "cmdReadOnly";
            this.cmdReadOnly.Size = new System.Drawing.Size(87, 22);
            this.cmdReadOnly.TabIndex = 5;
            this.cmdReadOnly.Text = "Read only id fields";
            this.cmdReadOnly.UseVisualStyleBackColor = true;
            // 
            // frmOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 282);
            this.Controls.Add(this.grpLayerCheck);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOptions";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Alter Fabric layer";
            this.Activated += new System.EventHandler(this.frmOptionsEvent_Activated);
            this.grpLayerCheck.ResumeLayout(false);
            this.grpLayerCheck.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.GroupBox grpLayerCheck;
        private System.Windows.Forms.CheckBox chkControlPnts;
        private System.Windows.Forms.CheckBox chkPlans;
        private System.Windows.Forms.CheckBox chkParcel;
        private System.Windows.Forms.Button cmdCheckAll;
        private System.Windows.Forms.Button cmdClearAll;
        private System.Windows.Forms.Button cmdReadOnly;
    }
}