# ADR-0003 — Solo dependencias Open Source

**Estado:** Aceptado
**Decisores:** Arquitectura del proyecto + Desarrollador principal

## Contexto

WalrusWallet es un proyecto de portafolio cuyo objetivo es que el desarrollador practique herramientas que pueda usar en un trabajo real. No todas las empresas disponen de presupuesto para licencias comerciales de librerías, por lo que depender de paquetes comerciales limita el valor del portafolio como muestra de competencias transferibles.

Al escanear la documentación y la solución ya construida se identificó que el stack de testing recomendaba **FluentAssertions**, cuya versión 8 y posteriores requieren una **licencia paga para uso comercial** (v7 permanece Apache 2.0 solo con correcciones de bugs). El resto del stack referenciado (xUnit, Testcontainers, Vitest, React Testing Library, Playwright, Serilog, NetArchTest, Swashbuckle/Scalar, paquetes de `Microsoft.Extensions.*`, Keycloak, PostgreSQL, MongoDB, Redis, RabbitMQ, NGINX) es open source.

## Decisión

1. **Restricción permanente:** el proyecto **no incorpora dependencias comerciales**. Todo paquete NuGet, npm o imagen de contenedor debe tener una **licencia open source reconocida** (MIT, Apache 2.0, BSD, ISC, MPL, LGPL, etc.).
2. **Verificación previa:** antes de agregar cualquier dependencia nueva, se debe verificar su licencia. Si una librería es dual (gratis para OSS / paga para uso comercial), se considera **comercial** y queda fuera.
3. **Reemplazo aplicado:** FluentAssertions se reemplaza por **Shouldly** (licencia MIT, API fluida equivalente, ampliamente adoptada en la industria .NET).
4. **Excepciones:** ninguna en Fase 1. Cualquier excepción futura debe registrarse como un nuevo ADR que justifique el caso específico.

## Consecuencias

**Positivas**
- El portafolio demuestra competencias con herramientas que cualquier empresa puede adoptar sin costo de licencia.
- No existe riesgo legal ni de auditoría por uso de librerías con licencias comerciales restrictivas.
- El desarrollador practica la disciplina de revisar licencias antes de incorporar dependencias — hábito valorado en entornos profesionales.

**Trade-offs**
- Algunas librerías comerciales pueden ofrecer mejor ergonomía o funcionalidades extra; se asume el costo de buscar y aprender la alternativa open source equivalente.
- Mantenerse en v7 de FluentAssertions (aún open source con bugfixes) era una opción, pero su eventual EOL y la ambigüedad de la licencia dual lo descartan para un portafolio que busca herramientas sostenibles a largo plazo.

## Alternativas consideradas

- **Mantener FluentAssertions v7 (Apache 2.0):** rechazada. Recibe solo correcciones limitadas y su modelo de licenciamiento dual proyecta incertidumbre sobre la sostenibilidad del paquete como elección a largo plazo.
- **Usar solo aserciones nativas de xUnit (`Assert.*`):**rechazada como estándar global. Viable para tests puntuales, pero se prefiere una librería fluida (Shouldly) para mantener legibilidad y consistencia en el cuerpo de tests.

## Aplicaciones concretas en el repositorio

- `README.MD` (tabla de stack de testing): FluentAssertions → Shouldly.
- `docs/architecture/fase1-especificacion.md` (herramientas recomendadas): FluentAssertions → Shouldly.

## Pendiente para revisión futura

Antes de cada fase que introduzca nuevas dependencias (ej. Fase 5 — Payments, Fase 9 — Observabilidad), revisar que las librerías seleccionadas (mensajería, tracing, object storage, etc.) cumplan esta restricción. Registrar cualquier sustitución en este ADR o en uno nuevo si la decisión es específica del contexto de esa fase.