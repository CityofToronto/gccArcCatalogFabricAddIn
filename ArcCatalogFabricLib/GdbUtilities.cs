using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcCatalog;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesOleDB;
using ESRI.ArcGIS.Cadastral;
using ESRI.ArcGIS.GeoDatabaseExtensions;
using ESRI.ArcGIS.esriSystem;
using stdole;

namespace ArcCatalogFabricLib
{
    static class FabricDataDefinition
    {
        public const string GDB_DIRECT_CONNECT_KEYWORD = "sde:oracle11g:";
        public const string GDB_DIRECT_CONNECT_KEYWORD1 = "sde:oracle:";
        public const string GDB_DEFAULT_VERSION_NAME = "SDE.DEFAULT";
        public const string FABRIC_NAME = "CADASTRAL"; // default
        public const string CADAST_SCHEMA = "CADAST";  // default

        public const string GDB_DOMAIN_PLANTYPE = "PlanType";
        public const string GDB_DOMAIN_PLANPARCEL = "PlanParcel";
        public const string GDB_DOMAIN_MUNICIPALPARCEL = "MunicipalParcel";

        public const string GDB_SUBTYPE_PLANPARCEL = "Plan Parcel";
        public const string GDB_SUBTYPE_MUNICIPALPARCEL = "Municipal Parcel";
        public const string GDB_SUBTYPE_SUBDIVISIONPARCEL = "Subdivision fabric";
        public const string GDB_SUBTYPE_PRIVATEROADPARCEL = "Private Road";
        public const string GDB_SUBTYPE_EASEMENTPARCEL = "Easement";

        // Feature type coding
        public const string SUBDIVISION_FABRIC_FEATURE = "GEOGRAPHY";
        public const string PLAN_SUBDIVISION_FEATURE = "SUBDIVISION";
        public const string PLAN_REFERENCE_FEATURE = "REFERENCE";
        public const string PLAN_CONDO_FEATURE = "CONDOPLAN";
        public const string PLAN_OTHER_FEATURE = "OTHER";
        public const string MUNICIPAL_PARCEL_FEATURE = "COMMON";
        public const string CONDO_PARCEL_FEATURE = "CONDO";
        public const string CORRIDOR_PARCEL_FEATURE = "CORRIDOR";
        public const string RESERVE_PARCEL_FEATURE = "RESERVE";
        public const string PRIVATEROAD_PARCEL_FEATURE = "PRIVATE_ROAD";
        public const string EASEMENT_PARCEL_FEATURE = "EASEMENT";


        public const string MP_ATTR_TABLE = "MUNICIPAL_PARCEL_ATTR";
        public const string MP_AROLL_ATTR_TABLE = "MUNICIPAL_PARCEL_AROLL";
        public const string CORR_ATTR_TABLE = "CORRIDOR_ATTR";
#if _old_definition_
        public const string CONDO_ATTR_TABLE = "CONDOMINIUM";  //TODO: must be changed in production
#else
        public const string CONDO_ATTR_TABLE = "CONDO_ATTR";
#endif
        public const string RES_ATTR_TABLE = "RESERVE_ATTR";
        public const string PRRD_ATTR_TABLE = "PRIVATE_ROAD_ATTR";

        public const string PLAN_UNKNOWN = "UNKNOWN";

        public const string PLAN_TYPE_GROUP_FIELD = "PLAN_TYPEGRP";
        public const string PARCEL_ID_FIELD = "PARCELID";
        public const string PARCEL_TYPE_FIELD = "TYPE";
        public const string PARCEL_FEATURE_TYPE_FIELD = "FEATURE_TYPE";
        public const string PARCEL_CONSTRUCTION_FIELD = "CONSTRUCTION";
        public const string CONDO_ID_FIELD = "CONDO_ATTR_ID";
        public const string CORRIDOR_ID_FIELD = "CORRIDOR_ATTR_ID";
        public const string CORRIDOR_NAME_ID_FIELD = "CORRIDOR_NAME_ID";
        public const string RESERVE_ID_FIELD = "RESERVE_ATTR_ID";
        public const string MP_AROLL_ID_FIELD = "MP_AROLL_ID";
        public const string AROLL_FIELD = "AROLL";
        public const string AROLL_ID_FIELD = "AROLL_ID";
        public const string PR_ROAD_ID_FIELD = "PRIVATE_ROAD_ATTR_ID";
        public const string PARCEL_RECORD_ID_FIELD = "RECORD_ID";

