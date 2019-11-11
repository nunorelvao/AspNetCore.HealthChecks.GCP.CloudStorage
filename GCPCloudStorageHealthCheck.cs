using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.HealthChecks.GCP.CloudStorage
{
    namespace HealthChecks.RabbitMQ
    {
        public class GcpCloudStorageHealthCheck : IHealthCheck
        {
            private readonly string _projectId;
            private readonly string _bucket;
            private StorageClient _client;
            private readonly GoogleCredential _credential;

            public GcpCloudStorageHealthCheck(string projectId, string bucket = null)
            {

                if (projectId == null)
                    throw new ArgumentNullException(nameof(projectId));
                //if (bucket == null)
                //    throw new ArgumentNullException(nameof(bucket));
                _projectId = projectId;
                _bucket = bucket = projectId;
                _credential = null;
            }

            public GcpCloudStorageHealthCheck(GoogleCredential credential, string projectId, string bucket = null)
            {

                if (projectId == null)
                    throw new ArgumentNullException(nameof(projectId));
                //if (bucket == null)
                //    throw new ArgumentNullException(nameof(bucket));
                _projectId = projectId;
                _bucket = bucket = projectId;
                _credential = credential;

            }


            public Task<HealthCheckResult> CheckHealthAsync(
              HealthCheckContext context,
              CancellationToken cancellationToken = default(CancellationToken))
            {

                try
                {
                    CreateConnection();


                    if (!string.IsNullOrWhiteSpace(_bucket))
                    {
                        var clientBasePath =
                            _client.ListObjects(_bucket, null, new ListObjectsOptions() { PageSize = 1 });
                        if (clientBasePath.ToList().Count == 0)
                        {
                            throw new Exception("Bucket Read error");

                        }
                    }
                    else
                    {
                        var clientBasePath =
                            _client.ListBuckets(_projectId, new ListBucketsOptions() { PageSize = 1 });

                        if (clientBasePath.ToList().Count == 0)
                        {
                            throw new Exception("Bucket Read error");

                        }
                    }

                    return Task.FromResult<HealthCheckResult>(HealthCheckResult.Healthy((string)null, (IReadOnlyDictionary<string, object>)null));

                }
                catch (Exception ex)
                {
                    return Task.FromResult<HealthCheckResult>(new HealthCheckResult(context.Registration.FailureStatus, (string)null, ex, (IReadOnlyDictionary<string, object>)null));
                }
            }

            private void CreateConnection()
            {
                if (_credential != null)
                {
                    _client = StorageClient.Create(_credential);
                }
                else
                {
                    _client = StorageClient.Create();
                }
            }
        }
    }
}
