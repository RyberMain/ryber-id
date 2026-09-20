using RyberID.Identity.Application.Passkeys;
using RyberID.Identity.Application.Sessions;

namespace RyberID.Identity.Application.Authentication;

public sealed class SignInWithPasskey(
    CompletePasskeyAuthentication completePasskeyAuthentication,
    CreateSession createSession)
{
    public async Task<SignInResult> ExecuteAsync(
        Guid ceremonyId,
        string assertionResponseJson,
        CancellationToken cancellationToken = default)
    {
        var identity =
            await completePasskeyAuthentication.ExecuteAsync(
                ceremonyId,
                assertionResponseJson,
                cancellationToken);

        var createdSession =
            await createSession.ExecuteAsync(
                identity,
                cancellationToken);

        return new SignInResult(
            identity.UserId,
            createdSession.Session,
            createdSession.Token);
    }
}
