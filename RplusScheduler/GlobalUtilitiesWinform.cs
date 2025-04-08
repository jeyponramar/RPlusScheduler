using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Globalization;
namespace RplusScheduler
{
    public static class GlobalUtilitiesWinform
    {
        public static bool IsValidaTable(DataTable dttbl)
        {
            if (dttbl != null)
            {
                if (dttbl.Rows.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static string ConvertToString(object data)
        {
            if (data == null)
            {
                return "";
            }
            if (data == DBNull.Value) return "";
            if (Convert.ToString(data) == "undefined") return "";
            return Convert.ToString(data).Trim();
        }
        public static string ConvertToDateTime(object date)
        {
            string strdate = "";
            if (date == DBNull.Value) return "";
            strdate = String.Format("{0:dd-mmm-yyyy hh:mm}", date);
            if (Convert.ToDateTime(date).Hour >= 12)
            {
                strdate += " PM";
            }
            else
            {
                strdate += " AM";
            }
            if (strdate.Contains("1900") || strdate.Contains("2000")) strdate = "";
            //strdate = strdate.Replace(" 12:00 AM", "");
            return strdate;
        }
        public static DateTime ConvertToDDMMToDateTime(object date)
        {
            string strdate = "";
            if (date == DBNull.Value) return (DateTime) date;
            strdate = date.ToString();
            Array arr = strdate.Split('-');
            strdate = arr.GetValue(1).ToString() + "-" + arr.GetValue(0).ToString() + "-" + arr.GetValue(2).ToString();
            return Convert.ToDateTime(strdate);
        }
        public static int ConvertToInt(object data)
        {
            try
            {
                if (data == DBNull.Value) return 0;
                if (data == null || Convert.ToString(data) == "" || Convert.ToString(data) == "undefined") return 0;
                if (Convert.ToString(data).ToLower() == "false") return 0;
                if (Convert.ToString(data).ToLower() == "true") return 1;
                if (Convert.ToString(data).ToLower() == "on") return 1;
                if (data == DBNull.Value) return 0;
                data = data.ToString().Replace(",", "").Replace(".00", "");
                return Convert.ToInt32(data);
            }
            catch (Exception ex)
            {

            }
            return 0;
        }
        public static bool ConvertToBool(object data)
        {
            try
            {
                if (data == null) return false;
                if (data == DBNull.Value) return false;
                string strdata = Convert.ToString(data).ToLower().Trim();
                if (strdata == "" || strdata == "false" || strdata == "off" || strdata == "no" || strdata == "0" || strdata == "undefind") return false;
                if (strdata == "true" || strdata == "yes" || strdata == "on" || strdata == "1") return true;

                return Convert.ToBoolean(data);
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        public static string GetSetting(string settingName)
        {
            string query = "select setting_settingvalue from tbl_setting where setting_settingname='" + settingName + "'";
            DataRow dr = DbTableWinform.ExecuteSelectRow(query);
            if (dr == null) return "";
            return Convert.ToString(dr["setting_settingvalue"]);
        }
        public static void UpdateSetting(string settingName, string settingValue)
        {
            string query = "update tbl_setting set setting_settingvalue='" + settingValue + "',setting_modifieddate=getdate() where setting_settingname='" + settingName + "'";
            DbTableWinform.ExecuteQuery(query);
        }
    }
}