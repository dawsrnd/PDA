using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Database
{
    public class db
    {
        readonly SqlConnection cnn = new SqlConnection(ConnString.ServerLocal());
        readonly SqlConnection cnn_db_daws = new SqlConnection(ConnString.db_daws());

        public string GetStringDaws(string Query)
        {
            string result = "";
            SqlDataReader sqlDR;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = Query;
            cmd.Connection = cnn_db_daws;
            if (cnn_db_daws.State == ConnectionState.Closed)
            {
                cnn_db_daws.Open();
                sqlDR = cmd.ExecuteReader();
                if (sqlDR.HasRows)
                {
                    sqlDR.Read();
                    result = sqlDR[0].ToString();
                }
                cnn_db_daws.Close();
            }
            return result;
        }

        public int GetInitDaws(string Query)
        {
            int result = 0;
            SqlDataReader sqlDR;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = Query;
            cmd.Connection = cnn_db_daws;
            if (cnn_db_daws.State == ConnectionState.Closed)
            {
                cnn_db_daws.Open();
                sqlDR = cmd.ExecuteReader();
                if (sqlDR.HasRows)
                {
                    sqlDR.Read();
                    result = Convert.ToInt32(sqlDR[0].ToString());
                }
                cnn_db_daws.Close();
            }
            return result;
        }

      
        public DataTable GetDataTableDaws(string query)
        {
            using (SqlCommand cmd = new SqlCommand(query))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = cnn_db_daws;
                    sda.SelectCommand = cmd;
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        //[LOCAL]

        public int GetInt(string Query)
        {
            int result = 0;
            SqlDataReader sqlDR;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = Query;
            cmd.Connection = cnn;
            if (cnn.State == ConnectionState.Closed)
            {
                cnn.Open();
                sqlDR = cmd.ExecuteReader();
                if (sqlDR.HasRows)
                {
                    sqlDR.Read();
                    result = Convert.ToInt32(sqlDR[0].ToString());
                }
                cnn.Close();
            }
            return result;
        }

        public string GetString(string Query)
        {
            string result = "";
            SqlDataReader sqlDR;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = Query;
            cmd.Connection = cnn;
            if (cnn.State == ConnectionState.Closed)
            {
                cnn.Open();
                sqlDR = cmd.ExecuteReader();
                if (sqlDR.HasRows)
                {
                    sqlDR.Read();
                    result = sqlDR[0].ToString();
                }
                cnn.Close();
            }
            return result;
        }

        public void DeleteLN(string Query)
        {
            using (var cmd = new SqlCommand(Query, cnn))
            {
                cmd.Connection = cnn;
                cnn.Open();
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
        }


        public DataTable GetDataTable(string query)
        {
            using (SqlCommand cmd = new SqlCommand(query))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = cnn;
                    sda.SelectCommand = cmd;
                    using (DataTable dt = new DataTable())
                    {
                        sda.Fill(dt);
                        return dt;
                    }
                }
            }
        }


        public void SP_INSERT_LENDEDSTAFF(string Numero, int area, int planta)
        {
            using (var cmd = new SqlCommand("SP_INSERT_LENDEDSTAFF", cnn))
            {
                cmd.Parameters.Add("@Numero", SqlDbType.VarChar, 15).Value = Numero;
                cmd.Parameters.Add("@area", SqlDbType.Int).Value = area;
                cmd.Parameters.Add("@planta", SqlDbType.Int).Value = planta;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = cnn;
                cnn.Open();
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
        }

        public void SP_INSERTORUPDATE_ATTENDANCE(int @IdLine, string @numEmpleado, string @Estacion, string @Tipo, int @depto, int @orden, int @planta)
        {
            using (var cmd = new SqlCommand("SP_INSERTORUPDATE_ATTENDANCE", cnn))
            {
                cmd.Parameters.Add("@IdLine", SqlDbType.Int).Value = @IdLine;
                cmd.Parameters.Add("@numEmpleado", SqlDbType.VarChar, 15).Value = @numEmpleado;
                cmd.Parameters.Add("@Estacion", SqlDbType.VarChar, 150).Value = @Estacion;
                cmd.Parameters.Add("@Tipo", SqlDbType.VarChar, 150).Value = @Tipo;
                cmd.Parameters.Add("@depto", SqlDbType.Int).Value = @depto;
                cmd.Parameters.Add("@orden", SqlDbType.Int).Value = @orden;
                cmd.Parameters.Add("@planta", SqlDbType.Int).Value = @planta;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = cnn_db_daws;
                cnn_db_daws.Open();
                cmd.ExecuteNonQuery();
                cnn_db_daws.Close();
            }
        }

        public string SP_GET_TOTAL_LENDED(int plant)
        {
            string total = "0";
            using (var cmd = new SqlCommand("SP_GET_TOTAL_LENDED", cnn))
            {
                cmd.Parameters.Add("@plant", SqlDbType.Int).Value = plant;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = cnn;
                cnn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                total = reader["total"].ToString();
                cnn.Close();
            }
            return total;
        }

        public string SP_COUNT_ATTENDANCE(int IdLine)
        {
            string total = "0";
            using (var cmd = new SqlCommand("SP_COUNT_ATTENDANCE", cnn_db_daws))
            {
                cmd.Parameters.Add("@IdLine", SqlDbType.Int).Value = IdLine;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = cnn_db_daws;
                cnn_db_daws.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                total = reader["tot"].ToString();
                cnn_db_daws.Close();
            }
            return total;
        }


        public DataTable SP_READ_PERSONAL_ESCANEADO(string turno, int Planta, string from)
        {
            DataTable result = new DataTable();
            using (SqlCommand cmd = new SqlCommand("SP_READ_PERSONAL_ESCANEADO", cnn_db_daws))
            {
                cmd.Parameters.Add("@turno", SqlDbType.VarChar, 15).Value = turno;
                cmd.Parameters.Add("@Planta", SqlDbType.Int).Value = Planta;
                cmd.Parameters.Add("@from", SqlDbType.VarChar, 15).Value = from;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = cnn_db_daws;
                cnn_db_daws.Open();
                result.Load(cmd.ExecuteReader());
            }
            cnn_db_daws.Close();
            return result;
        }


        public DataTable SP_READ_PERSONAL_POR_LINEA(string IdLinea, string resultdate, string resultshift, string plant)
        {
            DataTable result = new DataTable();
            using (SqlCommand cmd = new SqlCommand("SP_READ_PERSONAL_POR_LINEA", cnn_db_daws))
            {
                cmd.Parameters.Add("@IdLinea", SqlDbType.Int).Value = IdLinea;
                cmd.Parameters.Add("@resultdate", SqlDbType.VarChar,15).Value = resultdate;
                cmd.Parameters.Add("@resultshift", SqlDbType.Int).Value = resultshift;
                cmd.Parameters.Add("@plant", SqlDbType.Int).Value = plant;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = cnn_db_daws;
                cnn_db_daws.Open();
                result.Load(cmd.ExecuteReader());
            }
            cnn_db_daws.Close();
            return result;
        }
    }
}
