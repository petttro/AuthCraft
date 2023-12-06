using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthCraft.Common.DomainObjects;

public class ClientList : List<ClientList.Client>
{
    public const string SectionName = "clients";

    public Client Get(Guid key) => this.FirstOrDefault(x => x.Key == key);

    public class Client
    {
        public string Application { get; set; }

        public Guid Key { get; set; }

        public List<ResourceClaims> ResourceClaims { get; set; }

        public string Description { get; set; }

        public int TokenExpirationMinutes { get; set; }
    }
}
