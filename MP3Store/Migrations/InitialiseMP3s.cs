using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Configuration;
using MP3Store.Models;

namespace MP3Store.Migrations
{
    public static class InitialiseMP3s
    {
        public static void go()
        {
            const String partitionName = "MP3s_Partition_1";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("MP3s");

            // If table doesn't already exist in storage then create and populate it with some initial values, otherwise do nothing
            if (!table.Exists())
            {
                // Create table if it doesn't exist already
                table.CreateIfNotExists();

                // Create the batch operation.
                TableBatchOperation batchOperation = new TableBatchOperation();

                // Create a mp3 entity and add it to the table.
                MP3Entity mp3Obj1 = new MP3Entity(partitionName, "1");
                mp3Obj1.Artist = "Bob Marley";
                mp3Obj1.Title = "Widget";
                mp3Obj1.MP3Blob = "BlobFor11";

                // Create another mp3 entity and add it to the table.
                MP3Entity mp3Obj2 = new MP3Entity(partitionName, "2");
                mp3Obj2.Artist = "Santana";
                mp3Obj2.Title = "Material";
                mp3Obj2.MP3Blob = "BlobFor22";

                // Create another mp3 entity and add it to the table.
                MP3Entity mp3Obj3 = new MP3Entity(partitionName, "3");
                mp3Obj3.Artist = "Thingy";
                mp3Obj3.Title = "Widget";
                mp3Obj3.MP3Blob = "BlobFor33";

                // Create another mp3 entity and add it to the table.
                MP3Entity mp3Obj4 = new MP3Entity(partitionName, "4");
                mp3Obj4.Artist = "Skepta";
                mp3Obj4.Title = "Material";
                mp3Obj4.MP3Blob = "BlobFor4";

                // Add mp3 entities to the batch insert operation.
                batchOperation.Insert(mp3Obj1);
                batchOperation.Insert(mp3Obj2);
                batchOperation.Insert(mp3Obj3);
                batchOperation.Insert(mp3Obj4);

                // Execute the batch operation.
                table.ExecuteBatch(batchOperation);
            }

        }
    }
}