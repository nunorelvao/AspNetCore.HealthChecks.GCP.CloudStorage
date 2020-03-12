using System;
using System.Linq;
using AspNetCore.HealthChecks.GCP.CloudStorage;
using AspNetCore.HealthChecks.GCP.CloudStorage.DependencyInjection;
using FluentAssertions;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Xunit;

namespace Tests
{
    public class CloudStorageUnitTest
    {
        [Fact]
        public void add_health_check_when_properly_configured()
        {

            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddGcpCloudStorage(setup =>
                {
                    setup.ProjectId = Guid.NewGuid().ToString();
                    setup.Bucket = "mybucket";
                    setup.GoogleCredential = GoogleCredential.GetApplicationDefault();
                    //OTHER WAYS TO AUTHENTICATE
                    //setup.GoogleCredential = GoogleCredential.FromAccessToken("xxxxxxxxx");
                    //setup.GoogleCredential = GoogleCredential.FromComputeCredential(new ComputeCredential());
                    //setup.GoogleCredential = GoogleCredential.FromServiceAccountCredential(new ServiceAccountCredential(null));
                    //setup.GoogleCredential = GoogleCredential.FromFile("./path");
                    //setup.GoogleCredential = GoogleCredential.FromJson("./path/jsonfile.json");
                    //setup.GoogleCredential = GoogleCredential.FromStream(Stream.Null);
                });

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("gcpcloudstorage");
            check.GetType().Should().Be(typeof(GcpCloudStorageHealthCheck));
        }
        [Fact]
        public void add_named_health_check_when_properly_configured()
        {
            var services = new ServiceCollection();

            services.AddHealthChecks()
                .AddGcpCloudStorage(setup =>
                {
                    setup.ProjectId = Guid.NewGuid().ToString();
                    setup.Bucket = "mybucket";
                    setup.GoogleCredential = GoogleCredential.GetApplicationDefault();
                    //OTHER WAYS TO AUTHENTICATE
                    //setup.GoogleCredential = GoogleCredential.FromAccessToken("xxxxxxxxx");
                    //setup.GoogleCredential = GoogleCredential.FromComputeCredential(new ComputeCredential());
                    //setup.GoogleCredential = GoogleCredential.FromServiceAccountCredential(new ServiceAccountCredential(null));
                    //setup.GoogleCredential = GoogleCredential.FromFile("./path");
                    //setup.GoogleCredential = GoogleCredential.FromJson("./path/jsonfile.json");
                    //setup.GoogleCredential = GoogleCredential.FromStream(Stream.Null);
                }, name: "my-cloud-storage-group");

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("my-cloud-storage-group");
            check.GetType().Should().Be(typeof(GcpCloudStorageHealthCheck));
        }
    }
}