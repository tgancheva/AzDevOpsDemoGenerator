using System.Text.Json.Serialization;

namespace ADOGenerator.Models
{ 
    public class ImportSourceCodeRequest
    {
        [JsonPropertyName("parameters")]
        public ImportSourceCodeParameters Parameters { get; set; }
    }

    public class ImportSourceCodeParameters
    {
        [JsonPropertyName("gitSource")]
        public ImportSourceCodeRemoteSource RemoteSource { get; set; }

        [JsonPropertyName("localSource")]
        public ImportSourceCodeLocalSource LocalSource { get; set; }

        [JsonPropertyName("serviceEndpointId")]
        public string ServiceEndpointId { get; set; }

        [JsonPropertyName("deleteServiceEndpointAfterImportIsDone")]
        public bool DeleteServiceEndpointAfterImportIsDone { get; set; }
    }

    public class ImportSourceCodeRemoteSource
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class ImportSourceCodeLocalSource
    {
        [JsonPropertyName("sourcePath")]
        public string SourcePath { get; set; }

        [JsonPropertyName("targetPath")]
        public string TargetPath { get; set; }

        [JsonPropertyName("branches")]
        public List<string> Branches { get; set; }
    }
}
