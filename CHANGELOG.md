# Cambios

## [3.0.0] - 2026-11-26

Entrega final: validación del MVP con comunicación síncrona entre microservicios e interfaz web.

### Microservicio de Reservas (nuevo)

- Dominio con el agregado `Booking` y los value objects `BookableSession` y `BookingClient`: cliente activo, sesión programada y futura, cupo disponible y cancelación antes de que empiece la sesión.
- Casos de uso con CQRS: reservar (el afiliado para sí mismo o recepción para un cliente), cancelar, mis reservas, listado de reservas, inscritos de una sesión y horario con cupos disponibles.
- Persistencia con EF Core y SQL Server (`FitCupoBookings`), con un índice único filtrado que evita reservas repetidas.
- API protegida con el JWT de Identity y autorización por rol y por usuario.

### Comunicación síncrona entre microservicios

- Reservas consulta por HTTP a Clientes (`GET /api/clients/me` y `/api/clients/{id}`) y a Clases (`GET /api/sessions/{id}` y `/api/sessions`) con `IHttpClientFactory` y clientes tipados.
- El JWT del usuario viaja en cada llamada, para que cada microservicio aplique sus permisos.
- Cada llamada queda registrada en la consola con su resultado y su duración. Si Clases o Clientes no responden, Reservas devuelve 503.

### API Gateway (nuevo)

- Punto de entrada único con YARP en el puerto 5100, con CORS para la interfaz web.
- `GET /api/status` consulta en el momento el estado de los cuatro microservicios.

### Interfaz web (nueva)

- Aplicación Blazor WebAssembly en el puerto 5300 que usa solo el API Gateway.
- Inicio de sesión con JWT y menú según el rol.
- Afiliado: horario con cupos, reservar, mis reservas y cancelar.
- Recepción y administrador: registrar, buscar, activar y desactivar clientes, y reservar para un cliente.
- Instructor: programar y cancelar sesiones, y ver los inscritos.

### Documentación

- README con el MVP, el flujo principal, la arquitectura con el Gateway, el diagrama de la comunicación síncrona y la ejecución completa.
- `scripts/run-all.cmd` inicia los cuatro microservicios, el Gateway y la interfaz web. `FitCupo.sln` incluye todos los proyectos.

## [2.0.0] - 2026-11-16

Entrega 2: autenticación y autorización con JWT en tres microservicios.

### Microservicio Identity (nuevo)

- Solución con Clean Architecture: Application, Infrastructure y API.
- ASP.NET Core Identity con Entity Framework Core y SQL Server: usuarios, roles y la relación muchos a muchos `UserRoles`.
- Seeders de roles (`Admin`, `Receptionist`, `Instructor`, `Member`) y de cinco usuarios iniciales. Las contraseñas se guardan como hash.
- Inicio de sesión con correo y contraseña, con bloqueo temporal después de 5 intentos fallidos.
- Emisión de un JWT firmado con HMAC-SHA256, con vigencia de 60 minutos y los claims `sub`, `email`, `name`, `role`, `iss`, `aud` y `jti`.
- Endpoints `POST /api/auth/login`, `GET /api/auth/me` y `GET /api/users` (solo administradores).

### Microservicio de Clases (nuevo)

- Dominio con los agregados `GymClass` y `ClassSession` y el value object `TimeSlot`, con sus reglas de negocio.
- Catálogo de clases (crear, actualizar, activar, desactivar, consultar y listar) y horario de sesiones (programar, cancelar y consultar por rango de fechas).
- Validación de cruces de horario por salón y por instructor.
- Tres clases iniciales creadas con la migración.

### Seguridad en Clientes y Clases

- Validación del JWT de Identity en cada microservicio: firma, emisor, audiencia, algoritmo y vigencia.
- Autorización por roles con `[Authorize(Roles = ...)]` y, en Clases, por usuario: un instructor solo cancela sus sesiones.
- Respuestas 401 y 403 con ProblemDetails que explican el motivo.
- Nuevo endpoint `GET /api/clients/me` para que el afiliado consulte su propio perfil.

### Documentación

- README con el flujo de autenticación, los usuarios iniciales, la matriz de permisos y los pasos para ejecutar los tres microservicios.
- Colección de Postman con los ocho escenarios de seguridad y pruebas automáticas, archivos `.http` y evidencias.
- Script `scripts/run-all.cmd` y solución `FitCupo.sln` con los tres microservicios.

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
