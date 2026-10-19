#!/usr/bin/env bash
#
# Deploy da API e do PostgreSQL no cluster kind local.
#
# Executar no WSL, a partir da raiz do repositório, depois do terraform apply:
#   bash scripts/deploy-local.sh
#
# Etapas: build da imagem -> carga no kind -> kubectl apply -k k8s/ -> aguarda os rollouts.

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CLUSTER_NAME="${CLUSTER_NAME:-garagehub}"
NAMESPACE="${NAMESPACE:-garagehub}"
IMAGE="garagehub-api:local"
KUBECTL=(kubectl --context "kind-${CLUSTER_NAME}" -n "${NAMESPACE}")

if [[ ! -f "${ROOT_DIR}/k8s/secrets.env" ]]; then
  echo "Arquivo k8s/secrets.env não encontrado." >&2
  echo "Crie-o a partir do modelo e defina valores próprios:" >&2
  echo "  cp k8s/secrets.env.example k8s/secrets.env" >&2
  exit 1
fi

if ! kind get clusters 2>/dev/null | grep -qx "${CLUSTER_NAME}"; then
  echo "Cluster kind '${CLUSTER_NAME}' não encontrado. Execute antes: terraform -chdir=infra apply" >&2
  exit 1
fi

echo "==> Build da imagem ${IMAGE}"
docker build -t "${IMAGE}" "${ROOT_DIR}"

echo "==> Carregando a imagem nos nós do cluster ${CLUSTER_NAME}"
kind load docker-image "${IMAGE}" --name "${CLUSTER_NAME}"

# A tag local é sempre a mesma, então em um redeploy o Deployment não muda
# e é preciso reiniciar os pods para que usem a imagem recém-carregada.
api_ja_existia=false
if "${KUBECTL[@]}" get deployment garagehub-api >/dev/null 2>&1; then
  api_ja_existia=true
fi

echo "==> Aplicando os manifestos de k8s/"
"${KUBECTL[@]}" apply -k "${ROOT_DIR}/k8s"

if [[ "${api_ja_existia}" == true ]]; then
  "${KUBECTL[@]}" rollout restart deployment/garagehub-api
fi

echo "==> Aguardando o PostgreSQL"
"${KUBECTL[@]}" rollout status statefulset/garagehub-db --timeout=300s

echo "==> Aguardando o Mailpit"
"${KUBECTL[@]}" rollout status deployment/garagehub-mailpit --timeout=300s

echo "==> Aguardando a API"
"${KUBECTL[@]}" rollout status deployment/garagehub-api --timeout=300s

"${KUBECTL[@]}" get pods,services

echo
echo "API disponível em http://localhost:30080 (Swagger: http://localhost:30080/swagger)"
echo "Mailpit (e-mails enviados): kubectl --context kind-${CLUSTER_NAME} -n ${NAMESPACE} port-forward svc/garagehub-mailpit 8025:8025"
echo "  e acesse http://localhost:8025"
