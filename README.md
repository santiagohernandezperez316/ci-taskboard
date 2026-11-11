# TaskBoard — Proyecto de Integración Continua

Proyecto del módulo **Énfasis Profesional I (Integración Continua)** — Politécnico Grancolombiano.
Autor: **Santiago Hernández Pérez**.

API REST de gestión de tareas (.NET 8 + PostgreSQL 16) desplegada en **dos contenedores Docker comunicados entre sí**.

## Arquitectura

| Contenedor      | Imagen                         | Rol                         | Puerto                      |
|-----------------|--------------------------------|-----------------------------|-----------------------------|
| `taskboard-api` | `taskboard-api:1.0` (propia)   | API REST .NET 8             | 8080 publicado al host      |
| `taskboard-db`  | `postgres:16-alpine` (oficial) | Base de datos               | 5432 solo en la red interna |

Ambos contenedores comparten la red bridge `taskboard-net`. La API se conecta a la base usando el
nombre de servicio `db`, que resuelve el DNS interno de Docker. Los datos persisten en el volumen `taskboard-pgdata`.

## Requisitos

- Docker Desktop (o Docker Engine) con Docker Compose v2
- .NET 8 SDK (solo para compilar o ejecutar pruebas fuera de Docker)

## Ejecución

```bash
cp .env.example .env              # ajustar la clave si se desea
docker compose up -d --build
docker compose ps                 # ambos deben quedar "healthy"
```

## Verificar la comunicación entre contenedores

```bash
./scripts/verify-communication.sh
```

El script muestra la red compartida, la resolución DNS de `db` desde `api`, la conexión TCP al
puerto 5432 y una tarea creada por la API que luego se consulta directamente en PostgreSQL.

Prueba de persistencia:

```bash
docker compose down               # elimina contenedores, conserva el volumen
docker compose up -d
curl http://localhost:8080/api/tasks   # la tarea sigue ahí
```

## Endpoints

| Método | Ruta                       | Descripción                         |
|--------|----------------------------|-------------------------------------|
| GET    | `/`                        | Info del servicio y hostname        |
| GET    | `/health`                  | Estado de la API y de la conexión a BD |
| GET    | `/api/tasks`               | Listar tareas                       |
| GET    | `/api/tasks/{id}`          | Obtener una tarea                   |
| POST   | `/api/tasks`               | Crear tarea `{ "title", "description" }` |
| PUT    | `/api/tasks/{id}/complete` | Marcar como completada              |
| DELETE | `/api/tasks/{id}`          | Eliminar tarea                      |

## Pruebas unitarias

```bash
dotnet test
```

## Limpieza

```bash
docker compose down -v            # -v elimina también el volumen de datos
```

## Hoja de ruta del módulo

- [x] Entrega 1 (semana 3): repositorio en GitHub + dos contenedores Docker comunicados
- [ ] Entrega 2 (semana 5): pipeline en Jenkins
- [ ] Entrega 3 (semanas 7-8): integración con Travis CI y Codeship
