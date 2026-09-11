# ADR-0001: Architecture Baseline

* **Status:** Accepted
* **Date:** 2026-09-10
* **Project:** RyberID
* **Organization:** RyberMain
* **Author:** Mark Will Ccallo Chambi

## Context

RyberID is a multiplatform identity and authentication platform intended to provide a centralized identity service for web, desktop, and mobile applications.

The project follows a passwordless-first approach, with modern authentication mechanisms such as Passkeys and WebAuthn forming the primary authentication model.

RyberID is maintained as a generic open-source project and must remain independent from institution-specific data, infrastructure, credentials, or business rules.

## Decision

RyberID will be developed as a monorepo containing the central identity service and the client applications that consume it.

The initial architecture is composed of:

* a centralized Identity API;
* a web client;
* a future mobile client;
* a future desktop client;
* shared packages where justified;
* infrastructure definitions;
* automated tests;
* architecture, authentication, and security documentation.

## Backend

The backend will use:

* ASP.NET Core;
* .NET 10 LTS;
* C#;
* Entity Framework Core 10;
* REST APIs;
* OpenAPI documentation.

The backend will act as the central authority for identities, authentication, sessions, devices, authorization, and security auditing.

The specific internal backend architecture will be defined in a separate ADR before the Identity API implementation begins.

## Database

PostgreSQL 18 will be used as the relational database platform.

Database access from the .NET backend will be performed through Entity Framework Core using the PostgreSQL provider selected during backend bootstrap.

Database schema design, migrations, identity entities, constraints, indexes, and auditing structures will be defined separately.

## Web Client

The web client will use:

* React 19.3;
* TypeScript.

The frontend build tooling and supporting libraries will be selected before the web application is generated.

The web client will consume the centralized Identity API and will not implement an independent identity store.

## Mobile Client

RyberID will include a mobile client.

The mobile framework has not yet been selected.

This decision will be made separately after evaluating the authentication, biometric, Passkey, platform integration, maintenance, and code-sharing requirements.

## Desktop Client

RyberID will include a desktop client.

The desktop framework has not yet been selected.

This decision will be made separately after evaluating Windows Hello, Passkey support, operating-system integration, security requirements, and code-sharing requirements.

## Authentication Model

RyberID follows the principle:

**Passwordless first, password fallback.**

The planned authentication architecture includes:

* Passkeys;
* WebAuthn;
* device-native biometric authorization where supported;
* cross-device authentication;
* QR-assisted authentication;
* session management;
* registered-device management;
* account recovery;
* password authentication as a fallback mechanism.

Authentication mechanisms will use established standards and platform APIs.

RyberID will not implement custom cryptographic algorithms.

## Authorization

Authentication and authorization will remain separate concerns.

The platform will provide centralized role and permission management.

The definitive authorization model will be documented before implementation.

## Sessions and Devices

Sessions and trusted or registered devices will be managed centrally by the Identity API.

The detailed session, token, revocation, refresh, and device-trust models will be defined in dedicated architecture and security decisions.

## Security

Security is a primary architectural requirement.

The project will:

* use established security standards;
* avoid custom cryptographic primitives;
* maintain authentication and security audit events;
* prevent secrets and credentials from being committed to the repository;
* separate public project configuration from production configuration;
* avoid real third-party personal or institutional data.

Detailed threat modeling and security controls will be documented separately.

## Infrastructure

Docker will be used to provide reproducible development and deployment environments where appropriate.

Infrastructure configuration will remain separate from application code under the `infrastructure/` directory.

Production deployment architecture has not yet been selected.

## Repository Structure

RyberID uses a monorepo with the following top-level organization:

```text
apps/
  web/
  mobile/
  desktop/

services/
  identity-api/

packages/

infrastructure/
  docker/
  database/

docs/
  architecture/
  authentication/
  security/

tests/

.github/
```

## Architectural Documentation

Significant architectural decisions will be recorded as Architecture Decision Records under:

```text
docs/architecture/
```

Each ADR will describe its context, decision, consequences, and status.

Accepted ADRs represent the architectural baseline of the project until they are superseded by a later ADR.

## Consequences

This decision establishes a single identity authority shared by all RyberID clients.

Web, mobile, and desktop applications will not maintain independent authentication systems.

Technologies that have not yet been formally selected remain intentionally undecided and must be addressed through subsequent ADRs before implementation.
