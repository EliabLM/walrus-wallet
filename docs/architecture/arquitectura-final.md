# WalrusWallet — Arquitectura Final (Visión de Destino)

**Ubicación recomendada:** `docs/architecture/arquitectura-final.md`
**Estado:** Visión de destino — no es un plan de inicio
**Relacionado con:** `ADR-0001-Monolito-Modular-Fase1.md`, `ADR-0002-Catalogo-Final-Microservicios-Microfrontends.md`, `WalrusWallet-Plan-Tecnico.md`

---

## 0. Qué es este documento y qué NO es

Este documento describe **a dónde llega WalrusWallet** después de las 12 fases del Plan Técnico (~40 semanas): microservicios, microfrontends, Kubernetes y observabilidad completa.

**No es** la arquitectura con la que se empieza. El punto de partida está en `ADR-0001-Monolito-Modular-Fase1.md`: un monolito modular, una sola SPA, un solo deployable. Cada pieza que aparece aquí se construye cuando el dominio que la justifica ya existe y está validado — nunca antes.

Si en algún momento este documento entra en conflicto con una decisión tomada en una fase concreta (por ejemplo, qué microservicio se extrae primero), **gana la decisión tomada en esa fase con su propio ADR**, y este documento se actualiza para reflejarla. Este documento es el norte; los ADR son el camino real recorrido.

---

## 1. Resumen ejecutivo

WalrusWallet, en su forma final, es una plataforma fintech SaaS compuesta por:

- Un **frontend distribuido** en microfrontends (Module Federation), organizados por dominio de negocio, servidos detrás de un host React único.
- Un **backend distribuido** en microservicios de dominio, cada uno con su propia base de datos, comunicándose de forma síncrona (REST/gRPC) y asíncrona (RabbitMQ + eventos de dominio).
- Una **capa de identidad** centralizada en Keycloak, con JWT propagado a través de un API Gateway.
- Una **plataforma de despliegue** en Kubernetes (vía k3s sobre VPS), con CI/CD automatizado y observabilidad completa (métricas, logs, trazas).

Esta arquitectura es el resultado de extracciones progresivas desde el monolito modular de Fase 1, nunca un punto de partida.

---

## 2. Relación con la hoja de ruta de evolución

```text
Monolito Modular → Clean Architecture → CQRS → Eventos de dominio →
RabbitMQ → Extracción de microservicio #1 → #2 → #3 → ... →
Module Federation → Docker → Docker Compose → Kubernetes → CI/CD →
Producción → Observabilidad → Optimización
```

Cada microservicio y cada microfrontend descrito en este documento tiene, en algún punto de esa cadena, un evento de origen: el momento en que se decidió que ya no convenía seguir viviendo dentro del monolito modular. Esa decisión se documenta como un ADR propio (`ADR-000N-Extraccion-<dominio>.md`) cuando ocurre, no se decide de antemano en este documento.

---

## 3. Diagrama C4 — Nivel 1: Contexto

```text
                              ┌────────────────────┐
                              │  Usuario final      │
                              │ (titular de cuenta)  │
                              └──────────┬───────────┘
                                         │ HTTPS
                                         ▼
                         ┌───────────────────────────────┐
                         │        WalrusWallet            │
                         │   Plataforma fintech SaaS       │
                         └───────────────────────────────┘
                          │            │            │
                          ▼            ▼            ▼
                  ┌─────────────┐ ┌──────────┐ ┌────────────────┐
                  │  Keycloak    │ │  Bancos /  │ │  Proveedores    │
                  │  (Identity   │ │  pasarelas │ │  de notificación│
                  │  Provider)   │ │  de pago   │ │  (email/push)   │
                  └─────────────┘ └──────────┘ └────────────────┘
```

WalrusWallet es el sistema central. Depende de Keycloak como proveedor de identidad externo, de integraciones bancarias/pasarelas de pago para movimientos reales, y de proveedores externos de email/push para notificaciones.

