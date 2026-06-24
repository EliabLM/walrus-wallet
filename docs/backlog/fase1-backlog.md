# WalrusWallet — Backlog Fase 1 (Foundation)

| Campo | Valor |
|---|---|
| Fase | Fase 1 — Foundation |
| Semanas | 1-4 (según `WalrusWallet-Plan-Tecnico.md`) |
| Mecanismo | Monolito modular, un solo deployable backend, una sola SPA frontend |
| Documentos relacionados | `ADR-0001-Monolito-Modular-Fase1.md`, `ADR-0002-Catalogo-Final-Microservicios-Microfrontends.md`, `WalrusWallet-Plan-Tecnico.md` |
| Ubicación recomendada | `docs/backlog/WalrusWallet-Fase1-Backlog.md` |

## 0. Alcance de este documento

Este backlog cubre **únicamente Fase 1** (Semanas 1-4 del Plan Técnico). No incluye Identity como servicio extraído, Module Federation, ni mensajería de negocio sobre RabbitMQ — esas piezas pertenecen a fases posteriores y construirlas ahora contradiría `ADR-0001`.

Cada épica corresponde a una semana del Plan Técnico, salvo la **Épica 5**, que es transversal: nace de la decisión 4 de `ADR-0001` (modelo de dominio de Identity limitado a `UserProfile` y `AuditEvent` en Fase 1) y aporta el soporte mínimo que necesita el login de la Épica 4.

## 1. Resumen de épicas

| Épica | Título | Semana | Historias |
|---|---|---|---|
| EPIC-01 | Fundación del Monorepo | 1 | US-01 a US-04 |
| EPIC-02 | Backend Foundation | 2 | US-05 a US-07 |
| EPIC-03 | Infraestructura Docker | 3 | US-08 a US-10 |
| EPIC-04 | Frontend Foundation (SPA única) | 4 | US-11, US-12 |
| EPIC-05 | Identity mínimo en el monolito | Transversal (2 y 4) | US-13, US-14 |

## 2. Convenciones de trabajo en GitHub

- Cada **Épica** se crea como un Issue con label `epic` y un checklist con sus historias de usuario.
- Cada **Historia de Usuario** se crea como un Issue independiente, con label `user-story`, vinculado al Milestone **"Fase 1 — Foundation"**.
- Cada Issue de historia se trabaja en su propia rama `feature/<numero-de-issue>-<slug>`, creada desde `develop`.
- El Pull Request de cada historia incluye `Closes #<numero-de-issue>` para cerrar el issue automáticamente al hacer merge.
- Las historias se trabajan **secuencialmente dentro de cada épica** para evitar conflictos estructurales (ej. no tiene sentido dockerizar el backend antes de que exista la solución .NET).

---

## EPIC-01 — Fundación del Monorepo

**Semana:** 1
**Objetivo:** que el repositorio se vea y se comporte como el de un equipo profesional, aunque todavía no exista funcionalidad de negocio.
**Aprenderás (Plan Técnico):** GitFlow, Monorepo, Convenciones, ADR, Documentación, Arquitectura C4.

### US-01 — Estructura del monorepo

**Como** desarrollador principal, **quiero** una estructura de carpetas estandarizada en el monorepo, **para** tener un punto de partida claro para backend, frontend, infraestructura y documentación.

**Criterios de aceptación:**
- [ ] Existen en la raíz las carpetas `backend/`, `frontend/`, `infra/`, `docs/`, `.github/`, `scripts/`.
- [ ] `docs/` contiene las subcarpetas `adr/`, `architecture/`, `roadmap/`, `backlog/`, `specs/`.
- [ ] Toda carpeta vacía que debe quedar trackeada en git contiene un `.gitkeep` (se elimina cuando llegue contenido real).
- [ ] El repositorio sigue GitFlow: existen las ramas `main` y `develop`; `develop` es la rama base para nuevas features.
- [ ] Existe un `README.md` placeholder en la raíz (se completa en US-02).

**Definition of Done:** la estructura de carpetas está commiteada en `develop` y documentada en `Contributing.md`.

---

### US-02 — Documentación base del proyecto

**Como** desarrollador principal, **quiero** documentación fundacional en español, **para** que cualquier persona —incluyéndome a futuro— entienda visión, alcance y arquitectura sin depender de memoria.

