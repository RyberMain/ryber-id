# ADR-0005: Browser Session Transport

* **Status:** Accepted
* **Date:** 2026-09-21
* **Project:** RyberID
* **Organization:** RyberMain
* **Author:** Mark Will Ccallo Chambi

## Context

RyberID already implements server-side sessions backed by PostgreSQL.

A successful Passkey authentication produces an opaque session token.

The plaintext token is returned only when the session is created. PostgreSQL
stores only its SHA-256 hash.

The HTTP boundary now requires a transport mechanism for browser clients.

ADR-0003 intentionally deferred the definitive authentication scheme until
the implementation required that decision.

## Decision

Browser clients will transport the opaque RyberID session credential using
a secure HTTP cookie.

The cookie name is:

    __Host-ryberid_session

The cookie must use:

* Secure;
* HttpOnly;
* SameSite=Strict;
* Path=/;
* no Domain attribute.

The cookie is non-persistent at the browser layer.

The authoritative session expiration remains the server-side Session
record and its configured session lifetime.

Session.Id is an internal identifier and is not used as the browser
credential.

The browser authentication scheme accepts the session credential only from
the designated cookie.

It does not accept session credentials from:

* query strings;
* URL parameters;
* request bodies;
* Authorization headers.

The session token remains opaque and will not be converted into a JWT.

## Authentication

ASP.NET Core authentication will use a custom authentication handler.

For an incoming cookie:

    cookie
      -> SHA-256
      -> ResolveActiveSession
      -> SessionIdentity
      -> ClaimsPrincipal

Missing cookies result in an anonymous request.

Unknown, expired, or revoked sessions fail authentication.

## Session Issuance

A successful Passkey authentication creates a new server-side Session.

The resulting opaque token is written directly to the secure browser cookie.

The plaintext session token is not returned in the JSON response.

## Logout

Logging out revokes the current server-side Session and removes the browser
cookie.

Deleting the client cookie alone is not considered sufficient logout.

## Native Clients

This decision applies to the browser transport.

Mobile and desktop credential transport will be decided separately when
those clients are implemented.

Adding another transport must not change the Domain Session model or the
existing opaque-token storage model.

## CSRF

SameSite=Strict is part of the browser-session protection model.

Before additional cookie-authenticated state-changing browser operations
are exposed, their anti-forgery requirements must be defined explicitly.

The current protected state-changing endpoint introduced with this decision
is logout, whose operation terminates the authenticated session.

## Consequences

Browser JavaScript does not receive or manage the plaintext session token.

RyberID retains server-side control over:

* session validity;
* expiration;
* revocation;
* ownership.

The HTTP transport remains independent from the Domain and Application
session models.
