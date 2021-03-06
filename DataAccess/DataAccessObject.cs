﻿using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using Slapper;
using System.Globalization;
using Npgsql;
namespace DataAccessLayer
{
    /// <summary>
    /// clase para el manejo del acceso a los datos en persistencia
    /// </summary>
    public class DataAccessObject
    {
        
        private string _ConnectionString = "";
        private string connectionStringName = "";
        private string providerName = "";
        DbProviderFactory factory;
        TextInfo textInfo;

        public string ConnectionStringName
        {
            get { return connectionStringName; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ConnectionStringName">Nombre de la conexión</param>
        public DataAccessObject(string ConnectionStringName)
        {
            connectionStringName = ConnectionStringName;
            var a = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            _ConnectionString = a.ConnectionString;
            providerName = a.ProviderName;
            factory = Npgsql.NpgsqlFactory.Instance;// DbProviderFactories.GetFactory(providerName);
            textInfo = new CultureInfo("en-US").TextInfo;
        }

        /// <summary>
        /// crea un objeto AtomicTransaction que controlara la persistencia de los datos
        /// </summary>
        /// <returns></returns>
        public AtomicTransaction CreateAtomicTransaction()
        {
            DbConnection conn = GetConnection();
            DbCommand cmd = factory.CreateCommand();
            cmd.Connection = conn;
            conn.Open();
            DbTransaction trans = conn.BeginTransaction();
            AtomicTransaction atomicTransaction = new AtomicTransaction(conn, trans, cmd);
            return atomicTransaction;
        }

        /// <summary>
        /// crea una nueva conexión a los datos en persistencia
        /// </summary>
        /// <returns>la nueva conexión</returns>
        public DbConnection GetConnection()
        {
            DbConnection curConn = factory.CreateConnection();
            curConn.ConnectionString = _ConnectionString;
            return curConn;
        }

        /// <summary>
        /// ejecuta un comando de lectura de los datos en persistencia
        /// </summary>
        /// <param name="cmdText">consulta sql</param>
        /// <returns>un listado de los datos</returns>
        public dynamic ExecuteReader(string cmdText, object parameters = null, bool isList = true)
        {
            DbConnection conn = GetConnection();
            DbCommand cmd = factory.CreateCommand();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            string[] propertyNames = parameters.GetType().GetProperties().Select(p => p.Name).ToArray();
            foreach (var prop in propertyNames)
            {
                object propValue = parameters.GetType().GetProperty(prop).GetValue(parameters, null);
                DbParameter param = cmd.CreateParameter();
                param.ParameterName = "@" + prop;
                param.Value = propValue;
                cmd.Parameters.Add(param);
            }

            conn.Open();
            DbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            List<IDictionary<string, object>> list = null;
            bool hasRows = reader.HasRows;
            if (hasRows)
            {
                list = DbDataReaderToList(reader);
            }
            reader.Dispose();
            reader.Close();
            reader.Dispose();
            if (hasRows)
            {
                if (isList)
                    return list;
                else
                    return list[0];
            }
            return null;
        }

        /// <summary>
        /// Metodo para ejecutar comandos de actualizacion e inserción de datos en persistencia
        /// </summary>
        /// <param name="cmdText">Comando sql</param>
        /// <param name="parameters">objetos que contiene los datos de entrada</param>
        /// <param name="atom">transacción atomic que controlara la persistencia de los datos</param>
        /// <returns>el numero de columnas afectadas</returns>
        public int ExecuteNonQuery(string cmdText, object parameters = null, AtomicTransaction atom = null)
        {
            DbConnection conn;
            DbCommand cmd;
            if (atom != null)
            {
                conn = atom.Conn;
                cmd = atom.Cmd;
            }
            else
            {
                conn = GetConnection();
                cmd = factory.CreateCommand();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            string[] propertyNames = parameters.GetType().GetProperties().Select(p => p.Name).ToArray();
            foreach (var prop in propertyNames)
            {
                object propValue = parameters.GetType().GetProperty(prop).GetValue(parameters, null);
                DbParameter param = cmd.CreateParameter();
                param.ParameterName = "@" + prop;
                param.Value = propValue;
                cmd.Parameters.Add(param);
            }
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                int result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Metodo que convierte cadena de texto a formato Pascal Casing
        /// </summary>
        /// <param name="value">Cadena de texto a convertir</param>
        /// <param name="textInfo">Objeto culturizado para la conversion</param>
        /// <returns></returns>
        public string ToTitleCase(string value, TextInfo textInfo)
        {
            string result = textInfo.ToTitleCase(value.ToLower()).Replace("_", string.Empty);
            return result;
        }

        private List<IDictionary<string, object>> DbDataReaderToList(DbDataReader reader)
        {
            List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
                string rowName, rowType;
            dynamic rowValue;

            while (reader.Read())
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    rowName = ToTitleCase(reader.GetName(i), textInfo);
                    rowValue = reader.GetValue(i);
                    rowType = reader.GetDataTypeName(i);

                    switch (rowType)
                    {
                        case "Decimal":
                            if (reader.IsDBNull(i)) row[rowName] = null;
                            else row[rowName] = (double)rowValue;
                            break;

                        case "Date":
                            if (reader.IsDBNull(i)) row[rowName] = null;
                            else row[rowName] = (DateTime)rowValue;
                            break;

                        default:
                            row[rowName] = reader.IsDBNull(i) ? null : rowValue;
                            break;
                    }
                }

                list.Add(row);
            }
            return list;
        }
    }
}
