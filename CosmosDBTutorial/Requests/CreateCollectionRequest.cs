using Microsoft.Azure.Documents;

namespace CosmosDBTutorial.Requests
{
    public class CreateCollectionRequest
    {
        public string Id { get; set; }

        public DocumentCollection ToCollection()
        {
            return new DocumentCollection { Id = Id };
        }
    }
}
