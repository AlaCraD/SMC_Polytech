using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//< add using >
using System.Data.SqlClient;    //*local DB
using System.Data;              //*ConnectionState, DataTable
//</ add using >



namespace ClassDB
{
    public static class ClsDB
    {
        //-------------------< Class: DB >-------------------
        public static SqlConnection Get_DB_Connection()
        {
            //--------< db_Get_Connection() >--------
            //< db oeffnen >
            string cn_String = Server.Properties.Settings.Default.Connection_db;
            SqlConnection cn_connection = new SqlConnection(cn_String);
            if (cn_connection.State != ConnectionState.Open) cn_connection.Open();
            //</ db oeffnen >


            //< output >
            return cn_connection;
            //</ output >
            //--------</ db_Get_Connection() >--------

        }



        public static DataTable Get_DataTable(string SQL_Text)
        {
            //--------< db_Get_DataTable() >--------
            SqlConnection cn_connection = Get_DB_Connection();

            //< get Table >
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(SQL_Text, cn_connection);
            adapter.Fill(table);
            //</ get Table >

            //< output >
            adapter.Dispose();
            return table;
            //</ output >

            //--------</ db_Get_DataTable() >--------
        }

        public static void Create_Table(string Table_Name)
        {
            SqlConnection cn_connection = Get_DB_Connection();
            SqlCommand sqlCommand = new SqlCommand($"CREATE TABLE {Table_Name}", cn_connection);
            sqlCommand.ExecuteNonQuery();
            sqlCommand.Dispose();

        }

        public static void Add_Column_to_Table(string Table, string Columns)
        {
            SqlConnection cn_connection = Get_DB_Connection();
            SqlCommand sqlCommand = new SqlCommand($"INSERT INTO {Table} ({Columns})", cn_connection);
            sqlCommand.ExecuteNonQuery();
            sqlCommand.Dispose();
        }

        public static DataSet Get_DataSet(string SQL_Text)
        {
            SqlConnection cn_connection = Get_DB_Connection();
            SqlDataAdapter adapter = new SqlDataAdapter(SQL_Text, cn_connection);
            DataSet dataSet = new DataSet();
            adapter.Fill(dataSet);
            adapter.Dispose();
            return dataSet;
        }

        public static void Execute_SQL(string SQL_Text)
        {
            //--------< Execute_SQL() >--------
            SqlConnection cn_connection = Get_DB_Connection();

            //< get Table >
            SqlCommand cmd_Command = new SqlCommand(SQL_Text, cn_connection);
            cmd_Command.ExecuteNonQuery();
            cmd_Command.Dispose();
            //</ get Table >

            //--------</ Execute_SQL() >--------
        }


        public static void Close_DB_Connection()
        {
            //--------< Close_DB_Connection() >--------

            //< db oeffnen >
            string cn_String = Server.Properties.Settings.Default.Connection_db;
            SqlConnection cn_connection = new SqlConnection(cn_String);
            if (cn_connection.State != ConnectionState.Closed) cn_connection.Close();
            cn_connection.Dispose();
            //</ db oeffnen >

            //--------</ Close_DB_Connection() >--------
        }
        //-------------------</ Class: DB >-------------------
    }
}