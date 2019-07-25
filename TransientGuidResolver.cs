using System;

public interface ITransientGuidResolver
{
    Guid GetGuid();
}

public class TransientGuidResolver : ITransientGuidResolver
{
    private readonly ITransientGuidProvider _guidProvider;

    public TransientGuidResolver(ITransientGuidProvider guidProvider)
    {
        _guidProvider = guidProvider;
    }

    public Guid GetGuid()
    {
        return _guidProvider.GetGuid();
    }
}