**Criterios de aceptación:**
- [ ] `README.md` describe propósito del proyecto, stack tecnológico objetivo y cómo levantar el entorno de Fase 1 (sin prometer funcionalidad de fases futuras).
- [ ] `docs/architecture/Vision.md` resume la visión de producto y referencia `arquitectura-final.md` como visión de destino, no como punto de partida.
- [ ] `docs/roadmap/Roadmap.md` resume las 12 fases del Plan Técnico (tabla con semanas y mecanismo: construye-en-monolito vs. extrae-microservicio).
- [ ] `docs/Glossary.md` define términos clave: monolito modular, bounded context, CQRS, Outbox/Inbox, idempotencia, Module Federation.
- [ ] `docs/Contributing.md` documenta GitFlow, Conventional Commits y el flujo de ramas por issue.
- [ ] `ADR-0001` y `ADR-0002` están ubicados en `docs/adr/`.

**Definition of Done:** un desarrollador nuevo puede entender el proyecto leyendo solo `docs/`, sin necesitar contexto adicional.

---

### US-03 — Configuración de convenciones y herramientas de colaboración

**Como** desarrollador principal, **quiero** GitFlow, Conventional Commits y GitHub Projects configurados, **para** que el repositorio se comporte como el de un equipo profesional desde el primer commit.

**Criterios de aceptación:**
- [ ] Tablero de GitHub Projects creado con columnas: Backlog, To Do, In Progress, In Review, Done.
- [ ] Labels creados: `epic`, `user-story`, `bug`, `docs`, `chore`, `ci`.
- [ ] Milestone **"Fase 1 — Foundation"** creado, con las 14 historias de este backlog asociadas.
- [ ] Conventional Commits aplicado desde el primer commit del repositorio (`feat`, `fix`, `docs`, `chore`, `refactor`, `test`, `ci`).
- [ ] Convención de branch naming documentada y aplicada: `feature/<numero-de-issue>-<slug>` desde `develop`.

**Definition of Done:** la primera historia de usuario (US-01) ya se cerró siguiendo esta convención de punta a punta.

---

### US-04 — Archivos base de configuración

**Como** desarrollador principal, **quiero** archivos de configuración base, **para** evitar artefactos basura en el control de versiones y tener una plantilla de variables de entorno desde el día uno.

**Criterios de aceptación:**
- [ ] `.gitignore` cubre artefactos de build de .NET (`bin/`, `obj/`) y de Node/React (`node_modules/`, `dist/`, `build/`).
- [ ] `.editorconfig` define reglas básicas de formato (indentación, encoding, EOL) consistentes entre backend y frontend.
- [ ] `.env.example` lista las variables esperadas para Fase 1 (cadenas de conexión PostgreSQL/MongoDB/Redis/RabbitMQ, URLs y realm de Keycloak), sin valores reales.
- [ ] `LICENSE` definido explícitamente (o se documenta como "proyecto privado de portafolio" si no aplica licencia abierta).

**Definition of Done:** clonar el repositorio en limpio no genera advertencias de archivos no ignorados ni variables de entorno faltantes sin documentar.

> **DoD de la Épica 1:** el proyecto parece un repositorio profesional aunque todavía no tenga funcionalidades de negocio.

---

## EPIC-02 — Backend Foundation

**Semana:** 2
**Objetivo:** un backend completamente funcional aunque no haga negocio todavía.
**Aprenderás (Plan Técnico):** Clean Architecture, Dependency Injection, Minimal APIs, Vertical Slice, Fluent Validation.
**Restricción (ADR-0001):** un único deployable `WalrusWallet.Api`. Nada de servicios separados todavía.

### US-05 — Solución .NET con Clean Architecture

**Como** desarrollador, **quiero** una solución .NET organizada en capas Clean Architecture, **para** que el backend tenga límites claros desde el primer commit de código, incluso siendo un monolito modular de un solo deployable.

**Criterios de aceptación:**
- [ ] Existe `WalrusWallet.sln` con los proyectos: `Domain`, `Application`, `Infrastructure`, `Api`, `SharedKernel`.
- [ ] `Domain` no referencia `Infrastructure` ni `Api` (verificado manualmente o con un test de arquitectura, ej. NetArchTest).
- [ ] `WalrusWallet.Api` es el único proyecto ejecutable de Fase 1 (ADR-0001, decisión 1).
- [ ] La solución compila sin errores ni warnings críticos.

