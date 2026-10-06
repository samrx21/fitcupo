# FitCupo

Plataforma de reservas de clases grupales para un gimnasio, construida con microservicios.
Proyecto final de **Programación Distribuida**.

## Equipo

- Samuel Metaute Restrepo
- Juana Ospina
- David Viloria
- Victor Manuel David
- Carlos Arango

## Microservicios

| Microservicio | Responsabilidad |
|---|---|
| Clientes | Registro y gestión de los afiliados del gimnasio (Entrega 1) |
| Identity | Usuarios, roles y tokens JWT |
| Clases | Horario de clases, instructores y aforo |
| Membresías | Planes y vigencia de las membresías |
| Reservas | Reserva de cupos en las clases |
| Notificaciones | Correos a los afiliados |

Todos se exponen a través de un API Gateway y se comunican por eventos con RabbitMQ.

## Flujo de trabajo

El repositorio usa **Git Flow**: `main` guarda las versiones entregadas, `develop` integra el trabajo
del equipo y cada tarea se desarrolla en una rama `feature/*` que se fusiona a `develop` por pull request.
