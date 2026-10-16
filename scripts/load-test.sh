#!/usr/bin/env bash
#
# Gera carga na API dentro do cluster para demonstrar o escalonamento automático (HPA).
#
# Executar no WSL, com a aplicação implantada (scripts/deploy-local.sh):
#   bash scripts/load-test.sh
#
# A carga é feita por um pod temporário que chama POST /api/Auth/login em paralelo.
# O login valida a senha com BCrypt, uma operação intensiva em CPU, o que eleva
# rapidamente o uso de CPU dos pods da API.
#
# Variáveis opcionais:
#   DURATION=180            duração da carga, em segundos
#   WORKERS=10              requisições simultâneas
#   SCALE_DOWN_WAIT=240     segundos acompanhando a redução após a carga (0 para não acompanhar)
#   INTERVAL=15             intervalo entre as leituras do HPA, em segundos

set -euo pipefail

CLUSTER_NAME="${CLUSTER_NAME:-garagehub}"
NAMESPACE="${NAMESPACE:-garagehub}"
DURATION="${DURATION:-180}"
WORKERS="${WORKERS:-10}"
SCALE_DOWN_WAIT="${SCALE_DOWN_WAIT:-240}"
INTERVAL="${INTERVAL:-15}"
LOAD_POD="garagehub-load-test"
KUBECTL=(kubectl --context "kind-${CLUSTER_NAME}" -n "${NAMESPACE}")

# Executado dentro do pod de carga (busybox)
LOAD_SCRIPT='
end=$(( $(date +%s) + DURATION ))
body="{\"email\":\"admin@garagehub.com\",\"senha\":\"Admin@123\"}"
i=0
while [ "$i" -lt "$WORKERS" ]; do
  (
    while [ "$(date +%s)" -lt "$end" ]; do
      wget -q -O /dev/null \
        --header "Content-Type: application/json" \
        --post-data "$body" \
        http://garagehub-api:8080/api/Auth/login || true
    done
  ) &
  i=$((i + 1))
done
wait
'

status() {
  echo
  echo "--- $(date +%H:%M:%S) ($1)"
  "${KUBECTL[@]}" get hpa garagehub-api
  "${KUBECTL[@]}" top pods -l app.kubernetes.io/name=garagehub-api 2>/dev/null || true
}

cleanup() {
  "${KUBECTL[@]}" delete pod "${LOAD_POD}" --ignore-not-found --wait=false >/dev/null
}
trap cleanup EXIT

"${KUBECTL[@]}" get hpa garagehub-api >/dev/null

cleanup
"${KUBECTL[@]}" wait --for=delete "pod/${LOAD_POD}" --timeout=60s >/dev/null 2>&1 || true

echo "==> Iniciando carga: ${WORKERS} requisições simultâneas por ${DURATION}s"
"${KUBECTL[@]}" run "${LOAD_POD}" \
  --image=busybox:1.37.0 \
  --restart=Never \
  --env="DURATION=${DURATION}" \
  --env="WORKERS=${WORKERS}" \
  --command -- sh -c "${LOAD_SCRIPT}" >/dev/null

status "início"

elapsed=0
while [[ "${elapsed}" -lt "${DURATION}" ]]; do
  sleep "${INTERVAL}"
  elapsed=$((elapsed + INTERVAL))
  status "carga: ${elapsed}s"
done

cleanup
echo
echo "==> Carga encerrada"

elapsed=0
while [[ "${elapsed}" -lt "${SCALE_DOWN_WAIT}" ]]; do
  sleep "${INTERVAL}"
  elapsed=$((elapsed + INTERVAL))
  status "após a carga: ${elapsed}s"
done

echo
echo "==> Eventos de escalonamento"
"${KUBECTL[@]}" describe hpa garagehub-api | sed -n '/^Events:/,$p'
