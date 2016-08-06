using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Amazon.S3;
using Amazon.S3.Model;
using System.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace S3XmlRepository
{
    public class S3XmlRepositoryImpl : IXmlRepository
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3XmlRepositoryConfiguration _configuration;

        public S3XmlRepositoryImpl(IAmazonS3 s3Client, S3XmlRepositoryConfiguration configuration)
        {
            _s3Client = s3Client;
            _configuration = configuration;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var request = new ListObjectsRequest {BucketName = _configuration.BucketName};
            var result = _s3Client.ListObjectsAsync(request).Result;
            if (result.IsTruncated)
            {
                throw new TooManyObjectsException();
            }
            var s3Objects = result.S3Objects.Select(o => GetElement(o.Key)).ToArray();
            return s3Objects;
        }

        private XElement GetElement(string key)
        {
            var request = new GetObjectRequest
            {
                BucketName = _configuration.BucketName,
                Key = key
            };
            // Does not support non-async in .NET Core:
            var result = _s3Client.GetObjectAsync(request).Result;
            using (var stream = result.ResponseStream)
            {
                var xDocument = XDocument.Load(stream);
                return xDocument.Elements().First(); 
            }
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            using (var memoryStream = new MemoryStream())
            {
                var document = new XDocument();
                document.Add(element);
                document.Save(memoryStream);
                var request = new PutObjectRequest
                {
                    BucketName = _configuration.BucketName,
                    Key = DateTime.UtcNow.ToString("s") + "_" + friendlyName,
                    InputStream = memoryStream,
                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
                };
                // Does not support non-async in .NET Core:
                var result = _s3Client.PutObjectAsync(request).Result;
            }
        }
    }
}
