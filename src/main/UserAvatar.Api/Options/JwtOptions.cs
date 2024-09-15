using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace UserAvatar.Api.Options;

/// <summary>
///     Options for JWT token
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    ///     Token issuer (producer).
    /// </summary>
    public string Issuer { get; set; } = "Server";

    /// <summary>
    ///     Token audience (consumer).
    /// </summary>
    public string Audience { get; set; } = "Client";

    /// <summary>
    ///     Token secret part.
    /// </summary>
    public string PrivateKey { get; set; } = "somePrivateKeyValue";

    /// <summary>
    ///     Token life time.
    /// </summary>
    public TimeSpan LifeTime { get; set; } = TimeSpan.FromDays(7);

    /// <summary>
    ///     Require HTTPS.
    /// </summary>
    public bool RequireHttps { get; set; } = false;

    /// <summary>
    ///     Getting a symmetric security key
    /// </summary>
    /// <returns></returns>
    public SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(PrivateKey));
    }
}
