# Cambios

## [1.0.0] - 2026-10-07

Entrega 1: primer microservicio de FitCupo.

### Microservicio de Clientes

- Solución en cuatro proyectos con Clean Architecture: Domain, Application, Persistence y API.
- Dominio con DDD: agregado `Client` y value objects `Document`, `Email`, `PhoneNumber` y `EmergencyContact`, con sus reglas de negocio.
- Casos de uso con CQRS: crear, actualizar, activar y desactivar clientes; consultar por Id y listar con paginación, búsqueda y filtro por estado.
- Mediador propio, repositorios y Unit of Work sobre Entity Framework Core y SQL Server, con la migración inicial.
- API REST con manejo global de errores en formato ProblemDetails (400, 404 y 409).
- Documentación de endpoints con OpenAPI y Scalar, archivo `.http` y colección de Postman con ejemplos exitosos y de errores.

### Documentación

- README con el diagrama de arquitectura de microservicios, la arquitectura del microservicio de Clientes y los pasos para ejecutarlo.

### Equipo

| Integrante | Aporte |
|---|---|
| Samuel Metaute Restrepo | Estructura de la solución, capa de Dominio y diagrama de arquitectura |
| Juana Ospina | Mediador, paginación y contratos de la capa de Aplicación |
| David Viloria | Casos de uso (commands y queries) |
| Victor Manuel David | Capa de Persistencia |
| Carlos Arango | Capa de Presentación y documentación de endpoints |
