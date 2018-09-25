using System.Data;
using Npgsql;

namespace Manisero.StreamProcessing.Utils
{
    public static class NpgsqlConnectionUtils
    {
        public static NpgsqlConnection OpenIfClosed(
            this NpgsqlConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            return connection;
        }
    }
}
