using System;
using System.Collections;
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
        private ERROR error;

        public DataRow()
        {
            this.datas = new Dictionary<string, object>();
            this.error = ERROR.NotSet;
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
                return this.datas.Count > 0 ? true : false;
            }
        }
        public ERROR Error
        {
            get { return this.error; }
            set { this.error = value; }
        }
    }

    public class DataList
    {
        private List<DataRow> datas;
        private ERROR error;

        public DataList()
        {
            this.datas = new List<DataRow>();
            this.error = ERROR.NotSet;
        }

        public DataRow GetListAt(int index)
        {
            return this.datas[index];
        }
        public int Count
        {
            get { return this.datas.Count; }
        }
        public bool HasData
        {
            get
            {
                return this.datas.Count > 0 ? true : false;
            }
        }
        public void Add(DataRow data)
        {
            this.datas.Add(data);
        }
        public ERROR Error
        {
            get { return this.error; }
            set { this.error = value; }
        }
        public IEnumerator GetEnumerator()
        {
            return (this.datas as IEnumerable).GetEnumerator();
        }
    }

    public enum ERROR
    {
        HasError,
        NoError,
        NotSet
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
            return await getDataRow(tableName, conditions, null);
        }

        public async Task<DataRow> getDataRow(string tableName, DataRow conditions, string additionalQuery)
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
                        if (additionalQuery != null && !String.IsNullOrWhiteSpace(additionalQuery)) query += " " + additionalQuery;
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
                        row.Error = ERROR.NoError;
                        return row;
                    }
                }
                catch (Exception ex)
                {
                    DataRow row = new DataRow();
                    row.Error = ERROR.HasError;
                    return row;
                }
            });
            return result;
        }
        public async Task<DataList> getDataList(string tableName, DataRow conditions)
        {
            return await getDataList(tableName, conditions, null);
        }
        public async Task<DataList> getDataList(string tableName, DataRow conditions, string additionalQuery)
        {
            DataList result = await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        DataList dataList = new DataList();
                        string query = String.Format("SELECT * FROM {0}", tableName);
                        if(conditions != null)
                        {
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
                        }
                        if (additionalQuery != null && !String.IsNullOrWhiteSpace(additionalQuery)) query += " " + additionalQuery;
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    DataRow row = new DataRow();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        row.Set(reader.GetName(i), reader.GetValue(i));
                                    }
                                    dataList.Add(row);
                                }
                            }
                        }
                        dataList.Error = ERROR.NoError;
                        return dataList;
                    }
                }
                catch (Exception ex)
                {
                    DataList list = new DataList();
                    list.Error = ERROR.HasError;
                    return list;
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
                        query += String.Format("INSERT INTO {0} (", tableName);
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
