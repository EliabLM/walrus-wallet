# ADR-0001 — Fase 1 como Monolito Modular

**Estado:** Aceptado
**Decisores:** Arquitectura del proyecto + Desarrollador principal

## Contexto

Las instrucciones del proyecto establecen un principio rector explícito: la arquitectura debe evolucionar desde un **monolito modular** hacia microservicios y microfrontends, extrayéndolos progresivamente solo cuando el dominio esté validado:

```
Monolito Modular → Clean Architecture → CQRS → Eventos de dominio →
RabbitMQ → Extracción de microservicios → Module Federation → ...
```

Al revisar los documentos de Fase 1 (`Especificacion.md`, `Backlog.md`, `Plan-tecnico.md`) se encontraron tres puntos donde el contenido ya construido se adelanta a ese principio:

1. La Especificación propone un **Identity Service** como solución independiente (`services/identity-service`) — ya tratado como un futuro deployable separado, antes de que exista un monolito que validar.
2. La Especificación y el Plan Técnico (Semana 4) proponen **Module Federation** con un Shell + Auth MFE + Dashboard MFE desde la semana 1, fragmentando el frontend antes de tener un dominio de negocio decidido.
3. La Especificación pide un flujo de negocio sobre **RabbitMQ** (evento `UserRegistered` + consumer) en la semana 3, antes de tener CQRS o eventos de dominio reales.

Estos tres puntos generan dos arquitecturas conviviendo en el mismo repositorio: la de las instrucciones del proyecto y la de facto de los documentos de Fase 1.

## Decisión

1. **Backend:** Fase 1 construye una única solución (`WalrusWallet.sln`) con un único deployable (`WalrusWallet.Api`). Identity se organiza como módulo interno dentro de las capas Clean Architecture compartidas (`Domain/Identity`, `Application/Identity`, `Infrastructure/Identity`), no como servicio aparte.
2. **Frontend:** Fase 1 construye una única SPA React (`frontend/web`), organizada por features (`features/auth`, `features/dashboard`). Module Federation queda fuera de Fase 1; se introduce cuando se extraiga el primer microservicio real, para que el corte del frontend refleje un dominio ya validado en el backend.
3. **RabbitMQ:** se levanta en Docker Compose y se valida conectividad básica (exchange/queue de prueba), sin lógica de negocio. El primer evento de dominio real se construye cuando exista un caso que lo justifique, después de CQRS.
4. **Modelo de dominio de Identity en Fase 1:** se limita a `UserProfile` y `AuditEvent`. `Organization` y `Workspace` se posponen a la fase de Companies.

## Consecuencias

**Positivas**
- Los límites de módulo se pueden reorganizar sin costo (refactor de carpetas, no migración de infraestructura) mientras el dominio no está validado.
- Cuando llegue la primera extracción de microservicio, el módulo Identity ya tiene capas limpias: la extracción será mecánica, no un rediseño.
- Un solo deployable y una sola SPA reducen la complejidad operativa de Fase 1, dejando esa complejidad para cuando aporte valor real.

**Trade-offs**
- No se practica Module Federation ni mensajería de negocio en Fase 1; ese aprendizaje se mueve a una fase posterior, donde será más representativo.
- El portafolio de Fase 1 muestra "monolito modular bien hecho" en lugar de "microservicios desde el día uno" — es un argumento defendible y explicable en entrevista, no una limitación a esconder.

## Alternativas consideradas

- **Mantener Identity Service y Module Federation desde Fase 1:** rechazada. Contradice el principio rector del proyecto y crea dos arquitecturas en conflicto dentro del mismo repositorio.
- **Eliminar RabbitMQ de Fase 1 por completo:** rechazada parcialmente. Se mantiene el contenedor para practicar Docker Compose y operación, pero sin lógica de negocio todavía.

## Pendiente para revisión futura

El Plan Técnico (Fase 2 — Identity, semanas 5-7) ya describe "Identity Service" y "User Service" como servicios separados inmediatamente después de Fase 1. Al llegar a esa fase debe decidirse explícitamente *cuándo* ocurre la primera extracción real de microservicio, siguiendo la cadena Monolito Modular → Clean Architecture → CQRS → Eventos → RabbitMQ → Extracción #1. Es probable que el primer módulo extraído no sea Identity, sino aquel donde el dominio realmente lo justifique primero.