        public const string IGE_TRANS_CREATE_ID_FIELD = "TRANS_ID_CREATE";
        public const string IGE_TRANS_DELETE_ID_FIELD = "TRANS_ID_EXPIRE";

    }
    static class GdbHelper
    {
        public static IGxDataset GetSelectedDataset(object m_application)
        {
            IGxApplication pApp = m_application as IGxApplication;
            IGxObject pGxObject = pApp.SelectedObject;
            IGxDataset pGxDataset = null;

            // Start with ArcSDE connection
            //if (pGxObject.Parent.FullName.ToLower() == "database connections")
            //    Console.WriteLine("database node");
            if (pGxObject.Parent.FullName.ToLower().Contains("database connections"))
            {
                if (pGxObject is ESRI.ArcGIS.Catalog.IGxDataset)
                {
                    pGxDataset = (IGxDataset)pGxObject;
                    //if (pUnk == null)
                    //    pUnk = (stdole.IUnknown) pApp.SelectedObject.InternalObjectName.Open();
                    //pGxDataset = (IGxDataset)pUnk;
                }
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Not a remote database");
            }

            return pGxDataset;
        }

        public static string OracleServiceName(IWorkspace parent)
        {
            string oracle_tns;
            IPropertySet properties = parent.ConnectionProperties;
            GetOracleTNSbyWorkspace(properties, out oracle_tns);
            return oracle_tns;
        }
        public static string OracleServiceName(IPropertySet properties)
        {
            string oracle_tns;
            GetOracleTNSbyWorkspace(properties, out oracle_tns);
            return oracle_tns;
        }
        public static IWorkspace OpenSqlWorkspace(IApplication p_application, IWorkspace parent, string user, string password)
        {
            return OpenOLEDBLWorkspaceInProc(p_application, parent, user, password);
        }
        public static ISqlWorkspace OpenSqlWorkspace1(IApplication p_application, IWorkspace parent, string user)
        {
            return OpenSQLWorkspaceInProc(p_application, parent, user);
        }
        public static IWorkspace OpenUserWorkspace(IApplication p_application, IWorkspace parent, string user)
        {
            return OpenArcSDEWorkspaceInProc(p_application, parent, user);
        }
        public static IWorkspace OpenUserWorkspace(IApplication p_application, IWorkspace parent, string user, string password)
        {
            return OpenArcSDEWorkspaceInProc(p_application, parent, user, password);
        }

        private static IWorkspaceFactory2 GetWorkspaceFactoryByApplication(IApplication p_application, string factory_type)
        {
            IObjectFactory objFactory = p_application as IObjectFactory; // Access ArcGIS application's process space !!!

            if ((factory_type == "esriDataSourcesGDB.SdeWorkspaceFactory") ||
                (factory_type == "esriDataSourcesGDB.SqlWorkspaceFactory") ||
                (factory_type == "esriDataSourcesOleDB.OLEDBWorkspaceFactory"))
            {
                Type t = Type.GetTypeFromProgID(factory_type);
                //string typeClsID = t.GUID.ToString("B");
                return (IWorkspaceFactory2)objFactory.Create(t.GUID.ToString("B"));
            }
            else
                return null;
        }

