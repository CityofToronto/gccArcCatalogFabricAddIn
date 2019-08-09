using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.ArcCatalog;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Cadastral;
using ESRI.ArcGIS.GeoDatabaseExtensions;
using ESRI.ArcGIS.esriSystem;

namespace ArcCatalogFabricLib
{
    public partial class frmUsers : Form
    {

        string _ora_SID = "";
        string _fabric_schema = "";
        string _fabric_name = "";
        object _fabric_pwd = null;
        string _new_password = "";
        string _user_schema = "";
        Boolean _apply_change = false;

        private bool _db_sql_access = false;
        const string _db_status_Bad = "Connection failed";
        const string _db_status_Ok = "Connection successful";

        public frmUsers(string fabricOwner, object pwd, string fabricName, string oraSID="")
        {
            InitializeComponent();

            _fabric_schema = fabricOwner;
            _fabric_pwd = pwd;
            _fabric_name = fabricName;
            if (_fabric_name.ToUpper().StartsWith(_fabric_schema.ToUpper()))
            {
                // strip schema from fabric name
                string[] db_item = _fabric_name.Split('.');
                _fabric_name = db_item[1].ToUpper();
            }
            _ora_SID = oraSID;
        }


        private void cmdExit_Click(object sender, EventArgs e)
        {
            _new_password = "";
            txtPassword.Text = "";
            txtPassword.Enabled = false;
            MyUserHelper.UserPWD = "";
            MyUserHelper.UserPWDChanged = false;
            MyUserHelper.UserSchema = "";
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            _apply_change = true;
            this.Close();
            this.DialogResult = DialogResult.OK;
        }


