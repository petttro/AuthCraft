using System;
using System.ComponentModel.DataAnnotations;

namespace AuthCraft.Api.Dto;

public class ClientAuthRequest
{
    [Required]
    public Guid? Key { get; set; }

    public string DeviceId { get; set; }

    public string PlatformType { get; set; }

    public string PlatformVersion { get; set; }

    public string AppVersion { get; set; }
}
