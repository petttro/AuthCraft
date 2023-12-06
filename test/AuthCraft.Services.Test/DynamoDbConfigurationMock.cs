using System;
using System.Collections.Generic;
using AuthCraft.Common.DomainObjects;

namespace AuthCraft.Services.Tests;

public static class DynamoDbConfigurationMock
{
    public static ClientList.Client GetConfigurationClient(string applicationName = null)
    {
        return new ClientList.Client
        {
            Application = applicationName ?? "testing",
            Key = new Guid("342ad1e6-e290-4c1c-a309-3001b408d47a"),
            ResourceClaims = new List<ResourceClaims>
            {
                new ResourceClaims { Key = "Applications", Values = new List<string> { "Create", "Read", "Update", "Delete" } },
                new ResourceClaims { Key = "Auth", Values = new List<string> { "Create" } },
                new ResourceClaims { Key = "Configurations", Values = new List<string> { "Create", "Read" } },
                new ResourceClaims { Key = "Devices", Values = new List<string> { "Create", "Read", "Update", "Delete" } },
                new ResourceClaims { Key = "ExternalConfig", Values = new List<string> { "Read" } },
                new ResourceClaims { Key = "GlUserCredentials", Values = new List<string> { "Create", "Read", "Update" } },
                new ResourceClaims { Key = "IpGeolocation", Values = new List<string> { "Read" } }
            },
            Description = "Automated and manual QA access to the entire system",
            TokenExpirationMinutes = 240
        };
    }
}
