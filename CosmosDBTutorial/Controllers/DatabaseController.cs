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
    [Route("api/databases")]
    public class DatabaseController : Controller
    {
        // Settings --> Keys --> URI
        private const string CosmosDBURI = "https://localhost:8081";
        // Settings --> Keys --> PRIMARY KEY
        private const string CosmosDBPrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        private readonly DocumentClient _documentClient;

        public DatabaseController()
        {
            _documentClient = new DocumentClient(new Uri(CosmosDBURI), CosmosDBPrimaryKey);
        }

        [HttpGet]
        public IActionResult GetAllDatabases()
        {
            List<Database> databases = _documentClient.CreateDatabaseQuery().ToList();
            return Ok(databases);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetDatabaseById(string id)
        {
            Database database = _documentClient.CreateDatabaseQuery()
                                               .Where(x => x.Id == id)
                                               .AsEnumerable()
                                               .FirstOrDefault();

            if (database == null)
            {
                return NotFound();
            }

            return Ok(database);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDatabase([FromBody] CreateDatabaseRequest request)
        {
            if (ExistsDatabase(request.Id))
            {
                return BadRequest();
            }

            ResourceResponse<Database> response = await _documentClient.CreateDatabaseAsync(request.ToDatabase());
            return Ok(response.Resource);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteDatabase(string id)
        {
            if (!ExistsDatabase(id))
            {
                return NotFound();
            }

            Uri databaseUri = UriFactory.CreateDatabaseUri(id);
            await _documentClient.DeleteDatabaseAsync(databaseUri);
            return Ok();
        }

        private bool ExistsDatabase(string id)
        {
            Database database = _documentClient.CreateDatabaseQuery()
                                               .Where(x => x.Id == id)
                                               .AsEnumerable()
                                               .FirstOrDefault();
            return database != null ? true : false;
        }
    }
}
