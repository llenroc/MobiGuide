﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseConnector
{
    public enum ERROR
    {
        HasError,
        NoError,
        NotSet
    }
    public enum SQLStatementType
    {
        SELECT_ONE,
        SELECT_LIST,
        INSERT,
        UPDATE,
        DELETE
    }
    public class DBConnector
    {
        private static string connectionString;

        public DBConnector()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString;
        }

        public async Task<DataRow> GetDataRow(string Query)
        {
            DataRow result = await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand(Query, con);
                        con.Open();
                        DataRow row = new DataRow();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                    for (int i = 0; i < reader.FieldCount; i++)
                                        row.Set(reader.GetName(i), reader.GetValue(i));
                        }
                        row.Error = ERROR.NoError;
                        return row;
                    }
                }
                catch (Exception)
                {
                    DataRow row = new DataRow();
                    row.Error = ERROR.HasError;
                    return row;
                }
            });
            return result;
        }

        public async Task<DataRow> GetDataRow(string tableName, DataRow conditions)
        {
            return await GetDataRow(tableName, conditions, null);
        }

        public async Task<DataRow> GetDataRow(string tableName, DataRow conditions, string additionalQuery)
        {
            DataRow result = await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = string.Format("SELECT * FROM {0}", tableName);
                        if (conditions.Count > 0)
                        {
                            query += " WHERE";
                            for (int i = 0; i < conditions.Count; i++)
                            {
                                query += string.Format(" {0} = '{1}'", conditions.GetKeyAt(i), 
                                    conditions.GetAt(i));
                                if (i < conditions.Count - 1) query += " AND";
                            }
                        }
                        if (additionalQuery != null && !string.IsNullOrWhiteSpace(additionalQuery)) query += " " + additionalQuery;
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();
                        DataRow row = new DataRow();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                    for (int i = 0; i < reader.FieldCount; i++)
                                        row.Set(reader.GetName(i), reader.GetValue(i));
                        }
                        row.Error = ERROR.NoError;
                        return row;
                    }
                }
                catch (Exception)
                {
                    DataRow row = new DataRow();
                    row.Error = ERROR.HasError;
                    return row;
                }
            });
            return result;
        }

        public async Task<DataList> GetDataList(string tableName, DataRow conditions)
        {
            return await GetDataList(tableName, conditions, null);
        }

        public async Task<DataList> GetDataList(string tableName, DataRow conditions, string additionalQuery)
        {
            DataList result = await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        DataList dataList = new DataList();
                        string query = string.Format("SELECT * FROM {0}", tableName);
                        if(conditions != null)
                            if (conditions.Count > 0)
                            {
                                query += " WHERE";
                                for (int i = 0; i < conditions.Count; i++)
                                {
                                    query += string.Format(" {0} = '{1}'", conditions.GetKeyAt(i),
                                        conditions.GetAt(i));
                                    if (i < conditions.Count - 1) query += " AND";
                                }
                            }
                        if (additionalQuery != null && !string.IsNullOrWhiteSpace(additionalQuery)) query += " " + additionalQuery;
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    DataRow row = new DataRow();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                        row.Set(reader.GetName(i), reader.GetValue(i));
                                    dataList.Add(row);
                                }
                        }
                        dataList.Error = ERROR.NoError;
                        return dataList;
                    }
                }
                catch (Exception)
                {
                    DataList list = new DataList();
                    list.Error = ERROR.HasError;
                    return list;
                }
            });
            return result;
        }

        public async Task<bool> UpdateDataRow(string tableName, DataRow dataToUpdate, DataRow condition)
        {
            bool result = await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = string.Format("UPDATE {0} SET", tableName);
                        for (int i = 0; i < dataToUpdate.Count; i++)
                        {
                            query += string.Format(" {0} = @{1}", dataToUpdate.GetKeyAt(i), dataToUpdate.GetKeyAt(i));
                            if (i < dataToUpdate.Count - 1) query += ",";
                        }
                        if(condition.Count > 0)
                        {
                            query += " WHERE";
                            for (int i = 0; i < condition.Count; i++)
                            {
                                query += string.Format(" {0} = '{1}'", condition.GetKeyAt(i), condition.GetAt(i));
                                if (i < condition.Count - 1) query += " AND";
                            }
                        }
                        SqlCommand cmd = new SqlCommand(query, con);

                        SqlCommand typeCmd = con.CreateCommand();
                        typeCmd.CommandText = string.Format("SET FMTONLY ON; SELECT * FROM {0}; SET FMTONLY OFF", tableName);
                        con.Open();
                        using (SqlDataReader reader = typeCmd.ExecuteReader())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                SqlDbType type = (SqlDbType)(int)reader.GetSchemaTable().Rows[i]["ProviderType"];
                                string key = reader.GetName(i);
                                if (dataToUpdate.ContainKey(key))
                                {
                                    int indexOfKey = dataToUpdate.IndexOf(key);
                                    cmd.Parameters.Add(string.Format("@{0}", dataToUpdate.GetKeyAt(indexOfKey)), type).Value = dataToUpdate.GetAt(indexOfKey);
                                }
                            }
                        }

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                } catch (Exception)
                {
                    return false;
                }
            });
            return result;
        }

        public async Task<bool> UpdateBlobData(string tableName, string blobColumnName, string filePath, DataRow condition)
        {
            bool result = await Task.Run(() => 
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = string.Format("UPDATE {0} ", tableName);
                        query += string.Format("SET {0} = ", blobColumnName);
                        if (!string.IsNullOrWhiteSpace(filePath))
                            query += string.Format("(SELECT BulkColumn FROM OPENROWSET(BULK  '{0}', SINGLE_BLOB) AS x)", filePath);
                        else query += "NULL";
                        if(condition.Count > 0)
                        {
                            query += " WHERE";
                            for (int i = 0; i < condition.Count; i++)
                            {
                                query += string.Format(" {0} = '{1}'", condition.GetKeyAt(i), condition.GetAt(i));
                                if (i < condition.Count - 1) query += " AND";
                            }
                        }
                        SqlCommand cmd = new SqlCommand(query, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    return true;
                } catch (Exception)
                {
                    return false;
                }
            });
            return result;
        }

        public async Task<bool> CreateNewRow(string tableName, DataRow data, string uniqueColumn)
        {
            bool result = await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        //create query
                        string query = string.Empty;
                        if (!string.IsNullOrWhiteSpace(uniqueColumn))
                            query += "DECLARE @id uniqueidentifier SET @id = NEWID() ";
                        query += string.Format("INSERT INTO {0} (", tableName);
                        if (!string.IsNullOrWhiteSpace(uniqueColumn))
                            query += string.Format("{0},", uniqueColumn);
                        for(int i = 0; i < data.Count; i++)
                        {
                            query += data.GetKeyAt(i);
                            if (i < data.Count - 1) query += ",";
                        }
                        query += ") VALUES (";
                        if(!string.IsNullOrWhiteSpace(uniqueColumn)) query += "@id, ";
                        for (int i = 0; i < data.Count; i++)
                        {
                            query += string.Format("@{0}", data.GetKeyAt(i));
                            if (i < data.Count - 1) query += ", ";
                        }
                        query += ")";
                        SqlCommand cmd = new SqlCommand(query, con);

                        //try to map sqldbtype to values
                        SqlCommand typeCmd = con.CreateCommand();
                        typeCmd.CommandText = string.Format("SET FMTONLY ON; SELECT * FROM {0}; SET FMTONLY OFF", tableName);
                        con.Open();
                        using (SqlDataReader reader = typeCmd.ExecuteReader())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                SqlDbType type = (SqlDbType)(int)reader.GetSchemaTable().Rows[i]["ProviderType"];
                                string key = reader.GetName(i);
                                if (data.ContainKey(key))
                                {
                                    int indexOfKey = data.IndexOf(key);
                                    cmd.Parameters.Add(string.Format("@{0}", data.GetKeyAt(indexOfKey)), type).Value = data.GetAt(indexOfKey);
                                }
                            }
                        }
                        cmd.ExecuteNonQuery();
                        con.Close();
                        return true;
                    }
                } catch (Exception)
                {
                    return false;
                }
            });
            return result;
        }

        public async Task<DataRow> CreateNewRowAndGetUId(string tableName, DataRow data, string uniqueColumn)
        {
            DataRow result = await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        //create query
                        string query = string.Empty;
                        if (!string.IsNullOrWhiteSpace(uniqueColumn))
                            query += "DECLARE @id uniqueidentifier SET @id = NEWID() ";
                        query += string.Format("INSERT INTO {0} (", tableName);
                        if (!string.IsNullOrWhiteSpace(uniqueColumn))
                            query += string.Format("{0},", uniqueColumn);
                        for (int i = 0; i < data.Count; i++)
                        {
                            query += data.GetKeyAt(i);
                            if (i < data.Count - 1) query += ",";
                        }
                        query += string.Format(") OUTPUT INSERTED.{0} VALUES (", uniqueColumn);
                        if (!string.IsNullOrWhiteSpace(uniqueColumn)) query += "@id, ";
                        for (int i = 0; i < data.Count; i++)
                        {
                            query += string.Format("@{0}", data.GetKeyAt(i));
                            if (i < data.Count - 1) query += ", ";
                        }
                        query += ")";
                        SqlCommand cmd = new SqlCommand(query, con);

                        //try to map sqldbtype to values
                        SqlCommand typeCmd = con.CreateCommand();
                        typeCmd.CommandText = string.Format("SET FMTONLY ON; SELECT * FROM {0}; SET FMTONLY OFF", tableName);
                        con.Open();
                        using (SqlDataReader reader = typeCmd.ExecuteReader())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                SqlDbType type = (SqlDbType)(int)reader.GetSchemaTable().Rows[i]["ProviderType"];
                                string key = reader.GetName(i);
                                if (data.ContainKey(key))
                                {
                                    int indexOfKey = data.IndexOf(key);
                                    cmd.Parameters.Add(string.Format("@{0}", data.GetKeyAt(indexOfKey)), type).Value = data.GetAt(indexOfKey);
                                }
                            }
                        }
                        Guid newId = (Guid) cmd.ExecuteScalar();
                        con.Close();
                        DataRow res = new DataRow(
                                string.Format("{0}", uniqueColumn), newId
                            );
                        res.Error = ERROR.NoError;
                        return res;
                    }
                }
                catch (Exception)
                {
                    DataRow error = new DataRow();
                    error.Error = ERROR.HasError;
                    return error;
                }
            });
            return result;
        }

        public async Task<object> CustomQuery(SQLStatementType type, string query)
        {
            return await Task.Run(() =>
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    con.Open();
                    try
                    {
                        switch (type)
                        {
                            case SQLStatementType.SELECT_LIST:
                                try
                                {
                                    DataList dataList = new DataList();
                                    using (SqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        if (reader.HasRows)
                                            while (reader.Read())
                                            {
                                                DataRow row = new DataRow();
                                                for (int i = 0; i < reader.FieldCount; i++)
                                                    row.Set(reader.GetName(i), reader.GetValue(i));
                                                dataList.Add(row);
                                            }
                                        dataList.Error = ERROR.NoError;
                                        return (object)dataList;
                                    }
                                } catch (Exception)
                                {
                                    DataList error = new DataList();
                                    error.Error = ERROR.HasError;
                                    return error;
                                }
                            case SQLStatementType.DELETE:
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                    return true;
                                }
                                catch (Exception)
                                {
                                    return false;
                                }
                            default:
                                return true;
                        }
                    } catch (Exception)
                    {
                        return false;
                    }
                }
            });
        }

        public async Task<string> GetFullNameFromUid(string uid)
        {
            DataRow row = await GetDataRow("UserAccount", new DataRow("UserAccountId", uid));
            if(row.HasData && row.Error == ERROR.NoError)
                return string.Format("{0} {1}", row.Get("FirstName"), row.Get("LastName"));
            return "";
        }
    }
}
