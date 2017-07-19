using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using TimeTrackingDemo.Models;

namespace TimeTrackingDemo.ViewModels
{
    public class TimeTraceVM
    {
        string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SidEntities"].ConnectionString;
        public List<TimeTrace> GetTimeSheet(string user)
        {
            var dataSet = new DataSet();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetTimesheetsForUser";
                    //connection.Open();
                    command.Parameters.Add("@user", SqlDbType.NVarChar).Value = user;
                    connection.Open();
                    command.ExecuteNonQuery();

                    var reader = new SqlDataAdapter(command);
                    reader.Fill(dataSet);

                }
            }
            catch (Exception e)
            {
                throw e;

            }
            TimeTrace tt = new TimeTrace();
            tt.TimeTracesList = new List<TimeTrace>();
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                
                foreach (DataRow dr in dataSet.Tables[0].Rows)
                {
                    tt.TimeTracesList.Add(new TimeTrace(dr["UserName"].ToString(),Convert.ToDateTime(dr["Date"]), Convert.ToInt32(dr["ID"]), Convert.ToDateTime(dr["TimeSheetIN"]),Convert.ToDateTime(dr["TimeSheetOUT"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(dr["TimeSheetOUT"])), dr["Status"].ToString(), Convert.ToInt32( dr["TotalHours"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TotalHours"]))));
                }

            }
            return tt.TimeTracesList;
        }
    }
}