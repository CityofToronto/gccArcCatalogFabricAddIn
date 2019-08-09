<<<<<<< HEAD
#define _OLED_
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.ArcCatalog;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Cadastral;
using ESRI.ArcGIS.GeoDatabaseExtensions;
using ESRI.ArcGIS.esriSystem;

namespace ArcCatalogFabricLib
{
    /// <summary>
    /// Summary description for cmdMakeFabricLayer.
    /// </summary>
    [Guid("e4e4bf32-ecb3-4429-8c8a-c56e90e99c4d")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ArcCatalogFabricLib.cmdMakeFabricLayer")]
    public sealed class cmdMakeFabricLayer : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            GxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            GxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private string MyFabric = "CadastralFabric";
        private frmUsers MyUsersForm; // My UI

        private IApplication m_application;
        public cmdMakeFabricLayer()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "Fabric layer user change"; //localizable text
            base.m_caption = "Change layer connection";  //localizable text
            base.m_message = "Change layer connection";  //localizable text 
            base.m_toolTip = "Change layer connection";  //localizable text 
            base.m_name = "MyFabricLayerUser_ArcCatalogCommand";   //unique id, non-localizable (e.g. "MyCategory_ArcCatalogCommand")
            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        private const string USER_PROFILE = "USERPROFILE";
        string _user;
        string _instance;
        object _password;
        string _server;
        bool _fabric_selected;
        bool _dir_connect;

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            m_application = hook as IApplication;

            //Disable if it is not ArcCatalog
            if (hook is IGxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {

            SetupFabric();

            if (!_fabric_selected) return;

            MyUserHelper.FabricSchema = _user;
            MyUserHelper.FabricPWD = _password; // "tsnine"; // password;
            MyUserHelper.EncryptedPWD = true;

            IGxDataset pGxDataset = GdbHelper.GetSelectedDataset(m_application);
            ICadastralFabric fab = (ICadastralFabric)pGxDataset.Dataset;
            IDataset ds = fab as IDataset;
            IName dsName = ds.FullName;
            System.Diagnostics.Trace.WriteLine("Fabric: " + dsName.NameString);
            string dbDSName = ((IGxObject)pGxDataset).Parent.Name;
            System.Diagnostics.Trace.WriteLine("Dataset: " + dbDSName);

            // Check if direct connection SID maybe provide otherwise use only ESRI connections

            if (_dir_connect)
            {
                //if (!MyUserHelper.EncryptedPWD)
                //    MyUsersForm = new frmUsers(_user, "tsnine", dsName.NameString, _server);
                //else
                {
                    MyUserHelper.SDE = false;
#if _OLED_
#endif
                    // IWorkspace sqlGDB = null;
                    ISqlWorkspace sqlGDB = null;

                    // Create new workspace to connect to generec database
                    if ((sqlGDB = GdbHelper.OpenSqlWorkspace1(m_application, ((IDataset)pGxDataset.Dataset).Workspace, _user)) != null)
                    {
                        MyUserHelper.SQLDatabase = (IWorkspace)sqlGDB;
                        //MyUserHelper.SQLDatabase = (ISqlWorkspace) ((IDataset)pGxDataset.Dataset).Workspace;
                        MyUsersForm = new frmUsers(_user, _password, dsName.NameString, _server);
                    }
                    else
                        return;
                }                    
            }                
            else
            {
                MyUserHelper.SDE = true;
                MyUsersForm = new frmUsers(_user, _password, dsName.NameString);
            }
                

            MyUsersForm.ShowDialog();

            DialogResult res = MyUsersForm.DialogResult;

            MyUserHelper.IsUserFormOpen = false;
            if (MyUserHelper.SQLDatabase != null)
            {
                Marshal.ReleaseComObject(MyUserHelper.SQLDatabase);
                MyUserHelper.SQLDatabase = null;
            }

            //            if (res == DialogResult.Yes || res == DialogResult.OK)
            //             if (res != DialogResult.Cancel)

            if (res == DialogResult.Yes || res == DialogResult.OK)
            {
                System.Diagnostics.Trace.WriteLine("Do actions");

                CadastralFabricLayerFactoryClass fab_layer_create = new CadastralFabricLayerFactoryClass();

                Boolean prOk = false;
                IWorkspace userWksp = null;
                IDataset datasetNew = null;
                IName dsNameNew = null;

#region Make new workspace
                try
                {
                    IMouseCursor pWorking = new MouseCursor(); pWorking.SetCursor(2);

                    if (!MyUserHelper.UserPWDChanged)
                        userWksp = GdbHelper.OpenUserWorkspace(m_application, ((IDataset)pGxDataset.Dataset).Workspace, MyUserHelper.UserSchema);
                    else
                        userWksp = GdbHelper.OpenUserWorkspace(m_application, ((IDataset)pGxDataset.Dataset).Workspace, MyUserHelper.UserSchema, MyUserHelper.UserPWD);

                    datasetNew = GdbHelper.OpenCadastralDataset(userWksp, dbDSName, dsName.NameString);

                    prOk = true;

                    pWorking.SetCursor(1);
                    pWorking = null;
                }
                catch(System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Castom tool");
                }
#endregion

                if (!prOk)
                {
                    if (userWksp != null)
                    {
                        Marshal.ReleaseComObject(userWksp);
                    }
                    return;
                }


                ILayerFactory pNewLayerFactory = fab_layer_create as ILayerFactory;
                dsNameNew = datasetNew.FullName;
                dsNameNew.Open();

                if (pNewLayerFactory.get_CanCreate(dsNameNew))
                {
                    IEnumLayer enum_la = pNewLayerFactory.Create(dsNameNew);

                    ILayer la = enum_la.Next();
                    //while (la != null)
                    //{
                    //    System.Diagnostics.Trace.WriteLine("Fabric layer : " + la.Name);
                    //    la = enum_la.Next();
                    //}
                    if (la != null)
                    {
                        // Create a new diualog
                        IGxDialog save_layer = new GxDialogClass();
                        save_layer.set_StartingLocation(System.Environment.GetEnvironmentVariable(USER_PROFILE)+"\\Documents");
                        save_layer.Title = "Enter new Cadastral layer name";
                        // IGxObjectFilter layer_filter = new GxFilterLayersClass();
                        save_layer.ObjectFilter = new GxFilterLayersClass();

                        if (save_layer.DoModalSave(0))
                        {
                            // Create a new GxLayer
                            ESRI.ArcGIS.Catalog.IGxLayer gxLayer = new ESRI.ArcGIS.Catalog.GxLayerClass();
                            ESRI.ArcGIS.Catalog.IGxFile gxFile = (ESRI.ArcGIS.Catalog.IGxFile)gxLayer; //Explicit Cast
                            string loc = ((IGxFile)save_layer.FinalLocation).Path;
                            if (!save_layer.Name.ToLower().EndsWith(".lyr")) gxFile.Path = loc + "/" + save_layer.Name + ".lyr";
                            else gxFile.Path = loc + "/" + save_layer.Name;
                            if (save_layer.ReplacingObject)
                            {
                                // Delelet first
                                if (System.IO.File.Exists(gxFile.Path)) System.IO.File.Delete(gxFile.Path);
                            }

                            gxFile.New();

                            //GxLayerFactoryClass fab_layer = new GxLayerFactoryClass();
                            //IGxObjectFactory pFabLayer = (IGxObjectFactory)fab_layer;
                            //IGxObjectFactoryFileExtensions pFabLayerExt = (IGxObjectFactoryFileExtensions)fab_layer;
                            // pFabLayer.Catalog = pGxDataset as IGxCatalog;
                            //System.Diagnostics.Trace.WriteLine("RelevantExtensions: <" + pFabLayerExt.RelevantExtensions + ">");

                            la.Visible = false;
                            gxLayer.Layer = la;
                            gxFile.Close(true);
                        }
                    }

                }

                Marshal.ReleaseComObject(dsNameNew);
                Marshal.ReleaseComObject(datasetNew);
                Marshal.ReleaseComObject(userWksp);

                //fab_layer.

            }

            MyUsersForm.Dispose();

        }

        public override bool Enabled
        {
            get
            {
                IGxApplication gxApp = m_application as IGxApplication;
                IGxDataset gxDataset = null;
                esriDatasetType dsType = esriDatasetType.esriDTAny;
                // pUnk = null;

                if (gxApp != null)
                {
                    gxDataset = gxApp.SelectedObject as IGxDataset;
                    if (gxDataset != null) dsType = gxDataset.Type;
                    //pUnk = (stdole.IUnknown) gxApp.SelectedObject.InternalObjectName.Open();
                }

                return (dsType == esriDatasetType.esriDTCadastralFabric);

                // return base.Enabled;
            }
        }

#endregion

        void SetupFabric()
        {
            _dir_connect = false;

            //IGxDatabase pGxDatabase;
            IGxDataset pGxDataset;
            //String      schemaUser;

            //if (!TestCadastralExtension()) return;
            // Get current database connection
            //if  (!TestSelectedItem()) return;

            if ((pGxDataset = GdbHelper.GetSelectedDataset(m_application)) != null)
            {
                if (pGxDataset.Type == esriDatasetType.esriDTFeatureDataset)
                {
                    System.Windows.Forms.MessageBox.Show("Select fabric dataset");
                }
                else
                {
                    if (pGxDataset.Type == esriDatasetType.esriDTCadastralFabric)
                    {
                        //if (System.Windows.Forms.MessageBox.Show("Do you want to alter fabric dataset", "Setup Fabric", MessageBoxButtons.YesNo) == DialogResult.No)
                        //    return;

                        // Do change
                        // UpdateFabricSchema(ExtractSchemaName(pGxDataset.Dataset), (ICadastralFabric)pGxDataset.Dataset);

                        IGxObject pFD = ((IGxObject) pGxDataset).Parent;
                        IGxDatabase pDB = pFD.Parent as IGxDatabase;
                        IWorkspaceName2 pWName = (IWorkspaceName2) pDB.WorkspaceName;
                        

                        if (pDB.IsConnected)
                        {
                            IWorkspace wrksp = pDB.Workspace;

                            if (wrksp.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                            {
                                // Getr ptoperties
                                IPropertySet pConnectionProperties = pWName.ConnectionProperties; // wrksp.ConnectionProperties;
                                //string[] n = { "USER", "INSTANCE", "PASSWORD", "SERVER" };
                                //object v;
                                //                            pConnectionProperties.GetProperties(n, out v);
                                _user = (string)pConnectionProperties.GetProperty("USER");
                                _instance = (string)pConnectionProperties.GetProperty("INSTANCE");
                                _password = pConnectionProperties.GetProperty("PASSWORD");
                                //var _password = pConnectionProperties.GetProperty("PASSWORD");
                                _server = (string)pConnectionProperties.GetProperty("SERVER");

                                string ora_tns = GdbHelper.OracleServiceName(pConnectionProperties);

                                if (ora_tns != "")
                                {
                                    //Direct connection
                                    _dir_connect = true;
                                    if (_server.ToUpper() != ora_tns.ToUpper())
                                        _server = ora_tns.ToUpper();
                                }

                                _fabric_selected = true;
                            }
                        }

                    }
                }
            }
            else
            {
                _user = "";
                _instance ="";
                _password = null;
                //var _password = pConnectionProperties.GetProperty("PASSWORD");
                _server = "";
            }


        }


#region"Add Layer File to ActiveView"
        // ArcGIS Snippet Title:
        // Add Layer File to ActiveView
        // 
        // Long Description:
        // Add a layer file (.lyr) into the active view when the path (on disk or network) is specified.
        // 
        // Add the following references to the project:
        // ESRI.ArcGIS.Carto
        // ESRI.ArcGIS.Catalog
        // ESRI.ArcGIS.Geodatabase
        // ESRI.ArcGIS.System
        // ESRI.ArcGIS.SystemUI
        // 
        // Intended ArcGIS Products for this snippet:
        // ArcGIS Desktop (ArcEditor, ArcInfo, ArcView)
        // 
        // Applicable ArcGIS Product Versions:
        // 9.2
        // 9.3
        // 9.3.1
        // 10.0
        // 
        // Required ArcGIS Extensions:
        // (NONE)
        // 
        // Notes:
        // This snippet is intended to be inserted at the base level of a Class.
        // It is not intended to be nested within an existing Method.
        // 

        ///<summary>Add a layer file (.lyr) into the active view when the path (on disk or network) is specified.</summary>
        ///      
        ///<param name="activeView">An IActiveview interface</param>
        ///<param name="layerPathFile">A System.String that is the path\filename of a layer file. Example: "C:\temp\mylayer.lyr".</param>
        ///      
        ///<remarks></remarks>
        public void AddLayerToActiveView(ESRI.ArcGIS.Carto.IActiveView activeView, System.String layerPathFile)
        {

            if (activeView == null || layerPathFile == null || !layerPathFile.EndsWith(".lyr"))
            {
                return;
            }

            // Create a new GxLayer
            ESRI.ArcGIS.Catalog.IGxLayer gxLayer = new ESRI.ArcGIS.Catalog.GxLayerClass();

            ESRI.ArcGIS.Catalog.IGxFile gxFile = (ESRI.ArcGIS.Catalog.IGxFile)gxLayer; //Explicit Cast

            // Set the path for where the layerfile is located on disk
            gxFile.Path = layerPathFile;

            // Test if we have a valid layer and add it to the map
            if (!(gxLayer.Layer == null))
            {
                ESRI.ArcGIS.Carto.IMap map = activeView.FocusMap;
                map.AddLayer(gxLayer.Layer);
            }
        }
#endregion
    }
}
=======
#define _OLED_
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.ArcCatalog;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Cadastral;
using ESRI.ArcGIS.GeoDatabaseExtensions;
using ESRI.ArcGIS.esriSystem;

namespace ArcCatalogFabricLib
{
    /// <summary>
    /// Summary description for cmdMakeFabricLayer.
    /// </summary>
    [Guid("e4e4bf32-ecb3-4429-8c8a-c56e90e99c4d")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ArcCatalogFabricLib.cmdMakeFabricLayer")]
    public sealed class cmdMakeFabricLayer : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            GxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            GxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private string MyFabric = "CadastralFabric";
        private frmUsers MyUsersForm; // My UI

        private IApplication m_application;
        public cmdMakeFabricLayer()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "Fabric layer user change"; //localizable text
            base.m_caption = "Change layer connection";  //localizable text
            base.m_message = "Change layer connection";  //localizable text 
            base.m_toolTip = "Change layer connection";  //localizable text 
            base.m_name = "MyFabricLayerUser_ArcCatalogCommand";   //unique id, non-localizable (e.g. "MyCategory_ArcCatalogCommand")
            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        private const string USER_PROFILE = "USERPROFILE";
        string _user;
        string _instance;
        object _password;
        string _server;
        bool _fabric_selected;
        bool _dir_connect;

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            m_application = hook as IApplication;

            //Disable if it is not ArcCatalog
            if (hook is IGxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {

            SetupFabric();

            if (!_fabric_selected) return;

            MyUserHelper.FabricSchema = _user;
            MyUserHelper.FabricPWD = _password; // "tsnine"; // password;
            MyUserHelper.EncryptedPWD = true;

            IGxDataset pGxDataset = GdbHelper.GetSelectedDataset(m_application);
            ICadastralFabric fab = (ICadastralFabric)pGxDataset.Dataset;
            IDataset ds = fab as IDataset;
            IName dsName = ds.FullName;
            System.Diagnostics.Trace.WriteLine("Fabric: " + dsName.NameString);
            string dbDSName = ((IGxObject)pGxDataset).Parent.Name;
            System.Diagnostics.Trace.WriteLine("Dataset: " + dbDSName);

            // Check if direct connection SID maybe provide otherwise use only ESRI connections

            if (_dir_connect)
            {
                //if (!MyUserHelper.EncryptedPWD)
                //    MyUsersForm = new frmUsers(_user, "tsnine", dsName.NameString, _server);
                //else
                {
                    MyUserHelper.SDE = false;
#if _OLED_
#endif
                    // IWorkspace sqlGDB = null;
                    ISqlWorkspace sqlGDB = null;

                    // Create new workspace to connect to generec database
                    if ((sqlGDB = GdbHelper.OpenSqlWorkspace1(m_application, ((IDataset)pGxDataset.Dataset).Workspace, _user)) != null)
                    {
                        MyUserHelper.SQLDatabase = (IWorkspace)sqlGDB;
                        //MyUserHelper.SQLDatabase = (ISqlWorkspace) ((IDataset)pGxDataset.Dataset).Workspace;
                        MyUsersForm = new frmUsers(_user, _password, dsName.NameString, _server);
                    }
                    else
                        return;
                }                    
            }                
            else
            {
                MyUserHelper.SDE = true;
                MyUsersForm = new frmUsers(_user, _password, dsName.NameString);
            }
                

            MyUsersForm.ShowDialog();

            DialogResult res = MyUsersForm.DialogResult;

            MyUserHelper.IsUserFormOpen = false;
            if (MyUserHelper.SQLDatabase != null)
            {
                Marshal.ReleaseComObject(MyUserHelper.SQLDatabase);
                MyUserHelper.SQLDatabase = null;
            }

            //            if (res == DialogResult.Yes || res == DialogResult.OK)
            //             if (res != DialogResult.Cancel)

            if (res == DialogResult.Yes || res == DialogResult.OK)
            {
                System.Diagnostics.Trace.WriteLine("Do actions");

                CadastralFabricLayerFactoryClass fab_layer_create = new CadastralFabricLayerFactoryClass();

                Boolean prOk = false;
                IWorkspace userWksp = null;
                IDataset datasetNew = null;
                IName dsNameNew = null;

#region Make new workspace
                try
                {
                    IMouseCursor pWorking = new MouseCursor(); pWorking.SetCursor(2);

                    if (!MyUserHelper.UserPWDChanged)
                        userWksp = GdbHelper.OpenUserWorkspace(m_application, ((IDataset)pGxDataset.Dataset).Workspace, MyUserHelper.UserSchema);
                    else
                        userWksp = GdbHelper.OpenUserWorkspace(m_application, ((IDataset)pGxDataset.Dataset).Workspace, MyUserHelper.UserSchema, MyUserHelper.UserPWD);

                    datasetNew = GdbHelper.OpenCadastralDataset(userWksp, dbDSName, dsName.NameString);

                    prOk = true;

                    pWorking.SetCursor(1);
                    pWorking = null;
                }
                catch(System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "Castom tool");
                }
#endregion

                if (!prOk)
                {
                    if (userWksp != null)
                    {
                        Marshal.ReleaseComObject(userWksp);
                    }
                    return;
                }


                ILayerFactory pNewLayerFactory = fab_layer_create as ILayerFactory;
                dsNameNew = datasetNew.FullName;
                dsNameNew.Open();

                if (pNewLayerFactory.get_CanCreate(dsNameNew))
                {
                    IEnumLayer enum_la = pNewLayerFactory.Create(dsNameNew);

                    ILayer la = enum_la.Next();
                    //while (la != null)
                    //{
                    //    System.Diagnostics.Trace.WriteLine("Fabric layer : " + la.Name);
                    //    la = enum_la.Next();
                    //}
                    if (la != null)
                    {
                        // Create a new diualog
                        IGxDialog save_layer = new GxDialogClass();
                        save_layer.set_StartingLocation(System.Environment.GetEnvironmentVariable(USER_PROFILE)+"\\Documents");
                        save_layer.Title = "Enter new Cadastral layer name";
                        // IGxObjectFilter layer_filter = new GxFilterLayersClass();
                        save_layer.ObjectFilter = new GxFilterLayersClass();

                        if (save_layer.DoModalSave(0))
                        {
                            // Create a new GxLayer
                            ESRI.ArcGIS.Catalog.IGxLayer gxLayer = new ESRI.ArcGIS.Catalog.GxLayerClass();
                            ESRI.ArcGIS.Catalog.IGxFile gxFile = (ESRI.ArcGIS.Catalog.IGxFile)gxLayer; //Explicit Cast
                            string loc = ((IGxFile)save_layer.FinalLocation).Path;
                            if (!save_layer.Name.ToLower().EndsWith(".lyr")) gxFile.Path = loc + "/" + save_layer.Name + ".lyr";
                            else gxFile.Path = loc + "/" + save_layer.Name;
                            if (save_layer.ReplacingObject)
                            {
                                // Delelet first
                                if (System.IO.File.Exists(gxFile.Path)) System.IO.File.Delete(gxFile.Path);
                            }

                            gxFile.New();

                            //GxLayerFactoryClass fab_layer = new GxLayerFactoryClass();
                            //IGxObjectFactory pFabLayer = (IGxObjectFactory)fab_layer;
                            //IGxObjectFactoryFileExtensions pFabLayerExt = (IGxObjectFactoryFileExtensions)fab_layer;
                            // pFabLayer.Catalog = pGxDataset as IGxCatalog;
                            //System.Diagnostics.Trace.WriteLine("RelevantExtensions: <" + pFabLayerExt.RelevantExtensions + ">");

                            la.Visible = false;
                            gxLayer.Layer = la;
                            gxFile.Close(true);
                        }
                    }

                }

                Marshal.ReleaseComObject(dsNameNew);
                Marshal.ReleaseComObject(datasetNew);
                Marshal.ReleaseComObject(userWksp);

                //fab_layer.

            }

            MyUsersForm.Dispose();

        }

        public override bool Enabled
        {
            get
            {
                IGxApplication gxApp = m_application as IGxApplication;
                IGxDataset gxDataset = null;
                esriDatasetType dsType = esriDatasetType.esriDTAny;
                // pUnk = null;

                if (gxApp != null)
                {
                    gxDataset = gxApp.SelectedObject as IGxDataset;
                    if (gxDataset != null) dsType = gxDataset.Type;
                    //pUnk = (stdole.IUnknown) gxApp.SelectedObject.InternalObjectName.Open();
                }

                return (dsType == esriDatasetType.esriDTCadastralFabric);

                // return base.Enabled;
            }
        }

#endregion

        void SetupFabric()
        {
            _dir_connect = false;

            //IGxDatabase pGxDatabase;
            IGxDataset pGxDataset;
            //String      schemaUser;

            //if (!TestCadastralExtension()) return;
            // Get current database connection
            //if  (!TestSelectedItem()) return;

            if ((pGxDataset = GdbHelper.GetSelectedDataset(m_application)) != null)
            {
                if (pGxDataset.Type == esriDatasetType.esriDTFeatureDataset)
                {
                    System.Windows.Forms.MessageBox.Show("Select fabric dataset");
                }
                else
                {
                    if (pGxDataset.Type == esriDatasetType.esriDTCadastralFabric)
                    {
                        //if (System.Windows.Forms.MessageBox.Show("Do you want to alter fabric dataset", "Setup Fabric", MessageBoxButtons.YesNo) == DialogResult.No)
                        //    return;

                        // Do change
                        // UpdateFabricSchema(ExtractSchemaName(pGxDataset.Dataset), (ICadastralFabric)pGxDataset.Dataset);

                        IGxObject pFD = ((IGxObject) pGxDataset).Parent;
                        IGxDatabase pDB = pFD.Parent as IGxDatabase;
                        IWorkspaceName2 pWName = (IWorkspaceName2) pDB.WorkspaceName;
                        

                        if (pDB.IsConnected)
                        {
                            IWorkspace wrksp = pDB.Workspace;

                            if (wrksp.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                            {
                                // Getr ptoperties
                                IPropertySet pConnectionProperties = pWName.ConnectionProperties; // wrksp.ConnectionProperties;
                                //string[] n = { "USER", "INSTANCE", "PASSWORD", "SERVER" };
                                //object v;
                                //                            pConnectionProperties.GetProperties(n, out v);
                                _user = (string)pConnectionProperties.GetProperty("USER");
                                _instance = (string)pConnectionProperties.GetProperty("INSTANCE");
                                _password = pConnectionProperties.GetProperty("PASSWORD");
                                //var _password = pConnectionProperties.GetProperty("PASSWORD");
                                _server = (string)pConnectionProperties.GetProperty("SERVER");

                                string ora_tns = GdbHelper.OracleServiceName(pConnectionProperties);

                                if (ora_tns != "")
                                {
                                    //Direct connection
                                    _dir_connect = true;
                                    if (_server.ToUpper() != ora_tns.ToUpper())
                                        _server = ora_tns.ToUpper();
                                }

                                _fabric_selected = true;
                            }
                        }

                    }
                }
            }
            else
            {
                _user = "";
                _instance ="";
                _password = null;
                //var _password = pConnectionProperties.GetProperty("PASSWORD");
                _server = "";
            }


        }


#region"Add Layer File to ActiveView"
        // ArcGIS Snippet Title:
        // Add Layer File to ActiveView
        // 
        // Long Description:
        // Add a layer file (.lyr) into the active view when the path (on disk or network) is specified.
        // 
        // Add the following references to the project:
        // ESRI.ArcGIS.Carto
        // ESRI.ArcGIS.Catalog
        // ESRI.ArcGIS.Geodatabase
        // ESRI.ArcGIS.System
        // ESRI.ArcGIS.SystemUI
        // 
        // Intended ArcGIS Products for this snippet:
        // ArcGIS Desktop (ArcEditor, ArcInfo, ArcView)
        // 
        // Applicable ArcGIS Product Versions:
        // 9.2
        // 9.3
        // 9.3.1
        // 10.0
        // 
        // Required ArcGIS Extensions:
        // (NONE)
        // 
        // Notes:
        // This snippet is intended to be inserted at the base level of a Class.
        // It is not intended to be nested within an existing Method.
        // 

        ///<summary>Add a layer file (.lyr) into the active view when the path (on disk or network) is specified.</summary>
        ///      
        ///<param name="activeView">An IActiveview interface</param>
        ///<param name="layerPathFile">A System.String that is the path\filename of a layer file. Example: "C:\temp\mylayer.lyr".</param>
        ///      
        ///<remarks></remarks>
        public void AddLayerToActiveView(ESRI.ArcGIS.Carto.IActiveView activeView, System.String layerPathFile)
        {

            if (activeView == null || layerPathFile == null || !layerPathFile.EndsWith(".lyr"))
            {
                return;
            }

            // Create a new GxLayer
            ESRI.ArcGIS.Catalog.IGxLayer gxLayer = new ESRI.ArcGIS.Catalog.GxLayerClass();

            ESRI.ArcGIS.Catalog.IGxFile gxFile = (ESRI.ArcGIS.Catalog.IGxFile)gxLayer; //Explicit Cast

            // Set the path for where the layerfile is located on disk
            gxFile.Path = layerPathFile;

            // Test if we have a valid layer and add it to the map
            if (!(gxLayer.Layer == null))
            {
                ESRI.ArcGIS.Carto.IMap map = activeView.FocusMap;
                map.AddLayer(gxLayer.Layer);
            }
        }
#endregion
    }
}
>>>>>>> a733fe169500b9a9582ad48df12e6f3e86b79f11
