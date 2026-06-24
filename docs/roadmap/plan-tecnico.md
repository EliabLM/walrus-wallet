# WalrusWallet — Plan Técnico de Implementación (Reestructurado)

## Datos generales

| Campo | Valor |
|---|---|
| Duración estimada | 40 semanas |
| Ritmo | 12 horas / semana |
| Esfuerzo total estimado | ≈ 480 horas |

> **Nota de coherencia (versión reestructurada):** esta versión incorpora `ADR-0001-Monolito-Modular-Fase1.md` y `ADR-0002-Catalogo-Final-Microservicios-Microfrontends.md`. La versión anterior de este plan tenía dos problemas que esta versión resuelve:
> 1. Las Fases 2-7 ya hablaban de "Identity Service", "Company Service", "Workspace Service", etc. como si la extracción ocurriera de inmediato en cada fase — eso contradice la cadena de evolución del propio proyecto (`Monolito Modular → Clean Architecture → CQRS → Eventos de dominio → RabbitMQ → Extracción de microservicio`), que exige CQRS, eventos de dominio y RabbitMQ con lógica real **antes** de cualquier extracción.
> 2. Billing, Audit, File, Bank, Budget y Scheduler no tenían fase asignada, y Module Federation no tenía un punto de entrada claro.
>
> Esta versión corrige ambos puntos: las Fases 2-5 **construyen módulos dentro del monolito** (sin extraer nada todavía), la Fase 6 ejecuta la **primera extracción real** una vez que CQRS/Eventos/RabbitMQ ya están probados, y a partir de ahí los módulos nuevos nacen directamente como servicios extraídos. El catálogo final de 12 microservicios y 4 microfrontends + Host queda fijado por ADR-0002.

---

## Cómo leer este plan: el mecanismo de evolución

Cada fase indica explícitamente si **construye dentro del monolito** o si **extrae un microservicio**. La regla, heredada de ADR-0001 y aplicada de forma consistente en toda la vida del proyecto (no solo en Fase 1):

```text
1. Un dominio nuevo nace como módulo dentro del monolito modular
   (Domain/<Modulo>, Application/<Modulo>, Infrastructure/<Modulo>),
   con su propio esquema de base de datos dentro de la misma instancia
   de PostgreSQL/MongoDB (ej. identity.*, company.*, wallet.*).

2. El módulo madura: Clean Architecture limpia, CQRS donde aporte valor,
   eventos de dominio disparados primero en memoria (in-process).

3. Cuando el proyecto ya tiene al menos un módulo con eventos de dominio
   reales, esos eventos se promueven a RabbitMQ (Outbox/Inbox/Idempotency).

4. Solo entonces se extrae el primer microservicio: el esquema de base
   de datos migra a una instancia propia, el módulo se convierte en su
   propio deployable, y el resto del monolito le habla por API/eventos
   en lugar de llamadas in-process.

5. Una vez probado el patrón de extracción con el primer servicio,
   los módulos siguientes pueden nacer directamente como servicios
   extraídos — ya no necesitan pasar primero por el monolito, porque
   el contrato de extracción (API + eventos + base de datos propia)
   ya está validado.

6. El primer microfrontend (Module Federation) nace junto con la
   primera extracción de backend — nunca antes (ADR-0001).
```

Este plan aplica esa secuencia de forma literal: **Fases 2-5 construyen dentro del monolito. Fase 6 ejecuta la primera extracción. Fases 6 (segunda mitad) en adelante, los módulos nuevos nacen ya como microservicios.**

---

## FASE 1 — Foundation

**Semanas:** 1-4
**Mecanismo:** monolito modular, un solo deployable, una sola SPA (sin cambios respecto a ADR-0001)

### Semana 1 — Repositorio profesional

**Aprenderás:** GitFlow, Monorepo, Convenciones, ADR, Documentación, Arquitectura C4

| Bloque | Contenido |
|---|---|
| Crear repositorio | `walruswallet` |
| Crear estructura | `backend/`, `frontend/`, `infra/`, `docs/`, `.github/`, `scripts/`, `ADR/` |
| Crear documentación | README, Vision, Roadmap, Architecture, Contributing, Glossary |
| Crear archivos base | `LICENSE`, `.gitignore`, `.editorconfig`, `.env.example` |
| Configurar | Conventional Commits, GitHub Projects, Milestones, Issues, Labels |

