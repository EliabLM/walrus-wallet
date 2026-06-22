# WalrusWallet — Módulo 00: Fundación del Monorepo

**Fase:** 1 — Foundation
**Semana (Plan Técnico):** 1
**Estado:** Propuesto — pendiente de tu revisión de diseño
**Precede a:** Módulo 01 — Backend Foundation (esqueleto Clean Architecture, Semana 2)

---

## 0. Por qué este es el siguiente paso

ADR-0001 ya resolvió el conflicto entre el documento original y el principio rector del proyecto: Fase 1 es monolito modular, una sola SPA, sin Identity Service ni Module Federation todavía. La Especificación Revisada ya es coherente con eso.

El Plan Técnico, Semana 1, es la primera tarea ejecutable: crear el repositorio profesional. Es fundación pura — al cerrar este módulo **todavía no existe código de negocio**, ni siquiera esqueleto de backend o frontend. Eso es el Módulo 01.

---

## 1. Especificación funcional y técnica

### 1.1 Objetivo del módulo

Dejar un repositorio que se vea y se comporte como el de una empresa real — estructura, documentación, convenciones y configuración de GitHub — listo para que el Módulo 01 empiece a construir sobre una base ordenada.

### 1.2 Incluye

- Estructura de carpetas del monorepo (backend, frontend, infra, docs, tests, .github, scripts).
- Documentación base (README, Vision, Roadmap, Architecture, Contributing, Glossary, ADR).
- Archivos de configuración raíz (LICENSE, .gitignore, .editorconfig, .env.example).
- Convención de commits (Conventional Commits) y de ramas (GitFlow).
- Configuración de GitHub: Labels, Milestones, Issues, Projects.

### 1.3 No incluye (se queda para módulos posteriores)

- Código de backend o frontend (Módulo 01 / Semana 2 y 4).
- Docker Compose con servicios reales (Semana 3).
- Cualquier lógica de Identity (Fase 2).
- CI/CD funcional — en este módulo solo se *configura* el repo para soportarlo; el pipeline real llega con el primer build (Semana 2 en adelante).

### 1.4 Estructura del monorepo (autoritativa)

Reconcilio aquí la estructura de alto nivel del Plan Técnico con el detalle ya definido en la Especificación Revisada (sección 5), que es la versión vigente tras ADR-0001:

```text
WalrusWallet/
├── backend/
│   └── src/                    ← vacío en este módulo, lo llena Módulo 01
├── frontend/
│   └── web/                    ← vacío en este módulo, lo llena Semana 4
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
│   │   └── vision.md
│   ├── adr/
│   │   └── ADR-0001-Monolito-Modular-Fase1.md
│   └── roadmap/
│       └── roadmap.md
├── scripts/
│   └── .gitkeep                ← placeholder, se llena cuando haya algo que automatizar
├── .github/
│   ├── ISSUE_TEMPLATE/
│   ├── PULL_REQUEST_TEMPLATE.md
│   └── workflows/               ← vacío en este módulo, primer workflow real en Módulo 01
├── docker-compose.yml           ← placeholder vacío, Semana 3 lo llena
├── docker-compose.prod.yml      ← placeholder vacío
├── .env.example
├── .gitignore
├── .editorconfig
├── LICENSE
├── CONTRIBUTING.md
├── GLOSSARY.md
└── README.md
```

> Nota: `scripts/` (mencionado en el Plan Técnico) y la organización `docs/adr/` (de la Especificación Revisada) conviven sin conflicto; no hay decisión arquitectónica que reconciliar entre ambos documentos en este punto, solo nivel de detalle.

### 1.5 Archivos base de configuración

| Archivo | Propósito |
|---|---|
| `README.md` | Qué es WalrusWallet, cómo se levanta (cuando exista algo que levantar), enlace a `docs/` |
| `LICENSE` | Recomendado MIT para visibilidad de portafolio (puedes optar por “All rights reserved” si prefieres no liberar el código) |
| `.gitignore` | Reglas para .NET + Node (bin/, obj/, node_modules/, .env, etc.) |
| `.editorconfig` | Reglas de formato consistentes entre C# y TypeScript |
| `.env.example` | Variables que existirán (placeholders, sin valores reales) — se completa en Semana 3 |

### 1.6 Documentación base (`docs/`)

