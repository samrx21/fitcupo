# Evidencias de seguridad · Entrega 2

Resultados reales de la colección [`FitCupo-Seguridad.postman_collection.json`](postman/FitCupo-Seguridad.postman_collection.json),
ejecutada con Newman contra los tres microservicios recién iniciados (bases de datos vacías).

**Resultado: 40 peticiones, 57 de 57 pruebas exitosas.**

Para repetirlo: inicia los servicios con `scripts\run-all.cmd` y ejecuta la colección en Postman con **Run collection**.

## Inicialización de Identity con los seeders

Consola de Identity al iniciar por primera vez:

```
Seeder: rol Admin creado.
Seeder: rol Receptionist creado.
Seeder: rol Instructor creado.
Seeder: rol Member creado.
Seeder: usuario admin@fitcupo.com creado con el rol Admin.
Seeder: usuario recepcion@fitcupo.com creado con el rol Receptionist.
Seeder: usuario instructor@fitcupo.com creado con el rol Instructor.
Seeder: usuario instructor2@fitcupo.com creado con el rol Instructor.
Seeder: usuario afiliado@fitcupo.com creado con el rol Member.
```

Tablas `Users`, `UserRoles` y `Roles` en SQL Server. La contraseña se guarda como hash de ASP.NET Core Identity, no en texto plano:

```
Email Rol PasswordHash
----- --- ------------
admin@fitcupo.com Admin AQAAAAIAAYagAAAAEMRacGG8w...
instructor2@fitcupo.com Instructor AQAAAAIAAYagAAAAEF5fXomLD...
instructor@fitcupo.com Instructor AQAAAAIAAYagAAAAENHGl/f/v...
afiliado@fitcupo.com Member AQAAAAIAAYagAAAAEB3+x3Si9...
recepcion@fitcupo.com Receptionist AQAAAAIAAYagAAAAEJ/KYx+ix...
```

## 1. Microservicios en ejecución

| Prueba | Petición | Token | Código | Resultado | Respuesta |
|---|---|---|:-:|:-:|---|
| Identity responde | `GET localhost:5170/openapi/v1.json` | sin token | **200** | ✅ | Documento OpenAPI de FitCupo.Identity.API / v1 |
| Clientes responde | `GET localhost:5180/openapi/v1.json` | sin token | **200** | ✅ | Documento OpenAPI de FitCupo.Clients.API / v1 |
| Clases responde | `GET localhost:5190/openapi/v1.json` | sin token | **200** | ✅ | Documento OpenAPI de FitCupo.Classes.API / v1 |

## 2. Usuarios y roles creados por los seeders

| Prueba | Petición | Token | Código | Resultado | Respuesta |
|---|---|---|:-:|:-:|---|
| Iniciar sesión como administrador | `POST localhost:5170/api/auth/login` | sin token | **200** | ✅ | JWT emitido para Andrea Ruiz con roles Admin; vence 2026-11-14T14:44:04.9603633Z |
| Listar usuarios con sus roles (Admin) | `GET localhost:5170/api/users` | admin | **200** | ✅ | afiliado@fitcupo.com (Member); instructor2@fitcupo.com (Instructor); instructor@fitcupo.com (Instructor); admin@fitcupo.com (Admin); recepcion@fitcupo.com (Receptionist) |
| Filtrar usuarios por rol Instructor | `GET localhost:5170/api/users?role=Instructor` | admin | **200** | ✅ | instructor2@fitcupo.com (Instructor); instructor@fitcupo.com (Instructor) |

## 3. Autenticación y emisión del JWT

