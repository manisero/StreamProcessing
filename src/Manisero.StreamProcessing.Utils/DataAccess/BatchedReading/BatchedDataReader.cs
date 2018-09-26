using System;
using System.Collections.Generic;
using System.Linq;

namespace Manisero.StreamProcessing.Utils.DataAccess.BatchedReading
{
    public class BatchedDataReader<TRow>
    {
        private readonly string _tableName;
        private readonly IDictionary<string, Func<TRow, int>> _keyGetters;
        private readonly string _connectionString;
        private readonly int _batchSize;

        private IDictionary<string, int> _previousBatchLastRowKey;
        private bool _noMoreData;

        public BatchedDataReader(
            string tableName,
            ICollection<string> keyColumns,
            string connectionString,
            int batchSize)
        {
            _tableName = tableName;
            _keyGetters = keyColumns.ToDictionary(x => x, x => typeof(TRow).GetProperty(x).CreateGetter<TRow, int>());
            _connectionString = connectionString;
            _batchSize = batchSize;

            _previousBatchLastRowKey = keyColumns.ToDictionary(x => x, x => -1);
        }

        public ICollection<TRow> ReadNext()
        {
            if (_noMoreData)
            {
                return null;
            }

            var batch = ReadBatch();

            if (batch.Count < _batchSize)
            {
                _noMoreData = true;
            }

            if (batch.Count == 0)
            {
                return null;
            }

            var lastRow = batch.Last();
            _previousBatchLastRowKey = _keyGetters.ToDictionary(x => x.Key, x => x.Value(lastRow));

            return batch;
        }

        private ICollection<TRow> ReadBatch()
        {
            return new TRow[0];
        }
    }
}
