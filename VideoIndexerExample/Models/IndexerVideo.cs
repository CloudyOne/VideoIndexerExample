using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoIndexerExample.Models
{
    public class IndexerVideo
    {
        public string AccountId { get; set; }

        public string Id { get; set; }

        public string Partition { get; set; }

        public string ExternalId { get; set; }

        public string Metadata { get; set; }

        public string Name { get; set; }

        public object Description { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public DateTime LastIndexingStartTime { get; set; }

        public string Organization { get; set; }

        public string PrivacyMode { get; set; }

        public string UserName { get; set; }

        public bool IsOwned { get; set; }

        public bool IsBase { get; set; }

        public string State { get; set; }

        public string ProcessingProgress { get; set; }

        public int DurationInSeconds { get; set; }

        public string ThumbnailUrl { get; set; }

        public IndexerSocial Social { get; set; }

        public List<string> SearchMatches { get; set; }

        public string IndexingPreset { get; set; }

        public string StreamingPreset { get; set; }

        public string SourceLanguage { get; set; }
    }
}
