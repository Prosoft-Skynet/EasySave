using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EasySaveCore.src.Models
{
    /// <summary>
    /// Represents a word model.
    /// </summary>
    public class WordModel
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("fr")]
        public string Fr { get; set; }

        [JsonPropertyName("en")]
        public string En { get; set; }
        public WordModel(string Title, string Fr, string En)
        {
            this.Title = Title;
            this.Fr = Fr;
            this.En = En;
        }

    }
}
