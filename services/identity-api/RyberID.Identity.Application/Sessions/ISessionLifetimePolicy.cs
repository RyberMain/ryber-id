namespace RyberID.Identity.Application.Sessions;

public interface ISessionLifetimePolicy
{
    TimeSpan GetLifetime();
}