**Definition of Done:** la solución corre localmente con `dotnet run` desde `Api` sin pasos manuales adicionales.

---

### US-06 — Configuración transversal del backend

**Como** desarrollador, **quiero** DI, logging, configuración y manejo global de excepciones configurados, **para** no repetir boilerplate en cada feature futura.

**Criterios de aceptación:**
- [ ] Inyección de dependencias registrada por capa (`Application` e `Infrastructure` se registran desde `Api`, no al revés).
- [ ] Logging configurado con Serilog (mínimo sink de consola).
- [ ] Configuración basada en Options Pattern (`appsettings.json` + clases de opciones fuertemente tipadas, sin "magic strings" para leer configuración).
- [ ] Middleware global de manejo de excepciones devuelve un formato de error JSON consistente y no expone stack traces en entorno de producción.

**Definition of Done:** forzar una excepción no controlada en un endpoint de prueba devuelve el formato de error estándar, no una página de error de ASP.NET.

---

### US-07 — Endpoints base y observabilidad mínima

**Como** desarrollador, **quiero** endpoints `/health` y `/version`, Swagger y Health Checks, **para** verificar que el backend está vivo y documentado desde el día uno.

**Criterios de aceptación:**
- [ ] `GET /health` responde `200` e incluye el resultado de los Health Checks configurados.
- [ ] `GET /version` responde con la versión/commit hash de la build.
- [ ] Swagger UI disponible en entorno de desarrollo, documentando ambos endpoints.
- [ ] Health Checks de ASP.NET Core configurados vía el paquete estándar (`Microsoft.Extensions.Diagnostics.HealthChecks` o equivalente).

**Definition of Done:** ambos endpoints son visibles y ejecutables desde Swagger UI sin configuración adicional.

> **DoD de la Épica 2:** existe un backend completamente funcional aunque no haga negocio.

---

## EPIC-03 — Infraestructura Docker

**Semana:** 3
**Objetivo:** `docker compose up` levanta toda la plataforma.
**Aprenderás (Plan Técnico):** Docker, Docker Compose, Networking, Volumes, Images.
**Restricción (ADR-0001):** RabbitMQ se valida con un mensaje de prueba — **sin lógica de negocio**.

### US-08 — Dockerfiles multi-stage

**Como** desarrollador, **quiero** Dockerfiles multi-stage para backend y frontend, **para** producir imágenes optimizadas y reproducibles localmente y en el CI/CD futuro.

**Criterios de aceptación:**
- [ ] Dockerfile de backend usa build multi-stage (SDK de .NET para build/publish, runtime ASP.NET para la imagen final).
- [ ] Dockerfile de frontend usa build multi-stage (Node para build, servidor estático ligero para runtime).
- [ ] Ambas imágenes construyen exitosamente con `docker build` sin errores.
- [ ] La imagen final de cada lado no incluye herramientas de build (verificado revisando el tamaño y las capas de la imagen).

**Definition of Done:** ambas imágenes corren de forma independiente con `docker run` y responden correctamente.

---

### US-09 — Docker Compose con infraestructura completa

**Como** desarrollador, **quiero** un `docker-compose.yml` que levante toda la infraestructura de Fase 1, **para** tener un entorno de desarrollo reproducible con un solo comando.

**Criterios de aceptación:**
- [ ] `docker compose up` levanta: PostgreSQL, MongoDB, Redis, RabbitMQ, Keycloak, NGINX (y backend/frontend, ya dockerizados en US-08).
- [ ] Las variables de entorno se inyectan desde `.env` (ninguna credencial hardcodeada en el compose).
- [ ] Redes y volúmenes están nombrados explícitamente, sin depender de defaults implícitos de Docker.
- [ ] Cada servicio con estado (PostgreSQL, MongoDB, Redis, RabbitMQ) usa un volumen persistente.
- [ ] NGINX enruta correctamente hacia backend y frontend (verificado con una petición manual a través de NGINX).

**Definition of Done:** un desarrollador nuevo puede clonar el repositorio, copiar `.env.example` a `.env`, y levantar toda la plataforma con un único comando.

---

