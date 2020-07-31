namespace IntegrationTests.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public partial class Book
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Id { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("authorId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? AuthorId { get; set; }

        [JsonProperty("links", NullValueHandling = NullValueHandling.Ignore)]
        public List<Link> Links { get; set; }

        public static Book FromJson(string json) => JsonConvert.DeserializeObject<Book>(json, IntegrationTests.Models.Converter.Settings);

    }
}
