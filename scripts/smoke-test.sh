#!/usr/bin/env bash
#
# Verificação rápida da API implantada: health checks, autenticação e um endpoint protegido.
# Usado pelo pipeline de CI/CD após o deploy e também pode ser executado localmente:
#   bash scripts/smoke-test.sh
#
# Variáveis opcionais:
#   BASE_URL=http://localhost:30080
#   ADMIN_EMAIL / ADMIN_PASSWORD   credenciais do usuário criado pelo seed

set -euo pipefail

BASE_URL="${BASE_URL:-http://localhost:30080}"
ADMIN_EMAIL="${ADMIN_EMAIL:-admin@garagehub.com}"
ADMIN_PASSWORD="${ADMIN_PASSWORD:-Admin@123}"

falhas=0

verificar() {
  local descricao="$1" esperado="$2" obtido="$3"

  if [[ "${obtido}" == "${esperado}" ]]; then
    echo "OK    ${descricao} (${obtido})"
  else
    echo "FALHA ${descricao}: esperado ${esperado}, obtido ${obtido}"
    falhas=$((falhas + 1))
  fi
}

status_http() {
  curl -s -o /dev/null -w "%{http_code}" --max-time 10 "$@" || true
}

echo "==> Smoke test em ${BASE_URL}"

verificar "GET /health/live" 200 "$(status_http "${BASE_URL}/health/live")"
verificar "GET /health/ready" 200 "$(status_http "${BASE_URL}/health/ready")"
verificar "GET /swagger/index.html" 200 "$(status_http "${BASE_URL}/swagger/index.html")"
verificar "GET /api/Clientes sem token" 401 "$(status_http "${BASE_URL}/api/Clientes")"

resposta_login="$(curl -s --max-time 10 -X POST "${BASE_URL}/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"${ADMIN_EMAIL}\",\"senha\":\"${ADMIN_PASSWORD}\"}" || true)"

token="$(sed -n 's/.*"token":"\([^"]*\)".*/\1/p' <<<"${resposta_login}")"

if [[ -n "${token}" ]]; then
  echo "OK    POST /api/Auth/login (token recebido)"
  verificar "GET /api/Clientes com token" 200 \
    "$(status_http -H "Authorization: Bearer ${token}" "${BASE_URL}/api/Clientes")"
  verificar "GET /api/OrdensServico com token" 200 \
    "$(status_http -H "Authorization: Bearer ${token}" "${BASE_URL}/api/OrdensServico")"
else
  echo "FALHA POST /api/Auth/login: token não retornado"
  falhas=$((falhas + 1))
fi

if [[ "${falhas}" -gt 0 ]]; then
  echo "==> ${falhas} verificação(ões) falharam"
  exit 1
fi

echo "==> Todas as verificações passaram"
