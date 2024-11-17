var builder = DistributedApplication.CreateBuilder(args);

var seq = builder.AddSeq("seq")
                 .ExcludeFromManifest();

builder.AddProject<Projects.App>("app").WithReference(seq);

await builder.Build().RunAsync();
