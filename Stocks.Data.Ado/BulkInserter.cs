using System.Collections.Generic;
using System.Linq;

namespace Stocks.Data.Ado
{
    public abstract class BulkInserter<T> where T : class, new()
    {
        public abstract void BulkInsert(string connectionString, string destinationTableName, IEnumerable<T> payload);

        public void BulkInsert(string connectionString, string destinationTableName, params T[] payload)
        {
            BulkInsert(connectionString, destinationTableName, payload.AsEnumerable());
        }
    }
}
