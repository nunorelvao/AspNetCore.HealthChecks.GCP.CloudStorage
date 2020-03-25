using FluentAssertions;
using FunctionalTests.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.HealthChecks.GCP.CloudStorage.DependencyInjection;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Storage.v1;
using Google.Cloud.Storage.V1;
using Xunit;
using Xunit.Abstractions;

namespace FunctionalTests
{
    //[Collection("execution")]
    public class CloudStorageFunctionalTest
    {
        private readonly ITestOutputHelper _testOutputHelper;
        //private readonly ExecutionFixture _fixture;


        //public FunctionalTest(ExecutionFixture fixture)
        //{
        //    _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        //}

        private string jsonCredTest;


        public CloudStorageFunctionalTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            jsonCredTest = Environment.GetEnvironmentVariable("GCP_STORAGE_SA_CRED");
            Console.WriteLine("RUNNING FUNCTIONAL TEST");
        }

        [Fact]
        public async Task be_healthy_if_hc_is_available()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(services =>
                {

                    services.AddHealthChecks()
                        .AddGcpCloudStorage(setup =>
                        {
                            setup.ProjectId = "fluent-outpost-271614";
                            setup.Bucket = null;
                            setup.GoogleCredential =
                                !string.IsNullOrWhiteSpace(jsonCredTest)
                                    ? GoogleCredential.FromJson(jsonCredTest)
                                    : GoogleCredential.FromFile("c:\\tmp\\gkey\\nuno-relvao-271614-146b13a7f6eb.json");

                            //OTHER WAYS TO AUTHENTICATE
                            //setup.GoogleCredential = GoogleCredential.GetApplicationDefault();no
                            //setup.GoogleCredential = GoogleCredential.FromComputeCredential(new ComputeCredential());
                            //setup.GoogleCredential = GoogleCredential.FromServiceAccountCredential(new ServiceAccountCredential(null));
                            //setup.GoogleCredential = GoogleCredential.FromFile("./path");
                            //setup.GoogleCredential = GoogleCredential.FromJson("./path/jsonfile.json");
                            //setup.GoogleCredential = GoogleCredential.FromStream(Stream.Null);
                        }, name: "my-cloud-storage-group", null, new string[] { "cloudstoragehc" });

                })
                .Configure(app =>
                {
                    app.UseHealthChecks("/health", new HealthCheckOptions()
                    {
                        Predicate = r => r.Tags.Contains("cloudstoragehc")
                    });
                });



            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest($"/health")
                .GetAsync();


            response.StatusCode
                .Should().Be(HttpStatusCode.OK);

        }

        [Fact]
        public async Task be_unhealthy_if_hc_is_not_available()
        {

            var webHostBuilder = new WebHostBuilder()
                .UseStartup<DefaultStartup>()
                .ConfigureServices(services =>
                {

                    services.AddHealthChecks()
                        .AddGcpCloudStorage(setup =>
                        {
                            setup.ProjectId = "nonexistingproject";
                            setup.Bucket = "nonexistingbucket";
                            setup.GoogleCredential = GoogleCredential.FromAccessToken("xxxxxxxxx");

                            //OTHER WAYS TO AUTHENTICATE
                            //setup.GoogleCredential = GoogleCredential.GetApplicationDefault();
                            //setup.GoogleCredential = GoogleCredential.FromComputeCredential(new ComputeCredential());
                            //setup.GoogleCredential = GoogleCredential.FromServiceAccountCredential(new ServiceAccountCredential(null));
                            //setup.GoogleCredential = GoogleCredential.FromFile("./path");
                            //setup.GoogleCredential = GoogleCredential.FromJson("./path/jsonfile.json");
                            //setup.GoogleCredential = GoogleCredential.FromStream(Stream.Null);
                        }, name: "my-cloud-storage-group", null, new string[] { "cloudstoragehc" });

                })
                .Configure(app =>
                {
                    app.UseHealthChecks("/health", new HealthCheckOptions()
                    {
                        Predicate = r => r.Tags.Contains("cloudstoragehc")
                    });
                });

            var server = new TestServer(webHostBuilder);

            var response = await server.CreateRequest($"/health")
                .GetAsync();

            response.StatusCode
                .Should().Be(HttpStatusCode.ServiceUnavailable);

        }
    }
}
