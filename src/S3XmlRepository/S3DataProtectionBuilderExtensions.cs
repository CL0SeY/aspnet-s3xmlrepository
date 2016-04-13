using System;
using Amazon.S3;
using Microsoft.AspNet.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace aspnet_s3_xmlrepository
{
    public static class S3DataProtectionBuilderExtensions
    {
        private static void RemoveAllServicesOfType(IServiceCollection services, Type serviceType)
        {
            // We go backward since we're modifying the collection in-place.
            for (int i = services.Count - 1; i >= 0; i--)
            {
                if (services[i]?.ServiceType == serviceType)
                {
                    services.RemoveAt(i);
                }
            }
        }

        private static void Use(IServiceCollection services, ServiceDescriptor descriptor)
        {
            RemoveAllServicesOfType(services, descriptor.ServiceType);
            services.Add(descriptor);
        }

        /// <summary>
        /// Configures the data protection system to persist keys to the specified directory.
        /// This path may be on the local machine or may point to a UNC share.
        /// </summary>
        /// <param name="builder">The <see cref="IDataProtectionBuilder"/>.</param>
        /// <param name="directory">The directory in which to store keys.</param>
        /// <returns>A reference to the <see cref="IDataProtectionBuilder" /> after this operation has completed.</returns>
        public static Microsoft.AspNet.DataProtection.DataProtectionConfiguration PersistKeysToS3(this Microsoft.AspNet.DataProtection.DataProtectionConfiguration builder, S3XmlRepositoryConfiguration configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            Use(builder.Services, IXmlRepository_S3(configuration));
            return builder;
        }

        public static ServiceDescriptor IXmlRepository_S3(S3XmlRepositoryConfiguration configuration)
        {
            return ServiceDescriptor.Singleton<IXmlRepository>(services => new S3XmlRepository(services.GetService<AmazonS3Client>(), configuration));
        }
    }
}