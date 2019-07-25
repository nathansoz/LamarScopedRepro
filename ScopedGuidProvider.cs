using System;

public interface IScopedGuidProvider
{
    Guid GetGuid();
}

public class ScopedGuidProvider : IScopedGuidProvider
{
    private readonly Guid _myGuid = Guid.NewGuid();

    public Guid GetGuid()
    {
        return _myGuid;
    }
}