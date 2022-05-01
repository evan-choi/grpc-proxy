﻿using System.IO;

namespace Grpc.Proxy.Tools.Tests;

internal static class ResourceManager
{
    public static string GetString(string resource)
    {
        resource = $"Grpc.Proxy.Tools.Tests.Resources.{resource}";
        using var stream = typeof(ResourceManager).Assembly.GetManifestResourceStream(resource);
        using var streamReader = new StreamReader(stream!);
        return streamReader.ReadToEnd();
    }
}
