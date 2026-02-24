# Torvalds Reminder

## Descripción del Proyecto

Demostración sistema de recordatorio de facturas vencidas construido con **.NET 10**, **MongoDB** y **Angular**.

## ¿Qué hace?

Al ejecutar el proceso de recordatorio, el sistema revisa cada factura pendiente y aplica la siguiente transición:

| Estado actual | Email enviado | Estado nuevo |
|---|---|---|
| `primerrecordatorio` | *"Su factura ha pasado a segundo recordatorio"* | `segundorecordatorio` |
| `segundorecordatorio` | *"Su factura será desactivada"* | `desactivado` |

El frontend muestra un resumen de todas las facturas y expone un botón para ejecutar el proceso y otro para resetear los datos a su estado inicial.

---

## Stack

| Capa | Tecnología |
|---|---|
| Backend | ASP.NET Core (.NET 10) |
| Base de datos | MongoDB 8 |
| Frontend | Angular |
| Email (local) | Mailpit |
| Orquestación | Docker Compose |

---

## Arquitectura

El backend sigue **Clean Architecture**, separando responsabilidades en cuatro capas:

- **Domain** — Entidades (`Client`, `Invoice`) y enums (`InvoiceStatus`). Sin dependencias externas.
- **Application** — Interfaces de repositorios y servicios. Define contratos sin saber cómo se implementan.
- **Infrastructure** — Implementaciones concretas: MongoDB, MailKit y el seeder de data para cuando el proyecto se arranca por primera vez.
- **API** — Controllers REST, inyección de dependencias y configuración del servidor.

El frontend usa un **proxy de desarrollo** (`proxy.conf.json`) para redirigir las llamadas a `/api` al backend, evitando problemas de CORS (dentro de la misma instancia de red de Docker).

Este proyecto se encuentra aislado en una instancia de Docker para evitar conflictos con el entorno local y garantizar que el comportamiento sea consistente en cualquier máquina. No se requiere instalar .NET, Node.js ni MongoDB, con tener Docker es suficiente para levantar todo el sistema.

---

## Pasos para levantar el proyecto

### Prerrequisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Pasos

```bash
# 1. Clonar el repositorio
git clone https://github.com/Natsume-197/TorvaldsReminder
cd TorvaldsReminder

# 2. Crear el archivo de entorno
cp .env.example .env

# 3. Levantar todo
docker compose up --build
```

### URLs disponibles

| Servicio | URL |
|---|---|
| Frontend | http://localhost:4200 |
| API + Swagger (docs) | http://localhost:5000/swagger |
| Mailpit (emails) | http://localhost:8025 |

> Importante: los emails enviados por la aplicación son capturados por Mailpit y nunca llegan a ningún destinatario real.

---

## Datos de prueba

Al arrancar la API por primera vez, el `DatabaseSeeder` inserta automáticamente 3 clientes y 6 facturas en distintos estados. El proceso es **idempotente**: si los datos ya existen, no inserta nada.

Para resetear los datos a su estado inicial desde el frontend, usa el botón **Reset Data**, que internamente llama a:

```
POST /api/seed/reset
```

Para un reset completo eliminando el volumen de Mongo:

```bash
docker compose down -v && docker compose up --build
```

---

## Tests

```bash
cd backend
dotnet test
```

Los tests cubren la lógica del `InvoiceProcessingService` usando mocks (xUnit + Moq + FluentAssertions), sin tocar MongoDB ni enviar emails reales.

