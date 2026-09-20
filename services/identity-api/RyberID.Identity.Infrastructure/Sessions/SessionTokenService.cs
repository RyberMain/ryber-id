using System.Security.Cryptography;
using System.Text;
using RyberID.Identity.Application.Sessions;

namespace RyberID.Identity.Infrastructure.Sessions;

internal sealed class SessionTokenService
    : ISessionTokenService
{
    private const int TokenSizeBytes = 32;

    public SessionTokenMaterial Issue()
    {
        var randomBytes =
            RandomNumberGenerator.GetBytes(
                TokenSizeBytes);

        var value =
            EncodeBase64Url(
                randomBytes);

        var hash =
            ComputeHash(
                value);

        return new SessionTokenMaterial(
            value,
            hash);
    }

    public byte[] Hash(
        string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            token);

        return ComputeHash(
            token);
    }

    private static byte[] ComputeHash(
        string token)
    {
        var bytes =
            Encoding.UTF8.GetBytes(
                token);

        return SHA256.HashData(
            bytes);
    }

    private static string EncodeBase64Url(
        byte[] value)
    {
        return Convert
            .ToBase64String(value)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