        private static IWorkspaceFactory2 GetWorkspaceFactoryByApplication(IApplication p_application)
        {
            IObjectFactory objFactory = p_application as IObjectFactory; // Access ArcGIS application's process space !!!
            Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
            //string typeClsID = t.GUID.ToString("B");
            return (IWorkspaceFactory2)objFactory.Create(t.GUID.ToString("B"));
        }
        private static IWorkspaceFactory2 GetWorkspaceFactoryByApplication1_(IApplication p_application)
        {
            IObjectFactory objFactory = p_application as IObjectFactory; // Access ArcGIS application's process space !!!
            Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.SqlWorkspaceFactory");
            //string typeClsID = t.GUID.ToString("B");
            return (IWorkspaceFactory2)objFactory.Create(t.GUID.ToString("B"));
        }
        private static IWorkspaceFactory2 GetWorkspaceFactoryByApplication2(IApplication p_application)
        {
            IObjectFactory objFactory = p_application as IObjectFactory; // Access ArcGIS application's process space !!!
            Type t = Type.GetTypeFromProgID("esriDataSourcesOleDB.OLEDBWorkspaceFactory");
            //string typeClsID = t.GUID.ToString("B");
            return (IWorkspaceFactory2)objFactory.Create(t.GUID.ToString("B"));
        }

        private static IWorkspace OpenArcSDEWorkspaceInProc(IApplication p_application, IWorkspace parent, string user)
        {
            return OpenArcSDEWorkspaceInProc(p_application, parent, user, parent.ConnectionProperties.GetProperty("PASSWORD"), "SDE.DEFAULT");
        }
        private static IWorkspace OpenArcSDEWorkspaceInProc(IApplication p_application, IWorkspace parent, string user, string password)
        {
            return OpenArcSDEWorkspaceInProc(p_application, parent, user, password, "SDE.DEFAULT");
        }
        private static IWorkspace OpenArcSDEWorkspaceInProc(IApplication p_application, IWorkspace parent, string user, object password, string version)
        {
            IWorkspace workspace = null;

            if (password.ToString().Length == 0)
                return null;

            IPropertySet pConnectionProperties = new PropertySetClass();

            pConnectionProperties.SetProperty("SERVER", parent.ConnectionProperties.GetProperty("SERVER").ToString());
            pConnectionProperties.SetProperty("INSTANCE", parent.ConnectionProperties.GetProperty("INSTANCE").ToString());
            pConnectionProperties.SetProperty("USER", user);
            pConnectionProperties.SetProperty("PASSWORD", password);
            pConnectionProperties.SetProperty("VERSION", version);

            //It creates an object in the target application process space 
            IWorkspaceFactory2 workspaceFactory = GetWorkspaceFactoryByApplication(p_application);

            // IWorkspaceFactory2 workspaceFactory = new ESRI.ArcGIS.Geodatabase.WorkspaceFactoryClass();

            try
            {
                workspace = workspaceFactory.Open(pConnectionProperties, 0);
            }
            catch (Exception e)
            {
                if (workspace != null)
                    Marshal.ReleaseComObject(workspace);
#if debug
                if (e.Message.ToLower() == "operation failed")
                    System.Diagnostics.Debug.WriteLine("ArcSDE failed to create client process");
                else
                    System.Diagnostics.Debug.WriteLine("Exception in OpenArcSDEWorkspaceInProc: " + e.Message);
#endif
            }
            finally
            {
                Marshal.ReleaseComObject(workspaceFactory);
            }
            return workspace;
        }

        private static ISqlWorkspace OpenSQLWorkspaceInProc(IApplication p_application, IWorkspace parent, string user)
        {
            return OpenSQLWorkspaceInProc(p_application, parent, user, parent.ConnectionProperties.GetProperty("PASSWORD"));
        }
        //private static IWorkspace OpenOLEDBLWorkspaceInProc(IApplication p_application, IWorkspace parent, string user, string password)
        //{
        //    return OpenOLEDBLWorkspaceInProc(p_application, parent, user, password);
        //}

