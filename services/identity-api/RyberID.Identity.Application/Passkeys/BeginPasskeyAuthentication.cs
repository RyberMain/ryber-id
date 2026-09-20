namespace RyberID.Identity.Application.Passkeys;

public sealed class BeginPasskeyAuthentication(
    IPasskeyAuthenticationOptionsFactory optionsFactory,
    IPasskeyAuthenticationStateStore stateStore)
{
    public async Task<PasskeyAuthenticationStart> ExecuteAsync(
        CancellationToken cancellationToken = default)
    {
        var options = optionsFactory.Create();

        var ceremonyId = Guid.CreateVersion7();

        var lifetime =
            TimeSpan.FromMilliseconds(
                (double)options.TimeoutMilliseconds);

        await stateStore.SaveAsync(
            ceremonyId,
            options.Json,
            lifetime,
            cancellationToken);

        return new PasskeyAuthenticationStart(
            ceremonyId,
            options.Json);
    }
}