### US-10 — Validación de conectividad de RabbitMQ sin lógica de negocio

**Como** desarrollador, **quiero** validar que RabbitMQ funciona en el entorno de Docker Compose, **para** tener el broker operativo y comprendido antes de construir cualquier evento de dominio real (ADR-0001).

**Criterios de aceptación:**
- [ ] Existe un exchange y una queue de prueba (creados manualmente o vía script de inicialización desechable).
- [ ] Se publica y se consume al menos un mensaje de prueba, confirmando conectividad end-to-end.
- [ ] **Explícitamente fuera de alcance:** ningún publisher/consumer de negocio (ej. `UserRegistered`) se implementa en esta historia — eso queda bloqueado hasta Fase 5, después de CQRS y eventos de dominio in-process (ADR-0001, decisión 3).
- [ ] La consola de administración de RabbitMQ es accesible localmente desde el navegador.

**Definition of Done:** queda evidencia (captura o log) de un mensaje de prueba publicado y consumido exitosamente, y la historia explicita que no se construyó lógica de negocio.

> **DoD de la Épica 3:** `docker compose up` levanta toda la plataforma; RabbitMQ queda validado solo con un mensaje de prueba.

---

## EPIC-04 — Frontend Foundation (SPA única)

**Semana:** 4
**Objetivo:** login funcionando sobre una SPA única.
**Aprenderás (Plan Técnico):** Arquitectura de SPA, React Architecture, Routing, integración con IdP externo (Keycloak).
**Restricción (ADR-0001):** sin Module Federation. Se introduce en Fase 6, junto con la primera extracción real de microservicio.

### US-11 — Aplicación React base

**Como** desarrollador, **quiero** una SPA React única organizada por features, **para** tener un frontend con estructura clara sin fragmentar el dominio antes de tiempo.

**Criterios de aceptación:**
- [ ] Existe un único proyecto `frontend/web` (sin Shell, sin remotes, sin Module Federation).
- [ ] Estructura por features: al menos `features/auth` y `features/dashboard`.
- [ ] Routing configurado con al menos las rutas: login, dashboard, 404.
- [ ] Layout base (header/sidebar/contenido) implementado y reutilizado entre rutas.

**Definition of Done:** la SPA corre con un único comando de desarrollo y navega entre las rutas base sin errores en consola.

---

### US-12 — Integración con Keycloak (login y sesión)

**Como** usuario final, **quiero** poder iniciar sesión contra Keycloak desde la SPA, **para** acceder a la plataforma de forma segura desde el primer corte funcional.

**Criterios de aceptación:**
- [ ] La SPA redirige a Keycloak para autenticación (Authorization Code Flow, idealmente con PKCE).
- [ ] Tras un login exitoso, la SPA gestiona el access token y el refresh token de forma segura.
- [ ] Existe manejo de sesión expirada (refresh automático o redirección a login).
- [ ] La ruta `dashboard` solo es accesible con sesión válida (ruta protegida).
- [ ] El primer login exitoso contra el backend crea o vincula el `UserProfile` correspondiente (depende de US-13).

**Definition of Done:** un usuario puede iniciar sesión, navegar al dashboard protegido y cerrar sesión, sin intervención manual.

> **DoD de la Épica 4:** login funcionando sobre una SPA única.

---

## EPIC-05 — Identity mínimo dentro del monolito

**Semanas:** transversal a 2 y 4
**Origen:** ADR-0001, decisión 4 — *"el modelo de dominio de Identity en Fase 1 se limita a `UserProfile` y `AuditEvent`. `Organization` y `Workspace` se posponen a la fase de Companies."*
**Objetivo:** dar soporte mínimo al login de la Épica 4 sin crear un servicio Identity separado ni anticipar el modelo de dominio completo de Fase 2.

### US-13 — Módulo Identity mínimo (`UserProfile`)

**Como** arquitecto, **quiero** un módulo Identity mínimo dentro de las capas compartidas de Clean Architecture, **para** vincular el usuario autenticado en Keycloak con un perfil interno, sin crear un servicio separado.

