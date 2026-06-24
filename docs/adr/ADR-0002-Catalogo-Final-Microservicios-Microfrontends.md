# ADR-0002 — Catálogo Final de Microservicios y Microfrontends

**Estado:** Aceptado
**Decisores:** Arquitectura del proyecto + Desarrollador principal
**Relacionado con:** ADR-0001-Monolito-Modular-Fase1.md (no la reemplaza, la complementa)

## Contexto

`WalrusWallet-Arquitectura-Final.md` (sección 5.2, versión inicial) identificó dos inconsistencias entre las Instrucciones del Proyecto (visión final, sección 3) y el Plan Técnico (fases 1-12):

1. **Servicios mencionados en la visión final sin fase asignada en el Plan Técnico:** Billing, Invoice, Audit, File, Bank, Budget, Scheduler.
2. **Servicios que aparecen en el Plan Técnico fase por fase pero no en la visión final:** Company, Workspace, Membership (Fase 3), Account, Balance, Transaction, Transfer (Fase 4-5), Push (Fase 6), Dashboard (Fase 7).

Al revisar el Plan Técnico con más detalle se encontró un tercer problema, más estructural: varias fases listan **múltiples "servicios" que en realidad son aggregates de un único bounded context** — el mismo patrón que ADR-0001 ya corrigió para Identity en Fase 1 (Identity Service + User Service → un solo módulo Identity). Ejemplos: "Company + Workspace + Membership" (Fase 3), "Wallet + Account + Balance" (Fase 4), "Payment + Transaction + Transfer" (Fase 5), "Notification + Email + Push" (Fase 6), "Reporting + Analytics + Dashboard" (Fase 7).

## Decisión

### 1. Catálogo final: 12 microservicios de dominio + API Gateway

| # | Microservicio | Aggregates/entidades internas | Fase de extracción | Base de datos |
|---|---|---|---|---|
| 1 | Identity | User, integración Keycloak | Fase 2 | PostgreSQL |
| 2 | Company | Company, Workspace, Membership | Fase 3 | PostgreSQL |
| 3 | Wallet | Wallet, Account, Balance, Transaction (ledger), Transfer interno | Fase 4 | PostgreSQL |
| 4 | Payment | Orquestación de pasarelas externas, idempotencia | Fase 5 | PostgreSQL + outbox |
| 5 | Bank | Vinculación bancaria externa / open banking | Fase 5 | PostgreSQL |
| 6 | Billing | Billing, Invoice | Fase 5 | PostgreSQL |
| 7 | Scheduler | Jobs recurrentes (transferencias/facturas programadas) | Fase 5 | Redis (estado) + PostgreSQL (definición) |
| 8 | File | Recibos, comprobantes, exportes | Fase 5 | Object storage + MongoDB (metadata) |
| 9 | Notification | Notification, canal Email, canal Push | Fase 6 | MongoDB |
| 10 | Reporting | Reporting, Analytics | Fase 7 | MongoDB |
| 11 | Budget | Presupuestos y metas definidos por el usuario | Fase 7 | PostgreSQL |
| 12 | Audit | Consumidor cross-cutting de eventos, trail de cumplimiento | Fase 7 | MongoDB |

`Dashboard` deja de existir como microservicio backend: la vista Dashboard la compone el Host del frontend agregando datos de Wallet, Reporting y Notification a través del API Gateway.

### 2. Catálogo final: Host + 4 microfrontends

| Microfrontend | Dominios que consume |
|---|---|
| Host (shell) | Vista Dashboard agregando Wallet, Reporting y Notification (no es un remoto de Module Federation) |
| Finance | Wallet, Bank, Scheduler |
| Billing | Billing, Payment, File |
| Reports | Reporting, Analytics, Budget |
| Administration | Identity, Company, Audit (vista solo-admin), preferencias de Notification |

### 3. Regla aplicada

Un microservicio se define por **bounded context de negocio**, no por cada línea de un backlog o de una fase del Plan Técnico. Si dos o más "servicios" mencionados en cualquier documento cambian siempre juntos y comparten el mismo modelo de datos, son aggregates de un mismo microservicio — no deployables separados. Esta es la misma regla que ADR-0001 aplicó a Identity en Fase 1; ADR-0002 la extiende a todo el catálogo final.

## Consecuencias

**Positivas**
- Las Instrucciones del Proyecto, el Plan Técnico y la Arquitectura Final quedan alineados en una sola lista de servicios, sin duplicidad ni huecos.
- Billing, Audit, File, Bank, Budget y Scheduler — que no tenían fase — ahora tienen una fase de origen concreta (todas caen en Fase 5 o Fase 7, sin necesidad de fases nuevas).
- Se evita crear deployables artificialmente pequeños (ej. un "Transaction Service" separado de Wallet) que solo añadirían complejidad de red y consistencia distribuida sin aportar un límite de dominio real.
- Microfrontends y microservicios quedan desacoplados de forma consciente: no hay relación 1:1, cada MFE consume varios servicios según lo que el usuario necesita ver, no según cómo está partido el backend.

**Trade-offs**
- Bank queda separado de Payment como decisión de diseño defendible pero discutible — en un dominio fintech más pequeño podría justificarse fusionarlos. Si en la práctica de Fase 5 se ve que ambos cambian siempre juntos, se documenta como excepción y se fusionan con un ADR de supplement.
- Scheduler se trata como microservicio propio en lugar de una librería de scheduling embebida en Payment/Billing. Esto es una elección pedagógica (practicar locks distribuidos) más que una necesidad estricta de dominio.
- Audit y Budget comparten el mismo patrón arquitectónico que Reporting (consumidor de eventos → read model propio), así que se agrupan en la misma fase (7) aunque su dominio de negocio sea distinto entre sí.

## Alternativas consideradas

- **Mantener un microservicio por cada línea del Plan Técnico (Identity Service, User Service, Company Service, Workspace Service, Membership Service, etc., por separado):** rechazada. Multiplicaría los deployables sin que exista un límite de dominio real entre ellos, contradiciendo el principio de extracción progresiva de ADR-0001.
- **Dejar a Billing, Audit, File, Bank, Budget y Scheduler sin fase asignada hasta que "surja la necesidad":** rechazada. Generaría el mismo tipo de incoherencia entre documentos que esta decisión busca cerrar.
- **Crear fases nuevas (13, 14...) para Billing/Audit/File/Bank/Budget/Scheduler:** rechazada por ahora. Todas encajan razonablemente en el alcance ya descrito de Fase 5 (Payments & Billing) o Fase 7 (Reports), sin necesidad de extender la duración total del plan.

## Pendiente para revisión futura

- ADR-0001 deja abierta la pregunta de **cuál microservicio se extrae primero en la práctica** (no necesariamente Identity, aunque sea el primero en el orden de fases). Este ADR define el catálogo final (el "qué"), pero no resuelve el orden real de extracción (el "cuándo") — eso se decide al llegar a Fase 2 con un ADR propio de extracción.
- Si al construir Fase 5 se confirma que Bank y Payment siempre cambian juntos en la práctica, se debe registrar un ADR de supplement fusionándolos, en lugar de editar este documento.
- Si Scheduler resulta sobre-dimensionado como microservicio independiente durante Fase 5, se puede degradar a librería embebida sin que esto rompa el resto del catálogo — es el aggregate con menor acoplamiento de toda la lista.