---

## 4. Diagrama C4 — Nivel 2: Contenedores

```text
Internet
   │
   ▼
NGINX (reverse proxy / TLS termination)
   │
   ├── React Host  ──────────────────────────────┐
   │     │ vista Dashboard (compuesta en el Host,  │
   │     │ no es un remoto)                        │
   │     │ Module Federation                      │
   │     ├── Finance MFE                           │
   │     ├── Billing MFE                           │
   │     ├── Reports MFE                            │
   │     └── Administration MFE                     │
   │                                                │
   └── API Gateway ─────────────────────────────────┘
          │
          │ (auth: valida JWT emitido por Keycloak)
          │
   ┌──────┴───────────────────────────────────────────────────┐
   │            Microservicios de dominio (catálogo ADR-0002)   │
   │                                                              │
   │  Identity · Company · Wallet · Payment · Bank · Billing      │
   │  Scheduler · File · Notification · Reporting · Budget · Audit│
   └──────┬───────────────────────────────────────────────────┘
          │
          ├── síncrono: REST / gRPC (consultas, comandos directos)
          └── asíncrono: RabbitMQ (eventos de dominio, outbox/inbox)
          │
   ┌──────┴────────────────────────────────┐
   │   PostgreSQL · MongoDB · Redis          │
   │   (una instancia lógica por servicio,    │
   │    no una BD compartida)                  │
   └─────────────────────────────────────────┘
          │
   ┌──────┴────────────────────────────────┐
   │   Observabilidad                         │
   │   Prometheus · Grafana · Loki · Jaeger   │
   └─────────────────────────────────────────┘
```

---

## 5. Catálogo de microservicios de dominio

> Esta sección queda **cerrada por `ADR-0002-Catalogo-Final-Microservicios-Microfrontends.md`**. Las versiones anteriores de este documento dejaban un grupo de servicios sin fase asignada (Audit, File, Bank, Budget, Scheduler) — ADR-0002 resolvió ese hueco consolidando aggregates que el Plan Técnico listaba como servicios sueltos.

| # | Microservicio | Aggregates/entidades internas | Fase de extracción | Base de datos |
|---|---|---|---|---|
| 1 | **Identity** | User + integración Keycloak (un solo bounded context) | Fase 2 | PostgreSQL |
| 2 | **Company** | Company, Workspace, Membership | Fase 3 | PostgreSQL |
| 3 | **Wallet** | Wallet, Account, Balance, Transaction (ledger), Transfer interno | Fase 4 | PostgreSQL (ACID, optimistic concurrency) |
| 4 | **Payment** | Orquestación de pasarelas externas, idempotencia | Fase 5 | PostgreSQL + outbox |
| 5 | **Bank** | Vinculación bancaria externa / open banking | Fase 5 | PostgreSQL |
| 6 | **Billing** | Billing, Invoice | Fase 5 | PostgreSQL |
| 7 | **Scheduler** | Jobs recurrentes (transferencias/facturas programadas) | Fase 5 | Redis (estado) + PostgreSQL (definición) |
| 8 | **File** | Recibos, comprobantes, exportes | Fase 5 | Object storage + MongoDB (metadata) |
| 9 | **Notification** | Notification, canal Email, canal Push | Fase 6 | MongoDB |
| 10 | **Reporting** | Reporting, Analytics | Fase 7 | MongoDB |
| 11 | **Budget** | Presupuestos y metas definidos por el usuario | Fase 7 | PostgreSQL |
| 12 | **Audit** | Consumidor cross-cutting de eventos de todos los dominios, trail de cumplimiento | Fase 7 | MongoDB |

`Dashboard` no es un microservicio: la vista de inicio la compone el Host del frontend agregando Wallet + Reporting + Notification a través del API Gateway, sin un backend propio.

### 5.1 Servicios de plataforma (no son dominio de negocio)

| Servicio | Responsabilidad |
|---|---|
| **API Gateway** | Punto único de entrada, enrutamiento, rate limiting, agregación de respuestas |

