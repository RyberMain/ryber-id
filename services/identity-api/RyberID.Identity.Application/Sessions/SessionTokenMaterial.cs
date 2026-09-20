using RyberID.Identity.Domain.Sessions;

namespace RyberID.Identity.Application.Sessions;

public sealed class SessionTokenMaterial
{
    private readonly byte[] _hash;

    public SessionTokenMaterial(
        string value,
        byte[] hash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        ArgumentNullException.ThrowIfNull(hash);

        if (hash.Length != Session.TokenHashSizeInBytes)
        {
            throw new ArgumentException(
                $"Session token hash must contain {Session.TokenHashSizeInBytes} bytes.",
                nameof(hash));
        }

        Value = value;
        _hash = hash.ToArray();
    }

    public string Value { get; }

    public byte[] Hash => _hash.ToArray();
}
