// Entity class for Azure table
using Microsoft.WindowsAzure.Storage.Table;

namespace MP3Store.Models
{

    public class MP3Entity : TableEntity
    {
        public string Artist { get; set; }
        public string MP3Blob { get; set; }
        public string Title { get; set; }

        public MP3Entity(string partitionKey, string mp3ID)
        {
            PartitionKey = partitionKey;
            RowKey = mp3ID;
        }

        public MP3Entity() { }

    }
}
