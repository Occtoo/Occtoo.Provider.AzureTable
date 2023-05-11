using Azure.Data.Tables;
using Occtoo.Onboarding.Sdk;
using Occtoo.Onboarding.Sdk.Models;
using System.Reflection;

namespace Occtoo.Provider.AzureTable
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var azureTable = "books";

            var books = GetDataFromAzureTable(azureTable);

            if (books != null && books.Any())
            {
                // create dynamic entiteis using occtoo onboarding nuget package
                var entities = GetEntitiesToOnboard(books);

                // send the data to specific occtoo datasource via your dataprovider created in the studio.
                OnboardDataToOcctoo(entities, "##DataProviderClientId##", "##DataProviderSecret##", "##OcctooSourceName##");
            }
        }
        public static List<Book> GetDataFromAzureTable(string tableName)
        {
            // populte the table with data
            TableClient _client = new TableClient("UseDevelopmentStorage=true", tableName);

            _client.Delete();
            _client.CreateIfNotExists();

            var books = new List<Book>()
            {
                new Book{PartitionKey="en", RowKey = Guid.NewGuid().ToString(), Timestamp = DateTime.UtcNow,Id = "1", Title = "Lord of the rings",Author = "Tolkien", Description = "A good book", Genre = "Sci-Fi", Price = "32.99", PublishDate = DateTime.Now.ToString()},
                new Book{PartitionKey="en", RowKey = Guid.NewGuid().ToString(), Timestamp = DateTime.UtcNow,Id = "2", Title = "The hobbit",Author = "Tolkien", Description = "A good book", Genre = "Sci-Fi", Price = "32.99", PublishDate = DateTime.Now.ToString()},
                new Book{PartitionKey="en", RowKey = Guid.NewGuid().ToString(), Timestamp = DateTime.UtcNow,Id = "3", Title = "The silmarrilion",Author = "Tolkien", Description = "A good book", Genre = "Sci-Fi", Price = "32.99", PublishDate = DateTime.Now.ToString()}
            };

            foreach (var book in books)
            {
                _client.AddEntity<Book>(book);
            }

            // query said table
            var returnBooks = _client.Query<Book>("PartitionKey eq 'en'", 100, null).ToList();
            return returnBooks;

        }
        public static List<DynamicEntity> GetEntitiesToOnboard(List<Book> books)
        {
            List<DynamicEntity> entities = new List<DynamicEntity>();
            foreach (var book in books)
            {
                DynamicEntity entity = new DynamicEntity();
                entity.Key = book.Id;

                foreach (PropertyInfo propertyInfo in book.GetType().GetProperties())
                {
                    if (propertyInfo.Name != nameof(book.Id))
                    {
                        DynamicProperty property = new DynamicProperty();
                        property.Id = propertyInfo.Name;
                        property.Value = propertyInfo.GetValue(book, null)?.ToString();
                        entity.Properties.Add(property);
                    }
                }
                entities.Add(entity);
            }
            return entities;
        }
        public static void OnboardDataToOcctoo(List<DynamicEntity> entities, string dataProviderClientId, string dataProviderSecret, string occtooSourceName)
        {

            var onboardingServliceClient = new OnboardingServiceClient(dataProviderClientId, dataProviderSecret);
            var response = onboardingServliceClient.StartEntityImport(occtooSourceName, entities);
            if (response.StatusCode != 202)
            {
                throw new Exception($"Batch import was not successful - status code: {response.StatusCode}. {response.Message}");
            }

            Console.WriteLine($"{entities.Count} {occtooSourceName} got onboarded to Occtoo!");
        }
    }
}