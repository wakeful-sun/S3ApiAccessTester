using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace S3ApiAccessTester.Storage
{
    interface IFileStorage
    {
        Task<byte[]> Read(string bucketName, string key);
        Task Create(string bucketName, string key, byte[] file, IDictionary<string, string> metadata, string contentType);
        Task<bool> Exists(string bucketName, string key);
        Task Delete(string bucketName, string key);
    }

    class S3FileStorage : IFileStorage
    {
        readonly IS3BucketClientFactory bucketClientFactory;
        readonly S3Options options;

        public S3FileStorage(IS3BucketClientFactory bucketClientFactory, S3Options options)
        {
            this.bucketClientFactory = bucketClientFactory;
            this.options = options;
        }

        public async Task Create(string bucketName, string key, byte[] file, IDictionary<string, string> metadata, string contentType)
        {
            using (IAmazonS3 client = bucketClientFactory.Create(options))
            {
                PutObjectRequest request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    CannedACL = S3CannedACL.AuthenticatedRead,
                    ContentType = contentType,
                    StorageClass = S3StorageClass.Standard,
                    InputStream = new MemoryStream(file),
                    AutoCloseStream = true
                };

                if (metadata != null)
                {
                    foreach (KeyValuePair<string, string> metadataEntry in metadata)
                    {
                        request.Metadata.Add(metadataEntry.Key, metadataEntry.Value);
                    }
                }
                await client.PutObjectAsync(request);
            }
        }

        public async Task<byte[]> Read(string bucketName, string key)
        {
            using (IAmazonS3 client = bucketClientFactory.Create(options))
            {
                GetObjectResponse response = await client.GetObjectAsync(bucketName, key);
                return await response.ResponseStream.ToArray();
            }
        }

        public async Task<bool> Exists(string bucketName, string key)
        {
            using (IAmazonS3 client = bucketClientFactory.Create(options))
            {
                try
                {
                    await client.GetObjectMetadataAsync(bucketName, key);
                }
                catch (AmazonS3Exception exception)
                {
                    if (exception.StatusCode == HttpStatusCode.NotFound)
                    {
                        return false;
                    }
                    throw;
                }
                return true;
            }
        }

        public async Task Delete(string bucketName, string key)
        {
            using (IAmazonS3 client = bucketClientFactory.Create(options))
            {
                await client.DeleteObjectAsync(bucketName, key);
            }
        }
    }

    public static class StreamHelper
    {
        public static async Task CopyStream(this Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        public static async Task<byte[]> ToArray(this Stream input)
        {
            using (var ms = new MemoryStream())
            {
                await CopyStream(input, ms);
                return ms.ToArray();
            }
        }
    }
}