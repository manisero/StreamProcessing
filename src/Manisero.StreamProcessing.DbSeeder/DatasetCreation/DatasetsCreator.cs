using System.Collections.Generic;
using Manisero.StreamProcessing.Domain;

namespace Manisero.StreamProcessing.DbSeeder.DatasetCreation
{
    public static class DatasetsCreator
    {
        public static IDictionary<short, Dataset> Create(
            string connectionString)
        {
            return new Dictionary<short, Dataset>(); // TODO
        }
    }
}
