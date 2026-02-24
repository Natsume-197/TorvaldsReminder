# Torvalds Reminder

## Descripción del Proyecto

Demostración sistema de recordatorio de facturas vencidas construido con **.NET 10**, **MongoDB** y **Angular**.

<img width="2560" height="1294" alt="image" src="https://github.com/user-attachments/assets/730a9892-5d28-4cbe-9281-0293a11ec400" />
<img width="2560" height="1294" alt="image" src="https://github.com/user-attachments/assets/2158b473-52a5-462d-9a78-2fa8f96fb1c7" />

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

El backend está estructurado siguiendo los principios de **Clean Architecture**, donde cada capa tiene una responsabilidad única y las dependencias siempre fluyen hacia adentro: desde la capa más externa (API) hacia el núcleo (Domain), nunca al revés.

```
API → Infrastructure → Application → Domain
```

#### Domain
El núcleo del sistema. Contiene las entidades de negocio (`Client`, `Invoice`) y los enums (`InvoiceStatus`). No tiene ninguna dependencia externa, lo que garantiza que la lógica central del negocio sea completamente independiente de frameworks, bases de datos o cualquier detalle de implementación.

#### Application
Define los contratos del sistema a través de interfaces (`IInvoiceRepository`, `IClientRepository`, `IEmailService`) y orquesta el flujo de negocio en `InvoiceProcessingService`. Esta capa sabe *qué* debe ocurrir, pero no *cómo*: desconoce si los datos vienen de MongoDB, PostgreSQL o un archivo en disco.

#### Infrastructure
Implementa los contratos definidos por Application. Aquí viven los repositorios de MongoDB, el servicio de email con MailKit y el `DatabaseSeeder` que precarga la base de datos al arrancar la API por primera vez. Es la única capa que conoce los detalles de las tecnologías externas.

#### API
Punto de entrada del sistema. Expone los endpoints REST, configura la inyección de dependencias en `Program.cs` y define la serialización JSON. Gracias a la inyección de dependencias, ningún controller tiene conocimiento directo de las implementaciones concretas — solo trabaja con interfaces.

### Frontend

Angular se comunica con el backend a través de un **proxy de desarrollo** (`proxy.conf.json`) que redirige las llamadas a `/api` al servicio de backend dentro de la red Docker, eliminando cualquier problema de CORS sin necesidad de configuraciones adicionales en el servidor.

El frontend usa un **proxy de desarrollo** (`proxy.conf.json`) para redirigir las llamadas a `/api` al backend, evitando problemas de CORS (dentro de la misma instancia de red de Docker).

### Contenedores

Este proyecto se encuentra completamente aislado en Docker para evitar conflictos con el entorno local y garantizar un comportamiento consistente en cualquier máquina. No se requiere tener .NET, Node.js ni MongoDB instalados, con Docker es suficiente para levantar todo el sistema. Esto también facilitará el tema de los despliegues en la nube de ser requerido.

Todos los servicios se comunican a través de una red interna, usando los nombres de los contenedores como hostnames (`mongo`, `api`, `mailpit`, `frontend`).

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

Los tests validan exclusivamente la **lógica de negocio** de `InvoiceProcessingService`, que es el corazón del sistema. Para ello se usan mocks de las tres dependencias del servicio (`IInvoiceRepository`, `IClientRepository`, `IEmailService`), lo que permite probar el comportamiento en completo aislamiento sin necesidad de la base de datos, red o emails reales.

```bash
cd backend
dotnet test
```

Las pruebas están escritas con **xUnit**, **Moq** y **FluentAssertions**, siguiendo el patrón **Arrange / Act / Assert**.

### Casos cubiertos

| Test | Qué verifica |
|---|---|
| `PrimerRecordatorio_ActualizaASegundoYEnviaEmail` | Que una factura en primer recordatorio dispara el email correcto y actualiza el estado a `SegundoRecordatorio` |
| `SegundoRecordatorio_ActualizaADesactivadoYEnviaEmail` | Que una factura en segundo recordatorio dispara el email correcto y actualiza el estado a `Desactivado` |
| `ClienteNoEncontrado_NadaOcurre` | Que si el cliente asociado a una factura no existe, el proceso lo omite silenciosamente sin enviar emails ni modificar estados |
