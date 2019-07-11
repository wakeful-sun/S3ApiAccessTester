using System;
using System.Collections.Generic;
using System.IO;

namespace S3ApiAccessTester
{
    class UploadFileInfo
    {
        public UploadFileInfo(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                throw new ArgumentException($"File {filePath} does not exist.");
            }

            MimeTypes.FallbackMimeType = "application/octet-stream";
            MimeType = MimeTypes.GetMimeType($"{fileInfo.Name}.{fileInfo.Extension}");

            Content = File.ReadAllBytes(filePath);

            Metadata = new Dictionary<string, string>
            {
                { "fileName", fileInfo.Name },
                { "fileExtension", fileInfo.Extension }
            };
        }

        public string MimeType { get; }
        public byte[] Content { get; }
        public Dictionary<string, string> Metadata { get; }
    }
}