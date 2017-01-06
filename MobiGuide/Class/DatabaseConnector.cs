using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DatabaseConnector
{
    public class DataRow
    {
        private Dictionary<string, object> datas;

        public DataRow()
        {
            this.datas = new Dictionary<string, object>();
        }

        public DataRow(params object[] parameters)
        {
            this.datas = new Dictionary<string, object>();
            if(parameters.Length % 2 == 0)
            {
                for(int i = 0; i < parameters.Length; i += 2)
                {
                    this.datas.Add(parameters[i].ToString(), parameters[i + 1]);
                }
            }
        }

        public object Get(string key)
        {
            if (this.datas.ContainsKey(key)) return this.datas[key];
            else return null;
        }
        public void Set(string key, object data)
        {
            this.datas.Add(key, data);
        }
        public string GetKeyAt(int index)
        {
            return this.datas.ElementAt(index).Key;
        }
        public object GetAt(int index)
        {
            return this.datas.ElementAt(index).Value;
        }

        public int Count
        {
            get { return this.datas.Count; }
        }
        public bool ContainKey(string key)
        {
            for(int i = 0; i < this.datas.Count; i++)
            {
                if (this.datas.ElementAt(i).Key.Equals(key)) return true;
            }
            return false;
        }
        public bool HasData
        {
            get
            {
                if (this.datas.Count > 0 && !this.datas.ContainsKey("error")) return true;
                else return false;
            }
        }
        public bool NoError
        {
            get
            {
                if (this.datas.ContainsKey("error")) return false;
                else return true;
            }
        }
    }
    public class DBConnector
    {
        private static string connectionString;
        public DBConnector()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
        }

        public async Task<DataRow> getDataRow(string tableName, DataRow conditions)
        {
            DataRow result = await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = String.Format("SELECT * FROM {0}", tableName);
                        if (conditions.Count > 0)
                        {
                            query += " WHERE";
                            for (int i = 0; i < conditions.Count; i++)
                            {
                                query += String.Format(" {0} = '{1}'", conditions.GetKeyAt(i), 
                                    conditions.GetAt(i).ToString());
                                if (i < conditions.Count - 1) query += " AND";
                            }
                        }
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();
                        DataRow row = new DataRow();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        row.Set(reader.GetName(i), reader.GetValue(i));
                                    }
                                }
                            }
                        }
                        return row;
                    }
                }
                catch (Exception ex)
                {
                    DataRow error = new DataRow();
                    error.Set("error", ex.Message);
                    return error;
                }
            });
            return result;
        }

        public async Task<bool> updateDataRow(string tableName, DataRow dataToUpdate, DataRow condition)
        {
            bool result = await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = String.Format("UPDATE {0} SET", tableName);
                        for (int i = 0; i < dataToUpdate.Count; i++)
                        {
                            query += String.Format(" {0} = '{1}'", dataToUpdate.GetKeyAt(i), dataToUpdate.GetAt(i));
                            if (i < dataToUpdate.Count - 1) query += ",";
                        }
                        if(condition.Count > 0)
                        {
                            query += " WHERE";
                            for (int i = 0; i < condition.Count; i++)
                            {
                                query += String.Format(" {0} = '{1}'", condition.GetKeyAt(i), condition.GetAt(i));
                                if (i < condition.Count - 1) query += " AND";
                            }
                        }
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                } catch (Exception ex)
                {
                    return false;
                }
            });
            return result;
        }

        public async Task<bool> updateBlobData(string tableName, string blobColumnName, 
            string filePath, DataRow condition)
        {
            bool result = await Task.Run(() => 
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = String.Format("UPDATE {0} ", tableName);
                        query += String.Format("SET {0} = ", blobColumnName);
                        if (!String.IsNullOrWhiteSpace(filePath))
                            query += String.Format("(SELECT BulkColumn FROM OPENROWSET(BULK  '{0}', SINGLE_BLOB) AS x)", filePath);
                        else query += "NULL";
                        if(condition.Count > 0)
                        {
                            query += String.Format(" WHERE");
                            for (int i = 0; i < condition.Count; i++)
                            {
                                query += String.Format(" {0} = '{1}'", condition.GetKeyAt(i), condition.GetAt(i));
                                if (i < condition.Count - 1) query += " AND";
                            }
                        }
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    return true;
                } catch (Exception ex)
                {
                    return false;
                }
            });
            return result;
        }

        public async Task<bool> createNewRow(string tableName, DataRow data, string uniqueColumn)
        {
            bool result = await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = String.Empty;
                        if (!String.IsNullOrWhiteSpace(uniqueColumn))
                            query += String.Format("DECLARE @id uniqueidentifier SET @id = NEWID() ");
                        query += "INSERT INTO UserAccount (";
                        if (!String.IsNullOrWhiteSpace(uniqueColumn))
                        {
                            query += String.Format("{0},", uniqueColumn);
                        }
                        for(int i = 0; i < data.Count; i++)
                        {
                            query += data.GetKeyAt(i);
                            if (i < data.Count - 1) query += ",";
                        }
                        query += ") VALUES (";
                        if(!String.IsNullOrWhiteSpace(uniqueColumn)) query += "@id, ";
                        for (int i = 0; i < data.Count; i++)
                        {
                            query += String.Format("'{0}'", data.GetAt(i));
                            if (i < data.Count - 1) query += ", ";
                        }
                        query += ")";
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        return true;
                    }
                } catch (Exception ex)
                {
                    return false;
                }
            });
            return result;
        }
    }
}
