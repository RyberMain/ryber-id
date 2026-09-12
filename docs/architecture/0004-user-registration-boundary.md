# ADR-0004: User Registration Boundary

* **Status:** Accepted
* **Date:** 2026-09-12
* **Project:** RyberID
* **Organization:** RyberMain
* **Author:** Mark Will Ccallo Chambi

## Context

RyberID currently defines a base `User` domain entity identified by an application-generated UUIDv7.

The Application layer contains a `CreateUser` use case capable of creating and persisting this base entity.

The project has not yet defined:

* a public registration flow;
* a login identifier such as email, username, or phone number;
* credential enrollment requirements;
* Passkey registration requirements;
* a public HTTP route for account creation;
* authorization or anti-abuse requirements for account creation.

Introducing any of those elements before they are explicitly designed would make them implicit architectural decisions.

## Decision

The `User` entity represents the stable internal identity of an account.

Its current identifier remains:

```text
User.Id
```

No additional user attributes are introduced by this decision.

Authentication credentials and authentication mechanisms must remain separate from the base `User` identity.

Passkeys, password fallback credentials, recovery mechanisms, sessions, devices, and other authentication-related state must not be modeled as properties added directly to `User` without a dedicated design decision.

## User Creation

`CreateUser` remains an Application-layer capability.

It must not be exposed as a public HTTP endpoint until the registration flow defines:

* the required registration input;
* the authentication or credential-enrollment relationship;
* whether registration is public or restricted;
* the HTTP contract;
* the security requirements associated with account creation.

The existence of the Application use case does not imply that anonymous public account creation is allowed.

## Public API

No public user-registration route is established by this ADR.

A route must not be introduced merely because the `CreateUser` use case exists.

The HTTP registration contract will be defined together with the first concrete authentication or registration flow.

## Identity and Authentication Separation

The base user identity and authentication credentials are separate concerns.

The existence of a `User` must not require the Domain entity itself to contain:

* passwords;
* password hashes;
* Passkey credential material;
* session tokens;
* refresh tokens;
* device credentials.

Those concerns require their own domain and security models.

## Consequences

RyberID retains a minimal and stable base identity model.

The project avoids prematurely selecting email, username, phone number, or another identifier as the account-registration mechanism.

The Application layer may continue evolving independently from the HTTP registration contract.

The first public account-creation endpoint will only be implemented after its registration and authentication requirements are explicitly defined.
