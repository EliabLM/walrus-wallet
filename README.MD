# 🦭 WalrusWallet

> Plataforma fintech SaaS como laboratorio de React, .NET, Clean Architecture, microservicios, microfrontends, Docker y despliegue real.

<!-- [![Estado](https://img.shields.io/badge/estado-Fase%201%20%E2%80%94%20Foundation-blue)]()
[![Fase actual](https://img.shields.io/badge/m%C3%B3dulo-00%20%E2%80%94%20Fundaci%C3%B3n%20del%20monorepo-lightgrey)]()
[![Licencia](https://img.shields.io/badge/licencia-MIT-green)]() -->

---

## 📌 Qué es WalrusWallet

WalrusWallet es una plataforma fintech para gestionar **identidad, cuentas, movimientos, notificaciones y reportes financieros**.

No es un proyecto de negocio en producción: es un **proyecto de portafolio e ingeniería**, construido módulo a módulo, que busca demostrar el ciclo completo de un producto real — desde la fundación del repositorio hasta microservicios, microfrontends, observabilidad y despliegue en producción.

---

## 🧭 Filosofía de evolución arquitectónica

Este proyecto **no empieza con microservicios**. Evoluciona como lo haría un equipo real: primero valida el dominio sobre una base simple, y solo después fragmenta la arquitectura.

```text
Monolito Modular → Clean Architecture → CQRS → Eventos de dominio →
RabbitMQ → Extracción de microservicio #1 → ... → Module Federation →
Docker → Docker Compose → Kubernetes → CI/CD → Producción → Observabilidad
```

Este criterio se documenta formalmente en [`docs/adr/ADR-0001-Monolito-Modular-Fase1.md`](docs/adr/ADR-0001-Monolito-Modular-Fase1.md) y prevalece sobre cualquier plan que proponga dividir el sistema antes de tiempo.

La **arquitectura final** está descrita en [`docs/architecture/`](docs/architecture/).

---

## 🚧 Estado actual del proyecto

| Campo             | Valor                                                         |
| ----------------- | ------------------------------------------------------------- |
| Fase              | **Fase 1 — Foundation** (semanas 1-4 del Plan Técnico)        |
| Módulo en curso   | **Módulo 00 — Fundación del monorepo**                        |
| Código de negocio | Aún no existe — este módulo es fundación pura                 |
| Próximo módulo    | Módulo 01 — Backend Foundation (esqueleto Clean Architecture) |

> En esta etapa el repositorio tiene estructura, documentación y convenciones, pero todavía no hay backend, frontend ni infraestructura funcionando. Esa es una decisión deliberada.

---

## 🏗️ Arquitectura de Fase 1 (monolito modular)

Fase 1 construye los cimientos de la plataforma como **un único deployable de backend** y **una única SPA de frontend**, sin fragmentar todavía en microservicios ni microfrontends.

```text
Browser
  └── React App (SPA única)
       ├── features/auth
       └── features/dashboard (placeholder)

NGINX
  ├── / → React App
  └── /api → Backend

Backend .NET (monolito modular, un solo deployable)
  ├── Api
  ├── Application/Identity
  ├── Domain/Identity
  ├── Infrastructure/Identity
  └── SharedKernel

Infraestructura
  ├── PostgreSQL · MongoDB · Redis
  ├── RabbitMQ (conectividad probada, sin lógica de negocio aún)
  └── Keycloak
```

Detalle completo en [`docs/architecture/`](docs/architecture/) y en la especificación de Fase 1.

---

## 🧱 Stack tecnológico

| Capa                | Tecnología                                                                                     |
| ------------------- | ---------------------------------------------------------------------------------------------- |
| **Frontend**        | React, TypeScript, Vite, React Router, TanStack Query, Zustand, React Hook Form, Zod, Tailwind |
| **Backend**         | .NET, Clean Architecture, Minimal APIs                                                         |
| **Autenticación**   | Keycloak (OIDC / JWT)                                                                          |
| **Bases de datos**  | PostgreSQL (transaccional) · MongoDB (documental/auditoría) · Redis (cache, idempotencia)      |
| **Mensajería**      | RabbitMQ                                                                                       |
| **Infraestructura** | Docker, Docker Compose, NGINX                                                                  |
| **Testing**         | xUnit, FluentAssertions, Testcontainers, Vitest, React Testing Library, Playwright             |
| **CI/CD**           | GitHub Actions                                                                                 |
| **Despliegue**      | VPS económico → k3s/Kubernetes cuando la base sea estable                                      |

---

## 📂 Estructura del repositorio

```text
WalrusWallet/
├── backend/
│   └── src/                     # vacío en este módulo — lo llena Módulo 01
├── frontend/
│   └── web/                     # vacío en este módulo — lo llena Semana 4
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
│   │   └── ADR-0001-Monolito-Modular-Fase1.md
│   ├── roadmap/
│   └── modules/
├── scripts/
├── .github/
│   ├── ISSUE_TEMPLATE/
│   ├── PULL_REQUEST_TEMPLATE.md
│   └── workflows/
├── docker-compose.yml            # placeholder — se llena en Semana 3
├── docker-compose.prod.yml
├── .env.example
└── README.md
```

---

## 🚀 Cómo empezar

> ⚠️ **Aún no hay nada que levantar.** El repositorio está en etapa de fundación (Módulo 00). Esta sección se completará en el Módulo 01 (backend) y en la Semana 3 (Docker Compose).

Cuando la base técnica esté lista, el objetivo es que el proyecto completo se levante con un solo comando:

```bash
docker compose up
```

### Prerrequisitos (previstos)

- Docker y Docker Compose
- .NET SDK
- Node.js + npm/pnpm
- Git

---

## 🗺️ Roadmap

El proyecto se construye en 12 fases, desde la fundación hasta producción y observabilidad. Resumen:

| Fase | Nombre                        | Semanas |
| ---- | ----------------------------- | ------- |
| 1    | Foundation (monolito modular) | 1-4     |
| 2    | Identity                      | 5-7     |
| 3    | Companies                     | 8-10    |
| 4    | Wallet                        | 11-14   |
| 5    | Payments                      | 15-18   |
| 6    | Notifications                 | 19-21   |
| 7    | Reports                       | 22-24   |
| 8    | API Gateway                   | 25-26   |
| 9    | Observabilidad                | 27-30   |
| 10   | Kubernetes                    | 31-35   |
| 11   | DevOps                        | 36-38   |
| 12   | Producción                    | 39-40   |

Detalle completo en [`docs/roadmap/WalrusWallet-Plan-Tecnico.md`](docs/roadmap/WalrusWallet-Plan-Tecnico.md).

---

## 📚 Documentación

| Documento                                                                                    | Contenido                                                             |
| -------------------------------------------------------------------------------------------- | --------------------------------------------------------------------- |
| [`docs/architecture/vision.md`](docs/architecture/vision.md)                                 | Qué es WalrusWallet y qué problema resuelve                           |
| [`docs/architecture/`](docs/architecture/)                                                   | Arquitectura de Fase 1 y arquitectura final de destino                |
| [`docs/adr/ADR-0001-Monolito-Modular-Fase1.md`](docs/adr/ADR-0001-Monolito-Modular-Fase1.md) | Por qué Fase 1 es monolito modular y no microservicios desde el día 1 |
| [`docs/roadmap/WalrusWallet-Plan-Tecnico.md`](docs/roadmap/WalrusWallet-Plan-Tecnico.md)     | Las 12 fases del Plan Técnico                                         |

---

## 🤝 Convenciones de contribución

- **Commits:** [Conventional Commits](https://www.conventionalcommits.org/) — `feat(identity): add user profile entity`
- **Ramas:** GitFlow — `main` (desplegable), `develop` (integración), `feature/*`, `release/*`, `hotfix/*`
- **Proceso:** cada módulo se diseña, se revisa y se aprueba antes de escribir código (ver dinámica de trabajo del proyecto)

---

## 📄 Licencia

Este proyecto usa licencia **MIT** — ver [`LICENSE`](LICENSE).

---
