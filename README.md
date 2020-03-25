# AspNetCore.HealthChecks.GCP.CloudStorage
Extension to be used in .Net core projects using Xabaril/AspNetCore.Diagnostics.HealthChecks

>Please visit the project at Xabaril/AspNetCore.Diagnostics.HealthChecks

more info at:
[AspNetCore.Diagnostics.HealthChecks](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/README.md)

>NUGET INSTALL
``` PowerShell
Install-Package AspNetCore.HealthChecks.GCP.CloudStorage
```

>SAMPLE CODE USING DEFAULT GOOGLE CREDENTIALS ENV VARIABLE AND ONLY PROJECTID
```csharp
//OLD VERSION  (2.0.5 AND DOWN)
public void ConfigureServices(IServiceCollection services)
{
    services.AddHealthChecks()
       .AddGcpCloudStorage(
            projectId: "myprojectid",
            bucket: "mybucket (Not mandatory if not provided only projectid will me targeted to be monitored",
            name: "myname",
            failureStatus: HealthStatus.Degraded,
            tags: new string[] { "mytag" });

//NEW VERSION (2.0.6 and UP)
 services.AddHealthChecks()
    .AddGcpCloudStorage(setup =>
        {
            setup.ProjectId = "projectId";
            setup.Bucket = null;
            setup.GoogleCredential = GoogleCredential.GetApplicationDefault();
            //OTHER WAYS TO AUTHENTICATE
            //setup.GoogleCredential = GoogleCredential.GetApplicationDefault();no
            //setup.GoogleCredential = GoogleCredential.FromComputeCredential(new ComputeCredential());
            //setup.GoogleCredential = GoogleCredential.FromServiceAccountCredential(new ServiceAccountCredential(null));
            //setup.GoogleCredential = GoogleCredential.FromFile("./path/tojsonfile.json");
            //setup.GoogleCredential = GoogleCredential.FromJson("jsonstring");
            //setup.GoogleCredential = GoogleCredential.FromStream(Stream.Null);
        }, name: "my-cloud-storage-group", null, new string[] { "cloudstoragehc" });
}
```

>SAMPLE CODE PASSING GOOGLE CREDENTIALS AND A SPECIFIC BUCKET
```csharp
//OLD VERSION  (2.0.5 AND DOWN)
public void ConfigureServices(IServiceCollection services)
{

    var googleCredentialFile = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile("my_credential_cloud_file.json");

        services.AddHealthChecks()
        .AddGcpCloudStorage(
                googleCredential: googleCredentialFile
                projectId: "myprojectid",
                bucket: "mybucket (Not mandatory if not provided only projectid will me targeted to be monitored",
                name: "myname",
                failureStatus: HealthStatus.Degraded,
                tags: new string[] { "mytag" });
    }

 //NEW VERSION (2.0.6 and UP)
 services.AddHealthChecks()
    .AddGcpCloudStorage(setup =>
        {
            setup.ProjectId = "projectId";
            setup.Bucket = "bucketname";
            setup.GoogleCredential = GoogleCredential.FromFile("./path/tojsonfile.json");
            //OTHER WAYS TO AUTHENTICATE
            //setup.GoogleCredential = GoogleCredential.GetApplicationDefault();no
            //setup.GoogleCredential = GoogleCredential.FromComputeCredential(new ComputeCredential());
            //setup.GoogleCredential = GoogleCredential.FromServiceAccountCredential(new ServiceAccountCredential(null));
            //setup.GoogleCredential = GoogleCredential.FromFile("./path/tojsonfile.json");
            //setup.GoogleCredential = GoogleCredential.FromJson("jsonstring");
            //setup.GoogleCredential = GoogleCredential.FromStream(Stream.Null);
        }, name: "my-cloud-storage-group", null, new string[] { "cloudstoragehc" });

}
```


>ABOUT THE AUTHOR

```comment
I am Nuno Relvão a passionate Senior .Net Developer, that already helped lead projects and teams to anchieve more. I am still learning the many paths of life and work, and will problably will continue so for a long time... :)
```
