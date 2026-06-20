# WalrusWallet — Plan Técnico de Implementación

## Datos generales

| Campo | Valor |
|---|---|
| Duración estimada | 40 semanas |
| Ritmo | 12 horas / semana |
| Esfuerzo total estimado | ≈ 480 horas |

> **Nota de coherencia:** Fase 1 (semanas 1-4) y el detalle de la nota en Semana 4 ya reflejan las decisiones de `ADR-0001-Monolito-Modular-Fase1.md` (SPA única sin Module Federation, backend como un solo deployable). Fase 2 en este documento todavía describe "Identity Service" y "User Service" como servicios separados — este punto queda señalado como **pendiente de revisión** en el propio ADR-0001 y debe decidirse explícitamente al llegar a esa fase.

---

## FASE 1 — Foundation

**Duración:** 4 semanas

### Semana 1 — Repositorio profesional

**Objetivo:** crear el repositorio profesional. Al finalizar la semana todavía no habrá código de negocio, pero sí una "empresa vacía".

**Aprenderás**
- GitFlow
- Monorepo
- Convenciones
- ADR
- Documentación
- Arquitectura C4

**Tareas**

| Bloque | Contenido |
|---|---|
| Crear repositorio | `walruswallet` |
| Crear estructura | `backend/`, `frontend/`, `infra/`, `docs/`, `.github/`, `scripts/`, `ADR/` |
| Crear documentación | README, Vision, Roadmap, Architecture, Contributing, Glossary |
| Crear archivos base | `LICENSE`, `.gitignore`, `.editorconfig`, `.env.example` |
| Configurar | Conventional Commits, GitHub Projects, Milestones, Issues, Labels |

**Tiempo estimado:** 12 horas

**Definition of Done:** el proyecto parece un repositorio profesional aunque todavía no tenga funcionalidades.

---

### Semana 2 — Backend Foundation

**Objetivo:** construir el Backend Foundation.

**Aprenderás**
- Clean Architecture
- Dependency Injection
- Minimal APIs
- Vertical Slice
- Fluent Validation

**Tareas**

| Bloque | Contenido |
|---|---|
| Crear solución | `WalrusWallet.sln` |
| Crear proyectos | Domain, Application, Infrastructure, Api, SharedKernel |
| Configurar | DI, Logging, Configuration, Options Pattern, Global Exception Middleware |
| Crear endpoints | `/health`, `/version` |
| Agregar | Swagger, Serilog, Health Checks |

**Tiempo estimado:** 12 horas

**Definition of Done:** existe un backend completamente funcional aunque no haga negocio.

---

### Semana 3 — Infraestructura Docker

**Objetivo:** infraestructura Docker.

**Aprenderás**
- Docker
- Docker Compose
- Networking
- Volumes
- Images

**Tareas**

| Bloque | Contenido |
|---|---|
| Crear Dockerfile | Backend, Frontend |
| Crear compose y levantar | PostgreSQL, MongoDB, Redis, RabbitMQ, Keycloak, NGINX |
| Configurar | Variables, Networks, Volumes |

**Resultado esperado:** `docker compose up` levanta toda la plataforma.

---

### Semana 4 — Frontend Foundation (SPA única)

**Objetivo:** Frontend Foundation como SPA única.

**Aprenderás**
- Arquitectura de SPA
- React Architecture
- Routing
- Integración con IdP externo (Keycloak)

**Tareas**
- Crear la aplicación React (un solo proyecto, **sin** Module Federation)
- Crear Layout
- Crear Routing
- Implementar feature de autenticación (login)
- Integrar Keycloak
- Manejar sesión (token, refresh)

**Resultado:** login funcionando sobre una SPA única.

> **Nota:** Module Federation se introduce más adelante, al extraer el primer microservicio real, para que el corte del frontend refleje un dominio ya validado en el backend (ver `ADR-0001`).

---

## FASE 2 — Identity

**Semanas:** 5-7

