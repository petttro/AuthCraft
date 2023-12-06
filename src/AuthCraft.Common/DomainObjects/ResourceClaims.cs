using System.Collections.Generic;

namespace AuthCraft.Common.DomainObjects;

public class ResourceClaims
{
    public string Key { get; set; }

    public List<string> Values { get; set; }
}
