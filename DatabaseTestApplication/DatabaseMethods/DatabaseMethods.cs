using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseTestApplicatie.DatabaseMethods
{
    public class DatabaseMethods
    {
        /// <values>
        /// Data field,etc.
        /// </values>

        private SqlCommand sql;
        private SqlDataReader dataReader;
        private SqlDataAdapter adapter;

        public string[,]? SelectAllRecords(SqlConnection cnn, string tableName, int innerSizeArray)
        {
            int total = CountRecords(cnn, tableName);

            if (total > 0)
            {
                sql = new SqlCommand("SELECT * FROM "+ tableName + ";", cnn);
                dataReader = sql.ExecuteReader();

                string[,] allData = new string[total, innerSizeArray];

                int index = 0;

                while (dataReader.Read())
                {
                    for (int i = 0; i < innerSizeArray; i++)
                    {
                        allData[index, i] = dataReader.GetValue(i).ToString();
                    }

                    index++;
                }

                dataReader.Close();
                sql.Dispose();

                return allData;
            }

            return null;
        }

        public string[]? SelectSpecificRecords(SqlConnection cnn, string tableName, string select)
        {
            int total = CountRecords(cnn, tableName);

            if (total > 0)
            {
                try
                {
                    sql = new SqlCommand("SELECT " + select + " FROM "+ tableName + ";", cnn);
                    dataReader = sql.ExecuteReader();

                    string[] allData = new string[total];
                    int index = 0;

                    while (dataReader.Read())
                    {
                        allData[index] = dataReader.GetValue(0).ToString();
                        index++;
                    }

                    dataReader.Close();
                    sql.Dispose();

                    return allData;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            return null;
        }

        public void InsertRecord(SqlConnection cnn, string tableName, string[] columnNames, string[] values)
        {
            //Get and set next id.
            int nextId = GetMaxId(cnn, tableName) + 1;
            values[0] = nextId + "";

            sql = new SqlCommand("SET IDENTITY_INSERT " + tableName + " ON;", cnn);            
            sql.ExecuteNonQuery();

            string columns = JoinStrings(", ", columnNames);

            //Place @ for column names. Prepare escaping values.
            for(int i = 0; i < columnNames.Length; i++)
            {
                columnNames[i] = "@" + columnNames[i];
            }

            string valuesWithSign = JoinStrings(", ", columnNames);

            sql = new SqlCommand();
            sql.CommandText = "INSERT INTO " + tableName + "(" + columns + ") VALUES(" + valuesWithSign + ")";

            //Escape values below.
            for (int i = 0; i < columnNames.Length; i++)
            {
                sql.Parameters.AddWithValue(columnNames[i], values[i]);
            }           
            
            sql.Connection = cnn;
            sql.ExecuteNonQuery();

            sql = new SqlCommand("SET IDENTITY_INSERT " + tableName + " OFF;", cnn);
            sql.ExecuteNonQuery();

            sql.Dispose();
        }

        public int CountRecords(SqlConnection cnn, string tableName)
        {
            sql = new SqlCommand("SELECT COUNT(Id) AS total FROM " + tableName + ";", cnn);
            dataReader = sql.ExecuteReader();

            int totalRecords = 0;

            while (dataReader.Read())
            {
                totalRecords = Convert.ToInt32(dataReader.GetValue(0));
            }

            dataReader.Close();
            sql.Dispose();

            return totalRecords;
        }

        public int GetMaxId(SqlConnection cnn, string tableName)
        {
            sql = new SqlCommand("SELECT MAX(Id) AS max FROM " + tableName + ";", cnn);
            dataReader = sql.ExecuteReader();

            int maxId = 0;

            while (dataReader.Read())
            {
                maxId = Convert.ToInt32(dataReader.GetValue(0));
            }

            dataReader.Close();
            sql.Dispose();

            return maxId;
        }

        public  void UpdateRecord(SqlConnection cnn, string tableName, string updateThisColumn, string updateValue, int updateThisId)
        {            
            sql = new SqlCommand();
            sql.CommandText = "UPDATE " + tableName + " SET " + updateThisColumn + "=@escapedUpdate WHERE Id=" + updateThisId + ";";
            //Escape updateValue below.
            sql.Parameters.AddWithValue("@escapedUpdate", updateValue);
            sql.Connection = cnn;
            sql.ExecuteNonQuery();

            sql.Dispose();
        }

        public void DeleteRecordById(SqlConnection cnn, string tableName, int deleteThisId)
        {
            string deleteString = "DELETE " + tableName + " WHERE Id=" + deleteThisId + ";";

            sql = new SqlCommand(deleteString, cnn);

            adapter = new SqlDataAdapter();
            adapter.DeleteCommand = sql;
            adapter.DeleteCommand.ExecuteNonQuery();

            sql.Dispose();
            adapter.Dispose();
        }

        /// <summary>
        /// JoinStrings() - Concatenate string[] arr using a separator.
        /// </summary>
        private string JoinStrings(string separator, string[] arr)
        {
            return string.Join(separator, arr);
        }

    }
}
