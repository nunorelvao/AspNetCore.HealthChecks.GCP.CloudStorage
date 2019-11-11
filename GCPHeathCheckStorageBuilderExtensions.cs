﻿using System;
using System.Collections.Generic;
using AspNetCore.HealthChecks.GCP.CloudStorage.HealthChecks.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.HealthChecks.GCP.CloudStorage
{
    public static class GcpHeathCheckStorageBuilderExtensions
    {

        private const string NAME = "gcpcloudstorage";

        public static IHealthChecksBuilder AddGCPCloudStorage(
            this IHealthChecksBuilder builder,
            string projectId,
            string bucket,
            string name = null,
            HealthStatus? failureStatus = null,
            IEnumerable<string> tags = null)
        {
            return builder.Add(
                new HealthCheckRegistration(name ?? "gcpcloudstorage",
                    (Func<IServiceProvider, IHealthCheck>)(sp =>
                        (IHealthCheck)new GcpCloudStorageHealthCheck(projectId, bucket)), failureStatus, tags));
        }
    }
}