### 5.2 Decisiones de diseño abiertas a ajuste

ADR-0002 marca tres puntos como juicio de diseño, no como verdad cerrada para siempre:

- **Bank vs. Payment:** se mantienen separados porque son compliance/proveedores distintos (agregación bancaria vs. rieles de pago). Si en Fase 5 se confirma que siempre cambian juntos, se fusionan con un ADR de supplement.
- **Scheduler como microservicio propio:** elegido para practicar scheduling distribuido (locks con Redis); es razonable degradarlo a librería embebida en Payment/Billing si resulta sobre-dimensionado.
- **Audit y Budget en Fase 7:** agrupados junto a Reporting porque comparten el mismo patrón arquitectónico (consumidor de eventos → read model propio), aunque su dominio de negocio sea distinto entre sí.

---

## 6. Microfrontends (Module Federation)

División por dominio de negocio, no por página, según el principio del proyecto. Catálogo cerrado por ADR-0002: **Host + 4 remotos** (no 5 — la vista Dashboard vive en el Host, no es un remoto separado):

```text
React Host (shell)
 │  └── vista Dashboard: agrega Wallet, Reporting y Notification vía Gateway
 │      (composición en el Host, NO es un remoto de Module Federation)
 │
 ├── Finance        → consume Wallet, Bank, Scheduler
 ├── Billing        → consume Billing, Payment, File
 ├── Reports        → consume Reporting, Analytics, Budget
 └── Administration → consume Identity, Company, Audit (vista solo-admin), preferencias de Notification
```

Cada microfrontend es un proyecto React independiente, con su propio pipeline de build y despliegue, expuesto como remoto de Module Federation y consumido por el Host. No hay relación 1:1 entre microfrontends y microservicios: cada MFE consume los servicios que el usuario necesita ver, no una partición espejo del backend.

**Regla de corte:** un microfrontend se separa cuando el microservicio (o grupo de microservicios) de dominio que representa ya fue extraído del backend. El frontend nunca se fragmenta por delante del backend — así lo establece ADR-0001 para Fase 1, y el mismo criterio aplica en cada extracción posterior.

---

## 7. API Gateway

Responsabilidades:

- Único punto de entrada HTTP/HTTPS desde NGINX hacia los microservicios.
- Validación de JWT emitido por Keycloak (no reimplementa auth, solo verifica).
- Enrutamiento por dominio (`/api/wallet/*`, `/api/payments/*`, `/api/reports/*`, etc.).
- Rate limiting y throttling por cliente/usuario.
- Agregación ligera de respuestas cuando un microfrontend necesita datos de más de un servicio (evitando "chatty" calls desde el navegador).
- Compresión y CORS.

No contiene lógica de negocio. Es infraestructura de borde, no un servicio de dominio.

---

## 8. Comunicación entre servicios

### 8.1 Síncrona

- **REST** para integraciones simples consulta/respuesta entre microfrontend ↔ Gateway ↔ microservicio.
- **gRPC** para comunicación servicio a servicio de alta frecuencia o con contratos estrictos (ej. Payment → Wallet para verificar saldo antes de autorizar).

### 8.2 Asíncrona

- **RabbitMQ** como broker de eventos de dominio.
- Cada microservicio publica eventos de su propio dominio (`PaymentCompleted`, `TransferRequested`, `UserRegistered`, etc.) y consume los eventos de otros dominios que le interesan.
- **Outbox Pattern**: cada servicio que publica eventos transaccionales los escribe primero en una tabla `outbox` dentro de su propia transacción de base de datos, y un proceso aparte los publica a RabbitMQ — evita perder eventos si el broker falla.
- **Inbox Pattern**: cada consumidor registra los eventos ya procesados para garantizar **idempotencia** ante reintentos o entregas duplicadas.
- **Dead Letter Queue** para mensajes que fallan repetidamente, con reintento controlado.

---

