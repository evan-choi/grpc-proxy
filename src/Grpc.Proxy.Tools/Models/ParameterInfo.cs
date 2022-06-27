namespace Grpc.Proxy.Tools.Models;

internal readonly struct ParameterInfo
{
    public string Type { get; }

    public string Name { get; }

    public ParameterInfo(string type, string name)
    {
        Type = type;
        Name = name;
    }

    public override string ToString()
    {
        return $"{Type} {Name}";
    }
}
