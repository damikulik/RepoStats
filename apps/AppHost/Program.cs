using System.Net.Sockets;

var builder = DistributedApplication.CreateBuilder(args);

var seq = builder.AddSeq("seq", 5123)
    .WithAnnotation(new EndpointAnnotation(ProtocolType.Tcp, uriScheme: "http", name: "seq", port: 5124, targetPort: 5341))
    .ExcludeFromManifest();

builder.AddProject<Projects.App>("app").WithReference(seq);

await builder.Build().RunAsync();
