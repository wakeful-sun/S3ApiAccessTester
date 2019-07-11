using Amazon.Runtime;
using Amazon.S3;

namespace S3ApiAccessTester.Storage
{
    interface IS3BucketClientFactory
    {
        IAmazonS3 Create(S3Options settings);
    }

    class S3BucketClientFactory : IS3BucketClientFactory
    {
        public IAmazonS3 Create(S3Options settings)
        {
            AWSCredentials credentials = new BasicAWSCredentials(settings.AccessKey, settings.SecretKey);
            AmazonS3Config config = new AmazonS3Config
            {
                ServiceURL = settings.Api,
                ForcePathStyle = true
            };

            return new AmazonS3Client(credentials, config);
        }
    }
}
