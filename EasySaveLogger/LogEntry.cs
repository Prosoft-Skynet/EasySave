using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EasySaveLogger
{
    public class LogEntry
    {
        
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("backupName")]
        public string JobName { get; set; }

        [JsonPropertyName("sourcePath")]
        public string SourcePath { get; set; }

        [JsonPropertyName("destinationPath")]
        public string TargetPath { get; set; }

        [JsonPropertyName("fileSize")]
        public long FileSize { get; set; }

        [JsonPropertyName("transferTimeMs")]
        public long TransferTime { get; set; }
        


        public LogEntry(string timestamp, string jobName, string sourcePath, string targetPath, long fileSize, long transferTime)
        {
            this.Timestamp = timestamp;
            this.JobName = jobName;
            this.SourcePath = sourcePath;
            this.TargetPath = targetPath;
            this.FileSize = fileSize;
            this.TransferTime = transferTime;
        }
    }
}