**Criterios de aceptación:**
- [ ] Existen `Domain/Identity`, `Application/Identity` e `Infrastructure/Identity` dentro de la solución única — **no** existe un proyecto ni deployable separado.
- [ ] La entidad `UserProfile` se persiste vinculada al `sub` (subject) del token de Keycloak; Keycloak sigue siendo la única fuente de verdad de credenciales (no se duplica gestión de contraseñas).
- [ ] Existe al menos un caso de uso de `Application` (ej. *ObtenerOCrearPerfilDeUsuarioAutenticado*) cubierto con pruebas unitarias.
- [ ] El esquema de base de datos usado es `identity.*`, dentro de la misma instancia de PostgreSQL del monolito (no una base de datos separada).
- [ ] **Explícitamente fuera de alcance:** `Organization` y `Workspace` (ADR-0001, decisión 4; se construyen en la fase de Companies).

**Definition of Done:** el caso de uso de creación/obtención de perfil está testeado y listo para ser consumido por US-12.

---

### US-14 — `AuditEvent` mínimo

**Como** responsable de cumplimiento, **quiero** que las acciones sensibles mínimas (ej. login exitoso/fallido) generen un `AuditEvent`, **para** sentar la base de trazabilidad de auditoría antes de que exista un servicio Audit dedicado (Fase 7).

**Criterios de aceptación:**
- [ ] Existe la entidad `AuditEvent` (tipo de evento, timestamp, identificador de usuario si aplica, origen) dentro de `Domain/Identity`.
- [ ] Al completar el flujo de login de US-12, se registra al menos un `AuditEvent` real de punta a punta.
- [ ] El `AuditEvent` se persiste en el esquema `identity.*` de la misma instancia de PostgreSQL — no se crea infraestructura de mensajería ni servicio separado para esto en Fase 1.
- [ ] Los eventos de auditoría se disparan de forma síncrona/in-process; no hay publicación a RabbitMQ todavía (eso corresponde a Fase 5 en adelante, según el Plan Técnico).

**Definition of Done:** un login exitoso queda registrado como `AuditEvent` consultable directamente en la base de datos.

> **DoD de la Épica 5:** el módulo Identity tiene límites de Clean Architecture limpios — anticipando la Definition of Done formal de Fase 2 — y existe un flujo de auditoría mínimo end-to-end, sin acoplarse a infraestructura de mensajería.

---

## 3. Orden recomendado de ejecución

```text
EPIC-01  US-01 → US-02 → US-03 → US-04
EPIC-02  US-05 → US-06 → US-07
EPIC-05  US-13                              ← antes de cerrar US-12
EPIC-03  US-08 → US-09 → US-10
EPIC-04  US-11 → US-12
EPIC-05  US-14                              ← depende de que US-12 esté cerrada
```

Notas de dependencia:
- **US-13** (perfil mínimo de Identity) debe estar resuelta antes de cerrar **US-12** (login), porque el login en su criterio de aceptación crea/vincula el `UserProfile`.
- **US-14** (AuditEvent) depende de que el flujo de login de **US-12** ya esté funcionando, porque el primer evento de auditoría real es justamente el login exitoso.
- **EPIC-03** (Docker) no depende estrictamente de **EPIC-05**, pero sí de **EPIC-02** (necesita un backend ya compilable para dockerizarlo).

## 4. Trazabilidad con los ADR del proyecto

| Restricción aplicada | Origen | Dónde se aplica en este backlog |
|---|---|---|
| Un único deployable backend (`WalrusWallet.Api`) | ADR-0001, decisión 1 | US-05 |
| Identity como módulo interno, no como servicio | ADR-0001, decisión 1 y 4 | US-13, US-14 |
| Sin Module Federation en Fase 1 | ADR-0001, decisión 2 | US-11 |
| RabbitMQ sin lógica de negocio en Fase 1 | ADR-0001, decisión 3 | US-10 |
| Modelo de Identity limitado a `UserProfile` + `AuditEvent` | ADR-0001, decisión 4 | US-13, US-14 |
| Catálogo final de microservicios (contexto, no implementación en Fase 1) | ADR-0002 | Roadmap.md (US-02), como referencia de hacia dónde evoluciona Identity en fases futuras |

Ninguna historia de este backlog crea un deployable adicional al monolito ni un remoto de Module Federation. Cuando este backlog se complete, el sistema sigue siendo, arquitectónicamente, un monolito modular de un solo deployable y una sola SPA — exactamente el punto de partida que exige ADR-0001 antes de avanzar a Fase 2.
