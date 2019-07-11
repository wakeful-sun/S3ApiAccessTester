using System;
using System.Linq;
using System.Threading.Tasks;
using S3ApiAccessTester.Storage;

namespace S3ApiAccessTester
{
    class ComplexTest
    {
        readonly IFileStorage fileStorage;
        readonly string bucket;

        public bool ComplexTestHasFailed { get; private set; }

        public ComplexTest(IFileStorage fileStorage, string bucket)
        {
            this.fileStorage = fileStorage;
            this.bucket = bucket;
        }

        public void ExpectFileToExist(string key)
        {
            string testDescription = $"Object [{key}] should exist";
            ExecuteCheck(testDescription, async () =>
            {
                bool objectExist = await fileStorage.Exists(bucket, key);
                if (!objectExist)
                {
                    throw new Exception(testDescription);
                }
            });
        }

        public void ExpectFileToNotExist(string key)
        {
            string testDescription = $"Object '{key}' should not exist";
            ExecuteCheck(testDescription, async () =>
            {
                bool objectExist = await fileStorage.Exists(bucket, key);
                if (objectExist)
                {
                    throw new Exception(testDescription);
                }
            });
        }

        public void CanCreateFile(string key, UploadFileInfo uploadFileInfo)
        {
            string testDescription = $"Object can be created by '{key}' key";
            ExecuteCheck(testDescription, async () =>
            {
                await fileStorage.Create(bucket, key, uploadFileInfo.Content, uploadFileInfo.Metadata, uploadFileInfo.MimeType);
            });
        }

        public void CanReadFile(string key, UploadFileInfo uploadFileInfo)
        {
            string testDescription = $"Object can be downloaded by '{key}' key";
            ExecuteCheck(testDescription, async () =>
            {
                byte[] fileContent = await fileStorage.Read(bucket, key);
                if (!fileContent.SequenceEqual(uploadFileInfo.Content))
                {
                    throw new Exception($"{testDescription} test failed. Downloaded file content is different from expected file content.");
                }
            });
        }

        public void CanDeleteFile(string key)
        {
            string testDescription = $"Object can be deleted by '{key}' key";
            ExecuteCheck(testDescription, async () =>
            {
                await fileStorage.Delete(bucket, key);
            });
        }

        void ExecuteCheck(string testDescription, Func<Task> func)
        {
            try
            {
                if (ComplexTestHasFailed)
                {
                    PrintInColor("[SKIP]\t", ConsoleColor.DarkYellow);
                }
                else
                {
                    Task.WaitAll(func.Invoke());
                    PrintInColor("[OK]\t", ConsoleColor.Green);
                }

                Console.WriteLine(testDescription);
            }
            catch (Exception e)
            {
                ComplexTestHasFailed = true;

                PrintInColor("[FAIL]\t", ConsoleColor.Red);
                Console.Write($"{testDescription}. Error message: ");
                PrintInColor(e.Message, ConsoleColor.Red);
                Console.WriteLine();
            }
        }

        void PrintInColor(string text, ConsoleColor color)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = originalColor;
        }
    }
}