| Prueba | Petición | Token | Código | Resultado | Respuesta |
|---|---|---|:-:|:-:|---|
| Iniciar sesión como recepción | `POST localhost:5170/api/auth/login` | sin token | **200** | ✅ | JWT emitido para Mateo Torres con roles Receptionist; vence 2026-11-14T14:44:05.4359993Z |
| Iniciar sesión como instructor | `POST localhost:5170/api/auth/login` | sin token | **200** | ✅ | JWT emitido para Carolina Ríos con roles Instructor; vence 2026-11-14T14:44:05.5669053Z |
| Iniciar sesión como segundo instructor | `POST localhost:5170/api/auth/login` | sin token | **200** | ✅ | JWT emitido para Julián Pérez con roles Instructor; vence 2026-11-14T14:44:05.7014082Z |
| Iniciar sesión como afiliado | `POST localhost:5170/api/auth/login` | sin token | **200** | ✅ | JWT emitido para Laura Gómez con roles Member; vence 2026-11-14T14:44:05.8424171Z |
| 401 · Contraseña incorrecta | `POST localhost:5170/api/auth/login` | sin token | **401** | ✅ | **Credenciales inválidas**: El correo o la contraseña no son correctos. |
| 400 · Correo y contraseña vacíos | `POST localhost:5170/api/auth/login` | sin token | **400** | ✅ | **Datos inválidos**: El correo y la contraseña son requeridos. |
| Validar el token: ¿quién soy? | `GET localhost:5170/api/auth/me` | afiliado | **200** | ✅ | Laura Gómez (Member) |

## 4. Acceso sin token (401)

| Prueba | Petición | Token | Código | Resultado | Respuesta |
|---|---|---|:-:|:-:|---|
| Clientes sin token | `GET localhost:5180/api/clients` | sin token | **401** | ✅ | **No autenticado**: Debes enviar un token JWT en el encabezado Authorization: Bearer {token}. |
| Clases sin token | `GET localhost:5190/api/classes` | sin token | **401** | ✅ | **No autenticado**: Debes enviar un token JWT en el encabezado Authorization: Bearer {token}. |
| Identity /me sin token | `GET localhost:5170/api/auth/me` | sin token | **401** | ✅ | **No autenticado**: Debes enviar un token JWT en el encabezado Authorization: Bearer {token}. |

## 5. Acceso con token válido (200)

| Prueba | Petición | Token | Código | Resultado | Respuesta |
|---|---|---|:-:|:-:|---|
| Afiliado consulta el catálogo de clases | `GET localhost:5190/api/classes` | afiliado | **200** | ✅ | 3 registro(s) |
| Afiliado consulta el horario | `GET localhost:5190/api/sessions` | afiliado | **200** | ✅ | 0 registro(s) |
| Recepción consulta los clientes | `GET localhost:5180/api/clients` | recepción | **200** | ✅ | 0 registro(s) |

## 6. Autorización por roles: usuario con el rol requerido

| Prueba | Petición | Token | Código | Resultado | Respuesta |
|---|---|---|:-:|:-:|---|
| Recepción registra a la afiliada (Admin, Receptionist) | `POST localhost:5180/api/clients` | recepción | **201** | ✅ | Id `01a1da4d-1664-7ea2-b73d-93b95bd79c22` |
| Recepción busca a la afiliada | `GET localhost:5180/api/clients?search=afiliado@fitcupo.com` | recepción | **200** | ✅ | 1 registro(s) |
| Afiliada consulta su propio perfil (Member) | `GET localhost:5180/api/clients/me` | afiliado | **200** | ✅ | Perfil de Laura Gómez (afiliado@fitcupo.com) |
| Administrador desactiva un cliente (Admin) | `PATCH localhost:5180/api/clients/01a1da4d-1664-7ea2-b73d-93b95bd79c22/deactivate` | admin | **204** | ✅ | Sin contenido |
| Administrador reactiva el cliente (Admin) | `PATCH localhost:5180/api/clients/01a1da4d-1664-7ea2-b73d-93b95bd79c22/activate` | admin | **204** | ✅ | Sin contenido |
| Administrador crea una clase (Admin) | `POST localhost:5190/api/classes` | admin | **201** | ✅ | Id `01a1da4d-1908-79f6-bc8c-854ed384ba3a` |
| Instructor programa una sesión (Instructor) | `POST localhost:5190/api/sessions` | instructor | **201** | ✅ | Id `01a1da4d-19db-7d7c-b9c2-7ee13dc22054` |
| La sesión queda a nombre del instructor del token | `GET localhost:5190/api/sessions/01a1da4d-19db-7d7c-b9c2-7ee13dc22054` | afiliado | **200** | ✅ | Yoga en Salón 2 con Carolina Ríos, Scheduled |

