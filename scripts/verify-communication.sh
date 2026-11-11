#!/usr/bin/env bash
# Ejecuta las pruebas de comunicación entre los contenedores api y db.
# Uso (desde la raíz del repo, con los contenedores arriba): ./scripts/verify-communication.sh
set -euo pipefail

if [ -f .env ]; then set -a; source .env; set +a; fi
API="http://localhost:${API_PORT:-8080}"

step() { echo; echo "================ $1 ================"; }

step "1. Contenedores en ejecucion"
docker compose ps

step "2. Ambos contenedores en la red taskboard-net"
docker network inspect taskboard-net \
  --format '{{range .Containers}}{{.Name}}  ->  {{.IPv4Address}}{{"\n"}}{{end}}'

step "3. DNS interno: el contenedor api resuelve el nombre 'db'"
docker compose exec -T api getent hosts db

step "4. Conexion TCP desde api hacia db:5432"
docker compose exec -T api bash -c 'timeout 3 bash -c "</dev/tcp/db/5432" && echo "OK: puerto 5432 de db alcanzable desde api"'

step "5. Health check de la API (consulta la base de datos)"
curl -s "$API/health"; echo

step "6. Crear una tarea via API"
curl -s -X POST "$API/api/tasks" \
  -H "Content-Type: application/json" \
  -d '{"title":"Entrega 1 - Docker","description":"Dos contenedores comunicados"}'; echo

step "7. Listar tareas via API"
curl -s "$API/api/tasks"; echo

step "8. El mismo registro, consultado directamente en PostgreSQL"
docker compose exec -T db psql -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" \
  -c 'SELECT "Id", "Title", "IsCompleted", "CreatedAtUtc" FROM tasks ORDER BY "Id";'
