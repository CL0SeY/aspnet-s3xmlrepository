# ASP.NET S3XmlRepository
An implementation for S3 storage of IXmlRepository for use in ASP.NET Core.

This allows ASP.NET Core to store its keys in S3. (For information on the IXmlRepository class see here: http://docs.asp.net/en/latest/security/data-protection/extensibility/key-management.html#ixmlrepository)

This is useful in load balancing scenarios where cookies may be encrypted using different keys.

It is highly recommended you encrypt your bucket and secure it appropriately.

## Configuration
In Startup.cs:
```csharp
public void ConfigureServices(IServiceCollection services)
{
  //...
  var s3XmlRepositoryConfiguration = new S3XmlRepositoryConfiguration { BucketName = "my-secure-bucket" };
  services.ConfigureDataProtection(configure =>
  {
      configure.PersistKeysToS3(s3XmlRepositoryConfiguration);
  });

  services.AddTransient<AWSCredentials, StoredProfileAWSCredentials>();
  services.AddInstance(RegionEndpoint.GetBySystemName("my-region"));
  services.AddTransient<AmazonS3Client>();
  //...
}
```
This example uses the StoredProfileAWSCredentials. Other implementations of AWSCredentials are available in the [AWS SDK](https://aws.amazon.com/sdk-for-net/).

