using CommandLine;

namespace S3ApiAccessTester
{
    class S3Options
    {
        [Option(Required = true, HelpText = "S3 API URL. For example http://localhost:9000")]
        public string Api { get; set; }
        [Option(Required = true, HelpText = "S3 API access key.")]
        public string AccessKey { get; set; }
        [Option(Required = true, HelpText = "S3 API secret key.")]
        public string SecretKey { get; set; }

        [Option(Required = true, HelpText = "S3 bucket name.")]
        public string Bucket { get; set; }

        [Option(Required = true, HelpText = "S3 object key reference.")]
        public string Key { get; set; }
        [Option(Required = false, HelpText = "Path to the file you want to upload and test interactions with.")]
        public string Path { get; set; }
    }
}