**Definition of Done:** el proyecto parece un repositorio profesional aunque todavía no tenga funcionalidades.

### Semana 2 — Backend Foundation

**Aprenderás:** Clean Architecture, Dependency Injection, Minimal APIs, Vertical Slice, Fluent Validation

| Bloque | Contenido |
|---|---|
| Crear solución | `WalrusWallet.sln` (un único deployable: `WalrusWallet.Api`) |
| Crear proyectos | Domain, Application, Infrastructure, Api, SharedKernel |
| Configurar | DI, Logging, Configuration, Options Pattern, Global Exception Middleware |
| Crear endpoints | `/health`, `/version` |
| Agregar | Swagger, Serilog, Health Checks |

**Definition of Done:** existe un backend completamente funcional aunque no haga negocio.

### Semana 3 — Infraestructura Docker

**Aprenderás:** Docker, Docker Compose, Networking, Volumes, Images

| Bloque | Contenido |
|---|---|
| Crear Dockerfile | Backend, Frontend |
| Crear compose y levantar | PostgreSQL, MongoDB, Redis, RabbitMQ, Keycloak, NGINX |
| Configurar | Variables, Networks, Volumes |

**Resultado esperado:** `docker compose up` levanta toda la plataforma. RabbitMQ solo se valida con un mensaje de prueba — sin lógica de negocio (ADR-0001).

### Semana 4 — Frontend Foundation (SPA única)

**Aprenderás:** Arquitectura de SPA, React Architecture, Routing, Integración con IdP externo (Keycloak)

- Crear la aplicación React (un solo proyecto, **sin** Module Federation)
- Layout, Routing, feature de autenticación (login), integración Keycloak, manejo de sesión

**Resultado:** login funcionando sobre una SPA única.

> Module Federation se introduce en Fase 6, junto con la primera extracción real de microservicio (ADR-0001).

---

## FASE 2 — Identity

**Semanas:** 5-7
**Mecanismo:** construye **dentro del monolito** — módulo `Domain/Identity`, `Application/Identity`, `Infrastructure/Identity`, esquema propio `identity.*` en la misma instancia de PostgreSQL. **No se extrae todavía.**

**Aprenderás:** OAuth2, OIDC, JWT, Refresh Tokens, Claims, Roles, Permissions, organización de un módulo de dominio dentro de Clean Architecture compartida

**Se construye (dentro del monolito):**
- Módulo Identity: perfil de usuario interno (`UserProfile`), claims/roles/permissions avanzados, vínculo completo con Keycloak
- `AuditEvent` se mantiene dentro de Identity (se extrae como servicio Audit propio en Fase 7 — ver ADR-0002)

**Definition of Done:** el módulo Identity tiene límites de Clean Architecture impecables — Domain sin dependencias de Infrastructure, casos de uso testeados — de forma que, cuando llegue su turno de extracción (Fase 7), sea un refactor mecánico, no un rediseño.

---

## FASE 3 — Companies

**Semanas:** 8-10
**Mecanismo:** construye **dentro del monolito** — módulo `Domain/Company` con `Workspace` y `Membership` como aggregates internos del mismo módulo (no como módulos separados — ver ADR-0002). Esquema propio `company.*`. **No se extrae todavía.**

**Aprenderás:** DDD, Repositories, Specifications, Multi-tenancy

**Se construye (dentro del monolito):**
- Módulo Company: `Company`, `Workspace`, `Membership` como un solo bounded context
- Reglas de multi-tenancy: a qué Company/Workspace pertenece cada usuario autenticado

**Definition of Done:** un usuario puede pertenecer a una o más Companies/Workspaces, y el resto del monolito (Identity) puede consultar esa pertenencia sin acoplarse a las tablas internas de Company.

---

## FASE 4 — Wallet

**Semanas:** 11-14
**Mecanismo:** construye **dentro del monolito** — módulo `Domain/Wallet` con `Account`, `Balance`, `Transaction` (ledger) y `Transfer` interno como aggregates del mismo bounded context. Esquema propio `wallet.*`. **Aquí se introduce CQRS por primera vez en el proyecto.** **No se extrae todavía.**

**Aprenderás:** Transacciones, ACID, Optimistic Concurrency, Row Versioning, **CQRS (primera aparición)**, Domain Events in-process