        private void chkEnterPswd_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnterPswd.Checked)
            {
                txtPassword.Enabled = true;
            }
            else
            {
                _new_password = "";
                txtPassword.Text = "";
                txtPassword.Enabled = false;
            }
        }

        private void frmUsers_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_apply_change)
            {
                if (txtPassword.Text.Length > 0)
                {
                    _new_password = txtPassword.Text;
                    MyUserHelper.UserPWDChanged = true;
                    MyUserHelper.UserPWD = _new_password;
                }
                else
                    MyUserHelper.UserPWDChanged = false;
                MyUserHelper.UserSchema = _user_schema;
            }
            MyUserHelper.IsUserFormOpen = false;
        }

        private void lbxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_Loading) return;

            chkEnterPswd.Checked = false;
            txtPassword.Enabled = false;
            btnApply.Enabled = false;
            if (lbxUsers.SelectedIndex >=0)
            {
                _user_schema = lbxUsers.SelectedItem.ToString();
                btnApply.Enabled = true;
            }
        }


        bool m_Loading = false;

        private void frmUsers_OnLoad(object sender, EventArgs e)
        {
            MyUserHelper.IsUserFormOpen = true;

            labelDbStatus.Visible = false;

            m_Loading = true;

            lbxUsers.Items.Clear();

            if (MyUserHelper.EncryptedPWD && !MyUserHelper.SDE)
            {
                // Set activate tab
                this.tabCotrol.SelectedIndex = 1;
                // Only ESRI connection may be used
                // Depending on connection type must chose SDE or directconnection workspace
                ESRIAccess(_ora_SID, _fabric_schema);
            }
            else if (_ora_SID != "")
            {
                // OLE database connection
                SQLAccess(_ora_SID, _fabric_schema, (string)_fabric_pwd);
            }
            else
            {
                // Set activate tab
                this.tabCotrol.SelectedIndex = 2;
                TabPage tab0 = this.tabCotrol.SelectedTab;
                this.btnTest.Enabled = false;
                this.txtTNS.Enabled = true;
                this.txtOwnerPswd.Enabled = true;
                this.lblOwner.Text = "Passwoprd [" + _fabric_schema.ToUpper() + "] :";
                // System.Windows.Forms.MessageBox.Show("There was no correct password and/or database instance provided", "User select", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }


            m_Loading = false;
        }

        private void ESRIAccess(string _ora_SID, string _fabric_schema)
        {
            if (MyUserHelper.SQLDatabase != null)
            {
                string q_table_view = "DBA_TAB_PRIVS";
                string q_column = "GRANTEE";
                string s_query = "select " + q_column + " from " + q_table_view
                + " where OWNER='" + _fabric_schema.ToUpper() + "'"
                + " and TABLE_NAME like '" + _fabric_name.ToUpper() + "%'"
                + " and PRIVILEGE = 'SELECT'"
                + " and GRANTEE not like '%ROLE%'"
                + " and GRANTEE not like '%PRIVS%'"
                + " group by GRANTEE";
                
                ISqlWorkspace wrks = MyUserHelper.SQLDatabase as ISqlWorkspace;

                //IFeatureWorkspace featWrks = (IFeatureWorkspace)wrks;
                //IQueryDef2 queryDef = featWrks.CreateQueryDef() as IQueryDef2;

                ////Setting the SubFields, Tables, and WhereClause:
                ////Single table with a WhereClause
                //queryDef.Tables = "DBA_TAB_PRIVS";
                //queryDef.SubFields = "GRANTEE";
                //queryDef.WhereClause = "OWNER='" + _fabric_schema.ToUpper() + "'"
                //+ " and TABLE_NAME like '" + _fabric_name.ToUpper() + "%'"
                //+ " and PRIVILEGE = 'SELECT'"
                //+ " and GRANTEE not like '%ROLE%'"
                //+ " and GRANTEE not like '%PRIVS%'";
                //queryDef.PostfixClause = "group by GRANTEE";


                // Fill list
                lbxUsers.Items.Clear();

                //Using Evaluate:
                ICursor cursor = wrks.OpenQueryCursor(s_query);

                try
                {
                    // cursor = queryDef.Evaluate();

                    IRow row = cursor.NextRow();
                    while (row != null)
                    {
                        object val = row.get_Value(0);
                        try
                        {
                            string st_val = Convert.ToString(val);
                            // Console.WriteLine(reader[0].ToString());
                            lbxUsers.Items.Add(st_val);
                        }
                        catch { }
                        row = cursor.NextRow();
                    }

                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message, "Castom tool", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                finally
                {
                    if (cursor != null)
                    {
                        Marshal.ReleaseComObject(cursor);
                    }
                }
            }
            else
              throw new NullReferenceException();
        }

        void SQLAccess(string oracleSID, string applicationUser, string password)
        {
            string _cmd_source = "Data source connection";
            Boolean dbOpenStatus = false;

            _db_sql_access = false;

            OleDbConnection dbOra = null;
            dbOra = new OleDbConnection(); //"Data Source=MyOracleServer;Integrated Security=yes;");

            dbOra.ConnectionString = "Provider=OraOLEDB.Oracle;" +
                                   "Data Source=" + oracleSID +
                                   ";User ID=" + applicationUser +
                                   ";Password=" + password +
                                   ";PLSQLRSet=true;";
            //Using OLEDB .Net Data Provider features
            //";OLEDB.NET=true" +
            try
            {
                dbOra.Open();
                dbOpenStatus = true;
            }
            catch (OleDbException eOdbc)
            {
                // Try again !!!
                if (dbOra.State == ConnectionState.Broken || dbOra.State == ConnectionState.Closed)
                {
                    try
                    {
                        dbOra.Open();
                        dbOpenStatus = true;
                    }
                    catch { }
                }
                if (dbOra.State == ConnectionState.Connecting)
                {
                    int i = 0;
                    int wait_loop = 100;
                    while ((dbOra.State == ConnectionState.Connecting) && i < wait_loop)
                    {
                        i += 1;
                        System.Threading.Thread.Sleep(50);
                    }
                    dbOpenStatus = (dbOra.State == ConnectionState.Open);
                }

                if (!dbOpenStatus)
                {
#if debug
                Console.WriteLine("Error on database connection");
                Console.WriteLine(e.Message);
#endif
                    for (int i = 0; i < eOdbc.Errors.Count; i++)
                    {
                        System.Windows.Forms.MessageBox.Show("Index #" + i + "\n" +
                                "Message: " + eOdbc.Errors[i].Message + "\n" +
                                "Native: " + eOdbc.Errors[i].NativeError.ToString() + "\n" +
                                "Source: " + eOdbc.Errors[i].Source + "\n" +
                                "SQL: " + eOdbc.Errors[i].SQLState + "\n", _cmd_source);
                    }
                }

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(
                        "Message: " + e.Message + "\n" +
                        "Source: " + e.Source + "\n");
            }


            if (dbOpenStatus)
            {
                //Execute                 
                {
                    Boolean error_flag = false;
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select GRANTEE from DBA_TAB_PRIVS "
                                       + " where OWNER='" + _fabric_schema.ToUpper() + "'"
                                       + " and TABLE_NAME like '" + _fabric_name.ToUpper() + "%'"
                                       + " and PRIVILEGE = 'SELECT'"
                                       + " and GRANTEE not like '%ROLE%'"
                                       + " and GRANTEE not like '%PRIVS%'"
                                       + " group by GRANTEE";

                    cmd.Connection = dbOra;

                    try
                    {
                        cmd.Prepare();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
                        error_flag = true;
                    }

                    if (!error_flag)
                    {
                        // Fill list
                        lbxUsers.Items.Clear();

                        OleDbDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            // Console.WriteLine(reader[0].ToString());
                            lbxUsers.Items.Add(reader[0].ToString());
                        }
                        reader.Close();

                        _db_sql_access = true;
                    }
                }
                

                dbOra.Close();
                dbOpenStatus = false;
            }


        }

        private void txtTNS_Testing(object sender, EventArgs e)
        {
            this.btnTest.Enabled = ((txtTNS.Text.Length > 0) & (txtOwnerPswd.Text.Length > 0));
        }

        private void txtOwnerPswd_Testing(object sender, EventArgs e)
        {
            this.btnTest.Enabled = ((txtOwnerPswd.Text.Length > 0) & (txtOwnerPswd.Text.Length > 0));
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            // OLE database connection
            SQLAccess(_ora_SID, _fabric_schema, txtOwnerPswd.Text);

            labelDbStatus.Visible = true;
            if (_db_sql_access)
                labelDbStatus.Text = _db_status_Ok;
            else
                labelDbStatus.Text = _db_status_Bad;
        }

        private void txtTNS_Leave(object sender, EventArgs e)
        {
            if (txtTNS.Text.Length > 0)
            {
                _ora_SID = txtTNS.Text.ToUpper();
            }
        }

        private void txtOwnerPswd_Enter(object sender, EventArgs e)
        {
            if (txtTNS.Text.Length > 0)
            {
                txtTNS.Text = txtTNS.Text.ToUpper();
            }
        }
    }
}

