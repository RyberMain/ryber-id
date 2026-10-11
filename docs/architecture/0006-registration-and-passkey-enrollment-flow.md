# ADR-0006: Registration and Passkey Enrollment Flow

* **Status:** Accepted
* **Date:** 2026-10-11
* **Project:** RyberID
* **Organization:** RyberMain
* **Author:** Mark Will Ccallo Chambi

## Context

RyberID currently has a minimal `User` domain entity whose only attribute is
an application-generated UUIDv7.

The Application layer can create and persist that identity through
`CreateUser`. It can also begin and complete a Passkey registration ceremony
for a supplied user through `BeginPasskeyRegistration` and
`CompletePasskeyRegistration`.

The implemented persistence model keeps `User`, `PasskeyUserHandle`,
`PasskeyCredential`, and `Session` as separate records. A
`PasskeyUserHandle` and every `PasskeyCredential` reference an existing
`User`. The current authentication flow can verify a registered Passkey and
then create a server-side session.

The API exposes Passkey authentication and session operations, but it does not
expose user creation or Passkey registration. ADR-0004 intentionally left the
public registration flow and its credential-enrollment relationship
undefined.

The registration boundary must now be defined without introducing an email
address, phone number, persistent username, password, or any other human
identifier that the current domain does not contain.

## Decision

In RyberID, **registration** is the complete Application workflow that:

1. creates and persists a new `User` as the stable internal identity; and
2. verifies and persists that user's first `PasskeyCredential`.

`CreateUser` alone is not completed registration and will not be exposed as a
standalone public HTTP operation.

The `User` is created when initial Passkey enrollment begins. This ordering is
required by the current persistence model because the stable
`PasskeyUserHandle` used to create WebAuthn options references an existing
`User`.

A persisted `User` with no `PasskeyCredential` is an identity in progress of
enrollment. It is not a usable registered account and cannot authenticate
through the official public flow. Registration finishes only after the first
Passkey attestation is verified and its `PasskeyCredential` is durably
persisted for that `User`.

This lifecycle is derived from the presence of the first credential. This
decision does not add a registration-status property to `User`.

Passkey registration does not create a session. After successful initial
enrollment, the user can run the existing Passkey authentication flow, which
is the implemented path that establishes an authenticated identity and
creates a session.

## Registration lifecycle

The minimum lifecycle is:

```text
No identity
    -> User persisted and initial ceremony started
    -> Enrollment in progress
    -> First PasskeyCredential verified and persisted
    -> Registered account able to authenticate with that Passkey
```

The transition to enrollment in progress occurs before the browser or client
performs the WebAuthn ceremony. The transition to registered occurs only at
credential persistence; returning options or receiving an attestation is not
sufficient.

The `PasskeyUserHandle` is the opaque WebAuthn user handle and remains separate
from `User.Id`. The `userName` and `displayName` values required by the current
Passkey options factory are Application-supplied protocol metadata. Until a
human-facing identifier is separately designed, both values will use the
canonical lower-case `N` representation of `User.Id`. They are not accepted
from the anonymous caller, persisted as `User` attributes, or treated as login
identifiers.

## Initial Passkey enrollment

Initial enrollment is one Application workflow split across the two WebAuthn
ceremony requests.

Beginning initial enrollment must:

1. create and persist a new `User` through the existing user-creation
   capability;
2. create and persist the user's stable `PasskeyUserHandle`;
3. create Passkey registration options for that user;
4. store short-lived, one-time server-side ceremony state that binds the
   ceremony identifier, original options, `User.Id`, and the initial-enrollment
   purpose; and
5. return only the ceremony identifier and public WebAuthn options.

Completing initial enrollment must:

1. consume the initial-enrollment ceremony state;
2. obtain `User.Id` from that server-side state, not from the caller;
3. verify the attestation against the stored options;
4. verify that the attested user handle matches the stable handle belonging to
   that `User`;
5. enforce credential-identifier uniqueness; and
6. persist the first `PasskeyCredential` for the bound `User`.

The anonymous client does not choose or submit a user identifier, login
identifier, user name, or display name in either request.

The current low-level registration use cases remain reusable implementation
building blocks. Their acceptance of `userId`, `userName`, and `displayName`
does not make those values part of the public initial-registration contract.

## Additional Passkey enrollment

Additional enrollment is credential management for an already registered and
authenticated user. It is not account registration.

Both the begin and complete requests require an active authenticated session.
The Application workflow must obtain `User.Id` from that session identity. It
must not accept a target user identifier from the HTTP request.

The workflow reuses the user's existing `PasskeyUserHandle`, includes the
user's existing credential identifiers in WebAuthn exclusion data, and binds
the ceremony state to that authenticated user and the additional-enrollment
purpose. Completion must reject a ceremony that does not belong to the current
authenticated user or was created for another enrollment purpose.

Additional enrollment creates only another `PasskeyCredential`. It does not
create another `User`, replace the stable user handle, or create a new
session.

## Failure/interruption behavior

The browser ceremony is not a transaction spanning the client and database.
If the `User` is persisted but the initial Passkey is never successfully
persisted, the `User` remains an incomplete enrollment and is not a usable
registered account.

Registration ceremony state remains short-lived and single-use. Expired,
missing, already-consumed, invalid, or failed ceremony state cannot complete
registration. With the current consume-on-completion behavior, a failed
completion attempt requires a new ceremony.

The minimum flow does not expose a way for an anonymous caller to resume an
incomplete identity. Starting initial enrollment again creates a new `User`.
Automatic deletion, retention, or reclamation of incomplete users and their
handles is deliberately deferred; incomplete records must not be interpreted
as successfully registered accounts in the meantime.

