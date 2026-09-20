namespace RyberID.Identity.Application.Passkeys;

public interface IPasskeyRegistrationOptionsFactory
{
    PasskeyRegistrationOptions Create(
        byte[] userHandle,
        string userName,
        string displayName,
        IReadOnlyCollection<byte[]> existingCredentialIds);
}
