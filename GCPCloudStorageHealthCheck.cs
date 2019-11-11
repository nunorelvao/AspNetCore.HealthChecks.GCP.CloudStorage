using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.HealthChecks.GCP.CloudStorage
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
            _bucket = bucket;
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

                try
                {
                    if (!string.IsNullOrWhiteSpace(_bucket))
                        _client.GetBucket(_bucket);
                    else
                        _client.ListBuckets(_projectId).All(p => p.Id != _projectId);

                }
                catch (Google.GoogleApiException ex)
                {

                    if (ex.HttpStatusCode == HttpStatusCode.NotFound || ex.HttpStatusCode == HttpStatusCode.BadRequest)
                        return Task.FromResult<HealthCheckResult>(new HealthCheckResult(context.Registration.FailureStatus, (string)null, ex, (IReadOnlyDictionary<string, object>)null));
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
            _client = _credential != null ? StorageClient.Create(_credential) : StorageClient.Create();
        }
    }
}

