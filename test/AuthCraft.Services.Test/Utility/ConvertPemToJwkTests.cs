using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace AuthCraft.Services.Tests.Utility;

public class ConvertPemToJwkTests
{
    private readonly string _rsaPrivateKey =
    @"-----BEGIN RSA PRIVATE KEY-----
            MIICWgIBAAKBgFARO5/am2L6wakrWycLB+IMoQyjRry+WGURlxzXlaqed/paE6OY
            D2zqRDYCN4zSLeKD6Mh6m0dTyrcteGOJh3chdqNLH48n0M+CxlUpgBGrBekPueyP
            hrxVnFtsejNBbY17AWLbsgKk/tNLd3kYMqvmK5lWrSrp6EiospZFS2cTAgMBAAEC
            gYBASuY24WJe4344qlDilA/7ayqzLDT0AGCYn7xY86ABxogO5t/YQJU6Xw7w02zp
            zif8OPHmcdVYPCeOWZvPSbMVhdVfqPz1R9QghG3ANRBOEAtAaVz6UQX7e+vtpKeO
            1sDit0ndg57YA55sbiB8c0PY2JQVzOmB3/Ixjluwgygh+QJBAKALM2ZJWr+NCn0A
            WRklaQrB0BWfknez1cl8NcsMhyEX8U+EczCBABVk637Utpb4j6/2s5Hvzjfo9Wwg
            9b1sHhUCQQCAEptgejuuUCc5/54OZWTu7hgMuQb7MbRXGC+NMg3fzUbw65Ft3c/4
            vOL50ZQuKs967yRfBO0nChzGTd5bFeKHAkAruOOh3ghcrDthSucMZ5v6xFgEkEOD
            UC3njXdksVd5QwE5qAWQCq3rKlbrn1ECjYUznUIRbGH6BDqH/7kitpbBAkA16J8i
            cxEgBHkefqxHHwLnV60j3dZEcd2ZM4MAiesxIzUTP/UNK+rYpplb0o3vCPIgqvzC
            Sk6Quj72cdyAtquNAkAcPJEQvNTh+JXF4pJJIEY4ikFZGCb7fTRE1cjVq49Dn9UD
            6KemXGG+fVYLUcGb3f97Kv2NIqzaCA1w4ZsHCnYz
            -----END RSA PRIVATE KEY-----";

    private readonly string _rsaPublicKey =
        @"-----BEGIN PUBLIC KEY-----
            MIGeMA0GCSqGSIb3DQEBAQUAA4GMADCBiAKBgFARO5/am2L6wakrWycLB+IMoQyj
            Rry+WGURlxzXlaqed/paE6OYD2zqRDYCN4zSLeKD6Mh6m0dTyrcteGOJh3chdqNL
            H48n0M+CxlUpgBGrBekPueyPhrxVnFtsejNBbY17AWLbsgKk/tNLd3kYMqvmK5lW
            rSrp6EiospZFS2cTAgMBAAE=
            -----END PUBLIC KEY-----";

    [Fact]
    public void ConvertRsaPrivateKeyToJwk()
    {
        var keyId = Guid.NewGuid().ToString();
        var jsonWebKey = ConvertRsaToJwk(keyId, _rsaPrivateKey);
        var content = JsonExtensions.SerializeToJson(jsonWebKey);

        Assert.Equal(keyId, jsonWebKey.KeyId);
        Assert.Equal(SecurityAlgorithms.RsaSha256, jsonWebKey.Alg);
        Assert.Equal("sig", jsonWebKey.Use);
        Assert.True(jsonWebKey.HasPrivateKey);
        Assert.NotNull(content);
    }

    [Fact]
    public void ConvertRsaPublicKeyToJwk()
    {
        var keyId = Guid.NewGuid().ToString();
        var jsonWebKey = ConvertRsaToJwk(keyId, _rsaPublicKey);
        var content = JsonExtensions.SerializeToJson(jsonWebKey);

        Assert.Equal(keyId, jsonWebKey.KeyId);
        Assert.Equal(SecurityAlgorithms.RsaSha256, jsonWebKey.Alg);
        Assert.Equal("sig", jsonWebKey.Use);
        Assert.False(jsonWebKey.HasPrivateKey);
        Assert.NotNull(content);
    }

    private JsonWebKey ConvertRsaToJwk(string keyId, string rsaKey)
    {
        using var rsa = RSA.Create();

        rsa.ImportFromPem(rsaKey);

        var rsaSecurityKey = new RsaSecurityKey(rsa)
        {
            KeyId = keyId
        };

        var jsonWebKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(rsaSecurityKey);

        jsonWebKey.Alg = SecurityAlgorithms.RsaSha256;
        jsonWebKey.Use = "sig";

        return jsonWebKey;
    }
}
