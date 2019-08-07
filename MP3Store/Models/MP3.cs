// This is a Data Transfer Object (DTO) class. This is sent/received in REST requests/responses.
// Read about DTOS here: https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5
// Mp3Blob, SampleMp3Blob, SampleMp3URL and SampleDate to add dd

using System.ComponentModel.DataAnnotations;

namespace MP3Store.Models
{
    public class MP3
    {
        /// <summary>
        /// MP3 ID
        /// </summary>
        [Key]
        public string MP3ID { get; set; }

        /// <summary>
        /// Artist of the MP3
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// MP3Blob of the MP3
        /// </summary>
        public string MP3Blob { get; set; }

        /// <summary>
        /// Title of the MP3
        /// </summary>
        public string Title { get; set; }
    }
}