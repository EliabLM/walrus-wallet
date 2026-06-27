# WalrusWallet — Especificación de Fase 1 (Revisada)

## Monorepo + Base de Arquitectura para Microservicios, Microfrontends y DevOps

> Proyecto fintech de estudio para practicar React, .NET, Clean Architecture, microservicios, microfrontends, Docker y despliegue en VPS de bajo costo.

### Nota de coherencia

Esta versión incorpora las decisiones de **ADR-0001 — Fase 1 como Monolito Modular**. Fase 1 no construye servicios ni frontends separados todavía: construye un monolito modular bien hecho, con costuras internas limpias, listo para evolucionar. Ver `ADR-0001-Monolito-Modular-Fase1.md` para el detalle de cada decisión y su justificación.

---

## 1. Opinión sobre el documento original

El documento adjunto está bien encaminado y tiene una base sólida: define un dominio fintech creíble, usa microservicios, microfrontends, RabbitMQ, PostgreSQL, MongoDB, Redis, Docker y CI/CD. También tiene una estructura bastante profesional y ya piensa en despliegue real.

Mi ajuste principal es este:

- **Está demasiado avanzado para arrancar como Fase 1.**
- Mezcla la arquitectura final con la arquitectura inicial.
- Introduce demasiado pronto decisiones costosas de operación.
- Aún no separa con suficiente claridad lo que es **fundación** de lo que es **negocio**.

Para que el proyecto sirva realmente como portafolio y aprendizaje, la Fase 1 debe construir los cimientos: monorepo, autenticación, una SPA con su feature de login, un backend modular, Docker Compose, pruebas y despliegue mínimo. El negocio completo —y la fragmentación en microservicios y microfrontends— vendrá después, cuando el dominio esté validado (ver ADR-0001).

---

## 2. Nombre y objetivo del proyecto

**WalrusWallet**
Plataforma fintech SaaS para gestionar identidad, cuentas, movimientos, notificaciones y reportes financieros.

### Objetivo de la Fase 1

Construir la base técnica y de arquitectura del producto como un **monolito modular**, para que luego pueda evolucionar a microservicios y microfrontends reales sin rehacer la estructura.

En esta fase el foco no es el negocio, sino la plataforma.

---

## 3. Alcance real de la Fase 1

### Sí incluye

- Monorepo único.
- Una sola aplicación React (SPA), organizada por features.
- Backend .NET con Clean Architecture, en un único deployable.
- Keycloak para autenticación.
- Docker y Docker Compose.
- PostgreSQL, MongoDB y Redis levantados localmente.
- RabbitMQ levantado y con conectividad probada (sin lógica de negocio aún).
- NGINX como reverse proxy.
- Testing base: unit, integration y e2e.
- Pipeline inicial en GitHub Actions.
- Preparación para despliegue en VPS.

### No incluye aún

- Transferencias reales.
- Facturación.
- Pagos.
- Conciliaciones.
- Reportes avanzados.
- Kubernetes.
- Observabilidad avanzada.
- Escalado horizontal.
- **Microservicios independientes** — todo vive en un solo deployable (ver ADR-0001).
- **Module Federation / microfrontends** — toda la UI vive en una sola SPA (ver ADR-0001).
- Flujos de negocio sobre RabbitMQ (eventos de dominio reales).

---

## 4. Arquitectura recomendada para la Fase 1

### Enfoque

La Fase 1 es un **monolito modular** con dos capas principales:

1. **Una SPA React** con un feature de autenticación y un dashboard placeholder.
2. **Un backend con un único deployable**, organizado internamente por módulo de dominio (Identity), con contratos listos para crecer.

### Arquitectura lógica

```text
Browser
  └── React App (SPA)
       ├── features/auth
       └── features/dashboard (placeholder)

NGINX
  ├── / → React App
  └── /api → Backend

Backend .NET (monolito modular, un solo deployable)
  ├── Api
  ├── Application
  │    └── Identity/
  ├── Domain
  │    └── Identity/
  ├── Infrastructure
  │    └── Identity/
  └── SharedKernel

Infraestructura
  ├── PostgreSQL
  ├── MongoDB
  ├── Redis
  ├── RabbitMQ
  └── Keycloak
```

---

## 5. Monorepo

### Estructura propuesta