**Aprenderás**
- OAuth2
- OIDC
- JWT
- Refresh Tokens
- Claims
- Roles
- Permissions

**Se construirá**
- Identity Service
- User Service
- Audit inicial

---

## FASE 3 — Companies

**Semanas:** 8-10

**Aprenderás**
- DDD
- Repositories
- Specifications
- Multi-tenancy

**Servicios**
- Company
- Workspace
- Membership

---

## FASE 4 — Wallet

**Semanas:** 11-14

**Aprenderás**
- Transacciones
- ACID
- Optimistic Concurrency
- Row Versioning

**Servicios**
- Wallet
- Account
- Balance

---

## FASE 5 — Payments

**Semanas:** 15-18

**Aprenderás**
- CQRS
- RabbitMQ
- Outbox Pattern
- Inbox Pattern
- Idempotency

**Servicios**
- Payment
- Transaction
- Transfer

---

## FASE 6 — Notifications

**Semanas:** 19-21

**Aprenderás**
- MongoDB
- SignalR
- Consumers
- Retry
- Dead Letter Queue

**Servicios**
- Notification
- Email
- Push

---

## FASE 7 — Reports

**Semanas:** 22-24

**Aprenderás**
- MongoDB
- Read Models
- Aggregation
- Caching

**Servicios**
- Reporting
- Analytics
- Dashboard

---

## FASE 8 — API Gateway

**Semanas:** 25-26

**Aprenderás**
- Reverse Proxy
- Routing
- Rate Limiting
- Compression
- CORS

**Servicios**
- Gateway
- NGINX

---

## FASE 9 — Observabilidad

**Semanas:** 27-30

**Aprenderás**
- OpenTelemetry
- Prometheus
- Grafana
- Loki
- Jaeger
- Correlation Id

---

## FASE 10 — Kubernetes

**Semanas:** 31-35

**Aprenderás**
- Pods
- Deployments
- Ingress
- Secrets
- ConfigMaps
- Helm
- Autoscaling

---

## FASE 11 — DevOps

**Semanas:** 36-38

**Aprenderás**
- GitHub Actions
- Docker Registry
- Release
- Versionado
- Rollback

---

## FASE 12 — Producción

**Semanas:** 39-40

**Aprenderás**
- NGINX
- HTTPS
- Let's Encrypt
- Hardening
- Backups
- Monitoring
- Deployment

---

## Entregable final

Al finalizar las 40 semanas, el proyecto incluirá:

- 10+ microservicios
- 4 microfrontends
- Clean Architecture
- DDD
- CQRS
- RabbitMQ
- gRPC
- REST
- PostgreSQL
- MongoDB
- Redis
- Keycloak
- Docker
- Docker Compose
- Kubernetes
- GitHub Actions
- Observabilidad completa
- Despliegue en producción

---

## Propuesta adicional — Project Management Kit

Para acercar el proyecto a un estándar de equipo de producto real, se propone complementar los entregables técnicos con un **Project Management Kit**:

| Artefacto | Propósito |
|---|---|
| **PRD** (Product Requirements Document) | Visión del producto y objetivos de negocio |
| **ADR** (Architecture Decision Records) | Justificación de cada decisión técnica importante |
| **Diagramas C4** | Context, Container, Component y Deployment |
| **Backlog Scrum** | Épicas, historias de usuario y sprints |
| **DoR / DoD** | Definition of Ready y Definition of Done por historia |
| **Checklists** | Revisión de código, seguridad y despliegue |
| **Documentación de APIs** | OpenAPI/Swagger + colecciones de Postman |
| **Runbooks** | Cómo desplegar, monitorear, recuperar ante fallos y hacer rollback |
| **Changelog** | Versionado semántico |

Este conjunto de documentos busca que el repositorio demuestre no solo habilidades de programación, sino también de arquitectura, gestión técnica y DevOps — acercándose al estándar de un proyecto desarrollado por un equipo profesional, con mayor peso en procesos de selección para posiciones Senior o Tech Lead.
