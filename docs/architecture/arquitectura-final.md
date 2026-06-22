# WalrusWallet — Arquitectura Final (Visión de Destino)

**Ubicación recomendada:** `docs/architecture/arquitectura-final.md`
**Estado:** Visión de destino — no es un plan de inicio
**Relacionado con:** `ADR-0001-Monolito-Modular-Fase1.md`, `WalrusWallet-Plan-Tecnico.md`

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
   │     │ Module Federation                      │
   │     ├── Dashboard MFE                         │
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
   │                  Microservicios de dominio                 │
   │                                                              │
   │  User · Identity · Audit · Company · Workspace · Membership │
   │  Wallet · Account · Balance                                  │
   │  Billing · Invoice · Payment · Transaction · Transfer · Bank │
   │  Budget · Notification · Email · Push                        │
   │  Reporting · Analytics · Dashboard(svc) · File · Scheduler    │
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

### 5.1 Servicios con fase de origen confirmada en el Plan Técnico

| Servicio | Fase de origen | Responsabilidad | Base de datos |
|---|---|---|---|
| **Identity** | Fase 2 — Identity | Integración OIDC con Keycloak, gestión de claims/roles/permissions | PostgreSQL |
| **User** | Fase 2 — Identity | Perfil de usuario interno, vínculo con entidad de negocio | PostgreSQL |
| **Company** | Fase 3 — Companies | Datos de la organización/tenant | PostgreSQL |
| **Workspace** | Fase 3 — Companies | Espacios de trabajo dentro de una Company | PostgreSQL |
| **Membership** | Fase 3 — Companies | Relación usuario ↔ Company/Workspace, multi-tenancy | PostgreSQL |
| **Wallet** | Fase 4 — Wallet | Billetera del usuario, saldo agregado | PostgreSQL (ACID, optimistic concurrency) |
| **Account** | Fase 4 — Wallet | Cuentas asociadas a una Wallet | PostgreSQL |
| **Balance** | Fase 4 — Wallet | Cálculo y consistencia de saldo | PostgreSQL + Redis (cache de lectura) |
| **Payment** | Fase 5 — Payments | Orquestación de pagos, idempotencia | PostgreSQL + outbox table |
| **Transaction** | Fase 5 — Payments | Registro inmutable de movimientos | PostgreSQL |
| **Transfer** | Fase 5 — Payments | Transferencias entre cuentas/usuarios | PostgreSQL |
| **Notification** | Fase 6 — Notifications | Orquestación de notificaciones, plantillas | MongoDB |
| **Email** | Fase 6 — Notifications | Envío de correo, proveedor externo | MongoDB (logs de envío) |
| **Push** | Fase 6 — Notifications | Notificaciones push, SignalR | MongoDB (logs de envío) |
| **Reporting** | Fase 7 — Reports | Read models de reportes financieros | MongoDB |
| **Analytics** | Fase 7 — Reports | Agregaciones y métricas de negocio | MongoDB |

### 5.2 Servicios mencionados en las instrucciones del proyecto sin fase de origen todavía

Las instrucciones del proyecto (sección 3, arquitectura final) listan también **Audit, File, Bank, Budget y Scheduler** como microservicios de dominio. Al cruzarlos con el Plan Técnico:

| Servicio | Aparece en instrucciones (visión final) | Aparece en Plan Técnico (fase concreta) | Estado |
|---|---|---|---|
| **Audit** | Sí | No — hoy es la entidad `AuditEvent` dentro del módulo Identity (ADR-0001, Fase 1) | Pendiente: decidir en qué fase se extrae como servicio propio |
| **File** | Sí | No aparece en ninguna fase 1-12 | Pendiente: no tiene fase asignada |
| **Bank** | Sí | No aparece como servicio propio; lo más cercano es la integración bancaria implícita en Payments (Fase 5) | Pendiente: aclarar si es un servicio propio o parte de Payment |
| **Budget** | Sí | No aparece en ninguna fase 1-12 | Pendiente: no tiene fase asignada |
| **Scheduler** | Sí | No aparece en ninguna fase 1-12 | Pendiente: no tiene fase asignada |