## 9. CQRS, Outbox/Inbox e idempotencia

Los servicios con mayor complejidad de lectura/escritura (Payment, Wallet, Reporting) aplican CQRS:

- **Comandos**: validan reglas de negocio y escriben contra el modelo transaccional (PostgreSQL).
- **Queries**: leen contra modelos de lectura optimizados (a veces en MongoDB, a veces vistas materializadas en PostgreSQL), poblados de forma asíncrona vía eventos de dominio.

Esto es exactamente el paso "CQRS → Eventos de dominio" de la cadena de evolución: se introduce **antes** de extraer microservicios, no después, para que la extracción posterior sea mecánica.

---

## 10. Estrategia de persistencia por servicio

Siguiendo el criterio de bases de datos del proyecto:

| Tipo de dato | Motor | Servicios típicos |
|---|---|---|
| Transaccional (usuarios, pagos, facturas, cuentas, movimientos) | **PostgreSQL** | Identity, Company, Wallet, Payment, Bank, Billing, Budget |
| Documental (auditoría, logs de negocio, reportes, snapshots) | **MongoDB** | Notification, Reporting, Audit |
| Cache / sesiones / locks distribuidos / idempotencia / pub-sub | **Redis** | Transversal a todos los servicios; uso dedicado en Scheduler (estado de jobs) |
| Almacenamiento de objetos (no es uno de los tres motores anteriores) | **Object storage** (ej. S3-compatible) + MongoDB para metadata | File |

**Regla de oro:** *database per service*. Ningún microservicio accede directamente a la base de datos de otro; toda comunicación entre dominios pasa por API (síncrona) o eventos (asíncrona).

---

## 11. Identidad y seguridad

```text
Usuario → React Host → Keycloak (login) → JWT
                                              │
                                              ▼
                                        API Gateway
                                    (valida firma y expiración)
                                              │
                                              ▼
                                  Microservicio de dominio
                          (valida claims/roles específicos del caso de uso)
```

- **Keycloak** sigue siendo el único proveedor de identidad, igual que en Fase 1 — no se reemplaza, se le suman más clientes (uno por microfrontend si aplica) y más roles/scopes a medida que crecen los dominios.
- El **Gateway** centraliza la validación de firma/expiración del JWT; cada microservicio solo valida los claims/roles relevantes para su propio caso de uso (autorización fina, no autenticación).
- Comunicación servicio a servicio idealmente sobre mTLS dentro del clúster (Fase 10 — Kubernetes, con service mesh o configuración manual de certificados, según el alcance que se decida en esa fase).

---

## 12. Infraestructura en Kubernetes

```text
Namespace: walruswallet-prod
 ├── Deployment: api-gateway          (HPA, readiness/liveness probes)
 ├── Deployment: identity-svc
 ├── Deployment: company-svc
 ├── Deployment: wallet-svc
 ├── Deployment: payment-svc
 ├── Deployment: bank-svc
 ├── Deployment: billing-svc
 ├── Deployment: scheduler-svc
 ├── Deployment: file-svc
 ├── Deployment: notification-svc
 ├── Deployment: reporting-svc
 ├── Deployment: budget-svc
 ├── Deployment: audit-svc
 ├── Deployment: react-host
 ├── Deployment: finance-mfe
 ├── Deployment: billing-mfe
 ├── Deployment: reports-mfe
 ├── Deployment: administration-mfe
 ├── StatefulSet: rabbitmq
 ├── StatefulSet: postgres (o instancias gestionadas, una por servicio o un clúster con esquemas separados)
 ├── StatefulSet: mongodb
 ├── StatefulSet: redis
 ├── Ingress: walruswallet-ingress    (TLS, enrutamiento por host/path)
 ├── ConfigMap / Secret: por servicio
 └── PersistentVolumeClaim: por base de datos con estado
```

Elementos a cubrir (objetivos de aprendizaje de Fase 10 del Plan Técnico):

