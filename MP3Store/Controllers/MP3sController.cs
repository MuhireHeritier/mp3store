using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MP3Store.Models;
//using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace MP3Store.Controllers
{
    public class MP3sController : ApiController
    {
        private const String partitionName = "MP3s_Partition_1";

        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;
        private CloudTable table;

        public MP3sController()
        {
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());
            tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference("MP3s");
        }

        /// <summary>
        /// Get all mp3s
        /// </summary>
        /// <returns></returns>
        // GET: api/MP3s
        public IEnumerable<MP3> Get()
        {
            TableQuery<MP3Entity> query = new TableQuery<MP3Entity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionName));
            List<MP3Entity> entityList = new List<MP3Entity>(table.ExecuteQuery(query));

            // Basically create a list of MP3 from the list of MP3Entity with a 1:1 object relationship, filtering data as needed
            IEnumerable<MP3> mp3List = from e in entityList
                                               select new MP3()
                                               {
                                                   MP3ID = e.RowKey,
                                                   Artist = e.Artist,
                                                   MP3Blob = e.MP3Blob,
                                                   Title = e.Title
                                               };
            return mp3List;
        }

        // GET: api/MP3s/5
        /// <summary>
        /// Get a mp3
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(MP3))]
        public IHttpActionResult GetMP3(string id)
        {
            // Create a retrieve operation that takes a mp3 entity.
            TableOperation getOperation = TableOperation.Retrieve<MP3Entity>(partitionName, id);

            // Execute the retrieve operation.
            TableResult getOperationResult = table.Execute(getOperation);

            // Construct response including a new DTO as apprporiatte
            if (getOperationResult.Result == null) return NotFound();
            else
            {
                MP3Entity mp3Entity = (MP3Entity)getOperationResult.Result;
                MP3 p = new MP3()
                {
                    MP3ID = mp3Entity.RowKey,
                    Artist = mp3Entity.Artist,
                    MP3Blob = mp3Entity.MP3Blob,
                    Title = mp3Entity.Title
                };
                return Ok(p);
            }
        }

        // POST: api/MP3s
        /// <summary>
        /// Create a new mp3
        /// </summary>
        /// <param name="mp3"></param>
        /// <returns></returns>
        //[SwaggerResponse(HttpStatusCode.Created)]
        [ResponseType(typeof(MP3))]
        public IHttpActionResult PostMP3(MP3 mp3)
        {
            MP3Entity mp3Entity = new MP3Entity()
            {
                RowKey = getNewMaxRowKeyValue(),
                PartitionKey = partitionName,
                Artist = mp3.Artist,
                MP3Blob = mp3.MP3Blob,
                Title = mp3.Title
            };

            // Create the TableOperation that inserts the mp3 entity.
            var insertOperation = TableOperation.Insert(mp3Entity);

            // Execute the insert operation.
            table.Execute(insertOperation);

            return CreatedAtRoute("DefaultApi", new { id = mp3Entity.RowKey }, mp3Entity);
        }

        // PUT: api/MP3s/5
        /// <summary>
        /// Update a mp3
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mp3"></param>
        /// <returns></returns>
        //[SwaggerResponse(HttpStatusCode.NoContent)]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMP3(string id, MP3 mp3)
        {
            if (id != mp3.MP3ID)
            {
                return BadRequest();
            }

            // Create a retrieve operation that takes a mp3 entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<MP3Entity>(partitionName, id);

            // Execute the operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Assign the result to a MP3Entity object.
            MP3Entity updateEntity = (MP3Entity)retrievedResult.Result;

            updateEntity.Artist = mp3.Artist;
            updateEntity.MP3Blob = mp3.MP3Blob;
            updateEntity.Title = mp3.Title;

            // Create the TableOperation that inserts the mp3 entity.
            // Note semantics of InsertOrReplace() which are consistent with PUT
            // See: https://stackoverflow.com/questions/14685907/difference-between-insert-or-merge-entity-and-insert-or-replace-entity
            var updateOperation = TableOperation.InsertOrReplace(updateEntity);

            // Execute the insert operation.
            table.Execute(updateOperation);

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/MP3s/5
        /// <summary>
        /// Delete a mp3
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(MP3))]
        public IHttpActionResult DeleteMP3(string id)
        {
            // Create a retrieve operation that takes a mp3 entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<MP3Entity>(partitionName, id);

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result == null) return NotFound();
            else
            {
                MP3Entity deleteEntity = (MP3Entity)retrievedResult.Result;
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                // Execute the operation.
                table.Execute(deleteOperation);

                return Ok(retrievedResult.Result);
            }
        }

        private String getNewMaxRowKeyValue()
        {
            TableQuery<MP3Entity> query = new TableQuery<MP3Entity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionName));

            int maxRowKeyValue = 0;
            foreach (MP3Entity entity in table.ExecuteQuery(query))
            {
                int entityRowKeyValue = Int32.Parse(entity.RowKey);
                if (entityRowKeyValue > maxRowKeyValue) maxRowKeyValue = entityRowKeyValue;
            }
            maxRowKeyValue++;
            return maxRowKeyValue.ToString();
        }


    }
}