```text
WalrusWallet/
├── backend/
│   └── src/
│       ├── WalrusWallet.Api/
│       ├── WalrusWallet.Application/
│       ├── WalrusWallet.Domain/
│       ├── WalrusWallet.Infrastructure/
│       └── WalrusWallet.SharedKernel/
├── frontend/
│   └── web/
│       └── src/
│           ├── app/            # layout, router, providers
│           ├── features/
│           │   ├── auth/
│           │   └── dashboard/
│           └── shared/
├── tests/
│   ├── backend/
│   ├── frontend/
│   └── e2e/
├── infra/
│   ├── nginx/
│   ├── keycloak/
│   ├── postgres/
│   ├── mongodb/
│   ├── redis/
│   └── rabbitmq/
├── docs/
│   ├── architecture/
│   ├── adr/
│   └── roadmap/
├── .github/
│   └── workflows/
├── docker-compose.yml
├── docker-compose.prod.yml
├── .env.example
└── README.md
```

### Por qué así

- Un solo backend y un solo frontend: nada que fragmentar todavía.
- Te permite trabajar como un solo desarrollador sin complicarte con múltiples repos ni múltiples deployables.
- Es más realista para un proyecto de portafolio en etapa inicial.
- Hace más fácil CI/CD y despliegue mientras el dominio se valida.

---

## 6. Frontend de Fase 1

### Aplicación (SPA)

Una sola aplicación React. No hay host ni remotos: la carpeta `app/` cumple el rol de composición (layout, router, providers), pero **no es un Module Federation host**.

Responsabilidades:

- layout principal
- navegación global
- manejo de sesión
- rutas protegidas
- integración con Keycloak

### Feature: Auth

Debe contener:

- login
- registro
- recuperación de sesión
- cierre de sesión

### Feature: Dashboard

En Fase 1 será un dashboard básico de bienvenida, con:

- nombre del usuario
- estado de autenticación
- acceso a zonas futuras
- tarjetas placeholder para los módulos de negocio

### Recomendación técnica

- React
- TypeScript
- Vite
- React Router
- TanStack Query
- React Hook Form
- Zod
- Tailwind

> Module Federation se introduce en una fase posterior, cuando exista un primer microservicio extraído (ver ADR-0001).

---

## 7. Backend de Fase 1

### Recomendación profesional

Un único deployable, organizado por módulo de dominio dentro de las capas Clean Architecture. **No** se construye un "Identity Service" como solución aparte (ver ADR-0001).

### Módulo inicial: Identity

Responsabilidades:

- integración con Keycloak
- perfil de usuario interno
- vínculo entre usuario autenticado y entidad de negocio
- health checks
- endpoints base para el frontend

### Estructura Clean Architecture (un solo deployable)

```text
WalrusWallet (solución única)
├── Api                  ← único punto de despliegue
├── Application
│   └── Identity/
├── Domain
│   └── Identity/
├── Infrastructure
│   └── Identity/
├── SharedKernel
└── Tests
```

### Entidades iniciales (Fase 1)

- UserProfile
- AuditEvent

> `Organization` y `Workspace` se posponen a la fase de Companies, donde son necesarios y donde tienen sentido sus reglas de negocio.

### Base de datos

- **PostgreSQL** como base principal para datos relacionales
- **MongoDB** solo para auditoría documental o logs de negocio
- **Redis** para cache, session hints e idempotencia futura

---

## 8. Autenticación

### Elección

**Keycloak**

### En Fase 1 debe cubrir

- login
- logout
- refresh token
- roles base
- claims
- usuarios de prueba
- realm exportable para levantar en Docker

### Flujo

```text
React App (feature auth)
  → Keycloak
  → JWT
  → Backend .NET (vía interceptor HTTP)
```

### Recomendación

No reinventar autenticación con JWT casero. Para portafolio, Keycloak suma más valor y deja el backend enfocado en integración empresarial real.

---

## 9. Infraestructura local

### Servicios en Docker Compose

- nginx
- keycloak
- postgres
- mongodb
- redis
- rabbitmq
- api (backend, monolito modular)
- web (frontend, SPA)

### Meta de Fase 1

Levantar toda la plataforma con un solo comando:

```bash
docker compose up
```

---

## 10. RabbitMQ en Fase 1

RabbitMQ se levanta para practicar lo operacional, **no** para mensajería de negocio todavía (ver ADR-0001).

### Caso base en Fase 1

- crear un exchange y una queue de prueba
- publicar y consumir un mensaje simple, sin lógica de dominio detrás

### Objetivo

Aprender:

- exchange
- queue
- routing key
- producer
- consumer

El primer **evento de dominio real** (por ejemplo, `UserRegistered` con su consumidor de negocio) se construye en la fase donde exista CQRS y un caso que lo justifique — no antes.