- **Vision** (`docs/architecture/vision.md`): qué es WalrusWallet, a quién sirve (estudio de portafolio), qué problema de dominio fintech resuelve.
- **Roadmap** (`docs/roadmap/roadmap.md`): las 12 fases del Plan Técnico, resumidas, con el estado actual marcado en Fase 1 / Semana 1.
- **Architecture**: por ahora un README corto en `docs/architecture/` que enlaza a ADR-0001 y dice explícitamente “diagramas C4 detallados llegan en Módulo 01, cuando exista un sistema que diagramar”.
- **Contributing** (`CONTRIBUTING.md`): convención de commits y de ramas (ver 1.7).
- **Glossary** (`GLOSSARY.md`): términos de dominio (UserProfile, AuditEvent, etc. — se amplía módulo a módulo).
- **ADR**: `ADR-0001-Monolito-Modular-Fase1.md` ya escrito; este módulo solo lo ubica en `docs/adr/`.

### 1.7 Convenciones de Git

**Conventional Commits:**

```text
feat(identity): add user profile entity
fix(infra): correct postgres healthcheck path
docs(adr): add ADR-0001
chore(repo): initial monorepo structure
```

**GitFlow (ramas):**

```text
main        → siempre desplegable
develop     → integración
feature/*   → una rama por tarea (feature/repo-structure)
release/*   → se abre al cerrar una fase
hotfix/*    → corrección urgente sobre main
```

### 1.8 Configuración de GitHub

- **Labels** sugeridos: `phase-1`, `foundation`, `backend`, `frontend`, `infra`, `docs`, `bug`, `tech-debt`.
- **Milestones**: uno por fase (`Fase 1 — Foundation`, `Fase 2 — Identity`, …), tomados del Plan Técnico.
- **Project board** (columnas): `Backlog` → `Ready` → `In Progress` → `Review` → `Done`.

---

## 2. Diagrama

No hay sistema todavía, así que no aplica un diagrama C4 — eso empieza a tener sentido en el Módulo 01, cuando exista al menos un backend desplegable. Lo único representable en este módulo es el flujo de ramas:

```text
main ────────────────────────────────●──────────────▶
                                      ▲
develop ───●────●────●────●──────────┤
            \    \    \    \         │
             \    \    \    \        │
feature/*  ───●    ●    ●    ●       │
   (repo-structure, docs-base, github-config, ...)
                                      │
                              release/fase-1
```

---

## 3. Criterios de aceptación (Definition of Done de este módulo)

- [ ] El repositorio existe y tiene exactamente la estructura de carpetas de la sección 1.4.
- [ ] `README.md` explica el proyecto y enlaza a `docs/`.
- [ ] `docs/adr/ADR-0001-Monolito-Modular-Fase1.md` está presente y es accesible desde el README o desde `docs/architecture/`.
- [ ] `docs/roadmap/roadmap.md` refleja las 12 fases y marca Fase 1 / Semana 1 como “en curso”.
- [ ] `LICENSE`, `.gitignore`, `.editorconfig`, `.env.example` existen en la raíz.
- [ ] `CONTRIBUTING.md` documenta Conventional Commits y GitFlow tal como en 1.7.
- [ ] Ramas `main` y `develop` creadas; al menos una `feature/*` usada para construir este módulo (practicando el flujo desde ya).
- [ ] Labels, un Milestone (“Fase 1 — Foundation”) y un Project board configurados en GitHub.
- [ ] El repo no contiene código de backend ni frontend más allá de carpetas vacías con `.gitkeep` donde aplique.
- [ ] Puedes explicar, en una frase, por qué este módulo no incluye Docker Compose funcional todavía (eso es Semana 3, no este módulo).

---

## 4. Retos opcionales

- Configurar **commitlint** + un hook de pre-commit (Husky o equivalente) que rechace commits que no sigan Conventional Commits.
- Crear plantillas reales en `.github/ISSUE_TEMPLATE/` (bug report, feature request) y un `PULL_REQUEST_TEMPLATE.md` con checklist de DoD.
- Agregar un archivo `CODEOWNERS` (aunque seas el único desarrollador, practica el patrón).
- Dejar un borrador de `docs/architecture/c4-context.md` con el diagrama de Contexto C4 *vacío pero con el template listo*, para llenarlo en el Módulo 01.

---

## 5. Qué sigue después de este módulo

**Módulo 01 — Backend Foundation** (Semana 2 del Plan Técnico): crear `WalrusWallet.sln` con los proyectos Domain / Application / Infrastructure / Api / SharedKernel (vacíos de lógica de Identity todavía), DI, logging, `/health` y `/version`, Swagger. Sin Keycloak ni base de datos real aún — eso es Semana 3.

---

## 6. Pendiente de tu aprobación

No avanzamos a Módulo 01 hasta cerrar la revisión de este módulo. Si algo de la estructura, las convenciones o los criterios de aceptación no te convence, lo ajustamos ahora — es más barato cambiarlo aquí que después de tener carpetas y commits reales.