        private static IWorkspace OpenOLEDBLWorkspaceInProc(IApplication p_application, IWorkspace parent, string user, object password)
        {
            IWorkspace workspace = null;

            if (password.ToString().Length == 0)
                return null;

            IPropertySet pConnectionProperties = new PropertySetClass();
            //pConnectionProperties.SetProperty("CONNECTSTRING", "Provider=MSDAORA.1;Data source=mydatabase;User ID=oledb;Password=oledb");
            string sConnectionString = "Provider=OraOLEDB.Oracle;" +
                       "Data Source=" + parent.ConnectionProperties.GetProperty("SERVER").ToString() +
                       ";User ID=" + user +
                       ";Password=" + password +
                       ";PLSQLRSet=true;";
            pConnectionProperties.SetProperty("CONNECTSTRING", sConnectionString);

            //It creates an object in the target application process space 
            IWorkspaceFactory2 workspaceFactory = GetWorkspaceFactoryByApplication2(p_application);

            // IWorkspaceFactory2 workspaceFactory = new ESRI.ArcGIS.Geodatabase.WorkspaceFactoryClass();

            try
            {
                workspace = workspaceFactory.Open(pConnectionProperties, 0);
            }
            catch (Exception e)
            {
                if (workspace != null)
                    Marshal.ReleaseComObject(workspace);
                workspace = null;
#if debug
                if (e.Message.ToLower() == "operation failed")
                    System.Diagnostics.Debug.WriteLine("ArcSDE failed to create client process");
                else
                    System.Diagnostics.Debug.WriteLine("Exception in OpenArcSDEWorkspaceInProc: " + e.Message);
#endif
            }
            finally
            {
                Marshal.ReleaseComObject(workspaceFactory);
            }
            return workspace;
        }

        /// <summary>
        /// WARNING! ESRI changed SqlWorskspaceFactory class. The inrefaces implemanted by SdeWorkspaceFactory
        /// </summary>
        /// <param name="p_application"></param>
        /// <param name="parent"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static ISqlWorkspace OpenSQLWorkspaceInProc(IApplication p_application, IWorkspace parent, string user, object password)
        {
            ISqlWorkspace workspace = null;

            if (password.ToString().Length == 0)
                return null;

            IPropertySet pConnectionProperties = new PropertySetClass();

            pConnectionProperties.SetProperty("DBCLIENT", "oracle");
            pConnectionProperties.SetProperty("DB_CONNECTION_PROPERTIES", parent.ConnectionProperties.GetProperty("SERVER").ToString());
            // pConnectionProperties.SetProperty("SERVERINSTANCE", parent.ConnectionProperties.GetProperty("SERVER").ToString());
            // pConnectionProperties.SetProperty("INSTANCE", parent.ConnectionProperties.GetProperty("INSTANCE").ToString());
            pConnectionProperties.SetProperty("USER", user);
            pConnectionProperties.SetProperty("PASSWORD", password);
            // pConnectionProperties.SetProperty("VERSION", version);
            pConnectionProperties.SetProperty("AUTHENTICATION_MODE", "DBMS");

            //It creates an object in the target application process space 
            IWorkspaceFactory2 workspaceFactory = GetWorkspaceFactoryByApplication(p_application);

            // IWorkspaceFactory2 workspaceFactory = new ESRI.ArcGIS.Geodatabase.WorkspaceFactoryClass();

            try
            {
                // workspace = workspaceFactory.Open(pConnectionProperties, 0);
                workspace = workspaceFactory.Open(pConnectionProperties, 0) as ISqlWorkspace;
                IDatabaseConnectionInfo2 conn = (IDatabaseConnectionInfo2)workspace;
                string db = conn.ConnectedDatabase;
                string db_user = conn.ConnectedUser;
                esriConnectionDBMS db_dbms = conn.ConnectionDBMS;
                if (db_dbms != esriConnectionDBMS.esriDBMS_Oracle)
                {
                    Marshal.ReleaseComObject(workspace);
                    workspace = null;
                }
                

            }
            catch (Exception e)
            {
                if (workspace != null)
                    Marshal.ReleaseComObject(workspace);
#if debug
                if (e.Message.ToLower() == "operation failed")
                    System.Diagnostics.Debug.WriteLine("ArcSDE failed to create client process");
                else
                    System.Diagnostics.Debug.WriteLine("Exception in OpenArcSDEWorkspaceInProc: " + e.Message);
#endif
            }
            finally
            {
                Marshal.ReleaseComObject(workspaceFactory);
            }
            return workspace;
        }

