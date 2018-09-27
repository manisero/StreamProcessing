using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Manisero.StreamProcessing.Utils.DataAccess.BatchedReading
{
    public class BatchedDataReader<TRow>
    {
        private readonly Func<DbConnection> _connectionFactory;
        private readonly string _tableName;
        private readonly IDictionary<string, Func<TRow, int>> _keyGetters;
        private readonly int _batchSize;
        private readonly IDictionary<string, int> _filter;

        private IDictionary<string, int> _previousBatchLastRowKey;
        private bool _noMoreData;

        public BatchedDataReader(
            Func<DbConnection> connectionFactory,
            string tableName,
            ICollection<string> keyColumns,
            int batchSize,
            IDictionary<string, int> filter = null)
        {
            _connectionFactory = connectionFactory;
            _tableName = tableName;
            _keyGetters = keyColumns.ToDictionary(x => x, x => typeof(TRow).GetProperty(x).CreateGetter<TRow, int>());
            _batchSize = batchSize;
            _filter = filter;

            _previousBatchLastRowKey = keyColumns.ToDictionary(x => x, x => -1);
        }

        public IEnumerable<ICollection<TRow>> Enumerate()
        {
            var batch = ReadNext();

            while (batch != null)
            {
                yield return batch;
                batch = ReadNext();
            }
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
            using (var connection = _connectionFactory())
            {
                return BatchReader.Read<TRow>(connection, _tableName, _previousBatchLastRowKey, _batchSize, _filter);
            }
        }
    }
}