If the credential was durably persisted but the HTTP response was lost,
registration is complete according to server state. The new Passkey can be
used through the existing authentication flow.

## Application boundary

Application owns the end-to-end workflows and their transport-independent
rules.

Before the initial registration routes are exposed, Application must provide
an orchestration boundary that composes user creation with the beginning of
the first Passkey ceremony, and a completion boundary that resolves the bound
user from server-side ceremony state. An API endpoint must not coordinate
these persistence and ceremony steps itself.

Application also owns the distinction between initial and additional
enrollment, the ceremony's user and purpose binding, the first-credential
completion condition, and the requirement that additional enrollment targets
the authenticated user.

The existing `IPasskeyRegistrationStateStore`, which currently stores only
the serialized WebAuthn options, is not sufficient as the anonymous initial
registration boundary by itself. Its Application contract must represent the
bound user and enrollment purpose before HTTP registration is implemented.

Domain continues to own `User`, `PasskeyUserHandle`, `PasskeyCredential`, and
their invariants. No HTTP, session-cookie, FIDO2-library, or persistence
concerns move into Domain.

Infrastructure continues to implement PostgreSQL persistence, the stable
user/handle/credential relationships, FIDO2 option creation and verification,
and the short-lived ceremony-state store required by Application.

## HTTP boundary

The later HTTP implementation will expose four explicit ceremony endpoints
under the ADR-0003 `/api/v1` boundary:

| Endpoint | Access | Responsibility |
| --- | --- | --- |
| `POST /api/v1/passkeys/registration/initial/begin` | Anonymous | Create the internal user and begin first-Passkey enrollment. |
| `POST /api/v1/passkeys/registration/initial/complete` | Anonymous | Complete the bound first-Passkey ceremony. |
| `POST /api/v1/passkeys/registration/additional/begin` | Authenticated session | Begin enrollment for another Passkey owned by the current user. |
| `POST /api/v1/passkeys/registration/additional/complete` | Authenticated session | Complete the current user's additional-Passkey ceremony. |

The initial begin request has no identity fields. The initial complete request
contains only the ceremony identifier and attestation response already
represented by the existing registration HTTP contract. Neither initial
endpoint accepts `User.Id`.

The additional begin request has no target-user fields. The additional
complete request contains the ceremony identifier and attestation response;
the target `User.Id` comes exclusively from the authenticated session.

All four handlers must remain thin: they perform HTTP validation, obtain any
authenticated session context, invoke their Application workflow, and map the
outcome to HTTP. They must not invoke EF Core or coordinate user creation and
credential persistence directly.

The two initial endpoints must be explicitly designated anonymous. The two
additional endpoints must explicitly require authorization. No standalone
public `/users` creation endpoint is introduced.

## Security considerations

The initial ceremony identifier is a correlation value, not a caller-selected
user identity. Server-side state must be integrity-protected, short-lived,
single-use, and bound to the generated user and initial-enrollment purpose.
The client must not be trusted to provide or change that binding.

The existing WebAuthn verification requirements remain authoritative:
verification uses the original options, the configured relying-party and
origin rules, the returned user handle must match the stored handle, and a
credential identifier must be globally unique before persistence.

Additional enrollment is a security-sensitive, cookie-authenticated state
change. In accordance with ADR-0005, it must not be exposed without an
explicit anti-forgery control in addition to requiring an active session.
The concrete anti-forgery mechanism is deferred because none is selected in
the current codebase.

Anonymous initial enrollment also requires deployment anti-abuse controls.
Concrete rate limits, quotas, and challenge mechanisms are deferred; anonymous
classification must not be interpreted as a decision to leave the operation
unrestricted in production.

No session is issued from an incomplete identity, and successful initial
enrollment does not itself grant an authenticated session.

## Explicitly deferred decisions

This decision does not introduce or select:

* email, phone number, or a persistent human username;
* passwords or password fallback behavior;
* account recovery, OTP, or QR-assisted flows;
* roles or permissions;
* user-facing Passkey names or credential-management operations;
* resumption or recovery of an interrupted anonymous enrollment;
* retention or cleanup policy for incomplete users and user handles;
* concrete public-enrollment rate limits or anti-automation mechanisms;
* the anti-forgery mechanism for authenticated credential enrollment;
* recent-authentication or step-up requirements for adding a Passkey;
* mobile or desktop session transport.

These decisions require separate design and must not be inferred from the
registration endpoints defined here.

## Consequences

RyberID gains one explicit definition of registration: a stable internal
identity plus its first persisted Passkey credential.

The first Passkey and every later Passkey remain credentials separate from the
base `User` identity. No new `User` attributes or persistence schema are
required by this ADR.

The later HTTP implementation has an explicit public boundary for first
enrollment and an authenticated boundary for additional enrollment. It must
add Application orchestration and richer server-side registration state before
mapping the routes, but it does not need to choose a human account identifier
or redefine when registration is complete.

Interrupted initial enrollment can leave persistent users and handles without
credentials. This is accepted for the minimum flow; those records confer no
ability to authenticate, while their cleanup policy remains deferred.

## Relationship to ADR-0004

ADR-0004 remains authoritative for the minimal `User` identity, the separation
of identity from authentication credentials, and the prohibition against
exposing `CreateUser` merely because the use case exists.

This ADR complements ADR-0004 and supersedes only the parts that left the
registration workflow, first-credential relationship, public classification,
and future HTTP registration routes undecided. `CreateUser` becomes an internal
step of the public initial-enrollment workflow; it does not become a standalone
public capability.
