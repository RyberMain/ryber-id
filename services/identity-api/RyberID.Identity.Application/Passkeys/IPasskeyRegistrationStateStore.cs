namespace RyberID.Identity.Application.Passkeys;

public interface IPasskeyRegistrationStateStore
{
    Task SaveAsync(
        Guid ceremonyId,
        string optionsJson,
        TimeSpan lifetime,
        CancellationToken cancellationToken);

    Task<string?> GetAsync(
        Guid ceremonyId,
        CancellationToken cancellationToken);

    Task RemoveAsync(
        Guid ceremonyId,
        CancellationToken cancellationToken);
}
