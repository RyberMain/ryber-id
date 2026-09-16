# ADR-0002: Backend Internal Architecture

* **Status:** Accepted
* **Date:** 2026-09-12
* **Project:** RyberID
* **Organization:** RyberMain
* **Author:** Mark Will Ccallo Chambi

## Context

RyberID requires an internal backend structure that keeps domain rules, application workflows, infrastructure concerns, and the HTTP boundary separated.

ADR-0001 established that the Identity API uses ASP.NET Core, .NET 10, Entity Framework Core, REST, OpenAPI, and PostgreSQL, while requiring the internal backend architecture to be defined separately.

ADR-0003 establishes the HTTP boundary and requires Minimal API endpoints to delegate application behavior to the Application layer rather than implementing business or persistence logic directly.

The Identity API solution is organized into four projects:

* `RyberID.Identity.Domain`;
* `RyberID.Identity.Application`;
* `RyberID.Identity.Infrastructure`;
* `RyberID.Identity.Api`.

This ADR formalizes the responsibilities and dependency direction already established by that solution.

## Decision

RyberID will use a layered backend architecture with explicit dependency boundaries between Domain, Application, Infrastructure, and API.

The dependency direction is:

```text
Api
 ├── Application
 └── Infrastructure
       ├── Application
       └── Domain

Application
 └── Domain

Domain
 └── no RyberID project dependencies
```

Dependencies must point toward the domain and application boundaries rather than allowing infrastructure or transport concerns to propagate inward.

## Domain

`RyberID.Identity.Domain` contains the identity domain model and domain rules.

The Domain project may contain:

* entities;
* value objects;
* domain invariants;
* domain behavior;
* domain-specific types.

The Domain project must not depend on:

* ASP.NET Core;
* Entity Framework Core;
* PostgreSQL;
* HTTP contracts;
* infrastructure implementations.

Persistence and transport concerns must not determine the internal domain model.

## Application

`RyberID.Identity.Application` coordinates application-level behavior.

The Application project depends on Domain.

It is responsible for:

* application use cases;
* application workflows;
* application-level requests and outcomes;
* abstractions required by application behavior.

The Application project must not depend on:

* ASP.NET Core transport concerns;
* concrete Entity Framework Core implementations;
* PostgreSQL-specific implementations;
* API request or response contracts.

Application behavior must remain usable independently from the HTTP transport layer.

No mediator or CQRS framework is selected by this decision.

A dispatching framework must not be introduced unless implementation requirements later justify it.

## Infrastructure

`RyberID.Identity.Infrastructure` contains technical implementations required by the application and domain.

It depends on Application and Domain.

Infrastructure is responsible for concerns such as:

* Entity Framework Core persistence;
* PostgreSQL integration;
* database mappings;
* database migrations;
* infrastructure dependency registration;
* concrete implementations of application abstractions when those abstractions are introduced.

Infrastructure-specific details must not be moved into Domain.

## API

`RyberID.Identity.Api` is the HTTP boundary and application composition root.

It depends on Application and Infrastructure.

The API project is responsible for:

* application startup;
* dependency composition;
* HTTP endpoints;
* transport contracts;
* middleware;
* HTTP-specific configuration;
* OpenAPI exposure;
* translation between HTTP contracts and application-level requests and outcomes.

Minimal API endpoints must remain thin.

Endpoints must not:

* contain domain business rules;
* execute Entity Framework Core queries directly;
* contain persistence logic;
* instantiate infrastructure implementations directly.

## Dependency Injection

The API project is the composition root.

Concrete infrastructure services are registered through dependency injection and composed with application behavior at startup.

Domain must not depend on dependency injection infrastructure.

## Persistence

Entity Framework Core and PostgreSQL belong to Infrastructure.

Domain entities must not require Entity Framework Core attributes or PostgreSQL-specific types.

Entity mappings are maintained separately in Infrastructure.

Database migrations are maintained as infrastructure artifacts and represent the persistence model derived from the implemented domain mappings.

## HTTP Boundary

HTTP request and response contracts belong to the API project.

Domain entities must not be returned directly as public HTTP contracts.

The API translates between HTTP representations and the Application boundary.

The public HTTP contract remains governed by ADR-0003.

## Feature Organization

Capabilities are introduced only when their corresponding behavior is implemented.

Empty structures for future authentication, Passkeys, sessions, devices, authorization, account recovery, or security auditing capabilities must not be created in advance.

## Framework Selection

This ADR does not introduce:

* a mediator framework;
* a CQRS framework;
* a repository framework;
* a validation framework;
* a mapping framework.

Such mechanisms may be introduced only when concrete implementation requirements justify them and when they preserve the dependency boundaries defined here.

## Consequences

The Domain remains independent from transport and persistence technologies.

Application behavior remains separated from ASP.NET Core and concrete persistence implementations.

Infrastructure owns EF Core and PostgreSQL concerns.

The API remains the composition root and HTTP boundary.

This structure allows RyberID capabilities to evolve without coupling domain behavior directly to HTTP, database, or framework-specific implementation details.
