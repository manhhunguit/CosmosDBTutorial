using Microsoft.Azure.Documents;

namespace CosmosDBTutorial.Requests
{
    public class CreateDatabaseRequest
    {
        public string Id { get; set; }

        public Database ToDatabase()
        {
            return new Database { Id = Id };
        }
    }
}
