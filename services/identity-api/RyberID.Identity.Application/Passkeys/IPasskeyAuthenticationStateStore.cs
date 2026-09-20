namespace RyberID.Identity.Application.Passkeys;

public interface IPasskeyAuthenticationStateStore
{
    Task SaveAsync(
        Guid ceremonyId,
        string optionsJson,
        TimeSpan lifetime,
        CancellationToken cancellationToken);

    Task<string?> ConsumeAsync(
        Guid ceremonyId,
        CancellationToken cancellationToken);
}
