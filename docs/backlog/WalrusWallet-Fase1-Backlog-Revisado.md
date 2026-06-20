# WalrusWallet — Fase 1 Backlog Ejecutable (Revisado)

## Objetivo de esta fase

Convertir la definición conceptual de la Fase 1 en un backlog claro y construible, priorizado para avanzar de forma ordenada sobre un monorepo profesional con React, .NET, Keycloak, Docker Compose y una base lista para evolucionar a microservicios y microfrontends.

> Esta versión incorpora **ADR-0001 — Fase 1 como Monolito Modular**: en esta fase construimos un solo backend desplegable y una sola SPA, sin fragmentar todavía. Ver `ADR-0001-Monolito-Modular-Fase1.md`.

## Alcance de la Fase 1

En esta fase no se construye el negocio completo. El objetivo es dejar lista la base técnica y funcional mínima — como monolito modular — para que el proyecto pueda crecer con buena arquitectura.

### Entregables de la Fase 1

- Monorepo inicial organizado.
- Aplicación React única (SPA), con layout, rutas y feature de autenticación.
- Backend base en .NET con Clean Architecture, en un único deployable.
- Integración con Keycloak.
- Infraestructura local con Docker Compose.
- PostgreSQL, MongoDB, Redis y RabbitMQ listos (RabbitMQ sin lógica de negocio aún).
- NGINX como gateway/reverse proxy local.
- CI básico con GitHub Actions.
- Pruebas unitarias, de integración y end-to-end iniciales.

---

## Principios de construcción

1. Primero fundación, luego negocio.
2. Una sola arquitectura consistente en todo el repo: un backend, una SPA — sin fragmentar antes de tiempo.
3. Cada pieza nueva debe poder correr localmente con Docker.
4. El proyecto debe verse y sentirse como software real de empresa.
5. Cada historia debe dejar una base reutilizable para fases futuras, sin anticipar microservicios ni microfrontends.

---

## Épicas de la Fase 1

### Épica 1 — Fundación del monorepo

**Objetivo:** dejar el repositorio estructurado y documentado para crecer sin desorden.

**Historias de usuario**

**US-01 — Como desarrollador, quiero un monorepo bien organizado para ubicar frontend, backend, infraestructura y documentación.**
**Criterios de aceptación:**

- Existe una estructura raíz clara.
- El proyecto tiene carpetas separadas para `frontend`, `backend`, `infra`, `docs` y `.github`.
- Se incluyen archivos base como README, .gitignore y archivo de variables de entorno de ejemplo.
- Cualquier desarrollador nuevo entiende rápidamente dónde va cada cosa.

**Tareas técnicas:**

- Crear estructura base del repo.
- Definir convenciones de nombres.
- Crear README inicial.
- Crear `.env.example`.
- Definir ramas base de trabajo.

---

**US-02 — Como desarrollador, quiero documentación base del proyecto para entender la visión, el alcance y la arquitectura.**
**Criterios de aceptación:**

- Existe documentación de visión.
- Existe documentación de arquitectura.
- Existe documentación del roadmap.
- Existe un glosario mínimo de términos.
- El documento explica qué entra y qué no entra en Fase 1, incluyendo por qué no hay microservicios ni microfrontends todavía (referencia a ADR-0001).

**Tareas técnicas:**

- Crear carpeta `docs`.
- Redactar `Vision.md`.
- Redactar `Architecture.md`.
- Redactar `Roadmap.md`.
- Redactar `Glossary.md`.
- Incluir `adr/ADR-0001-Monolito-Modular-Fase1.md`.

---

### Épica 2 — Backend base en .NET

**Objetivo:** construir el esqueleto del backend siguiendo Clean Architecture, como un único deployable.

**Historias de usuario**

**US-03 — Como desarrollador, quiero una solución backend en Clean Architecture para separar dominio, aplicación, infraestructura y API.**
**Criterios de aceptación:**

- La solución tiene proyectos separados por capa, pero un único deployable (`Api`).
- La API depende de Application, Domain e Infrastructure de forma correcta.
- El módulo Identity vive organizado dentro de cada capa (`Application/Identity`, `Domain/Identity`, `Infrastructure/Identity`), no como proyecto/servicio aparte.
- No hay lógica de negocio metida directamente en el controlador o endpoint.
- La estructura permite extraer un microservicio más adelante sin rediseñar (ver ADR-0001).

**Tareas técnicas:**

