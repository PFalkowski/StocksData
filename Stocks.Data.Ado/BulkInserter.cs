using System.Collections.Generic;
using System.Linq;

namespace Stocks.Data.Ado
{
    public abstract class BulkInserter<T> where T : class, new()
    {
        public string ConnectionString { get; protected set; }

        public abstract void BulkInsert(string destinationTableName, IEnumerable<T> payload);

        public void BulkInsert(string destinationTableName, params T[] payload)
        {
            BulkInsert(destinationTableName, payload.AsEnumerable());
        }
    }
}
