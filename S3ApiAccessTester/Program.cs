using System;
using CommandLine;
using S3ApiAccessTester.Storage;

namespace S3ApiAccessTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<S3Options>(args).WithParsed(o =>
            {
                UploadFileInfo uploadFileInfo = new UploadFileInfo(o.Path);

                IS3BucketClientFactory s3ClientFactory = new S3BucketClientFactory();
                IFileStorage fileStorage = new S3FileStorage(s3ClientFactory, o);

                ComplexTest test = new ComplexTest(fileStorage, o.Bucket);

                test.ExpectFileToNotExist(o.Key);

                test.CanCreateFile(o.Key, uploadFileInfo);
                test.ExpectFileToExist(o.Key);
                test.CanReadFile(o.Key, uploadFileInfo);
                test.CanDeleteFile(o.Key);
            });
        }
    }
}