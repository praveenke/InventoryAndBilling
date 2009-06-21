using System;
using System.Collections.Generic;
using System.Text;

namespace POS.Database {
    public class DatabaseTransactionDetails {

        public enum QueryTypes {
            Insert, Update, Delete
        }

        private Dictionary<String, String> parameters;
        private string keyColumn;
        private string keyValue;
        private string table;
        private QueryTypes transType;

        public DatabaseTransactionDetails(QueryTypes transType) {
            parameters = new Dictionary<string, string>();
            this.transType = transType;
        }

        public string Table {
            get {
                return table;
            }
            set {
                table = value;
            }
        }

        public string KeyColumn {
            get {
                return keyColumn;
            }
            set {
                keyColumn = value;
            }
        }

        public string KeyValue {
            get {
                return keyValue;
            }
            set {
                keyValue = value;
            }
        }

        public QueryTypes TransType {
            get {
                return transType;
            }
            set {
                transType = value;
            }
        }

        public void addParameter(string column, string value) {
            parameters.Add(column, value);
        }

        public string getSqlStatement() {
            string sqlStatement = "";
            if (transType == QueryTypes.Insert) {
                string queryStart, columnList, valueList;
                queryStart = "INSERT INTO " + table;
                columnList = "(";
                valueList = "(";
                foreach (KeyValuePair<string, string> kvp in parameters) {
                    columnList = columnList + kvp.Key + ",";
                    valueList = valueList + "'" + kvp.Value + "',";
                }
                columnList = columnList.Substring(0, columnList.Length - 1);
                valueList = valueList.Substring(0, valueList.Length - 1);
                columnList = columnList + ")";
                valueList = valueList + ")";
                sqlStatement = queryStart + columnList + " VALUES" + valueList;
            }
            if (transType == QueryTypes.Update) {
                string queryStart, updateList, whereList;
                queryStart = "UPDATE " + table;
                updateList = " SET ";
                foreach (KeyValuePair<string, string> kvp in parameters) {
                    updateList = updateList + kvp.Key + " = " + "'" + kvp.Value + "',";
                }
                whereList = " WHERE " + KeyColumn + " = '" + keyValue + "'";
                updateList = updateList.Substring(0, updateList.Length - 1);
                sqlStatement = queryStart + updateList + whereList;
            }
            return sqlStatement;
        }

    }
}
