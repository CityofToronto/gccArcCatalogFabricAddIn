<<<<<<< HEAD
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
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
using stdole;



namespace ArcCatalogFabricLib
{
    /// <summary>
    /// Summary description for cmdFabricModify.
    /// </summary>
    [Guid("6e01696a-4794-4051-9486-20ac3835718c")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ArcCatalogFabricLib.cmdFabricModify")]
    public sealed class cmdFabricModify : BaseCommand
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

        private string  MyFabric = "CadastralFabric";
        //private IGxDocumentEvents m_DocEvents;
        stdole.IUnknown pUnk;
        ICadastralFabricSchemaEdit schemaEdit = null;
        private frmOptions MyOptForm; // My UI

        private IApplication m_application;

        public cmdFabricModify()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "Fabric dataset customizing"; //localizable text
            base.m_caption = "Add custom fields";  //localizable text
            base.m_message = "Add custom fields";  //localizable text 
            base.m_toolTip = "Add custom fields";  //localizable text 
            base.m_name = "MyFabricSetup_ArcCatalogCommand";   //unique id, non-localizable (e.g. "MyCategory_ArcCatalogCommand")
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
            MyOptForm = new frmOptions();
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add cmdFabricModify.OnClick implementation
            // Application.Run(optDialog);
            MyOptForm.ShowDialog();

            if ((!MyOptForm.mCancelChange) && !MyOptForm.NoneSelected)
            {
               SetupFabric();
            }

            //MyOptForm.Hide();
        }

        public override bool Enabled
        {
            get
            {
                IGxApplication gxApp = m_application as IGxApplication;
				IGxDataset gxDataset = null;
                esriDatasetType dsType = esriDatasetType.esriDTAny;
                pUnk = null;

                if (gxApp != null)
                {
                    gxDataset = gxApp.SelectedObject as IGxDataset;
                    if (gxDataset != null) dsType = gxDataset.Type;
                    //pUnk = (stdole.IUnknown) gxApp.SelectedObject.InternalObjectName.Open();
                }

                return (dsType == esriDatasetType.esriDTCadastralFabric);
            }
        }

        #endregion

       
        Boolean CadastralFabricDE(IFeatureDataset pFDS)
        {
            IFeatureDatasetExtensionContainer pFDSExtCont;
            IFeatureDatasetExtension pFDSExt;
            IDatasetContainer2 pDSCont;
            IDataset pDataset;
            ICadastralFabric pCadastralDataset;
            IDatasetComponent pDatasetComponent;
            IDEDataset pDEDS;
            IDECadastralFabric pFabricDE;
  
            pFDSExtCont = (IFeatureDatasetExtensionContainer) pFDS;
            pFDSExt = pFDSExtCont.FindExtension(esriDatasetType.esriDTCadastralFabric);
            pDSCont = (IDatasetContainer2) pFDSExt;
            pDataset = pDSCont.get_DatasetByName(esriDatasetType.esriDTCadastralFabric, MyFabric);
            pCadastralDataset = (ICadastralFabric) pDataset;
            if (pCadastralDataset != null) 
            {
                pDatasetComponent = (IDatasetComponent)pCadastralDataset;
                pDEDS = pDatasetComponent.DataElement;
                pFabricDE = (IDECadastralFabric) pDEDS;
                return true;
            }
            else
                return false;
        }


        void SetupFabric()
        {
            //IGxDatabase pGxDatabase;
            IGxDataset  pGxDataset;
            //String      schemaUser;

            //if (!TestCadastralExtension()) return;
            // Get current database connection
            //if  (!TestSelectedItem()) return;

            if ((pGxDataset=GdbHelper.GetSelectedDataset(m_application)) != null)
            {
                if (pGxDataset.Type == esriDatasetType.esriDTFeatureDataset)
                {
                    System.Windows.Forms.MessageBox.Show("Select fabric dataset");
                }
                else
                {
                    if (pGxDataset.Type == esriDatasetType.esriDTCadastralFabric)
                    {
                        if (System.Windows.Forms.MessageBox.Show("Do you want to alter fabric dataset","Setup Fabric",MessageBoxButtons.YesNo) == DialogResult.No)
                            return;

                        // Do change
                        UpdateFabricSchema(ExtractSchemaName(pGxDataset.Dataset), (ICadastralFabric) pGxDataset.Dataset);
                    }
                }
            }

        }

        Boolean TestCadastralExtension()
        {
            return true;
        }
        Boolean TestSelectedItem()
        {
            IGxApplication pApp = (IGxApplication) m_application;
            IGxObject pGxObject = pApp.SelectedObject;
            IGxDatabase2 pGxDatabase;
            //IGxGeodatabase pGxGDB;
            //IGxRemoteConnection pGxRemoteConn;
            IGxDataset pGxDataset;
            //String[] sMissingFldNames;

            if (pGxObject != null)
            {
                //Exit if common branch of catalog tree
                if ((pGxObject.FullName.ToLower() == "catalog") 
                    || (pGxObject.BaseName.ToLower() == "catalog")) {
                    System.Windows.Forms.MessageBox.Show("Select database");
                    return false;
                }
                if (pGxObject.Parent.FullName.ToLower() == "database connections")
                {
                    if (pGxObject is IGxDatabase)
                    {
                        pGxDatabase = (IGxDatabase2) pGxObject;
                        if (pGxDatabase.IsConnected)
                            System.Windows.Forms.MessageBox.Show("Select dataset");
                        else
                        {
                            //Try to connect
                            // TO DO: cnage mouse status
                            pGxDatabase.Connect();
                        }
                    }
                    return false;
                }
                if (pGxObject.Parent.FullName.ToLower().Contains("database connections"))
                {
                    if (pGxObject is IGxDataset)
                    {
                        pGxDataset = (IGxDataset) pGxObject;
                        if (pGxDataset.Type == esriDatasetType.esriDTFeatureDataset)
                        {
                            pGxDatabase = (IGxDatabase2) pGxObject.Parent;
                        }
                    }
                    return true;
                }
            }

            return false;
        }
        Boolean InitializeExtension()
        {
            ESRI.ArcGIS.esriSystem.IExtensionManager pExtMgr;
            ESRI.ArcGIS.esriSystem.IExtension pExt;
            int i;
            //  Set m_pApp = Application
            pExtMgr = (ESRI.ArcGIS.esriSystem.IExtensionManager) m_application;

            for (i = 0; i < pExtMgr.ExtensionCount; i++)
            {
                pExt = pExtMgr.get_Extension(i);
                Console.WriteLine("Extension name: " + pExt.Name);
            }
  
            /*
            pExt = m_application.FindExtensionByName(SURVEY_EXT$);
            if (pExt != null)
            {
                ESRI.ArcGIS.esriSystem.IExtensionConfig pExtConfig;
                pExtConfig = (ESRI.ArcGIS.esriSystem.IExtensionConfig) pExt;
                if (pExtConfig.State != ESRI.ArcGIS.esriSystem.esriExtensionState.esriESUnavailable)
                {
                    pExtConfig.State = ESRI.ArcGIS.esriSystem.esriExtensionState.esriESEnabled;
                    return true;
                }
    //'Set m_pCadastEdExtension = m_pApp.FindExtensionByName(CA_FABRIC_EXT$)
    //'Set m_pCadastEdExtensionMananager = m_pCadastEdExtension 'set cadastral extension manager to ICadastralEditor
    //'InitializeExtension = Not m_pCadastEdExtension Is Nothing
            }
             */
            return false;
        }

