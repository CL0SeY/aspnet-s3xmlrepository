using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.AspNet.DataProtection.Repositories;
using Amazon.S3;
using Amazon.S3.Model;
using System.Xml;
using System.Linq;

namespace S3XmlRepository
{
    public class S3XmlRepositoryImpl : IXmlRepository
    {
        private IAmazonS3 _s3Client;
        private S3XmlRepositoryConfiguration _configuration;

        public S3XmlRepositoryImpl(IAmazonS3 s3Client, S3XmlRepositoryConfiguration configuration)
        {
            _s3Client = s3Client;
            _configuration = configuration;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            var request = new ListObjectsRequest();
            request.BucketName = _configuration.BucketName;
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
            var request = new GetObjectRequest();
            request.BucketName = _configuration.BucketName;
            request.Key = key;
            var result = _s3Client.GetObjectAsync(request).Result;
            XmlDocument document = new XmlDocument();
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
                var result = _s3Client.PutObjectAsync(request).Result;
            }
        }
    }
}