## 7. Usuario autenticado sin el rol requerido (403)

| Prueba | Petición | Token | Código | Resultado | Respuesta |
|---|---|---|:-:|:-:|---|
| Afiliado intenta listar los clientes | `GET localhost:5180/api/clients` | afiliado | **403** | ✅ | **Acceso denegado**: Laura Gómez inició sesión, pero su rol (Member) no tiene permiso para este recurso. |
| Recepción intenta desactivar un cliente | `PATCH localhost:5180/api/clients/01a1da4d-1664-7ea2-b73d-93b95bd79c22/deactivate` | recepción | **403** | ✅ | **Acceso denegado**: Mateo Torres inició sesión, pero su rol (Receptionist) no tiene permiso para este recurso. |
| Instructor intenta crear una clase | `POST localhost:5190/api/classes` | instructor | **403** | ✅ | **Acceso denegado**: Carolina Ríos inició sesión, pero su rol (Instructor) no tiene permiso para este recurso. |
| Afiliado intenta programar una sesión | `POST localhost:5190/api/sessions` | afiliado | **403** | ✅ | **Acceso denegado**: Laura Gómez inició sesión, pero su rol (Member) no tiene permiso para este recurso. |
| Recepción intenta listar los usuarios de Identity | `GET localhost:5170/api/users` | recepción | **403** | ✅ | **Acceso denegado**: Mateo Torres inició sesión, pero su rol (Receptionist) no tiene permiso para este recurso. |
| Otro instructor intenta cancelar la sesión ajena | `PATCH localhost:5190/api/sessions/01a1da4d-19db-7d7c-b9c2-7ee13dc22054/cancel` | instructor 2 | **403** | ✅ | **Acceso denegado**: Solo puedes cancelar las sesiones que tú dictas. |

## 8. Token inválido o expirado (401)

| Prueba | Petición | Token | Código | Resultado | Respuesta |
|---|---|---|:-:|:-:|---|
| Token expirado (firma correcta, exp en el pasado) | `GET localhost:5190/api/classes` | token expirado | **401** | ✅ | **No autenticado**: El token expiró el 2026-11-14 12:44:09 UTC. Inicia sesión de nuevo. |
| Token modificado (el afiliado se cambia el rol a Admin) | `GET localhost:5180/api/clients` | token modificado | **401** | ✅ | **No autenticado**: La firma del token no es válida: el token fue modificado o no lo emitió Identity. |
| Token firmado con otra clave | `GET localhost:5190/api/classes` | token con otra clave | **401** | ✅ | **No autenticado**: La firma del token no es válida: el token fue modificado o no lo emitió Identity. |
| Texto que no es un JWT | `GET localhost:5170/api/auth/me` | texto que no es JWT | **401** | ✅ | **No autenticado**: El token no es válido. |

## 9. Reglas de negocio con el usuario del token

| Prueba | Petición | Token | Código | Resultado | Respuesta |
|---|---|---|:-:|:-:|---|
| 409 · Salón ocupado en ese horario | `POST localhost:5190/api/sessions` | instructor 2 | **409** | ✅ | **Conflicto con datos existentes**: El salón Salón 2 ya tiene una sesión en ese horario. |
| El instructor cancela su propia sesión | `PATCH localhost:5190/api/sessions/01a1da4d-19db-7d7c-b9c2-7ee13dc22054/cancel` | instructor | **204** | ✅ | Sin contenido |
| 400 · La sesión ya está cancelada | `PATCH localhost:5190/api/sessions/01a1da4d-19db-7d7c-b9c2-7ee13dc22054/cancel` | admin | **400** | ✅ | **Regla de negocio incumplida**: La sesión ya está cancelada. |