        void UpdateFabricSchema(String schemaUser, ICadastralFabric pCadastralDataset)
        {
            ITable pTable = pCadastralDataset.get_CadastralTable(esriCadastralFabricTable.esriCFTPlans);
            String[] MissingFldNames;

            if (CheckSchemaLock(pTable, schemaUser))
            {
                if (System.Windows.Forms.MessageBox.Show("Selected fabric tables are locked. Do you want to unlock them ?", "Setup Fabric", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
                ReleaseSchemaLock(pTable, schemaUser);
            }

            System.Windows.Forms.MessageBox.Show("Cadastral dataset version : " + GetFabricVersion((ICadastralFabric2)pCadastralDataset));

            IDatasetComponent dsComponent = (IDatasetComponent)pCadastralDataset;
            IDECadastralFabric dataElement1 = (IDECadastralFabric)dsComponent.DataElement;
            schemaEdit = (ICadastralFabricSchemaEdit) pCadastralDataset;

            //************ Validate that the correct fields are Present  ***************
            if (MyOptForm.Plans)
            {
                if (!FabricFieldChecker(pCadastralDataset, out MissingFldNames, esriCadastralFabricTable.esriCFTPlans))
                    UserVerifyFieldCreation(pCadastralDataset, MissingFldNames, esriCadastralFabricTable.esriCFTPlans);
            }

            if (MyOptForm.Parcels)
            {
                if (!FabricFieldChecker(pCadastralDataset, out MissingFldNames, esriCadastralFabricTable.esriCFTParcels))
                    UserVerifyFieldCreation(pCadastralDataset, MissingFldNames, esriCadastralFabricTable.esriCFTParcels);
            }

            if (MyOptForm.ControlPoints)
            {
                if (!FabricFieldChecker(pCadastralDataset, out MissingFldNames, esriCadastralFabricTable.esriCFTControl))
                    UserVerifyFieldCreation(pCadastralDataset, MissingFldNames, esriCadastralFabricTable.esriCFTControl);
            }

            if (schemaEdit != null)
                schemaEdit.UpdateSchema(dataElement1);
        }

        Boolean FabricFieldChecker(ICadastralFabric inFabricDataset, out String[] FabricSourceFields, esriCadastralFabricTable FabricMember)
        {
            long lFieldLocation;
            ITable inObjectClass = inFabricDataset.get_CadastralTable(FabricMember);
            List<String> CustomFields =new List<String>();
            Boolean result = true;

            switch(FabricMember) {
                case esriCadastralFabricTable.esriCFTPlans:
                    CustomFields.Add("TYPE");
                    CustomFields.Add("SOURCE_ID");
                    //CustomFields.Add("SOURCE_NAME_ID");
                    CustomFields.Add("PLAN_CATEGORY");
                    CustomFields.Add("PLAN_STATUS");
                    CustomFields.Add("PLAN_TYPEGRP");
                    CustomFields.Add("RECORD_ID");
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
                case esriCadastralFabricTable.esriCFTParcels:
                    CustomFields.Add("PARCELID");
                    CustomFields.Add("RECORD_ID");
                    CustomFields.Add("FEATURE_TYPE");
                    //CustomFields.Add("INSTRUMENT_NUMBER");
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
                case esriCadastralFabricTable.esriCFTControl:
                    CustomFields.Add("POINT_TYPE");
                    CustomFields.Add("STATN_NUM");
                    CustomFields.Add("STATN_NUM_COS");
                    CustomFields.Add("STATN_STATS");
                    CustomFields.Add("STATN_DESCRIPTION");
                    CustomFields.Add("MONMN_ORDER");
                    CustomFields.Add("NAD27_SCALE_FACTOR");
                    CustomFields.Add("LOCTN_DESCRIPTION");
                    CustomFields.Add("INSPE_DATE");
                    CustomFields.Add("ORIGINAL_DATUM_DEF");
                    CustomFields.Add("ORIGINAL_X");
                    CustomFields.Add("ORIGINAL_Y");
                    CustomFields.Add("ORIGINAL_Z");
                    CustomFields.Add("SURVEYEPOCH");
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
                default:
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
            }

            FabricSourceFields = new String[CustomFields.Count];

            foreach (String CustomField in CustomFields)
            {
                int i = CustomFields.IndexOf(CustomField);
                lFieldLocation = inObjectClass.FindField(CustomField);
                if (lFieldLocation < 0)
                {
                    FabricSourceFields[i] = "MISSING----->[" + CustomField + "]";
                    result = false;
                }
                else
                {
                    FabricSourceFields[i] = "[" + CustomField + "] found";
                }

            }
            return result;
        }

        void UserVerifyFieldCreation(ICadastralFabric inFabricDataset, String[] MissingFldNames, esriCadastralFabricTable FabricMember)
        {
            IDataset pDataset = (IDataset)inFabricDataset;
            String strMessage = "Required fields are missing from the " + pDataset.Name + " fabric dataset :\n\n";
            long lCnt = MissingFldNames.Length;

            for (int i=0; i < lCnt; i++)
                strMessage += MissingFldNames[i] + '\n';

            if (System.Windows.Forms.MessageBox.Show(strMessage + '\n' + "Do you want to create the missing fields and continue the process?","Fabric Source Field Checker",MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            else {
                if (!FabricAddFields(inFabricDataset, FabricMember)) {
                    System.Windows.Forms.MessageBox.Show("Failed to add required fields for " + pDataset.Name);
                    return;
                }
            }
        }

        Boolean FabricAddFields(ICadastralFabric inFabricDataset, esriCadastralFabricTable FabricMember)
        {
            IDataset pDataset = (IDataset)inFabricDataset;
            ITable inObjectClass = (ITable) inFabricDataset.get_CadastralTable(FabricMember);
            //IFields pFields;
            //IFieldsEdit pFldsEd;
            IField2 pField;
            IFieldEdit2 pFieldEdit;

            List<String> CustomFields = new List<String>();

            switch (FabricMember)
            {
                case esriCadastralFabricTable.esriCFTPlans:
                    CustomFields.Add("TYPE");
                    CustomFields.Add("SOURCE_ID");
                    //CustomFields.Add("SOURCE_NAME_ID");
                    CustomFields.Add("PLAN_CATEGORY");
                    CustomFields.Add("PLAN_STATUS");
                    CustomFields.Add("PLAN_TYPEGRP");
                    CustomFields.Add("RECORD_ID");
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
                case esriCadastralFabricTable.esriCFTParcels:
                    CustomFields.Add("PARCELID");
                    CustomFields.Add("RECORD_ID");
                    CustomFields.Add("FEATURE_TYPE");
                    //CustomFields.Add("INSTRUMENT_NUMBER");
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
                case esriCadastralFabricTable.esriCFTControl:
                    CustomFields.Add("POINT_TYPE");
                    CustomFields.Add("STATN_NUM");
                    CustomFields.Add("STATN_NUM_COS");
                    CustomFields.Add("STATN_STATS");
                    CustomFields.Add("STATN_DESCRIPTION");
                    CustomFields.Add("MONMN_ORDER");
                    CustomFields.Add("NAD27_SCALE_FACTOR");
                    CustomFields.Add("LOCTN_DESCRIPTION");
                    CustomFields.Add("INSPE_DATE");
                    CustomFields.Add("ORIGINAL_DATUM_DEF");
                    CustomFields.Add("ORIGINAL_X");
                    CustomFields.Add("ORIGINAL_Y");
                    CustomFields.Add("ORIGINAL_Z");
                    CustomFields.Add("SURVEYEPOCH");
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
                default:
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
            }

            // Create new Fields collection
            //pFields = new ESRI.ArcGIS.Geodatabase.FieldsClass();
            //pFldsEd = (IFieldsEdit) pFields;

            foreach (String CustomField in CustomFields)
            {
                if (inObjectClass.FindField(CustomField) < 0)
                {
                    // Create FID Field
                    pField = new ESRI.ArcGIS.Geodatabase.FieldClass();
                    pFieldEdit = (IFieldEdit2) pField;

                    if (CustomField.Contains("_ID"))
                    {
                        pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        pFieldEdit.Precision_2 = 12;
                        pFieldEdit.Scale_2 = 0;
                        pFieldEdit.Length_2 = 22;
                    }
                    else
                    {
                        switch (CustomField)
                        {
                            case "ORIGINAL_X":
                            case "ORIGINAL_Y":
                            case "ORIGINAL_Z":
                                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                                pFieldEdit.Precision_2 = 38;
                                pFieldEdit.Scale_2 = 20;
                                pFieldEdit.Length_2 = 22;
                                break;
                            case "SURVEYEPOCH":
                                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                                pFieldEdit.Precision_2 = 7;
                                pFieldEdit.Scale_2 = 6;
                                pFieldEdit.Length_2 = 12;
                                break;
                            case "NAD27_SCALE_FACTOR":
                                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                                pFieldEdit.Precision_2 = 7;
                                pFieldEdit.Scale_2 = 6;
                                pFieldEdit.Length_2 = 12;
                                break;
                            case "PARCELID":
                                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                                pFieldEdit.Precision_2 = 10;
                                pFieldEdit.Scale_2 = 0;
                                pFieldEdit.Length_2 = 22;
                                break;
                            case "POINT_TYPE":
                                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                                pFieldEdit.Length_2 = 4;
                                break;
                            case "INSPE_DATE":
                                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDate;
                                pFieldEdit.Length_2 = 7;
                                break;
                            default:
                                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                                switch (CustomField)
                                {
                                    case "STATN_DESCRIPTION":
                                        pFieldEdit.Length_2 = 300;
                                        break;
                                    case "PLAN_STATUS":
                                        pFieldEdit.Length_2 = 60;
                                        break;
                                    case "PLAN_TYPEGRP":
                                        pFieldEdit.Length_2 = 40;
                                        break;
                                    case "LOCTN_DESCRIPTION":
                                        pFieldEdit.Length_2 = 35;
                                        break;
                                    case "TYPE":
                                    case "FEATURE_TYPE":
                                    case "STATN_NUM":
                                    case "STATN_NUM_COS":
                                        pFieldEdit.Length_2 = 20;
                                        break;
                                    case "STATN_STATS":
                                    case "MONMN_ORDER":
                                        pFieldEdit.Length_2 = 10;
                                        break;
                                    default:
                                        pFieldEdit.Length_2 = 50;
                                        break;
                                }
                                break;
                        }
                    }

                    pFieldEdit.Editable_2 = true;
                    pFieldEdit.DefaultValue_2 = null;
                    pFieldEdit.IsNullable_2 = true;
                    pFieldEdit.Name_2 = CustomField;
                    pFieldEdit.AliasName_2 = CustomField;

                    try
                    {
                        //FldsEd.AddField(pField);
                        CadastralTableAddField(inFabricDataset, pField, FabricMember);
                        Marshal.ReleaseComObject(pField);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message, "FabricAddFields");
                        return false;
                    }

                }
            }
            // !!! OLD VERSION of CADASTRAL DATASET !!!
            //if (pFields.FieldCount > 0)
            //    CadastralTableFieldEdits(inFabricDataset, pFields, FabricMember);

            return true;
        }

        void CadastralTableFieldEdits(ICadastralFabric pCadaFab, IFields pAddFields, esriCadastralFabricTable FabricMember)
        {
            ESRI.ArcGIS.esriSystem.IArray pArr;
            ISchemaLock pSchLock;
            ICadastralTableFieldEdits pCATableFldEdits;
            ICadastralFabricSchemaEdit pSchemaEd;
            IDatasetComponent pDSComponent = (IDatasetComponent) pCadaFab;
            IDEDataset pDE = pDSComponent.DataElement;
            IDECadastralFabric pDECadaFab = pDE as IDECadastralFabric;
            IFields pExtFields;
            IFieldsEdit pNewFields;
            Boolean blnSchemaLockOn = false;
            Boolean blnExtAttrFound = false;
            int j, k, cnt;
            long fldCount;

            pArr = pDECadaFab.CadastralTableFieldEdits;

            cnt = pArr.Count;

            for (int i = 0; i < cnt; i++)
            {
                pCATableFldEdits = (ICadastralTableFieldEdits) pArr.get_Element(i);
                if (pCATableFldEdits.CadastralTable == FabricMember)
                {
                    fldCount = 0;
                    pNewFields = new ESRI.ArcGIS.Geodatabase.FieldsClass();
                    pExtFields = pCATableFldEdits.ExtendedAttributeFields;
                    // Copy extended fields
                    if (pExtFields != null)
                    {
                        fldCount = pExtFields.FieldCount;
                        for (j = 0; j < fldCount; j++)
                            pNewFields.AddField(pExtFields.get_Field(j));
                        k = j;
                    }
                    else
                    {
                        k = 0;
                    }
                    // Add new fields
                    fldCount = pAddFields.FieldCount;
                    for (j = 0; j < fldCount; j++)
                        pNewFields.AddField(pAddFields.get_Field(j));
                    
                    // Reset extended attributes
                    pCATableFldEdits.ExtendedAttributeFields = pNewFields;
                    blnExtAttrFound = true;
                }
            }

            if (!blnExtAttrFound)
            {
                pCATableFldEdits = new CadastralTableFieldEditsClass ();
                pCATableFldEdits.CadastralTable = FabricMember;
                pCATableFldEdits.ExtendedAttributeFields = pAddFields;
                pArr.Add(pCATableFldEdits);
            }
            Marshal.ReleaseComObject(pAddFields);

            // Set the CadastralTableFieldEdits property on the DE to the array
            pDECadaFab.CadastralTableFieldEdits = pArr;

            // Update the fabric featureclass schema
            pSchLock = (ISchemaLock) pCadaFab;
            try
            {
                //blnSchemaLockOn = true;
                //pSchLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                
                pSchemaEd = (ICadastralFabricSchemaEdit)pCadaFab;
                pSchemaEd.UpdateSchema(pDECadaFab);
                
                blnSchemaLockOn = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "CadastralTableFieldEdits");
            }
            finally
            {
                if (blnSchemaLockOn)
                    pSchLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
            }
        }

        void CadastralTableAddField(ICadastralFabric pCadaFab, IField pAddField, esriCadastralFabricTable FabricMember)
        {
            ESRI.ArcGIS.Geodatabase.ITable pTable = (ESRI.ArcGIS.Geodatabase.ITable)pCadaFab.get_CadastralTable(FabricMember);
            IClass pObjectClass = (IClass) pTable;
            ISchemaLock pSchLock = (ISchemaLock)pTable;
            // ALL Fabric locked:
            // ISchemaLock pSchLock = (ISchemaLock)pCadaFab;
            ICadastralFabricSchemaEdit2 schemaEdit2 = (ICadastralFabricSchemaEdit2)schemaEdit;
            long lFieldLocation;

            try
            {
                // set the exclusive lock
                pSchLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                schemaEdit2.ReleaseReadOnlyFields(pTable, FabricMember);
            }
            catch (Exception ex)
            {
                Marshal.ReleaseComObject(pTable);
                System.Diagnostics.Trace.WriteLine(ex.Message, "Failed to lock table");
                schemaEdit2.ResetReadOnlyFields(FabricMember);
                return;
            }

            //IFieldsEdit pFldsEd = (ESRI.ArcGIS.Geodatabase.IFieldsEdit) pTable.Fields;
            lFieldLocation = pTable.FindField(pAddField.Name);
            if (lFieldLocation < 0)
            {
                IField2 pField2 = new ESRI.ArcGIS.Geodatabase.FieldClass();
                IFieldEdit pFieldEdit = (ESRI.ArcGIS.Geodatabase.IFieldEdit)pField2;
                pFieldEdit.Type_2 = pAddField.Type;
                pFieldEdit.Editable_2 = pAddField.Editable;
                pFieldEdit.DefaultValue_2 = pAddField.DefaultValue;
                pFieldEdit.IsNullable_2 = pAddField.IsNullable;
                pFieldEdit.Name_2 = pAddField.Name;
                pFieldEdit.AliasName_2 = pAddField.AliasName;
                pFieldEdit.Length_2 = pAddField.Length;
                pFieldEdit.Precision_2 = pAddField.Precision;
                pFieldEdit.Scale_2 = pAddField.Scale;
                try
                {
                    pObjectClass.AddField(pField2);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message, "Failed on Add field");
                }
                finally
                {
                    //Marshal.ReleaseComObject(pField2);
                    //Marshal.ReleaseComObject(pTable);
                    schemaEdit2.ResetReadOnlyFields(FabricMember);
                    pSchLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                }
            }

        }

        Boolean CheckSchemaLock(ITable pTable, String schemaUser)
        {
            ISchemaLock pSchLock = (ISchemaLock) pTable;
            IEnumSchemaLockInfo pEnumSchLockInfo;
            ISchemaLockInfo pTableLockInfo;
            IObjectClass pObject = (IObjectClass) pTable;

            pSchLock.GetCurrentSchemaLocks(out pEnumSchLockInfo);
            pTableLockInfo = pEnumSchLockInfo.Next();

            while (pTableLockInfo != null) {
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
            ISchemaLock pSchLock = (ISchemaLock) pTable;
            IEnumSchemaLockInfo pEnumSchLockInfo;
            ISchemaLockInfo pTableLockInfo;
            IObjectClass pObject = (IObjectClass) pTable;

            pSchLock.GetCurrentSchemaLocks(out pEnumSchLockInfo);

            pTableLockInfo = pEnumSchLockInfo.Next();
            while (pTableLockInfo != null) {
                if ((pTableLockInfo.UserName.ToLower() == schemaUser.ToLower()) &&
                    (pTableLockInfo.TableName.ToUpper().Contains(pObject.AliasName.ToUpper())))
                {
                    // Check exclusive LOCK status
                    if (pTableLockInfo.SchemaLockType == esriSchemaLock.esriExclusiveSchemaLock)
                    {
                        try {
                        // release the exclusive lock
                        pSchLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                        }
                        catch (Exception ex) {
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

            pSQLSyntax = (ISQLSyntax) inDataset.Workspace;
            pSQLSyntax.ParseTableName(inDataset.Name, 
                                      out strDbName,
                                      out strOwnerName,
                                      out strName);

            return strOwnerName;
        }

        int GetFabricVersion(ICadastralFabric2 pFab)
        {
            IDatasetComponent pDSComponent = null;
            IDEDataset pDEDS = null;
            IDECadastralFabric2 pDECadaFab = null;

            try {
                pDSComponent = pFab as IDatasetComponent;
                pDEDS = pDSComponent.DataElement as IDEDataset;
                pDECadaFab = pDEDS as IDECadastralFabric2;
            }
            catch (Exception ex) {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Failed on GetFabricVersion");
            }

            return pDECadaFab.Version;
        }

    }
}
=======
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
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
using stdole;



namespace ArcCatalogFabricLib
{
    /// <summary>
    /// Summary description for cmdFabricModify.
    /// </summary>
    [Guid("6e01696a-4794-4051-9486-20ac3835718c")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ArcCatalogFabricLib.cmdFabricModify")]
    public sealed class cmdFabricModify : BaseCommand
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

        private string  MyFabric = "CadastralFabric";
        //private IGxDocumentEvents m_DocEvents;
        stdole.IUnknown pUnk;
        ICadastralFabricSchemaEdit schemaEdit = null;
        private frmOptions MyOptForm; // My UI

        private IApplication m_application;

        public cmdFabricModify()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "Fabric dataset customizing"; //localizable text
            base.m_caption = "Add custom fields";  //localizable text
            base.m_message = "Add custom fields";  //localizable text 
            base.m_toolTip = "Add custom fields";  //localizable text 
            base.m_name = "MyFabricSetup_ArcCatalogCommand";   //unique id, non-localizable (e.g. "MyCategory_ArcCatalogCommand")
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
            MyOptForm = new frmOptions();
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add cmdFabricModify.OnClick implementation
            // Application.Run(optDialog);
            MyOptForm.ShowDialog();

            if ((!MyOptForm.mCancelChange) && !MyOptForm.NoneSelected)
            {
               SetupFabric();
            }

            //MyOptForm.Hide();
        }

        public override bool Enabled
        {
            get
            {
                IGxApplication gxApp = m_application as IGxApplication;
				IGxDataset gxDataset = null;
                esriDatasetType dsType = esriDatasetType.esriDTAny;
                pUnk = null;

                if (gxApp != null)
                {
                    gxDataset = gxApp.SelectedObject as IGxDataset;
                    if (gxDataset != null) dsType = gxDataset.Type;
                    //pUnk = (stdole.IUnknown) gxApp.SelectedObject.InternalObjectName.Open();
                }

                return (dsType == esriDatasetType.esriDTCadastralFabric);
            }
        }

        #endregion

       
        Boolean CadastralFabricDE(IFeatureDataset pFDS)
        {
            IFeatureDatasetExtensionContainer pFDSExtCont;
            IFeatureDatasetExtension pFDSExt;
            IDatasetContainer2 pDSCont;
            IDataset pDataset;
            ICadastralFabric pCadastralDataset;
            IDatasetComponent pDatasetComponent;
            IDEDataset pDEDS;
            IDECadastralFabric pFabricDE;
  
            pFDSExtCont = (IFeatureDatasetExtensionContainer) pFDS;
            pFDSExt = pFDSExtCont.FindExtension(esriDatasetType.esriDTCadastralFabric);
            pDSCont = (IDatasetContainer2) pFDSExt;
            pDataset = pDSCont.get_DatasetByName(esriDatasetType.esriDTCadastralFabric, MyFabric);
            pCadastralDataset = (ICadastralFabric) pDataset;
            if (pCadastralDataset != null) 
            {
                pDatasetComponent = (IDatasetComponent)pCadastralDataset;
                pDEDS = pDatasetComponent.DataElement;
                pFabricDE = (IDECadastralFabric) pDEDS;
                return true;
            }
            else
                return false;
        }


        void SetupFabric()
        {
            //IGxDatabase pGxDatabase;
            IGxDataset  pGxDataset;
            //String      schemaUser;

            //if (!TestCadastralExtension()) return;
            // Get current database connection
            //if  (!TestSelectedItem()) return;

            if ((pGxDataset=GdbHelper.GetSelectedDataset(m_application)) != null)
            {
                if (pGxDataset.Type == esriDatasetType.esriDTFeatureDataset)
                {
                    System.Windows.Forms.MessageBox.Show("Select fabric dataset");
                }
                else
                {
                    if (pGxDataset.Type == esriDatasetType.esriDTCadastralFabric)
                    {
                        if (System.Windows.Forms.MessageBox.Show("Do you want to alter fabric dataset","Setup Fabric",MessageBoxButtons.YesNo) == DialogResult.No)
                            return;

                        // Do change
                        UpdateFabricSchema(ExtractSchemaName(pGxDataset.Dataset), (ICadastralFabric) pGxDataset.Dataset);
                    }
                }
            }

        }

        Boolean TestCadastralExtension()
        {
            return true;
        }
        Boolean TestSelectedItem()
        {
            IGxApplication pApp = (IGxApplication) m_application;
            IGxObject pGxObject = pApp.SelectedObject;
            IGxDatabase2 pGxDatabase;
            //IGxGeodatabase pGxGDB;
            //IGxRemoteConnection pGxRemoteConn;
            IGxDataset pGxDataset;
            //String[] sMissingFldNames;

            if (pGxObject != null)
            {
                //Exit if common branch of catalog tree
                if ((pGxObject.FullName.ToLower() == "catalog") 
                    || (pGxObject.BaseName.ToLower() == "catalog")) {
                    System.Windows.Forms.MessageBox.Show("Select database");
                    return false;
                }
                if (pGxObject.Parent.FullName.ToLower() == "database connections")
                {
                    if (pGxObject is IGxDatabase)
                    {
                        pGxDatabase = (IGxDatabase2) pGxObject;
                        if (pGxDatabase.IsConnected)
                            System.Windows.Forms.MessageBox.Show("Select dataset");
                        else
                        {
                            //Try to connect
                            // TO DO: cnage mouse status
                            pGxDatabase.Connect();
                        }
                    }
                    return false;
                }
                if (pGxObject.Parent.FullName.ToLower().Contains("database connections"))
                {
                    if (pGxObject is IGxDataset)
                    {
                        pGxDataset = (IGxDataset) pGxObject;
                        if (pGxDataset.Type == esriDatasetType.esriDTFeatureDataset)
                        {
                            pGxDatabase = (IGxDatabase2) pGxObject.Parent;
                        }
                    }
                    return true;
                }
            }

            return false;
        }
        Boolean InitializeExtension()
        {
            ESRI.ArcGIS.esriSystem.IExtensionManager pExtMgr;
            ESRI.ArcGIS.esriSystem.IExtension pExt;
            int i;
            //  Set m_pApp = Application
            pExtMgr = (ESRI.ArcGIS.esriSystem.IExtensionManager) m_application;

            for (i = 0; i < pExtMgr.ExtensionCount; i++)
            {
                pExt = pExtMgr.get_Extension(i);
                Console.WriteLine("Extension name: " + pExt.Name);
            }
  
            /*
            pExt = m_application.FindExtensionByName(SURVEY_EXT$);
            if (pExt != null)
            {
                ESRI.ArcGIS.esriSystem.IExtensionConfig pExtConfig;
                pExtConfig = (ESRI.ArcGIS.esriSystem.IExtensionConfig) pExt;
                if (pExtConfig.State != ESRI.ArcGIS.esriSystem.esriExtensionState.esriESUnavailable)
                {
                    pExtConfig.State = ESRI.ArcGIS.esriSystem.esriExtensionState.esriESEnabled;
                    return true;
                }
    //'Set m_pCadastEdExtension = m_pApp.FindExtensionByName(CA_FABRIC_EXT$)
    //'Set m_pCadastEdExtensionMananager = m_pCadastEdExtension 'set cadastral extension manager to ICadastralEditor
    //'InitializeExtension = Not m_pCadastEdExtension Is Nothing
            }
             */
            return false;
        }

        void UpdateFabricSchema(String schemaUser, ICadastralFabric pCadastralDataset)
        {
            ITable pTable = pCadastralDataset.get_CadastralTable(esriCadastralFabricTable.esriCFTPlans);
            String[] MissingFldNames;

            if (CheckSchemaLock(pTable, schemaUser))
            {
                if (System.Windows.Forms.MessageBox.Show("Selected fabric tables are locked. Do you want to unlock them ?", "Setup Fabric", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
                ReleaseSchemaLock(pTable, schemaUser);
            }

            System.Windows.Forms.MessageBox.Show("Cadastral dataset version : " + GetFabricVersion((ICadastralFabric2)pCadastralDataset));

            IDatasetComponent dsComponent = (IDatasetComponent)pCadastralDataset;
            IDECadastralFabric dataElement1 = (IDECadastralFabric)dsComponent.DataElement;
            schemaEdit = (ICadastralFabricSchemaEdit) pCadastralDataset;

            //************ Validate that the correct fields are Present  ***************
            if (MyOptForm.Plans)
            {
                if (!FabricFieldChecker(pCadastralDataset, out MissingFldNames, esriCadastralFabricTable.esriCFTPlans))
                    UserVerifyFieldCreation(pCadastralDataset, MissingFldNames, esriCadastralFabricTable.esriCFTPlans);
            }

            if (MyOptForm.Parcels)
            {
                if (!FabricFieldChecker(pCadastralDataset, out MissingFldNames, esriCadastralFabricTable.esriCFTParcels))
                    UserVerifyFieldCreation(pCadastralDataset, MissingFldNames, esriCadastralFabricTable.esriCFTParcels);
            }

            if (MyOptForm.ControlPoints)
            {
                if (!FabricFieldChecker(pCadastralDataset, out MissingFldNames, esriCadastralFabricTable.esriCFTControl))
                    UserVerifyFieldCreation(pCadastralDataset, MissingFldNames, esriCadastralFabricTable.esriCFTControl);
            }

            if (schemaEdit != null)
                schemaEdit.UpdateSchema(dataElement1);
        }

        Boolean FabricFieldChecker(ICadastralFabric inFabricDataset, out String[] FabricSourceFields, esriCadastralFabricTable FabricMember)
        {
            long lFieldLocation;
            ITable inObjectClass = inFabricDataset.get_CadastralTable(FabricMember);
            List<String> CustomFields =new List<String>();
            Boolean result = true;

            switch(FabricMember) {
                case esriCadastralFabricTable.esriCFTPlans:
                    CustomFields.Add("TYPE");
                    CustomFields.Add("SOURCE_ID");
                    //CustomFields.Add("SOURCE_NAME_ID");
                    CustomFields.Add("PLAN_CATEGORY");
                    CustomFields.Add("PLAN_STATUS");
                    CustomFields.Add("PLAN_TYPEGRP");
                    CustomFields.Add("RECORD_ID");
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
                case esriCadastralFabricTable.esriCFTParcels:
                    CustomFields.Add("PARCELID");
                    CustomFields.Add("RECORD_ID");
                    CustomFields.Add("FEATURE_TYPE");
                    //CustomFields.Add("INSTRUMENT_NUMBER");
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
                case esriCadastralFabricTable.esriCFTControl:
                    CustomFields.Add("POINT_TYPE");
                    CustomFields.Add("STATN_NUM");
                    CustomFields.Add("STATN_NUM_COS");
                    CustomFields.Add("STATN_STATS");
                    CustomFields.Add("STATN_DESCRIPTION");
                    CustomFields.Add("MONMN_ORDER");
                    CustomFields.Add("NAD27_SCALE_FACTOR");
                    CustomFields.Add("LOCTN_DESCRIPTION");
                    CustomFields.Add("INSPE_DATE");
                    CustomFields.Add("ORIGINAL_DATUM_DEF");
                    CustomFields.Add("ORIGINAL_X");
                    CustomFields.Add("ORIGINAL_Y");
                    CustomFields.Add("ORIGINAL_Z");
                    CustomFields.Add("SURVEYEPOCH");
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
                default:
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
            }

            FabricSourceFields = new String[CustomFields.Count];

            foreach (String CustomField in CustomFields)
            {
                int i = CustomFields.IndexOf(CustomField);
                lFieldLocation = inObjectClass.FindField(CustomField);
                if (lFieldLocation < 0)
                {
                    FabricSourceFields[i] = "MISSING----->[" + CustomField + "]";
                    result = false;
                }
                else
                {
                    FabricSourceFields[i] = "[" + CustomField + "] found";
                }

            }
            return result;
        }

        void UserVerifyFieldCreation(ICadastralFabric inFabricDataset, String[] MissingFldNames, esriCadastralFabricTable FabricMember)
        {
            IDataset pDataset = (IDataset)inFabricDataset;
            String strMessage = "Required fields are missing from the " + pDataset.Name + " fabric dataset :\n\n";
            long lCnt = MissingFldNames.Length;

            for (int i=0; i < lCnt; i++)
                strMessage += MissingFldNames[i] + '\n';

            if (System.Windows.Forms.MessageBox.Show(strMessage + '\n' + "Do you want to create the missing fields and continue the process?","Fabric Source Field Checker",MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            else {
                if (!FabricAddFields(inFabricDataset, FabricMember)) {
                    System.Windows.Forms.MessageBox.Show("Failed to add required fields for " + pDataset.Name);
                    return;
                }
            }
        }

        Boolean FabricAddFields(ICadastralFabric inFabricDataset, esriCadastralFabricTable FabricMember)
        {
            IDataset pDataset = (IDataset)inFabricDataset;
            ITable inObjectClass = (ITable) inFabricDataset.get_CadastralTable(FabricMember);
            //IFields pFields;
            //IFieldsEdit pFldsEd;
            IField2 pField;
            IFieldEdit2 pFieldEdit;

            List<String> CustomFields = new List<String>();

            switch (FabricMember)
            {
                case esriCadastralFabricTable.esriCFTPlans:
                    CustomFields.Add("TYPE");
                    CustomFields.Add("SOURCE_ID");
                    //CustomFields.Add("SOURCE_NAME_ID");
                    CustomFields.Add("PLAN_CATEGORY");
                    CustomFields.Add("PLAN_STATUS");
                    CustomFields.Add("PLAN_TYPEGRP");
                    CustomFields.Add("RECORD_ID");
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
                case esriCadastralFabricTable.esriCFTParcels:
                    CustomFields.Add("PARCELID");
                    CustomFields.Add("RECORD_ID");
                    CustomFields.Add("FEATURE_TYPE");
                    //CustomFields.Add("INSTRUMENT_NUMBER");
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
                case esriCadastralFabricTable.esriCFTControl:
                    CustomFields.Add("POINT_TYPE");
                    CustomFields.Add("STATN_NUM");
                    CustomFields.Add("STATN_NUM_COS");
                    CustomFields.Add("STATN_STATS");
                    CustomFields.Add("STATN_DESCRIPTION");
                    CustomFields.Add("MONMN_ORDER");
                    CustomFields.Add("NAD27_SCALE_FACTOR");
                    CustomFields.Add("LOCTN_DESCRIPTION");
                    CustomFields.Add("INSPE_DATE");
                    CustomFields.Add("ORIGINAL_DATUM_DEF");
                    CustomFields.Add("ORIGINAL_X");
                    CustomFields.Add("ORIGINAL_Y");
                    CustomFields.Add("ORIGINAL_Z");
                    CustomFields.Add("SURVEYEPOCH");
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
                default:
                    CustomFields.Add("TRANS_ID_CREATE");
                    CustomFields.Add("TRANS_ID_EXPIRE");
                    break;
            }

            // Create new Fields collection
            //pFields = new ESRI.ArcGIS.Geodatabase.FieldsClass();
            //pFldsEd = (IFieldsEdit) pFields;

            foreach (String CustomField in CustomFields)
            {
                if (inObjectClass.FindField(CustomField) < 0)
                {
                    // Create FID Field
                    pField = new ESRI.ArcGIS.Geodatabase.FieldClass();
                    pFieldEdit = (IFieldEdit2) pField;

                    if (CustomField.Contains("_ID"))
                    {
                        pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        pFieldEdit.Precision_2 = 12;
                        pFieldEdit.Scale_2 = 0;
                        pFieldEdit.Length_2 = 22;
                    }
                    else
                    {
                        switch (CustomField)
                        {
                            case "ORIGINAL_X":
                            case "ORIGINAL_Y":
                            case "ORIGINAL_Z":
                                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                                pFieldEdit.Precision_2 = 38;
                                pFieldEdit.Scale_2 = 20;
                                pFieldEdit.Length_2 = 22;
                                break;
                            case "SURVEYEPOCH":
                                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                                pFieldEdit.Precision_2 = 7;
                                pFieldEdit.Scale_2 = 6;
                                pFieldEdit.Length_2 = 12;
                                break;
                            case "NAD27_SCALE_FACTOR":
                                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                                pFieldEdit.Precision_2 = 7;
                                pFieldEdit.Scale_2 = 6;
                                pFieldEdit.Length_2 = 12;
                                break;
                            case "PARCELID":
                                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                                pFieldEdit.Precision_2 = 10;
                                pFieldEdit.Scale_2 = 0;
                                pFieldEdit.Length_2 = 22;
                                break;
                            case "POINT_TYPE":
                                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                                pFieldEdit.Length_2 = 4;
                                break;
                            case "INSPE_DATE":
                                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDate;
                                pFieldEdit.Length_2 = 7;
                                break;
                            default:
                                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                                switch (CustomField)
                                {
                                    case "STATN_DESCRIPTION":
                                        pFieldEdit.Length_2 = 300;
                                        break;
                                    case "PLAN_STATUS":
                                        pFieldEdit.Length_2 = 60;
                                        break;
                                    case "PLAN_TYPEGRP":
                                        pFieldEdit.Length_2 = 40;
                                        break;
                                    case "LOCTN_DESCRIPTION":
                                        pFieldEdit.Length_2 = 35;
                                        break;
                                    case "TYPE":
                                    case "FEATURE_TYPE":
                                    case "STATN_NUM":
                                    case "STATN_NUM_COS":
                                        pFieldEdit.Length_2 = 20;
                                        break;
                                    case "STATN_STATS":
                                    case "MONMN_ORDER":
                                        pFieldEdit.Length_2 = 10;
                                        break;
                                    default:
                                        pFieldEdit.Length_2 = 50;
                                        break;
                                }
                                break;
                        }
                    }

                    pFieldEdit.Editable_2 = true;
                    pFieldEdit.DefaultValue_2 = null;
                    pFieldEdit.IsNullable_2 = true;
                    pFieldEdit.Name_2 = CustomField;
                    pFieldEdit.AliasName_2 = CustomField;

                    try
                    {
                        //FldsEd.AddField(pField);
                        CadastralTableAddField(inFabricDataset, pField, FabricMember);
                        Marshal.ReleaseComObject(pField);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message, "FabricAddFields");
                        return false;
                    }

                }
            }
            // !!! OLD VERSION of CADASTRAL DATASET !!!
            //if (pFields.FieldCount > 0)
            //    CadastralTableFieldEdits(inFabricDataset, pFields, FabricMember);

            return true;
        }

        void CadastralTableFieldEdits(ICadastralFabric pCadaFab, IFields pAddFields, esriCadastralFabricTable FabricMember)
        {
            ESRI.ArcGIS.esriSystem.IArray pArr;
            ISchemaLock pSchLock;
            ICadastralTableFieldEdits pCATableFldEdits;
            ICadastralFabricSchemaEdit pSchemaEd;
            IDatasetComponent pDSComponent = (IDatasetComponent) pCadaFab;
            IDEDataset pDE = pDSComponent.DataElement;
            IDECadastralFabric pDECadaFab = pDE as IDECadastralFabric;
            IFields pExtFields;
            IFieldsEdit pNewFields;
            Boolean blnSchemaLockOn = false;
            Boolean blnExtAttrFound = false;
            int j, k, cnt;
            long fldCount;

            pArr = pDECadaFab.CadastralTableFieldEdits;

            cnt = pArr.Count;

            for (int i = 0; i < cnt; i++)
            {
                pCATableFldEdits = (ICadastralTableFieldEdits) pArr.get_Element(i);
                if (pCATableFldEdits.CadastralTable == FabricMember)
                {
                    fldCount = 0;
                    pNewFields = new ESRI.ArcGIS.Geodatabase.FieldsClass();
                    pExtFields = pCATableFldEdits.ExtendedAttributeFields;
                    // Copy extended fields
                    if (pExtFields != null)
                    {
                        fldCount = pExtFields.FieldCount;
                        for (j = 0; j < fldCount; j++)
                            pNewFields.AddField(pExtFields.get_Field(j));
                        k = j;
                    }
                    else
                    {
                        k = 0;
                    }
                    // Add new fields
                    fldCount = pAddFields.FieldCount;
                    for (j = 0; j < fldCount; j++)
                        pNewFields.AddField(pAddFields.get_Field(j));
                    
                    // Reset extended attributes
                    pCATableFldEdits.ExtendedAttributeFields = pNewFields;
                    blnExtAttrFound = true;
                }
            }

            if (!blnExtAttrFound)
            {
                pCATableFldEdits = new CadastralTableFieldEditsClass ();
                pCATableFldEdits.CadastralTable = FabricMember;
                pCATableFldEdits.ExtendedAttributeFields = pAddFields;
                pArr.Add(pCATableFldEdits);
            }
            Marshal.ReleaseComObject(pAddFields);

            // Set the CadastralTableFieldEdits property on the DE to the array
            pDECadaFab.CadastralTableFieldEdits = pArr;

            // Update the fabric featureclass schema
            pSchLock = (ISchemaLock) pCadaFab;
            try
            {
                //blnSchemaLockOn = true;
                //pSchLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                
                pSchemaEd = (ICadastralFabricSchemaEdit)pCadaFab;
                pSchemaEd.UpdateSchema(pDECadaFab);
                
                blnSchemaLockOn = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "CadastralTableFieldEdits");
            }
            finally
            {
                if (blnSchemaLockOn)
                    pSchLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
            }
        }

        void CadastralTableAddField(ICadastralFabric pCadaFab, IField pAddField, esriCadastralFabricTable FabricMember)
        {
            ESRI.ArcGIS.Geodatabase.ITable pTable = (ESRI.ArcGIS.Geodatabase.ITable)pCadaFab.get_CadastralTable(FabricMember);
            IClass pObjectClass = (IClass) pTable;
            ISchemaLock pSchLock = (ISchemaLock)pTable;
            // ALL Fabric locked:
            // ISchemaLock pSchLock = (ISchemaLock)pCadaFab;
            ICadastralFabricSchemaEdit2 schemaEdit2 = (ICadastralFabricSchemaEdit2)schemaEdit;
            long lFieldLocation;

            try
            {
                // set the exclusive lock
                pSchLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                schemaEdit2.ReleaseReadOnlyFields(pTable, FabricMember);
            }
            catch (Exception ex)
            {
                Marshal.ReleaseComObject(pTable);
                System.Diagnostics.Trace.WriteLine(ex.Message, "Failed to lock table");
                schemaEdit2.ResetReadOnlyFields(FabricMember);
                return;
            }

            //IFieldsEdit pFldsEd = (ESRI.ArcGIS.Geodatabase.IFieldsEdit) pTable.Fields;
            lFieldLocation = pTable.FindField(pAddField.Name);
            if (lFieldLocation < 0)
            {
                IField2 pField2 = new ESRI.ArcGIS.Geodatabase.FieldClass();
                IFieldEdit pFieldEdit = (ESRI.ArcGIS.Geodatabase.IFieldEdit)pField2;
                pFieldEdit.Type_2 = pAddField.Type;
                pFieldEdit.Editable_2 = pAddField.Editable;
                pFieldEdit.DefaultValue_2 = pAddField.DefaultValue;
                pFieldEdit.IsNullable_2 = pAddField.IsNullable;
                pFieldEdit.Name_2 = pAddField.Name;
                pFieldEdit.AliasName_2 = pAddField.AliasName;
                pFieldEdit.Length_2 = pAddField.Length;
                pFieldEdit.Precision_2 = pAddField.Precision;
                pFieldEdit.Scale_2 = pAddField.Scale;
                try
                {
                    pObjectClass.AddField(pField2);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message, "Failed on Add field");
                }
                finally
                {
                    //Marshal.ReleaseComObject(pField2);
                    //Marshal.ReleaseComObject(pTable);
                    schemaEdit2.ResetReadOnlyFields(FabricMember);
                    pSchLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                }
            }

        }

        Boolean CheckSchemaLock(ITable pTable, String schemaUser)
        {
            ISchemaLock pSchLock = (ISchemaLock) pTable;
            IEnumSchemaLockInfo pEnumSchLockInfo;
            ISchemaLockInfo pTableLockInfo;
            IObjectClass pObject = (IObjectClass) pTable;

            pSchLock.GetCurrentSchemaLocks(out pEnumSchLockInfo);
            pTableLockInfo = pEnumSchLockInfo.Next();

            while (pTableLockInfo != null) {
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
            ISchemaLock pSchLock = (ISchemaLock) pTable;
            IEnumSchemaLockInfo pEnumSchLockInfo;
            ISchemaLockInfo pTableLockInfo;
            IObjectClass pObject = (IObjectClass) pTable;

            pSchLock.GetCurrentSchemaLocks(out pEnumSchLockInfo);

            pTableLockInfo = pEnumSchLockInfo.Next();
            while (pTableLockInfo != null) {
                if ((pTableLockInfo.UserName.ToLower() == schemaUser.ToLower()) &&
                    (pTableLockInfo.TableName.ToUpper().Contains(pObject.AliasName.ToUpper())))
                {
                    // Check exclusive LOCK status
                    if (pTableLockInfo.SchemaLockType == esriSchemaLock.esriExclusiveSchemaLock)
                    {
                        try {
                        // release the exclusive lock
                        pSchLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                        }
                        catch (Exception ex) {
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

            pSQLSyntax = (ISQLSyntax) inDataset.Workspace;
            pSQLSyntax.ParseTableName(inDataset.Name, 
                                      out strDbName,
                                      out strOwnerName,
                                      out strName);

            return strOwnerName;
        }

        int GetFabricVersion(ICadastralFabric2 pFab)
        {
            IDatasetComponent pDSComponent = null;
            IDEDataset pDEDS = null;
            IDECadastralFabric2 pDECadaFab = null;

            try {
                pDSComponent = pFab as IDatasetComponent;
                pDEDS = pDSComponent.DataElement as IDEDataset;
                pDECadaFab = pDEDS as IDECadastralFabric2;
            }
            catch (Exception ex) {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Failed on GetFabricVersion");
            }

            return pDECadaFab.Version;
        }

    }
}
>>>>>>> a733fe169500b9a9582ad48df12e6f3e86b79f11