- **ConfigMaps y Secrets** por servicio — nunca variables de entorno hardcodeadas en la imagen.
- **Ingress** único para enrutar tráfico externo hacia el Gateway y hacia los microfrontends servidos como estáticos.
- **Horizontal Pod Autoscaler** en los servicios con carga variable (Payment, Notification).
- **Readiness/Liveness probes** en todos los deployments — un servicio "vivo pero no listo" no debe recibir tráfico.
- **Resource limits/requests** para evitar que un servicio degrade a los demás en el mismo nodo.
- **Rolling updates y rollback** como estrategia estándar de despliegue, no solo `kubectl apply` directo.
- **Helm** (u otra herramienta de templating) para no mantener YAML duplicado entre servicios.

---

## 13. Observabilidad

```text
Microservicio
   │  (instrumentado con OpenTelemetry)
   ├── Métricas ──────────────► Prometheus ──► Grafana (dashboards)
   ├── Logs estructurados ────► Loki ─────────► Grafana (exploración de logs)
   └── Trazas distribuidas ───► Jaeger ────────► Grafana / Jaeger UI
```

- **Correlation ID** propagado desde el Gateway a través de cada llamada síncrona y cada evento asíncrono, para poder seguir una transacción de punta a punta (ej. una transferencia que dispara Payment → Transaction → Notification).
- **Prometheus** para métricas técnicas (latencia, throughput, errores) y de negocio (pagos procesados por minuto, tasa de fallos de transferencia).
- **Grafana** como capa de visualización unificada sobre Prometheus, Loki y Jaeger.
- **Jaeger** para trazas distribuidas — imprescindible una vez que una operación de negocio atraviesa más de un microservicio.

---

## 14. CI/CD y estrategia de release

```text
Push a feature/*
   │
   ▼
Pull Request → develop
   │
   ▼
GitHub Actions:
   Restore → Build → Unit Tests → Integration Tests →
   Build Docker Image → Push a Registry → (en main) Deploy →
   Smoke Tests → Rollback automático si falla
```

- Cada microservicio y cada microfrontend tiene su **propio pipeline**, para no acoplar el release de un dominio al de otro — esto solo tiene sentido una vez que existen múltiples deployables (Fase 8 en adelante); en Fase 1, un solo pipeline cubre el único deployable de cada lado.
- **Versionado semántico** y **Changelog** por servicio.
- **Docker Registry** privado o GitHub Container Registry para las imágenes versionadas.

---

## 15. Despliegue y entorno de producción

Siguiendo la estrategia de despliegue definida en las instrucciones del proyecto:

1. **Desarrollo y validación**: Docker Compose en local (igual que en Fase 1, pero con más servicios a medida que se extraen).
2. **Producción inicial**: VPS de bajo costo (~5 USD/mes) con Docker — válido mientras el número de servicios sea manejable manualmente.
3. **Producción madura**: migración a **k3s** sobre el mismo VPS (o uno ligeramente mayor), sin saltar directamente a un clúster Kubernetes administrado de alto costo.

La razón se mantiene igual que en Fase 1: no se asume complejidad operacional (ni costo) que el proyecto todavía no necesita. Kubernetes llega cuando el número de microservicios ya hace insostenible operarlos a mano con Docker Compose — no antes, por "completitud" de portafolio.

---

## 16. Diagrama de despliegue (vista física simplificada)

```text
┌─────────────────────────────── VPS / k3s ───────────────────────────────┐
│                                                                            │
│   Ingress Controller (NGINX Ingress)                                      │
│        │                                                                  │
│        ├── react-host + MFEs (estáticos, servidos vía NGINX/CDN-like)     │
│        └── api-gateway ── microservicios (pods, varias réplicas c/u)       │
│                                  │                                         │
│                       RabbitMQ ─┼─ PostgreSQL ─ MongoDB ─ Redis            │
│                                  │                                         │
│                       Prometheus ─ Grafana ─ Loki ─ Jaeger                 │
│                                                                            │
└────────────────────────────────────────────────────────────────────────┘
                                   │
                                   ▼
                          Keycloak (puede vivir en el mismo clúster
                          o como servicio externo gestionado)
```