- Crear solución .NET.
- Crear proyectos `Api`, `Application`, `Domain`, `Infrastructure`, `SharedKernel`.
- Configurar referencias entre capas.
- Crear base de inyección de dependencias.
- Crear manejo centralizado de errores.

---

**US-04 — Como desarrollador, quiero endpoints base de salud y versión para validar el despliegue.**
**Criterios de aceptación:**

- Existen endpoints `/health` y `/version`.
- Los endpoints responden correctamente en local y en contenedor.
- La respuesta de health permite verificar el estado general de la app.

**Tareas técnicas:**

- Implementar endpoint de salud.
- Implementar endpoint de versión.
- Agregar validación básica de infraestructura.

---

**US-05 — Como desarrollador, quiero tener una base de autenticación integrada con Keycloak para no depender de un login casero.**
**Criterios de aceptación:**

- Keycloak corre en Docker Compose.
- El backend reconoce tokens válidos emitidos por Keycloak.
- El frontend puede iniciar sesión con Keycloak.
- La autenticación está lista para usarse en futuras funcionalidades.

**Tareas técnicas:**

- Configurar realm de Keycloak.
- Crear cliente para frontend.
- Crear cliente o configuración para backend.
- Validar JWT en la API.
- Documentar configuración de Keycloak.

---

### Épica 3 — Frontend de la aplicación (SPA)

**Objetivo:** dejar lista una única aplicación React, bien estructurada por features, sin fragmentar el frontend antes de tiempo (ver ADR-0001).

**Historias de usuario**

**US-06 — Como usuario, quiero una aplicación React con layout y navegación base para empezar a usar el sistema.**
**Criterios de aceptación:**

- Existe una sola aplicación React funcional (sin Module Federation).
- Tiene layout base (header, contenido, navegación).
- Las rutas principales están configuradas.
- El código está organizado por features (`features/`), listo para crecer.

**Tareas técnicas:**

- Crear app con Vite + React + TypeScript.
- Crear layout base y providers (router, query client, etc.).
- Configurar rutas principales.
- Definir carpetas `features/` y `shared/`.

---

**US-07 — Como usuario, quiero iniciar sesión desde el feature de autenticación de la aplicación.**
**Criterios de aceptación:**

- Existe página de login dentro de `features/auth`.
- El login redirige correctamente a la aplicación.
- Los errores de autenticación se muestran de forma clara.
- La sesión (token, refresh) se maneja en un hook/contexto reutilizable por el resto de features.

**Tareas técnicas:**

- Crear `features/auth` con el formulario de login.
- Integrar con Keycloak.
- Crear hook/contexto de sesión.
- Crear validaciones básicas del formulario.

> Module Federation se introduce en una fase posterior, cuando exista un primer microservicio extraído (ver ADR-0001), no en Fase 1.

---

### Épica 4 — Infraestructura local con Docker Compose

**Objetivo:** lograr que toda la base del proyecto arranque localmente con un solo comando.

**Historias de usuario**

**US-08 — Como desarrollador, quiero levantar toda la plataforma local con Docker Compose para no depender de instalaciones manuales.**
**Criterios de aceptación:**

- Existe `docker-compose.yml`.
- Con un solo comando se levantan servicios base.
- Los servicios se comunican entre sí en red interna.
- El proyecto documenta cómo iniciar, detener y resetear el entorno.

**Tareas técnicas:**

- Crear archivo de Docker Compose.
- Agregar backend, frontend e infraestructura.
- Crear redes y volúmenes.
- Configurar variables de entorno.
- Documentar comandos de uso.

---

**US-09 — Como desarrollador, quiero tener PostgreSQL, MongoDB, Redis y RabbitMQ disponibles desde el inicio.**
**Criterios de aceptación:**

- PostgreSQL levanta correctamente.
- MongoDB levanta correctamente.
- Redis responde correctamente.
- RabbitMQ levanta con su panel de administración y se valida con un mensaje de prueba (sin lógica de negocio).
- Cada servicio está disponible para futuras fases.

**Tareas técnicas:**

- Configurar contenedores base.
- Crear volúmenes persistentes.
- Definir usuarios y contraseñas por entorno.
- Verificar conectividad entre servicios.
- Probar conexión desde backend.

---

**US-10 — Como usuario, quiero acceder a la aplicación a través de un proxy centralizado.**
**Criterios de aceptación:**

- NGINX actúa como reverse proxy.
- El tráfico al frontend se enruta correctamente.
- La API queda accesible por una ruta común.
- La configuración está lista para crecer a producción.

