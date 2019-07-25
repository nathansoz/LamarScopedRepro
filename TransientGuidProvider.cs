using System;

public interface ITransientGuidProvider
{
    Guid GetGuid();
}

public class TransientGuidProvider : ITransientGuidProvider
{
    private readonly IScopedGuidProvider _scopedGuidProvider;

    public TransientGuidProvider(IScopedGuidProvider scopedGuidProvider)
    {
        _scopedGuidProvider = scopedGuidProvider;
    }

    public Guid GetGuid()
    {
        return _scopedGuidProvider.GetGuid();
    }
}