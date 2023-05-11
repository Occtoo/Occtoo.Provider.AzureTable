using Azure;
using Azure.Data.Tables;

namespace Occtoo.Provider.AzureTable
{
    public class Book : ITableEntity
    {

        public string Id { get; set; }

        public string Author { get; set; } = String.Empty;

        public string Title { get; set; } = String.Empty;

        public string Genre { get; set; } = String.Empty;

        public string Price { get; set; } = String.Empty;

        public string PublishDate { get; set; } = String.Empty;

        public string Description { get; set; } = String.Empty;
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
