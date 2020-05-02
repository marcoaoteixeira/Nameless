using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Nameless.Bookshelf.Web.Models {
    public sealed class AuthorModel {
        #region Public Properties

        [JsonProperty ("id")]
        public Guid ID { get; set; }

        [Required]
        [JsonProperty ("name")]
        public string Name { get; set; }

        [JsonProperty ("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty ("modificationDate")]
        public DateTime? ModificationDate { get; set; }

        #endregion
    }
}
