using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.HealthChecks.GCP.CloudStorage
{
    public class GcpCloudStorageHealthCheck : IHealthCheck
    {
        private StorageClient _client;
        private readonly CloudStorageOptions _cloudStorageOptions;

        public GcpCloudStorageHealthCheck(CloudStorageOptions cloudStorageOptions)
        {
            _cloudStorageOptions = cloudStorageOptions ?? throw new ArgumentNullException(nameof(cloudStorageOptions));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
          HealthCheckContext context,
          CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                CreateConnection();

                try
                {
                    if (!string.IsNullOrWhiteSpace(_cloudStorageOptions.Bucket))
                        _client.GetBucket(_cloudStorageOptions.Bucket);
                    else
                        _client.ListBuckets(_cloudStorageOptions.ProjectId).All(p => p.Id != _cloudStorageOptions.ProjectId);
                }
                catch (Google.GoogleApiException ex)
                {
                    return await Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, null, ex, null));
                }

                return await Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, (string)null, ex, null));
            }
        }

        private void CreateConnection()
        {
            _client = _cloudStorageOptions.GoogleCredential != null ? StorageClient.Create(_cloudStorageOptions.GoogleCredential) : StorageClient.Create();
        }
    }
}