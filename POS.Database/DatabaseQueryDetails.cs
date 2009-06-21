using System;
using System.Collections.Generic;
using System.Text;

namespace POS.Database {
    public class DatabaseQueryDetails {

        private List<string> selectList;
        private string table;
        private string whereClause;
        private string groupByClause;

        public DatabaseQueryDetails() {
            selectList = new List<string>();
        }

        public void Clear() {
            table = null;
            whereClause = null;
            groupByClause = null;
            selectList.Clear();
        }

        public string Table {
            get {
                return table;
            }
            set {
                table = value;
            }
        }

        public string WhereClause {
            get {
                return whereClause;
            }
            set {
                whereClause = value;
            }
        }

        public string GroupByClause {
            get {
                return groupByClause;
            }
            set {
                groupByClause = value;
            }
        }

        public void addSelectItem(string item) {
            selectList.Add(item);
        }

        public string getSqlStatement() {
            string sqlStatement = "";
            string queryStart, columnList;
            queryStart = "SELECT ";
            columnList = "";
            foreach (string kvp in selectList) {
                columnList = columnList + kvp + ", ";
            }
            columnList = columnList.Substring(0, columnList.Length - 2);
            sqlStatement = queryStart + columnList + " FROM " + table;
            if (whereClause != null && !whereClause.Equals("")) {
                sqlStatement = sqlStatement + " WHERE " + whereClause;
            }
            if (groupByClause != null && !groupByClause.Equals("")) {
                sqlStatement = sqlStatement + " GROUP BY " + groupByClause;
            }
            return sqlStatement;
        }

    }
}