**Se construye (dentro del monolito):**
- Módulo Wallet: `Wallet`, `Account`, `Balance`, `Transaction`, `Transfer` interno
- Separación Comando/Query: los comandos (`CreateTransfer`, `Deposit`) validan reglas de negocio contra el modelo transaccional; las queries (`GetBalance`, `GetTransactionHistory`) leen contra un modelo de lectura optimizado dentro de la misma base de datos
- Eventos de dominio (`BalanceUpdated`, `TransferCompleted`) se publican **in-process** (un mediador interno, no RabbitMQ todavía) — practicando el patrón antes de pagar el costo de infraestructura de mensajería real

**Definition of Done:** el saldo de una Wallet es siempre consistente bajo escritura concurrente (optimistic concurrency probado con tests), y existe al menos un evento de dominio disparándose in-process ante un cambio de saldo.

---

## FASE 5 — Payments & Billing

**Semanas:** 15-18
**Mecanismo:** construye **dentro del monolito** — módulos `Domain/Payment`, `Domain/Bank`, `Domain/Billing`, `Domain/Scheduler`, `Domain/File`, cada uno con su esquema propio. **Aquí los eventos de dominio se promueven de in-process a RabbitMQ real, con lógica de negocio.** Es el último escalón antes de la primera extracción.

**Aprenderás:** CQRS aplicado a Payment, RabbitMQ con lógica de negocio real, Outbox Pattern, Inbox Pattern, Idempotency, integración con pasarelas de pago, vinculación bancaria (open banking), generación de facturas, scheduling de jobs recurrentes, almacenamiento de archivos

**Se construye (dentro del monolito):**
- **Payment**: orquestación de pagos contra pasarelas externas, idempotencia por clave de operación
- **Bank**: vinculación de cuentas bancarias externas (open banking), distinto de Payment porque es agregación de cuentas, no movimiento de dinero
- **Billing**: `Billing` + `Invoice` como un solo bounded context
- **Scheduler**: jobs recurrentes (transferencias y facturas programadas) — primer uso de locks distribuidos con Redis
- **File**: almacenamiento de recibos/comprobantes/exportes (object storage + metadata en MongoDB)
- **Primer evento de dominio real sobre RabbitMQ** (ej. `PaymentCompleted`) con **Outbox Pattern** (el evento se escribe en una tabla `outbox` dentro de la misma transacción que el pago) y al menos un consumidor con **Inbox Pattern** para garantizar idempotencia

**Definition of Done:** existe al menos un flujo de negocio completo que: ejecuta un comando, escribe en Outbox, publica a RabbitMQ, y un consumidor lo procesa de forma idempotente — el mismo patrón que se repetirá en cada microservicio extraído de aquí en adelante.

---

## FASE 6 — Primera Extracción de Microservicios & Notifications

**Semanas:** 19-22
**Mecanismo:** **aquí ocurre la primera extracción real.** Con Clean Architecture, CQRS, eventos de dominio y RabbitMQ ya probados (Fases 2-5), el monolito está listo para empezar a descomponerse.

**Aprenderás:** Strangler Fig Pattern, Database-per-Service Migration, Anti-Corruption Layer, Contract Testing, Saga Pattern (coordinación entre servicios extraídos y el resto del monolito), **Module Federation (primera aparición real)**

### Semana 19 — ADR-0003 y primera extracción

- Redactar `ADR-0003-Primera-Extraccion-<Dominio>.md`, decidiendo formalmente cuál módulo se extrae primero. **Recomendación de este plan: Wallet** — es el módulo con CQRS y eventos de dominio más maduros desde Fase 4, y al ser el dominio más consultado por el resto (Payment, Billing, Reporting necesitan su saldo), extraerlo primero obliga a que todo lo que venga después ya nazca hablándole por API/eventos en lugar de in-process. *(Payment es la alternativa cercana si al llegar aquí se prefiere extraer primero el módulo con el contrato de eventos más rico — la decisión final se cierra con ADR-0003 en este punto del proyecto, no antes.)*
- Migrar el esquema `wallet.*` a su propia instancia de PostgreSQL
- Convertir `Wallet` en su propio deployable; el resto del monolito le habla por gRPC (consultas de saldo) y eventos (cambios de estado)
- Crear el **primer microfrontend remoto** (`Finance`) vía Module Federation, consumido por el Host — el corte de frontend refleja el dominio ya extraído en el backend (ADR-0001)

