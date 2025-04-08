using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace RplusScheduler
{
    public static class DbTableWinform
    {
        public static void ExecuteQuery(string query)
        {
            using (SqlConnection con = new SqlConnection(AppConstantsWinform.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        public static int Insert(Hashtable hstbl, string m)
        {
            return Insert(hstbl, m, true);
        }
        public static int Insert(Hashtable hstbl, string m, bool isCreatedDate)
        {
            return Insert(hstbl, m, isCreatedDate, 0);
        }
        public static int Insert(Hashtable hstbl, string m, bool isCreatedDate, int noOfRetry)
        {
            IDictionaryEnumerator enmCategoryDetails = hstbl.GetEnumerator();
            StringBuilder query = new StringBuilder();
            StringBuilder cols = new StringBuilder();
            StringBuilder vals = new StringBuilder();
            SqlCommand cmd = new SqlCommand();

            query.Append("insert into tbl_" + m + "(");
            if (isCreatedDate)
            {
                query.Append(m + "_createddate");
                vals.Append("getdate()");
            }
            while (enmCategoryDetails.MoveNext())
            {
                string val = Convert.ToString(enmCategoryDetails.Value);
                if (val == null) val = "";
                val = val.Trim();
                string key = m + "_" + enmCategoryDetails.Key.ToString().ToLower();
                if (isCreatedDate)
                {
                    cols.Append(",");
                    vals.Append(",");
                }
                else
                {
                    if (cols.ToString() != "")
                    {
                        cols.Append(",");
                        vals.Append(",");
                    }
                }
                if (key.Contains("_datetime"))
                {
                    key = key.Replace("_datetime", "");
                    cmd.Parameters.Add(new SqlParameter("@" + key, enmCategoryDetails.Value));
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@" + key, val));
                }
                cols.Append(key);
                vals.Append("@" + key);
                
            }
            query.Append(cols.ToString() + ")");
            query.Append(" values(" + vals.ToString() + ")");
            query.Append(";SELECT SCOPE_IDENTITY()");
            try
            {
                using (SqlConnection con = new SqlConnection(AppConstantsWinform.ConnectionString))
                {
                    con.Open();
                    cmd.Connection = con;
                    cmd.CommandText = query.ToString();
                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                    return id;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("String or binary data would be truncated"))
                {
                    bool issuccess = IncreaseTableColumnLength("tbl_"+m, hstbl);
                    if (issuccess)
                    {
                        if (noOfRetry > 0) return 0;
                        noOfRetry++;
                        Insert(hstbl, m, isCreatedDate, noOfRetry);
                    }
                }
            }
            return 0;
        }
        private static bool IncreaseTableColumnLength(string tableName, Hashtable hstbl)
        {
            try
            {
                string query = "select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableName + "' AND DATA_TYPE='varchar'";
                DataTable dttblcol = ExecuteSelectQuery(query);
                string prefix = tableName.Replace("tbl_", "");

                IDictionaryEnumerator enmCategoryDetails = hstbl.GetEnumerator();
                while (enmCategoryDetails.MoveNext())
                {
                    string val = enmCategoryDetails.Value.ToString().Trim();
                    string key = enmCategoryDetails.Key.ToString().ToLower();
                    if (!key.Contains("_"))
                    {
                        key = prefix + "_" + key;
                    }
                    int actualLength = val.Length;
                    for (int i = 0; i < dttblcol.Rows.Count; i++)
                    {
                        DataRow dr = dttblcol.Rows[i];
                        string datatype = GlobalUtilitiesWinform.ConvertToString(dr["DATA_TYPE"]);
                        bool isnullable = GlobalUtilitiesWinform.ConvertToBool(dr["IS_NULLABLE"]);
                        int maxlength = GlobalUtilitiesWinform.ConvertToInt(dr["CHARACTER_MAXIMUM_LENGTH"]);
                        if (maxlength <= 0) continue;
                        string colName = GlobalUtilitiesWinform.ConvertToString(dr["COLUMN_NAME"]);
                        if (colName == key)
                        {
                            if (actualLength > maxlength)
                            {
                                int size = actualLength;
                                if (size > 100 && size < 300)
                                {
                                    size = 300;
                                }
                                else if (size > 300 && size < 500)
                                {
                                    size = 500;
                                }
                                else if (size > 500 && size < 1000)
                                {
                                    size = 1000;
                                }
                                if (size > 1000)
                                {
                                    query = "alter table " + tableName + " alter column " + colName + " varchar(MAX)";
                                }
                                else
                                {
                                    query = "alter table " + tableName + " alter column " + colName + " varchar(" + size + ")";
                                }
                                if (isnullable)
                                {
                                    query += " NULL";
                                }
                                else
                                {
                                    query += " NOT NULL";
                                }
                                DbTableWinform.ExecuteQuery(query);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public static int Insert(string query)
        {
            query += ";SELECT SCOPE_IDENTITY()";
            using (SqlConnection con = new SqlConnection(AppConstantsWinform.ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                int id = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
                return id;
            }
        }
        public static DataTable ExecuteSelectQuery(string query)
        {
            DataTable dttbl = new DataTable();
            using (SqlConnection con = new SqlConnection(AppConstantsWinform.ConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dttbl);
            }
            return dttbl;
        }
        public static DataRow ExecuteSelectRow(string query, Hashtable hstblparam)
        {
            DataTable dttbl = ExecuteSelectQuery(query, hstblparam);
            DataRow dr = null;
            if (dttbl != null && dttbl.Rows.Count > 0) dr = dttbl.Rows[0];
            return dr;
        }
        public static DataTable ExecuteSelectQuery(string query, Hashtable hstblparam)
        {
            DataTable dttbl = new DataTable();
            using (SqlConnection con = new SqlConnection(AppConstantsWinform.ConnectionString))
            {
                IDictionaryEnumerator enmCategoryDetails = hstblparam.GetEnumerator();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = query;
                while (enmCategoryDetails.MoveNext())
                {
                    string val = Convert.ToString(enmCategoryDetails.Value);
                    string key = enmCategoryDetails.Key.ToString().ToLower();
                    cmd.Parameters.Add(new SqlParameter("@" + key, val));
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dttbl);
            }
            return dttbl;
        }
        public static DataRow ExecuteSelectRow(string query)
        {
            DataTable dttbl = ExecuteSelectQuery(query);
            DataRow dr = null;
            if (dttbl != null && dttbl.Rows.Count > 0) dr = dttbl.Rows[0];
            return dr;
        }
        public static int GetMasterId(string m, string colName, string val)
        {
            if (val == "") return 0;
            string query = "select * from tbl_" + m + " where " + m + "_" + colName + "='" + val + "'";
            DataRow dr = ExecuteSelectRow(query);
            if (dr == null) return 0;
            return Convert.ToInt32(dr[m + "_" + m + "id"]);
        }
    }
}

