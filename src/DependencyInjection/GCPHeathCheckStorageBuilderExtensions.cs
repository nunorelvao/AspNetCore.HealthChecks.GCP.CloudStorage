using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.HealthChecks.GCP.CloudStorage.DependencyInjection
{
    public static class GcpHeathCheckStorageBuilderExtensions
    {
        private const string NAME = "gcpcloudstorage";

        /// <summary>
        /// Add a health check for GCP CloudStorage.
        /// </summary>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="setup">The action to configure the Cloud Storage parameters.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'gcpcloudstorage' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
        public static IHealthChecksBuilder AddGcpCloudStorage(
            this IHealthChecksBuilder builder,
            Action<CloudStorageOptions> setup,
            string name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string> tags = default)
        {
            var options = new CloudStorageOptions();
            setup?.Invoke(options);

            return builder.Add(new HealthCheckRegistration(name ?? NAME,
                   sp => new GcpCloudStorageHealthCheck(options),
                   failureStatus,
                   tags));
        }

    }
}