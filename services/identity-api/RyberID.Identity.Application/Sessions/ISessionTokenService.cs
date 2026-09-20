namespace RyberID.Identity.Application.Sessions;

public interface ISessionTokenService
{
    SessionTokenMaterial Issue();

    byte[] Hash(
        string token);
}
