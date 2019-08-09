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
using ESRI.ArcGIS.Cadastral;
using ESRI.ArcGIS.GeoDatabaseExtensions;
using ESRI.ArcGIS.esriSystem;
using stdole;

namespace ArcCatalogFabricLib
{
    [Guid("19030566-739e-4587-ace3-b91e8b6de18c")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ArcCatalogFabricLib.cmdAttributeModify")]
    public class cmdAttributeModify : BaseCommand
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

        private IApplication m_application;
        private bool m_ReadOnlyIds;

        public cmdAttributeModify()
        {
            // Alter custom IDs defintion property to ReadOnly - in the time of table creation !!!
            m_ReadOnlyIds = true;
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "Fabric attributes customizing"; //localizable text
            base.m_caption = "Modify custom fields";  //localizable text
            base.m_message = "Modify custom fields";  //localizable text 
            base.m_toolTip = "Modify custom fields";  //localizable text 
            base.m_name = "MyFabricSetup_ArcCatalogCommand1";   //unique id, non-localizable (e.g. "MyCategory_ArcCatalogCommand")
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

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            m_application = hook as IApplication;
            /*
                        m_DocEvents = (IGxDocumentEvents) m_application.Document;

                        if (m_DocEvents != null)
                        {
                            m_DocEvents.OnContextMenu + =
                        }
            */
            //Disable if it is not ArcCatalog
            if (hook is IGxApplication)
                // TO DO: My check when fabric dataset selected
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
            IGxDataset pGxDataset;

            // TODO: Add cmdFabricModify.OnClick implementation
            if ((pGxDataset = GetSelectedTable()) != null)
            {
                UpdateCadastralTable(ExtractSchemaName(pGxDataset.Dataset), GetSelectedTable());
            }
        }

        public override bool Enabled
        {
            get
            {
                IGxApplication gxApp = m_application as IGxApplication;
                IGxDataset xDataset = null;
                esriDatasetType dsType = esriDatasetType.esriDTAny;

                if (gxApp != null)
                {
                    xDataset = gxApp.SelectedObject as IGxDataset;
                    if (xDataset != null) dsType = xDataset.Type;
                }

                return (dsType == esriDatasetType.esriDTTable);
            }
        }

        #endregion


        IGxDataset GetSelectedTable()
        {
            IGxApplication pApp = m_application as IGxApplication;
            IGxObject pGxObject = pApp.SelectedObject;
            IGxDataset pGxDataset = null;
            IGxDataset pGxTable = null;

            // Start with ArcSDE connection
            if (pGxObject.Parent.FullName.ToLower() == "database connections")
                Console.WriteLine("database node");
            if (pGxObject.Parent.FullName.ToLower().Contains("database connections"))
            {
                if (pGxObject is ESRI.ArcGIS.Catalog.IGxDataset)
                {
                    pGxDataset = (IGxDataset)pGxObject;
                    if (pGxDataset.Type  == esriDatasetType.esriDTTable)
                        pGxTable = pGxDataset;
                    //if (pUnk == null)
                    //    pUnk = (stdole.IUnknown) pApp.SelectedObject.InternalObjectName.Open();
                    //pGxDataset = (IGxDataset)pUnk;
                }
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Not a remote database");
            }

            return pGxTable;
        }

        void UpdateCadastralTable(String schemaUser, IGxDataset pGxDataset)
        {
            ITable pTable = (ITable) pGxDataset.Dataset;
            String[] MissingFldNames;

            if (CheckSchemaLock(pTable, schemaUser))
            {
                if (System.Windows.Forms.MessageBox.Show("Selected fabric tables are locked. Do you want to unlock them ?", "Setup Fabric", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
                ReleaseSchemaLock(pTable, schemaUser);
            }

            if (AttributeFieldChecker(pGxDataset.Dataset, out MissingFldNames))
            {
                UserVerifyFieldModification(pGxDataset, MissingFldNames);
            }
        }

        Boolean AttributeFieldChecker(IDataset inDataset, out String[] AttrSourceFields)
        {
            long lFieldLocation;
            ITable inObjectClass = (ITable) inDataset;
//            string CadastralTable = inDataset.Name;
            string strOwner = ExtractSchemaName(inDataset);
            string CadastralTable = inDataset.Name.Substring(strOwner.Length + 1).ToUpper();
            List<String> CustomFields = CadastralCustomAttributes(CadastralTable);
            Boolean result = true;



            AttrSourceFields = new String[CustomFields.Count];

            foreach (String CustomField in CustomFields)
            {
                int i = CustomFields.IndexOf(CustomField);
                lFieldLocation = inObjectClass.FindField(CustomField);
                if (lFieldLocation < 0)
                {
                    AttrSourceFields[i] = "MISSING----->[" + CustomField + "]";
                    result = false;
                }
                else
                {
                    AttrSourceFields[i] = "[" + CustomField + "] found";
                }

            }

            return result;
        }

        void UserVerifyFieldModification(IGxDataset inGxDataset, String[] MissingFldNames)
        {
            IDataset inDataset = inGxDataset.Dataset;
            String strMessage = "Required fields are about to be modified in the " + inDataset.Name + " table :\n\n";
            long lCnt = MissingFldNames.Length;

            for (int i = 0; i < lCnt; i++)
                strMessage += MissingFldNames[i] + '\n';

            if (System.Windows.Forms.MessageBox.Show(strMessage + '\n' + "Do you want to modify the fields and continue the process?", "Fabric Source Field Checker", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            else
            {
                if (!CadastralModifyFields(inDataset))
                {
                    System.Windows.Forms.MessageBox.Show("Failed to modify required fields for " + inDataset.Name + " table");
                    return;
                }
            }

        }

        Boolean CadastralModifyFields(IDataset inDataset)
        {
            ITable pTable = (ITable)inDataset;
            string strOwner = ExtractSchemaName(inDataset);
            string CadastralTable = inDataset.Name.Substring(strOwner.Length + 1).ToUpper();
            IFields pFields;
            //IFieldsEdit pFldsEd;
            IField pField;

            List<String> CustomFields = CadastralCustomAttributes(CadastralTable);

            switch (CadastralTable)
            {
                case "CONDOMINIUM":
                    CustomFields.Add("CONDO_ATTR_ID");
                    CustomFields.Add("PARCELID");
                    break;
                case "AROLL_ATTR":
                    CustomFields.Add("AROLL");
                    //CustomFields.Add("AROLL_ID");
                    CustomFields.Add("MP_REL_CODE");
                    break;
                case "CORRIDOR_ATTR":
                    CustomFields.Add("CORRIDOR_ATTR_ID");
                    CustomFields.Add("PARCELID");
                    CustomFields.Add("CORRIDOR_NAME_ID");
                    break;
                case "MUNICIPAL_PARCEL_ATTR":
                    CustomFields.Add("PARCELID");
                    CustomFields.Add("ADDRESS_ID");
                    break;
                case "MUNICIPAL_PARCEL_AROLL":
                    CustomFields.Add("MP_AROLL_ID");
                    //CustomFields.Add("PARCELID");
                    //CustomFields.Add("AROLL");
                    CustomFields.Add("AROLL_ID");
                    break;
                case "RESERVE_ATTR":
                    CustomFields.Add("RESERVE_ATTR_ID");
                    CustomFields.Add("PARCELID");
                    break;
            }

            CustomFields.Add("RECORD_ID");
            CustomFields.Add("TRANS_ID_CREATE");
            CustomFields.Add("TRANS_ID_EXPIRE");

            foreach (String CustomField in CustomFields)
            {
                if (pTable.FindField(CustomField) >= 0)
                {
                    // Modify Field
                    pFields = pTable.Fields;
                    pField = pFields.get_Field(pFields.FindField(CustomField));

                    if (CadastralTableModifyField(inDataset, pField))
                    {
                        //interface to alter the COM class extension for an object class.    
                        //cast for the IClassSchemaEdit      
                        //IObjectClass pObjectClass = (IObjectClass)pTable;
                        //IClassSchemaEdit3 classSchemaEdit = (IClassSchemaEdit3)pObjectClass;
                        //IFeatureWorkspaceSchemaEdit pFWSchemaEdit = (IFeatureWorkspaceSchemaEdit) inDataset.Workspace;
                        
                        ////set and exclusive lock on the class     
                        //ISchemaLock schemaLock = (ISchemaLock)inDataset;  
                        //schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);

                        //classSchemaEdit.AlterDefaultValue(pField.Name, pField.DefaultValue);

                        //pFWSchemaEdit.AlterInstanceCLSID(inDataset.Name, pObjectClass.CLSID);

                        //ESRI.ArcGIS.esriSystem.UID classUID = new ESRI.ArcGIS.esriSystem.UIDClass();
                        //classUID.Value = pObjectClass.CLSID;
                        //classSchemaEdit.AlterInstanceCLSID(classUID);


                        // Change the properties.
                        //IClassSchemaEdit2 classSchemaEdit = (IClassSchemaEdit2)pObjectClass;
                        //classSchemaEdit.AlterClassExtensionProperties(extensionProperties);

                      ////GUID for the C# project.
                      //classUID.Value = "{65a43962-8cc0-49c0-bfa3-015d0ff8350e}";
                      //classSchemaEdit.AlterClassExtensionCLSID(classUID, null);    
                      //release the exclusive lock     
                        
                        //schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);  
                    }

                    Marshal.ReleaseComObject(pField);
                }
            }

            Marshal.ReleaseComObject(pTable);
            return true;
        }

        List<String> CadastralCustomAttributes(string CadastralTable)
        {
            List<String> CustomFields = new List<String>();

            switch (CadastralTable)
            {
                case "CONDOMINIUM":
                    CustomFields.Add("CONDO_ATTR_ID");
                    CustomFields.Add("PARCELID");
                    break;
                case "AROLL_ATTR":
                    CustomFields.Add("AROLL");
                    //CustomFields.Add("AROLL_ID");
                    CustomFields.Add("MP_REL_CODE");
                    break;
                case "CORRIDOR_ATTR":
                    CustomFields.Add("CORRIDOR_ATTR_ID");
                    CustomFields.Add("PARCELID");
                    CustomFields.Add("CORRIDOR_NAME_ID");
                    break;
                case "MUNICIPAL_PARCEL_ATTR":
                    CustomFields.Add("PARCELID");
                    CustomFields.Add("ADDRESS_ID");
                    break;
                case "MUNICIPAL_PARCEL_AROLL":
                    CustomFields.Add("MP_AROLL_ID");
                    CustomFields.Add("PARCELID");
                    //CustomFields.Add("AROLL");
                    //CustomFields.Add("AROLL_ID");
                    break;
                case "RESERVE_ATTR":
                    CustomFields.Add("RESERVE_ATTR_ID");
                    CustomFields.Add("PARCELID");
                    break;
            }


            CustomFields.Add("RECORD_ID");
            CustomFields.Add("TRANS_ID_CREATE");
            CustomFields.Add("TRANS_ID_EXPIRE");

            return CustomFields;
        }

        bool CadastralTableModifyField(IDataset pCadaTab, IField pModifyField)
        {
            bool result = false;
            ESRI.ArcGIS.Geodatabase.ITable pTable = (ESRI.ArcGIS.Geodatabase.ITable)pCadaTab;
            //IClass pObjectClass = (IClass)pTable;
            IObjectClass pObjectClass = (IObjectClass)pTable;
            IClassSchemaEdit3 pClassEdit = (IClassSchemaEdit3)pObjectClass;
            ISchemaLock pSchLock = (ISchemaLock)pObjectClass;
            int lFieldLocation;

            lFieldLocation= pObjectClass.FindField(pModifyField.Name);
            //lFieldLocation = pTable.FindField(pModifyField.Name);
            if (lFieldLocation >= 0)
            {
                IField pField2 = pObjectClass.Fields.get_Field(lFieldLocation);
                //IField2 pField2 = (IField2) pModifyField;
                IFieldEdit2 pFieldEdit = (ESRI.ArcGIS.Geodatabase.IFieldEdit2)pField2;


                try
                {
                    // set the exclusive lock
                    pSchLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message, "Failed to lock table");
                    return result;
                }

                try
                {
                    /// "Editable" field property setting:
                    /// WARNING: only work when a new table created
                    /// Cannot change field property after table has been created !!!
                    if (m_ReadOnlyIds)
                    {
                        if (pFieldEdit.Editable)
                        {
                            pFieldEdit.Editable_2 = false;
                            pClassEdit.AlterFieldAliasName(pField2.Name,pField2.AliasName+"_test");
                            result = true;
                        }
                    }
                    else
                    {
                        if (!pFieldEdit.Editable)
                        {
                            pFieldEdit.Editable_2 = true;
                            pClassEdit.AlterFieldAliasName(pField2.Name, pField2.AliasName+"_test");
                            result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                    //System.Diagnostics.Trace.WriteLine(ex.Message, "Failed to Modify field");
                }
                finally
                {
                    pSchLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                    Marshal.ReleaseComObject(pField2);
                }
            }

            return result;
        }
        
        Boolean CheckSchemaLock(ITable pTable, String schemaUser)
        {
            ISchemaLock pSchLock = (ISchemaLock)pTable;
            IEnumSchemaLockInfo pEnumSchLockInfo;
            ISchemaLockInfo pTableLockInfo;
            IObjectClass pObject = (IObjectClass)pTable;

            pSchLock.GetCurrentSchemaLocks(out pEnumSchLockInfo);
            pTableLockInfo = pEnumSchLockInfo.Next();

            while (pTableLockInfo != null)
            {
                if ((pTableLockInfo.UserName.ToLower() == schemaUser.ToLower()) &&
                    (pTableLockInfo.TableName.ToUpper().Contains(pObject.AliasName.ToUpper())))
                {
                    // Check exclusive LOCK status
                    if (pTableLockInfo.SchemaLockType == esriSchemaLock.esriExclusiveSchemaLock)
                    {
                        return true;
                    }
                }
                pTableLockInfo = pEnumSchLockInfo.Next();
            }
            return false;
        }

        void ReleaseSchemaLock(ITable pTable, String schemaUser)
        {
            //************* Fix Lock ***************
            ISchemaLock pSchLock = (ISchemaLock)pTable;
            IEnumSchemaLockInfo pEnumSchLockInfo;
            ISchemaLockInfo pTableLockInfo;
            IObjectClass pObject = (IObjectClass)pTable;

            pSchLock.GetCurrentSchemaLocks(out pEnumSchLockInfo);

            pTableLockInfo = pEnumSchLockInfo.Next();
            while (pTableLockInfo != null)
            {
                if ((pTableLockInfo.UserName.ToLower() == schemaUser.ToLower()) &&
                    (pTableLockInfo.TableName.ToUpper().Contains(pObject.AliasName.ToUpper())))
                {
                    // Check exclusive LOCK status
                    if (pTableLockInfo.SchemaLockType == esriSchemaLock.esriExclusiveSchemaLock)
                    {
                        try
                        {
                            // release the exclusive lock
                            pSchLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLine(ex.Message, "Failed to unlock table");
                        }
                    }
                }
                pTableLockInfo = pEnumSchLockInfo.Next();
            }
        }

        String ExtractSchemaName(IDataset inDataset)
        {
            ISQLSyntax pSQLSyntax;
            String strDbName, strOwnerName, strName;

            pSQLSyntax = (ISQLSyntax)inDataset.Workspace;
            pSQLSyntax.ParseTableName(inDataset.Name,
                                      out strDbName,
                                      out strOwnerName,
                                      out strName);

            return strOwnerName;
        }

    }

}