### Semanas 20-21 — Extracción acelerada del núcleo financiero

Con el patrón ya probado, la extracción del resto de Fase 5 es mecánica:

- Extraer **Payment**, **Bank**, **Billing**, **Scheduler**, **File** como servicios independientes (cada uno migra su esquema a su propia base de datos)
- Actualizar el microfrontend `Billing` para consumir Payment, Billing y File ya como servicios remotos

### Semana 22 — Notification

- **Notification nace directamente como servicio extraído** (ya no pasa primero por el monolito — el contrato de extracción ya está validado): `Notification` con canal `Email` y canal `Push` como adaptadores internos del mismo servicio, no como servicios aparte (ADR-0002)
- Consume eventos de Payment/Billing/Wallet vía RabbitMQ para disparar notificaciones

**Definition of Done:** Wallet, Payment, Bank, Billing, Scheduler, File y Notification son deployables independientes, cada uno con su propia base de datos. Existe al menos un microfrontend remoto real (Finance) cargado vía Module Federation desde el Host.

---

## FASE 7 — Reports & Cierre del Catálogo

**Semanas:** 23-25
**Mecanismo:** se extraen los módulos restantes del monolito (Identity, Company) y se construyen directamente como servicios extraídos los últimos tres del catálogo (Reporting, Budget, Audit).

**Aprenderás:** MongoDB como almacén de read models, Aggregation, Caching, consumidores de eventos cross-dominio

**Se extrae (del monolito, ya maduro desde Fases 2-3):**
- **Identity** — se extrae al final, no al principio: no tenía un contrato de eventos tan rico como Wallet/Payment, así que esperar a este punto significa extraerlo con más experiencia acumulada en el equipo (consistente con la nota "Pendiente para revisión futura" de ADR-0001)
- **Company** — mismo razonamiento

**Se construye directamente como servicio extraído (nuevo, sin pasar por el monolito):**
- **Reporting** (`Reporting` + `Analytics`): read models poblados de forma asíncrona desde los eventos de Wallet/Payment/Billing
- **Budget**: presupuestos y metas definidos por el usuario, con alertas vía eventos hacia Notification
- **Audit**: consumidor cross-cutting de eventos de **todos** los dominios — el último en construirse porque recién ahora existen suficientes servicios productores de eventos como para que un trail de auditoría centralizado aporte valor real

**Frontend:**
- Completar los 4 microfrontends remotos: `Reports` (Reporting, Analytics, Budget) y `Administration` (Identity, Company, Audit, preferencias de Notification)
- El Host ya no tiene módulos "huérfanos" sin remoto — el catálogo de microfrontends queda cerrado (Host + Finance + Billing + Reports + Administration, según ADR-0002)

**Definition of Done:** los 12 microservicios del catálogo de ADR-0002 existen como deployables independientes con base de datos propia. El monolito original de Fase 1 ya no contiene lógica de negocio — su único rol histórico fue ser el punto de partida.

---

## FASE 8 — API Gateway

**Semanas:** 26-27

**Aprenderás:** Reverse Proxy, Routing, Rate Limiting, Compression, CORS

**Se construye:**
- **API Gateway**: ahora tiene sentido — antes de este punto, con uno o dos servicios extraídos, un Gateway dedicado habría sido sobre-ingeniería. Con 12 microservicios reales, centralizar enrutamiento, validación de JWT y rate limiting deja de ser opcional
- NGINX se reconfigura para enrutar todo el tráfico hacia el Gateway, y el Gateway hacia cada microservicio

**Definition of Done:** ningún microfrontend llama a un microservicio directamente — todo pasa por el Gateway.

---

## FASE 9 — Observabilidad

**Semanas:** 28-30

**Aprenderás:** OpenTelemetry, Prometheus, Grafana, Loki, Jaeger, Correlation Id

**Se construye:**
- Instrumentación OpenTelemetry en los 12 microservicios + Gateway
- Correlation ID propagado desde el Gateway a través de llamadas síncronas y eventos asíncronos
- Prometheus + Grafana (métricas), Loki (logs), Jaeger (trazas distribuidas)