**Tareas técnicas:**

- Configurar NGINX.
- Definir rutas para frontend y API.
- Preparar configuración para WebSocket en el futuro.
- Documentar el mapeo de rutas.

---

### Épica 5 — Calidad, pruebas y CI

**Objetivo:** asegurar que el proyecto no sea solo funcional, sino mantenible y verificable.

**Historias de usuario**

**US-11 — Como desarrollador, quiero pruebas unitarias para proteger la lógica base del proyecto.**
**Criterios de aceptación:**

- Existen pruebas unitarias mínimas.
- Las pruebas se ejecutan desde el pipeline.
- La lógica base importante está cubierta.

**Tareas técnicas:**

- Crear proyecto de tests.
- Configurar framework de pruebas.
- Escribir pruebas para servicios iniciales.
- Integrar tests en build local.

---

**US-12 — Como desarrollador, quiero pruebas de integración para validar la API con sus dependencias reales.**
**Criterios de aceptación:**

- Existe al menos una prueba de integración.
- La API puede probarse con base de datos real en entorno controlado.
- El test valida comportamiento end-to-end de backend base.

**Tareas técnicas:**

- Configurar pruebas de integración.
- Levantar dependencias en contenedores de prueba.
- Validar endpoints base.
- Documentar cómo ejecutarlas.

---

**US-13 — Como desarrollador, quiero una prueba end-to-end inicial del login para validar el flujo completo.**
**Criterios de aceptación:**

- El flujo login puede probarse desde navegador.
- La prueba cubre la SPA y la autenticación.
- El caso feliz funciona sin intervención manual.

**Tareas técnicas:**

- Configurar Playwright.
- Crear escenario de login.
- Validar navegación post-login.
- Guardar evidencia de prueba.

---

**US-14 — Como desarrollador, quiero un pipeline básico en GitHub Actions para validar cada cambio.**
**Criterios de aceptación:**

- El pipeline ejecuta build.
- El pipeline ejecuta pruebas.
- El pipeline detecta errores antes de merge.
- El archivo de workflow está versionado en el repo.

**Tareas técnicas:**

- Crear workflow de backend.
- Crear workflow de frontend.
- Crear workflow unificado de validación.
- Documentar el proceso de CI.

---

## Orden de construcción recomendado

### Paso 1 — Base documental y estructura

1. Crear monorepo.
2. Crear documentación inicial (incluyendo ADR-0001).
3. Definir convenciones del proyecto.
4. Crear archivos base del entorno.

### Paso 2 — Backend fundacional

1. Crear solución .NET (un solo deployable).
2. Separar capas con Clean Architecture; organizar Identity como módulo interno.
3. Agregar endpoints base.
4. Integrar validación de configuración.

### Paso 3 — Identidad

1. Levantar Keycloak.
2. Crear cliente de frontend.
3. Integrar autenticación en backend.
4. Integrar autenticación en frontend.

### Paso 4 — Frontend de la aplicación

1. Crear la app React base (un solo proyecto).
2. Crear layout y configurar rutas.
3. Construir el feature de autenticación.
4. Validar navegación base y sesión.

### Paso 5 — Infraestructura local

1. Crear Dockerfiles.
2. Crear docker-compose.
3. Agregar PostgreSQL, MongoDB, Redis y RabbitMQ.
4. Agregar NGINX.

### Paso 6 — Calidad y automatización

1. Añadir pruebas unitarias.
2. Añadir pruebas de integración.
3. Añadir prueba e2e de login.
4. Crear GitHub Actions.

---

## Definition of Done para Fase 1

La Fase 1 se considera terminada cuando se cumpla todo esto:

- El monorepo está organizado y documentado.
- El backend corre como un único deployable con Clean Architecture.
- La aplicación React corre como una sola SPA, con layout y rutas base.
- El feature de autenticación funciona.
- Keycloak está integrado.
- Docker Compose levanta todo el entorno.
- Las bases de datos y mensajería están disponibles (RabbitMQ sin lógica de negocio aún).
- Existen pruebas mínimas.
- Existe CI básico en GitHub Actions.
- El proyecto se puede ejecutar localmente con un solo comando.

---

## Siguiente paso inmediato

Con este backlog definido y coherente con ADR-0001, el siguiente paso es ejecutar Paso 1 a Paso 6 en orden, cerrando la revisión de diseño de cada módulo antes de escribir su código.
