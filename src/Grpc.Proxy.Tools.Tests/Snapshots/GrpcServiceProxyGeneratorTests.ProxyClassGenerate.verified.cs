//HintName: TestProxy.gen.cs
using Grpc.Proxy.Core;
using grpc = global::Grpc.Core;

namespace Grpc.Proxy.Tools.Tests.Proto;

public static partial class Test
{
    public abstract class TestProxy : TestBase
    {
        private readonly grpc::ChannelBase _channel;

        protected TestProxy(grpc::ChannelBase channel)
        {
            _channel = channel;
        }

    }
}
