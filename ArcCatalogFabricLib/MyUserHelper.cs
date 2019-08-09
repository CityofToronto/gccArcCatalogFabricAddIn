using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;

namespace ArcCatalogFabricLib
{
    class MyUserHelper
    {
        private bool m_UserFormOpen = false;
        private static MyUserHelper instance;
        private MyUserHelper m_user_Form = null;

        private static string _schema_Owner;
        private static object _schema_PWD;
        private static bool _pwd_encrypted;
        private static string _user_schema;
        private static string _user_password;
        private static bool _user_pwd_different;
        private static bool _sde_server;

        private static IWorkspace _sql_Workspace = null;

        //private constructor - external classes cannot create a 'new' EditHelper instance
        private MyUserHelper(string schemaOwner, object schemaPWD, bool encrypted, bool sdeServer = false)
        {
            _schema_Owner = schemaOwner;
            _schema_PWD = schemaPWD;
            _pwd_encrypted = encrypted;
            _sde_server = sdeServer;
            m_UserFormOpen = false;
        }

        public static string FabricSchema
        {
            set { _schema_Owner = value; }
        }
        public static object FabricPWD
        {
            set { _schema_PWD = value; }
        }
        public static bool EncryptedPWD
        {
            get { return _pwd_encrypted; }
            set { _pwd_encrypted = value; }
        }
        public static IWorkspace SQLDatabase
        {
            get { return _sql_Workspace;  }
            set { _sql_Workspace = value; }
        }
        public static bool SDE
        {
            get { return _sde_server; }
            set { _sde_server = value; }
        }


        public static string UserSchema
        {
            get { return _user_schema; }
            set { _user_schema = value; }
        }
        public static string UserPWD
        {
            get { return _user_password; }
            set { _user_password = value; }
        }
        public static bool UserPWDChanged
        {
            get { return _user_pwd_different; }
            set { _user_pwd_different = value; }
        }

        public static MyUserHelper TheUserForm
        {
            get
            {
                if (instance != null)
                {
                    return instance.m_user_Form;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (instance == null)
                {
                    if (_schema_Owner == "" || _schema_PWD == null)
                    {
                        throw new ApplicationException("Application error: schema owner was not set");
                    }
                    else
                        instance = new MyUserHelper(_schema_Owner, _schema_PWD, _pwd_encrypted, _sde_server);
                }

                instance.m_user_Form = value;

            }
        }
        public static bool IsUserFormOpen
        {
            get
            {
                if (instance != null)
                {
                    return instance.m_UserFormOpen;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (instance == null)
                {
                    if (_schema_Owner == "" || _schema_PWD == null)
                    {
                        throw new ApplicationException("Application error: schema owner was not set");
                    }
                    else
                        instance = new MyUserHelper(_schema_Owner, _schema_PWD, _pwd_encrypted, _sde_server);
                }

                instance.m_UserFormOpen = value;

            }
        }

    }
}
