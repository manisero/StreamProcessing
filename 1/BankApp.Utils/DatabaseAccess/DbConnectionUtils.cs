﻿using System.Data;

namespace BankApp.Utils.DatabaseAccess
{
    public static class DbConnectionUtils
    {
        public static TConnection OpenIfClosed<TConnection>(
            this TConnection connection)
            where TConnection : IDbConnection
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            return connection;
        }
    }
}