---

## 17. Trazabilidad: de la Fase 1 a la arquitectura final

| Fase del Plan Técnico | Qué aporta a esta arquitectura final |
|---|---|
| Fase 1 — Foundation | Monolito modular, SPA única, Docker Compose base — el punto de partida de todo lo anterior |
| Fase 2 — Identity | Primer candidato a extracción: Identity (User fusionado en el mismo bounded context) |
| Fase 3 — Companies | Multi-tenancy: Company (Workspace y Membership como aggregates internos) |
| Fase 4 — Wallet | Núcleo transaccional: Wallet (Account, Balance, Transaction y Transfer interno como aggregates internos) |
| Fase 5 — Payments & Billing | Introduce CQRS, Outbox/Inbox, primer uso real de RabbitMQ con lógica de negocio. Extrae Payment, Bank, Billing, Scheduler y File (ver ADR-0002) |
| Fase 6 — Notifications | Mensajería asíncrona orientada a consumidores externos: Notification (Email y Push como canales internos) |
| Fase 7 — Reports | Read models, agregaciones — primer caso fuerte de MongoDB como almacén de lectura. Extrae Reporting, Budget y Audit (mismo patrón: consumidor de eventos → read model propio, ver ADR-0002) |
| Fase 8 — API Gateway | Punto único de entrada — condición previa para multiplicar microservicios sin caos |
| Fase 9 — Observabilidad | OpenTelemetry, Prometheus, Grafana, Loki, Jaeger |
| Fase 10 — Kubernetes | Orquestación real de todo lo anterior |
| Fase 11 — DevOps | Pipelines por servicio, registry, rollback |
| Fase 12 — Producción | Hardening, backups, monitoreo continuo |

La extracción de microfrontends (Module Federation) no tiene una fase numerada propia en el Plan Técnico — ADR-0001 establece que ocurre "cuando se extraiga el primer microservicio real". Eso sitúa el primer corte de frontend probablemente entre Fase 2 y Fase 4, dependiendo de cuál dominio se extraiga primero — esa pregunta sigue abierta y se decide al llegar a Fase 2 (ver "Pendiente para revisión futura" en ADR-0001 y en ADR-0002).

---

## 18. Glosario de componentes (resumen)

| Término | Significado en este documento |
|---|---|
| **Microservicio de dominio** | Deployable independiente, con su propia base de datos, responsable de un único subdominio de negocio |
| **Microfrontend** | Proyecto React independiente, cargado por el Host vía Module Federation, responsable de un único dominio de negocio en la UI |
| **Outbox Pattern** | Tabla local donde un servicio escribe los eventos que debe publicar, dentro de la misma transacción que el cambio de negocio |
| **Inbox Pattern** | Registro de eventos ya procesados por un consumidor, para garantizar idempotencia |
| **Read Model** | Modelo de datos optimizado para lectura, poblado de forma asíncrona a partir de eventos de dominio |
| **API Gateway** | Punto único de entrada que enruta, autentica (valida JWT) y protege los microservicios internos |

---

## 19. Cómo mantener este documento vivo

- Este documento **no se reescribe completo** cada vez que cambia un detalle: se actualiza la sección puntual afectada, igual que el resto de documentos del proyecto.
- El catálogo de microservicios y microfrontends (secciones 5 y 6) está **cerrado por ADR-0002**. Si en la práctica de una fase se confirma que dos servicios del catálogo deben fusionarse o que uno debe partirse, esa decisión se registra con un ADR de supplement (ej. `ADR-0003`) — no editando ADR-0002 ni esta sección directamente.
- Si en algún punto la arquitectura real construida se desvía de lo descrito aquí, el ADR de esa decisión tiene prioridad y este documento se ajusta para reflejarlo — nunca al revés.
