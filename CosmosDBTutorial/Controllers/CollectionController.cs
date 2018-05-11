using CosmosDBTutorial.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDBTutorial.Controllers
{
    [Produces("application/json")]
    [Route("api/collections")]
    public class CollectionController : Controller
    {
        // Settings --> Keys --> URI
        private const string CosmosDBURI = "https://localhost:8081";
        // Settings --> Keys --> PRIMARY KEY
        private const string CosmosDBPrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        // Data Explorer --> Database
        private const string DatabaseId = "SampleDB";

        private readonly DocumentClient _documentClient;
        private readonly Uri _databaseUri;

        public CollectionController()
        {
            _documentClient = new DocumentClient(new Uri(CosmosDBURI), CosmosDBPrimaryKey);
            _databaseUri = UriFactory.CreateDatabaseUri(DatabaseId);
        }

        [HttpGet]
        public IActionResult GetAllCollection()
        {
            List<DocumentCollection> collections = _documentClient.CreateDocumentCollectionQuery(_databaseUri).ToList();
            return Ok(collections);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetCollectionById(string id)
        {
            DocumentCollection collection = _documentClient.CreateDocumentCollectionQuery(_databaseUri)
                                                           .Where(x => x.Id == id)
                                                           .AsEnumerable()
                                                           .FirstOrDefault();

            if (collection == null)
            {
                return NotFound();
            }

            return Ok(collection);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCollection([FromBody] CreateCollectionRequest request)
        {
            if (ExistsCollection(request.Id))
            {
                return BadRequest();
            }

            ResourceResponse<DocumentCollection> response = await _documentClient.CreateDocumentCollectionAsync(_databaseUri, request.ToCollection());
            return Ok(response.Resource);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCollection(string id)
        {
            if (!ExistsCollection(id))
            {
                return NotFound();
            }

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, id);
            await _documentClient.DeleteDocumentCollectionAsync(collectionUri);
            return Ok();
        }

        private bool ExistsCollection(string id)
        {
            DocumentCollection collection = _documentClient.CreateDocumentCollectionQuery(_databaseUri)
                                                           .Where(x => x.Id == id)
                                                           .AsEnumerable()
                                                           .FirstOrDefault();
            return collection != null ? true : false;
        }
    }
}