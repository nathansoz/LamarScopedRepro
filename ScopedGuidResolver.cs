using System;

public interface IScopedGuidResolver
{
    Guid GetGuid();
}

public class ScopedGuidResolver : IScopedGuidResolver
{
    private readonly ITransientGuidProvider _guidProvider;

    public ScopedGuidResolver(ITransientGuidProvider guidProvider)
    {
        _guidProvider = guidProvider;
    }

    public Guid GetGuid()
    {
        return _guidProvider.GetGuid();
    }
}