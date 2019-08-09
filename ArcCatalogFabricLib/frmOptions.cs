using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArcCatalogFabricLib
{
    public partial class frmOptions : Form
    {
        Boolean mCheckFabricParcels = false;
        Boolean mCheckFabricPlans = false;
        Boolean mCheckFabricControlPoints = false;
        public Boolean mCancelChange = true;

        #region Public members
        public frmOptions()
        {
            InitializeComponent();
        }

        public Boolean DoNotChange
        {
            get
            {
                return mCancelChange;
            }
        }
        public Boolean AllSelected
        {
            get
            {
                return IsAllChecked();
            }
        }
        public Boolean NoneSelected
        {
            get
            {
                return IsNoneChecked();
            }
        }
        public Boolean Parcels
        {
            get
            {
                return mCheckFabricParcels;
            }
        }
        public Boolean Plans
        {
            get
            {
                return mCheckFabricPlans;
            }
        }
        public Boolean ControlPoints
        {
            get
            {
                return mCheckFabricControlPoints;
            }
        }
        #endregion

        private Boolean IsAllChecked()
        {
            return (mCheckFabricParcels
                && mCheckFabricPlans
                && mCheckFabricControlPoints);
        }
        private Boolean IsAnyChecked()
        {
            return (mCheckFabricParcels
                || mCheckFabricPlans
                || mCheckFabricControlPoints);
        }
        private Boolean IsNoneChecked()
        {
            return ((mCheckFabricControlPoints == false)
                 && (mCheckFabricParcels == false)
                 && (mCheckFabricPlans == false));
        }

        private void OtptionsFormEvent_Ok(object sender, EventArgs e)
        {
            mCheckFabricParcels = this.chkParcel.Checked;
            mCheckFabricPlans = this.chkPlans.Checked;
            mCheckFabricControlPoints = this.chkControlPnts.Checked;
            mCancelChange = false;
            this.Hide();
        }

        private void OptionsFormEvent_Cancel(object sender, EventArgs e)
        {
            mCancelChange = true;
            this.Hide();
        }

        private void cmdClearAllEvent_Click(object sender, EventArgs e)
        {
            this.chkParcel.Checked = false;
            this.chkPlans.Checked = false;
            this.chkControlPnts.Checked = false;
            //cmdCheckAll.Enabled = true;
            //cmdClearAll.Enabled = false;
        }

        private void cmdCheckAllEvent_Click(object sender, EventArgs e)
        {
            this.chkParcel.Checked = true;
            this.chkPlans.Checked = true;
            this.chkControlPnts.Checked = true;
            //cmdClearAll.Enabled = true;
            //cmdCheckAll.Enabled = false;
        }

        private void chkParcelEvent_click(object sender, EventArgs e)
        {
            mCheckFabricParcels = this.chkParcel.Checked;
            RefreshButtons();
        }

        private void chkPlansEvent_Click(object sender, EventArgs e)
        {
            mCheckFabricPlans = this.chkPlans.Checked;
            RefreshButtons();
        }

        private void chkControlPntsEvent_Click(object sender, EventArgs e)
        {
            mCheckFabricControlPoints = this.chkControlPnts.Checked;
            RefreshButtons();
        }

        private void RefreshButtons()
        {
            this.cmdClearAll.Enabled = IsAnyChecked();
            this.cmdCheckAll.Enabled = ((IsNoneChecked() || IsAnyChecked()) 
                                     && !(IsAllChecked()));
        }

        private void frmOptionsEvent_Activated(object sender, EventArgs e)
        {
            RefreshButtons();
        }

        //private void grpLayerCheckEvent_Enter(object sender, EventArgs e)
        //{
        //    System.Windows.Forms.MessageBox.Show("Hey");
        //}
    }
}
