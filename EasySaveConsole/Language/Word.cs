using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EasySaveConsole.Language
{
    public class Word
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("fr")]
        public string Fr { get; set; }

        [JsonPropertyName("en")]
        public string En { get; set; }
        public Word(string Title, string Fr, string En) { 
            this.Title = Title;
            this.Fr = Fr;
            this.En = En;
        }

    }
}
