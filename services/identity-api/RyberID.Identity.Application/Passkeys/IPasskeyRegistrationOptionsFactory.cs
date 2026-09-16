namespace RyberID.Identity.Application.Passkeys;

public interface IPasskeyRegistrationOptionsFactory
{
    string Create(
        byte[] userHandle,
        string userName,
        string displayName,
        IReadOnlyCollection<byte[]> existingCredentialIds);
}
