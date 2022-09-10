using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace sqlWpfContacts {
    internal class DatabaseHelper {
        private string _connectionString;
        private bool _isConnected = false;
        private SqlConnection _dbConnection;
        private SqlCommand _sqlCommand;
        public string ConnectionString {
            get { return _connectionString; }
        }

        public bool IsConnected {
            get { return GetCurrentConnectionStatus(); }

        }
        public DatabaseHelper(string connectionString, bool connectNow = true) {
            _connectionString = connectionString;
            if (connectNow) {
                Connect();
            }
        }
        public bool Connect() {
            try {
                _dbConnection = new SqlConnection(_connectionString);
                _isConnected = true;
            } catch {
                _isConnected = false;
            }
            return _isConnected;
        }

        public object[][] ExecuteReader(string sqlStatement) {
            SqlDataReader queryReturnData = null;
            object[][] returnData = null;

            try {
                if (IsConnected) {
                    _dbConnection.Open();
                    _sqlCommand = new SqlCommand(sqlStatement, _dbConnection);
                    queryReturnData = _sqlCommand.ExecuteReader();
                    returnData = ConvertDataReaderTo2DArray(queryReturnData);
                    _dbConnection.Close();
                }

            } catch (SqlException) {
                throw new Exception("Invalid SQL");
            }

            return returnData;
        }

        public int ExecuteNonQuery(string sqlStatement) {
            int recordsAffected = -1;
            try {
                if (IsConnected) {
                    _dbConnection.Open();
                    _sqlCommand = new SqlCommand(sqlStatement, _dbConnection);
                    recordsAffected = _sqlCommand.ExecuteNonQuery();
                    _dbConnection.Close();
                }
            } catch (SqlException) {
                throw new Exception("Invalid SQL");
            }
            return recordsAffected;
        }

        public int GetTableRecordCount(string tableName) {
            string query = $"SELECT COUNT(*) FROM {tableName}";
            int count = 0;
            using (SqlConnection thisConnection = new SqlConnection(@"Data Source=DESKTOP-3TU21FQ\SQLEXPRESS;Integrated Security=True")) {
                using (SqlCommand cmdCount = new SqlCommand(query, thisConnection)) {
                    thisConnection.Open();
                    count = (int)cmdCount.ExecuteScalar();
                    thisConnection.Close();
                }
            }
            return count;
        }
        public bool FlushTable(string tableName) {
            bool tableFlushed = false;
            string query = $"DELETE FROM {tableName}";
            using (SqlConnection thisConnection = new SqlConnection(@"Data Source=DESKTOP-3TU21FQ\SQLEXPRESS;Integrated Security=True")) {
                using (SqlCommand cmd = new SqlCommand(query, thisConnection)) {
                    thisConnection.Open();
                    cmd.ExecuteNonQuery();
                    thisConnection.Close();
                    tableFlushed = true;
                }
            }
            return tableFlushed;
        }//end method

        public bool DeleteTable(string tableName) {
            string query = $"DROP TABLE {tableName}";
            bool tableDeleted = false;
            using (SqlConnection thisConnection = new SqlConnection(_connectionString)) {
                using (SqlCommand dropTable = new SqlCommand(query, thisConnection)) {
                    thisConnection.Open();
                    dropTable.ExecuteNonQuery();
                    tableDeleted = true;
                    thisConnection.Close();
                }
            }

            return tableDeleted;
        }//end method

        public bool AddTable(string tableName) {
            string query = $"CREATE TABLE {tableName} ([id] INT IDENTITY (1,1) NOT NULL, firstName varChar(20) NULL, lastName varchar(20) NULL)";
            bool tableAdded = false;
            using (SqlConnection thisConnection = new SqlConnection(_connectionString)) {
                using (SqlCommand cmd = new SqlCommand(query, thisConnection)) {
                    thisConnection.Open();
                    cmd.ExecuteNonQuery();
                    tableAdded = true;
                    thisConnection.Close();
                }
            }
            return tableAdded;
        }//end method

        public bool Connect(string newConnectionString) {//overload to connect to new db

            try {
                _dbConnection = new SqlConnection(newConnectionString);
                _isConnected = true;
            } catch {
                _isConnected = false;
            }
            return _isConnected;
        }//end method



        private object[][] ConvertDataReaderTo2DArray(SqlDataReader data) {
            object[,] returnData = null;
            List<object[]> lstRows = new List<object[]>();

            while (data.Read()) {
                object[] newRow = new object[data.FieldCount];
                for (int fieldIndex = 0; fieldIndex < data.FieldCount; fieldIndex++) {
                    newRow[fieldIndex] = data[fieldIndex];

                }
                lstRows.Add(newRow);
            }

            return lstRows.ToArray();
        }

        private bool GetCurrentConnectionStatus() {
            bool pastConnection = _dbConnection != null;
            bool currentlyConnected = false;

            if (pastConnection == true) {
                currentlyConnected = _dbConnection.State != System.Data.ConnectionState.Broken;
            }

            _isConnected = currentlyConnected;
            return currentlyConnected;
        }
    }//end class
}//end namespace