**Definition of Done:** una transacción que atraviesa Payment → Wallet → Notification puede seguirse de punta a punta con un único Correlation ID en Jaeger.

---

## FASE 10 — Kubernetes

**Semanas:** 31-35

**Aprenderás:** Pods, Deployments, Ingress, Secrets, ConfigMaps, Helm, Autoscaling

**Se construye:**
- Manifiestos (o Helm charts) para los 12 microservicios + Gateway + Host + 4 microfrontends
- ConfigMaps/Secrets por servicio, Ingress único, HPA en los servicios de mayor carga variable (Payment, Notification), Readiness/Liveness probes, Resource limits

**Definition of Done:** todo el sistema corre en un clúster k3s, con rolling updates y rollback probados manualmente al menos una vez.

---

## FASE 11 — DevOps

**Semanas:** 36-38

**Aprenderás:** GitHub Actions, Docker Registry, Release, Versionado, Rollback

**Se construye:**
- Un pipeline de CI/CD **por microservicio y por microfrontend** (ya no un pipeline único como en Fase 1 — esto solo tiene sentido ahora que existen múltiples deployables)
- Versionado semántico y changelog por servicio, registry de imágenes, rollback automático ante fallo de smoke test

**Definition of Done:** un cambio en un solo microservicio dispara únicamente su propio pipeline, sin rebuildear los otros 11.

---

## FASE 12 — Producción

**Semanas:** 39-40

**Aprenderás:** NGINX, HTTPS, Let's Encrypt, Hardening, Backups, Monitoring, Deployment

**Se construye:**
- TLS con Let's Encrypt, hardening de contenedores, backups automatizados de PostgreSQL/MongoDB, monitoreo continuo sobre el stack de observabilidad de Fase 9

**Definition of Done:** WalrusWallet corre en producción real, sobre el VPS/k3s definido en la estrategia de despliegue del proyecto.

---

## Entregable final

Al finalizar las 40 semanas, el proyecto incluye, según el catálogo cerrado por ADR-0002:

**12 microservicios de dominio:** Identity, Company, Wallet, Payment, Bank, Billing, Scheduler, File, Notification, Reporting, Budget, Audit — más el **API Gateway**.

**Host + 4 microfrontends:** Finance, Billing, Reports, Administration (la vista Dashboard vive en el Host, no es un remoto).

Más: Clean Architecture, DDD, CQRS, RabbitMQ (Outbox/Inbox), gRPC, REST, PostgreSQL, MongoDB, Redis, Keycloak, Docker, Docker Compose, Kubernetes, GitHub Actions, observabilidad completa (OpenTelemetry/Prometheus/Grafana/Loki/Jaeger) y despliegue en producción.

La diferencia con la versión anterior de este plan no es el catálogo final — es que ahora **se puede explicar, fase por fase, por qué cada servicio se extrajo cuando se extrajo**, en lugar de asumir que todos nacieron como microservicios desde el día uno. Ese argumento de "evolución deliberada" es, según las instrucciones del proyecto, el eje central del portafolio.

---

## Propuesta adicional — Project Management Kit

Para acercar el proyecto a un estándar de equipo de producto real, se mantiene la propuesta de complementar los entregables técnicos con un **Project Management Kit**:

| Artefacto | Propósito |
|---|---|
| **PRD** (Product Requirements Document) | Visión del producto y objetivos de negocio |
| **ADR** (Architecture Decision Records) | Justificación de cada decisión técnica importante — incluyendo, de aquí en adelante, `ADR-0003` (primera extracción) y los ADR de extracción posteriores |
| **Diagramas C4** | Context, Container, Component y Deployment |
| **Backlog Scrum** | Épicas, historias de usuario y sprints |
| **DoR / DoD** | Definition of Ready y Definition of Done por historia |
| **Checklists** | Revisión de código, seguridad y despliegue |
| **Documentación de APIs** | OpenAPI/Swagger + colecciones de Postman |
| **Runbooks** | Cómo desplegar, monitorear, recuperar ante fallos y hacer rollback |
| **Changelog** | Versionado semántico |

Este conjunto de documentos busca que el repositorio demuestre no solo habilidades de programación, sino también de arquitectura, gestión técnica y DevOps — acercándose al estándar de un proyecto desarrollado por un equipo profesional, con mayor peso en procesos de selección para posiciones Senior o Tech Lead.
