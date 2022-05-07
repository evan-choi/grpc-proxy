[![NuGet](https://img.shields.io/nuget/v/Evan.Grpc.Proxy.Tools)](https://www.nuget.org/packages/Evan.Grpc.Proxy.Tools)

# gRPC Proxy
Client based proxy code generator using [source generator](https://docs.microsoft.com/dotnet/csharp/roslyn-sdk/source-generators-overview)

for build man-in-the-middle attack gRPC server.

### Getting started
```
Install-Package Evan.Grpc.Proxy
Install-Package Evan.Grpc.Proxy.Tools
```

```csharp
var targetChannel = new Channel("127.0.0.1", 12345, ChannelCredentials.Insecure);
var targetInvoker = new ProxyCallInvoker(targetChannel.CreateCallInvoker());

var proxyService1 = new Example1.Example1Proxy(targetInvoker); // auto-generated proxy client
var proxyService2 = new Example2.Example2Proxy(targetInvoker); // auto-generated proxy client

var proxyServer = new Server();
server.Services.Add(Example1.BindService(proxyService1));
server.Services.Add(Example2.BindService(proxyService2));
```

### Advanced usage
Extends auto-generated proxy client and intercept request/response
```csharp
public sealed class ExampleImpl : Example.ExampleProxy
{
    public TestProxy(IProxyCallInvoker callInvoker) : base(callInvoker)
    {
    }
    
    public override async Task<HelloResponse> Hello(HelloRequest request, ServerCallContext context)
    {
        request.Message = "I'm proxy!";

        var response = await base.Hello(request, context);
        
        response.Message += ", Decorated by proxy!";

        return response;
    }
}
```
