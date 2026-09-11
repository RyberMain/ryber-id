# ADR-0003: HTTP API Contract

* **Status:** Accepted
* **Date:** 2026-09-11
* **Project:** RyberID
* **Organization:** RyberMain
* **Author:** Mark Will Ccallo Chambi

## Context

RyberID requires a stable HTTP boundary between the central Identity API and its web, mobile, desktop, and future external clients.

The API will expose security-sensitive capabilities including authentication, identity management, sessions, devices, authorization, account recovery, and security auditing.

The HTTP layer must remain explicit, consistent, versionable, documented, and independent from the internal domain model.

## Decision

The RyberID Identity API will use ASP.NET Core Minimal APIs as its HTTP endpoint model.

Controllers will not be used for the initial Identity API implementation.

Endpoints will remain thin transport boundaries and must delegate application behavior to the Application layer.

Business rules must not be implemented directly inside endpoint handlers.

## Endpoint Organization

Endpoints will be organized by capability.

The API project will use the following structure:

```text
RyberID.Identity.Api/
├── Endpoints/
├── Contracts/
├── Middleware/
├── Configuration/
└── Program.cs
```

Feature-specific endpoint definitions will be introduced only when the corresponding capability is implemented.

The initial capability boundaries established by ADR-0002 remain:

* Identity;
* Authentication;
* Passkeys;
* Sessions;
* Devices;
* Authorization;
* Account Recovery;
* Security Auditing.

Empty endpoint structures will not be created in advance.

## Route Groups

Related endpoints will be grouped using ASP.NET Core route groups.

Route groups will provide common route prefixes and shared endpoint metadata where appropriate.

Authentication and authorization requirements may be applied at route-group level when the entire capability shares the same security policy.

## API Base Path

The public HTTP API will use the following base path:

```text
/api/v1
```

The version segment represents the major public API contract version.

Feature routes will exist beneath this boundary.

## Versioning Strategy

RyberID will use URL-segment major versioning for its public HTTP API.

The initial public contract is:

```text
v1
```

Breaking HTTP contract changes require a new major API version.

Backward-compatible changes do not require a new major version.

A dedicated API-versioning library will not be introduced until the project requires simultaneous support for multiple API versions.

## HTTP Contracts

HTTP request and response models belong to the API boundary.

They must not expose Domain entities directly.

Transport contracts will be defined separately from:

* Domain entities;
* persistence models;
* infrastructure implementations.

The API layer is responsible for translating HTTP input into application-level requests and application outcomes into HTTP responses.

## JSON

JSON is the primary representation format for the HTTP API.

Public JSON contracts must use stable property names.

Changes to public contract names must be treated as API-contract changes.

Internal C# type names may evolve without affecting clients when the public JSON contract remains unchanged.

## HTTP Semantics

Endpoints must use HTTP methods according to their intended semantics.

The API will use standard HTTP status codes rather than encoding operation status only inside custom response bodies.

Successful and unsuccessful responses must remain consistent across capabilities.

Authentication and authorization failures must remain distinguishable at the HTTP boundary.

## Error Contract

RyberID will use ASP.NET Core `ProblemDetails` as the standard HTTP error representation.

The API will register the ASP.NET Core Problem Details service and use centralized exception handling.

Unhandled application exceptions must not expose stack traces, credentials, internal implementation details, database information, cryptographic material, or other sensitive information to clients.

Expected application failures must be translated into appropriate HTTP responses rather than exposed as unhandled exceptions.

## Validation Errors

Invalid HTTP input must produce a structured error response consistent with the API error contract.

Validation belongs at the appropriate boundary:

* transport validation for malformed or structurally invalid HTTP input;
* application validation for use-case requirements;
* domain validation for domain invariants.

The concrete validation mechanism will be selected separately before validation-dependent features are implemented.

## Authentication and Authorization

Authentication and authorization middleware will be configured centrally in the API application.

Endpoint authorization requirements must be declared explicitly.

Public endpoints must be intentionally designated as public.

The absence of an authorization requirement must not be used implicitly to classify a security-sensitive endpoint as public.

The definitive authentication schemes, credential validation mechanisms, and token model will be defined in dedicated security decisions.

## OpenAPI

The Identity API will expose an OpenAPI document using ASP.NET Core's supported OpenAPI integration.

OpenAPI metadata must describe the actual public HTTP contract.

OpenAPI generation must derive from the implemented API rather than from a separately maintained manual specification.

Interactive API documentation tooling is not selected by this ADR.

## Endpoint Responsibilities

An endpoint may:

* receive HTTP input;
* obtain authenticated-request context;
* invoke an Application-layer use case;
* translate the application result into an HTTP response;
* declare endpoint metadata.

An endpoint must not:

* implement domain business rules;
* execute database queries directly;
* access Entity Framework Core directly;
* instantiate infrastructure services directly;
* implement cryptographic algorithms;
* contain persistence logic.

## Application Boundary

Minimal API handlers interact with the Application layer through application services or use-case abstractions defined by that layer.

The HTTP layer must not become the application workflow layer.

The specific use-case dispatching mechanism will be selected only when implementation requirements justify it.

No mediator or CQRS framework is selected by this ADR.

## Dependency Direction

The HTTP boundary follows the dependency rules established by ADR-0002.

```text
HTTP Request
     ↓
Minimal API Endpoint
     ↓
Application
     ↓
Domain

Infrastructure
     ↑
Composition through dependency injection
```

The API project remains the composition root.

## Cancellation

Asynchronous operations initiated from HTTP requests must propagate request cancellation where supported.

Long-running application or infrastructure operations must not intentionally ignore request cancellation without a documented reason.

## Security

The API must not return:

* secrets;
* passwords;
* password hashes;
* private cryptographic keys;
* refresh-token secrets;
* internal security material;
* sensitive exception details.

Security-sensitive response contracts must expose only the information required by the client.

## Excluded Decisions

This ADR does not select:

* authentication token format;
* token lifetimes;
* refresh-token design;
* WebAuthn library;
* validation library;
* mediator library;
* mapping library;
* OpenAPI user-interface tooling;
* rate-limiting policies;
* CORS policy;
* logging implementation;
* observability implementation.

Those decisions will be made when their implementation becomes necessary.

## Consequences

The Identity API gains a lightweight HTTP layer based on the current ASP.NET Core API model.

Public contracts remain separated from Domain and persistence models.

The `/api/v1` boundary establishes an explicit major-version contract for all clients.

Centralized Problem Details handling provides a consistent foundation for API errors.

Feature-oriented route groups allow the HTTP surface to grow without requiring controllers or large centralized endpoint files.

## Supersession

This ADR remains authoritative for the RyberID HTTP API contract until explicitly superseded by a later ADR.
