using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace POS.Database {
    
    
    public class DatabaseConnectionSettings {

        private string dataSource;
        private string database;
        private string user;
        private string pass;

        public string DataSource {
            set {
                dataSource = value;
            }
            get {
                return dataSource;
            }
        }

        public string Database {
            set {
                database = value;
            }
            get {
                return database;
            }
        }

        public string User {
            set {
                user = value;
            }
            get {
                return user;
            }
        }

        public string Pass {
            set {
                pass = value;
            }
            get {
                return pass;
            }
        }

        public  string GetConnectionString() {
            return "Data Source='" + dataSource + "';Database='" + database + "';Persist Security Info=true;User Name='" + user + "';Password='" + pass + "'";
        }
        public static string GetMDBConnectionString() {
            return "provider=Microsoft.Jet.OLEDB.4.0;" +
 @"data source = POS.MDB";
        }
    }
}
