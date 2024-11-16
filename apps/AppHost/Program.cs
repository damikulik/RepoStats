using System.Net.Sockets;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.App>("app");

builder
    .AddResource(new ContainerResource("seq"))
    .WithAnnotation(new EndpointAnnotation(ProtocolType.Tcp, uriScheme: "http", name: "seq", port: 5341, targetPort: 80))
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithAnnotation(new ContainerImageAnnotation { Image = "datalust/seq", Tag = "latest" });

await builder.Build().RunAsync();