---

## 11. Redis en Fase 1

Redis se usará para:

- cache simple de consultas frecuentes
- pruebas de integración con almacenamiento rápido
- base para rate limiting o idempotencia futura

---

## 12. MongoDB en Fase 1

MongoDB se usará solo para:

- auditoría documental
- eventos de negocio
- logs estructurados de una entidad

No debe ser una dependencia central de la fase 1.

---

## 13. Testing

### Backend

- Unit tests
- Integration tests con base real en Docker

### Frontend

- Unit tests de componentes
- E2E para login y navegación

### Herramientas recomendadas

- xUnit
- Shouldly
- Testcontainers
- Vitest
- React Testing Library
- Playwright

---

## 14. DevOps de Fase 1

### Lo que sí debes practicar desde el inicio

- Dockerfile multi-stage
- Docker Compose
- variables de entorno
- healthchecks
- NGINX reverse proxy
- GitHub Actions
- build y test automáticos

### Pipeline mínimo

```text
Push
  → Restore
  → Build
  → Unit Tests
  → Integration Tests
  → Build Docker Images
  → E2E Smoke Test
```

---

## 15. VPS y despliegue

### Recomendación para tu caso

Dado el presupuesto de estudio, la mejor ruta es:

1. Desarrollo local con Docker Compose.
2. Despliegue inicial en un VPS económico.
3. Kubernetes después, cuando la base esté estable.

### Por qué

- con 5 USD/mes no conviene meter demasiada complejidad operacional desde el día uno
- primero aprendes a operar contenedores y redes
- después migras a k3s o a Kubernetes cuando ya tenga sentido

### Producción mínima

- NGINX
- Docker
- Docker Compose
- certificados SSL
- dominio propio
- reinicio automático de contenedores
- backups de datos

---

## 16. Roadmap de Fase 1

### Semana 1

- crear el monorepo
- definir documentación base
- levantar infraestructura local con Docker Compose
- configurar NGINX y Keycloak
- crear la app React base y el backend base (un solo deployable cada uno)

### Semana 2

- construir el módulo Identity dentro del backend
- conectar React con Keycloak
- guardar perfil interno de usuario
- implementar tests base

### Semana 3

- verificar conectividad de RabbitMQ (exchange/queue de prueba, sin lógica de negocio)
- integrar Redis
- conectar MongoDB para auditoría
- completar dashboard placeholder

### Semana 4

- endurecer Dockerfiles
- agregar GitHub Actions
- pulir README y diagramas
- dejar listo el despliegue en VPS

---

## 17. Definition of Done

La Fase 1 termina cuando puedas:

- clonar el repositorio
- ejecutar `docker compose up`
- iniciar sesión con Keycloak
- navegar por la aplicación React
- ver el dashboard base
- ejecutar tests sin errores
- levantar la plataforma completa en local sin instalar dependencias manuales adicionales

---

## 18. Decisiones importantes que sí justifican el portafolio

- Monorepo para máxima productividad.
- Monolito modular con límites de dominio limpios, listo para evolucionar a microservicios (ver ADR-0001).
- Keycloak para autenticación realista.
- Clean Architecture en el backend.
- Docker Compose como base operacional.
- VPS barato para producción inicial.
- RabbitMQ y Redis disponibles desde la fundación, sin forzar su uso de negocio antes de tiempo.

---

## 19. Lo que yo cambiaría del documento original

### Mantendría

- el dominio fintech
- PostgreSQL, MongoDB, Redis y RabbitMQ
- Keycloak
- React
- Docker y GitHub Actions
- la intención de operar en entorno real

### Reescribiría

- la división de microservicios desde el inicio
- la división de microfrontends desde el inicio (Module Federation)
- la parte de negocio avanzada
- la dependencia de OCI como solución principal
- el enfoque de múltiples bases por servicio desde la fase 1
- el tamaño del roadmap inicial

---

## 20. Siguiente paso recomendado

La Fase 1 ya está definida a nivel conceptual y ahora también coherente con ADR-0001.
El siguiente paso correcto es construir según el backlog ejecutable revisado, en el orden propuesto.

---

## 21. Resumen final

El documento original es bueno, pero estaba diseñado como arquitectura final. La versión adaptada aquí convierte esa idea en una ruta más inteligente para aprender y construir portafolio: primero un monolito modular sólido, luego la expansión a microservicios y microfrontends reales, cuando el dominio lo justifique.