        private static void GetOracleTNSbyWorkspace(IPropertySet pConnectionProperties, out string oracle_service)
        {
            oracle_service = "";
            string instance = pConnectionProperties.GetProperty("INSTANCE") as string;

            if (IsDirectConnection(instance))
                oracle_service = GetOracleServiceName(instance);

            //System.Diagnostics.Debug.WriteLine("vesrion: <{0}>", pConnectionProperties.GetProperty("VERSION").ToString());
        }

        private static Boolean IsDirectConnection(string instance)
        {
            return instance.StartsWith(FabricDataDefinition.GDB_DIRECT_CONNECT_KEYWORD) ||
                instance.StartsWith(FabricDataDefinition.GDB_DIRECT_CONNECT_KEYWORD1);
        }

        private static string GetOracleServiceName(string connection)
        {
            string[] dc_string = connection.Split(':');
            if (dc_string.Length == 3)
                return dc_string[2];
            else
                return "";
        }

        public static IDataset OpenCadastralDataset(IWorkspace workspace, string datasetName, string fabricName)
        {
            string ds_fabricName;
            IDatasetContainer3 datasetContainer3 = GetCadastralContainer(workspace, datasetName, out ds_fabricName);

            if (datasetContainer3 == null)
                return null;

            IDataset dataset = null;
            //return datasetContainer3.get_DatasetByName(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTCadastralFabric, fabricName);
            if (ds_fabricName.ToUpper() == fabricName.ToUpper())
            {
                dataset = datasetContainer3.get_DatasetByName(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTCadastralFabric, fabricName);
            }

            Marshal.ReleaseComObject(datasetContainer3);
            return dataset;
        }
        private static IDatasetContainer3 GetCadastralContainer(IWorkspace workspace, string datasetName, out string fabricName)
        {
            IDatasetContainer3 datasetContainer3 = null;
            fabricName = "";

            // SDE Geodatabase CadastralFabric dataset workspace
            IFeatureWorkspace featureWorkspace = (ESRI.ArcGIS.Geodatabase.IFeatureWorkspace)workspace;
            IFeatureDataset featureDataset = featureWorkspace.OpenFeatureDataset(datasetName);
            IEnumDataset enumDS = featureDataset.Subsets;
            IDataset test = null;

            enumDS.Reset();

            Boolean found = false;

            while (((test = enumDS.Next()) != null) && !found)
            {

                if (test.Type == esriDatasetType.esriDTCadastralFabric)
                {
                    fabricName = test.Name;
                    found = true;
                }

            }

            if (found)
            {
                IFeatureDatasetExtensionContainer featureDatasetExtensionContainer = featureDataset as ESRI.ArcGIS.Geodatabase.IFeatureDatasetExtensionContainer; // Dynamic Cast
                IFeatureDatasetExtension featureDatasetExt = featureDatasetExtensionContainer.FindExtension(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTCadastralFabric);
                datasetContainer3 = featureDatasetExt as ESRI.ArcGIS.Geodatabase.IDatasetContainer3; // Dynamic Cast
            }

            Marshal.ReleaseComObject(featureDataset);
            return datasetContainer3;
        }
    }
}