> **Nota de coherencia:** este documento no resuelve estos huecos por sí mismo — eso sería anticipar decisiones de fase, justo lo que ADR-0001 busca evitar. Se deja registrado aquí para que, al llegar a la fase donde cada uno tendría sentido (probablemente Fase 4 o 5 para Bank, y una fase nueva o ampliación de Fase 7 para Budget/Scheduler/File), se revise explícitamente — con el mismo patrón que ya usa ADR-0001 en su sección "Pendiente para revisión futura" sobre Identity Service/User Service en Fase 2.

### 5.3 Servicios de plataforma (no son dominio de negocio)

| Servicio | Responsabilidad |
|---|---|
| **API Gateway** | Punto único de entrada, enrutamiento, rate limiting, agregación de respuestas |
| **Dashboard (servicio de agregación)** | Compone datos de Reporting/Analytics/Wallet para la vista de inicio; no tiene base de datos propia, solo agrega |

---

## 6. Microfrontends (Module Federation)

División por dominio de negocio, no por página, según el principio del proyecto:

```text
React Host (shell)
 ├── Dashboard      → agrega Wallet, Payment, Notification (resumen)
 ├── Finance        → consume Wallet, Account, Balance
 ├── Billing        → consume Billing, Invoice, Payment, Transaction, Transfer
 ├── Reports        → consume Reporting, Analytics
 └── Administration → consume User, Identity, Company, Workspace, Membership
```

Cada microfrontend es un proyecto React independiente, con su propio pipeline de build y despliegue, expuesto como remoto de Module Federation y consumido por el Host.

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
| Transaccional (usuarios, pagos, facturas, cuentas, movimientos) | **PostgreSQL** | Identity, User, Company, Wallet, Account, Balance, Payment, Transaction, Transfer, Billing, Invoice |
| Documental (auditoría, logs de negocio, reportes, snapshots) | **MongoDB** | Audit, Notification, Email, Push, Reporting, Analytics |
| Cache / sesiones / locks distribuidos / idempotencia / pub-sub | **Redis** | Transversal a todos los servicios (no es una base "de un servicio", es infraestructura compartida de soporte) |

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
 ├── Deployment: user-svc
 ├── Deployment: company-svc
 ├── Deployment: wallet-svc
 ├── Deployment: payment-svc
 ├── Deployment: notification-svc
 ├── Deployment: reporting-svc
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
| Fase 2 — Identity | Primeros candidatos a extracción: Identity, User |
| Fase 3 — Companies | Multi-tenancy: Company, Workspace, Membership |
| Fase 4 — Wallet | Núcleo transaccional: Wallet, Account, Balance |
| Fase 5 — Payments | Introduce CQRS, Outbox/Inbox, primer uso real de RabbitMQ con lógica de negocio |
| Fase 6 — Notifications | Mensajería asíncrona orientada a consumidores externos (email/push) |
| Fase 7 — Reports | Read models, agregaciones — primer caso fuerte de MongoDB como almacén de lectura |
| Fase 8 — API Gateway | Punto único de entrada — condición previa para multiplicar microservicios sin caos |
| Fase 9 — Observabilidad | OpenTelemetry, Prometheus, Grafana, Loki, Jaeger |
| Fase 10 — Kubernetes | Orquestación real de todo lo anterior |
| Fase 11 — DevOps | Pipelines por servicio, registry, rollback |
| Fase 12 — Producción | Hardening, backups, monitoreo continuo |

La extracción de microfrontends (Module Federation) no tiene una fase numerada propia en el Plan Técnico — ADR-0001 establece que ocurre "cuando se extraiga el primer microservicio real". Eso sitúa el primer corte de frontend probablemente entre Fase 2 y Fase 4, dependiendo de cuál dominio se extraiga primero (ver la sección "Pendiente para revisión futura" de ADR-0001).

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
- Cuando una fase resuelva uno de los "pendientes" de la sección 5.2 (Audit, File, Bank, Budget, Scheduler), esa fase debe generar su propio ADR de extracción, y este documento se actualiza para mover el servicio de la tabla 5.2 a la tabla 5.1 con su fase de origen real.
- Si en algún punto la arquitectura real construida se desvía de lo descrito aquí (por ejemplo, se decide fusionar dos servicios que aquí aparecen separados), el ADR de esa decisión tiene prioridad y este documento se ajusta para reflejarlo — nunca al revés.
