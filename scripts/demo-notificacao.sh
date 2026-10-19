#!/usr/bin/env bash
#
# Demonstração das notificações por e-mail de alteração de status da Ordem de Serviço.
#
# Abre uma OS com dados do seed, altera o status (Em execução -> Finalizada -> Entregue)
# e confirma, pela API do Mailpit, que um e-mail foi recebido para cada alteração.
#
#   Docker Compose: BASE_URL=http://localhost:8080 bash scripts/demo-notificacao.sh
#   Kubernetes:     kubectl -n garagehub port-forward svc/garagehub-mailpit 8025:8025
#                   bash scripts/demo-notificacao.sh
#
# Variáveis opcionais:
#   BASE_URL=http://localhost:30080
#   MAILPIT_URL=http://localhost:8025
#   ADMIN_EMAIL / ADMIN_PASSWORD   credenciais do usuário criado pelo seed
#
# Requer curl e python3 (usado para ler as respostas JSON).

set -euo pipefail

BASE_URL="${BASE_URL:-http://localhost:30080}"
MAILPIT_URL="${MAILPIT_URL:-http://localhost:8025}"
ADMIN_EMAIL="${ADMIN_EMAIL:-admin@garagehub.com}"
ADMIN_PASSWORD="${ADMIN_PASSWORD:-Admin@123}"

# Lê um campo de um JSON recebido pela entrada padrão. Ex.: json 'd["token"]'
json() {
  python3 -c "import json, sys; d = json.load(sys.stdin); print($1)"
}

api() {
  curl -sf --max-time 15 \
    -H "Authorization: Bearer ${token}" \
    -H "Content-Type: application/json" \
    "$@"
}

echo "==> Autenticando em ${BASE_URL}"
token="$(curl -sf --max-time 15 -X POST "${BASE_URL}/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"${ADMIN_EMAIL}\",\"senha\":\"${ADMIN_PASSWORD}\"}" \
  | json 'd["token"]')"

echo "==> Selecionando cliente, veículo e serviço do seed"
cliente_id="$(api "${BASE_URL}/api/Clientes" \
  | json 'next(c["id"] for c in d if c["ativo"])')"

veiculo_id="$(api "${BASE_URL}/api/Veiculos/cliente/${cliente_id}" \
  | json 'next(v["id"] for v in d if v["ativo"])')"

servico_id="$(api "${BASE_URL}/api/Servicos" \
  | json 'next(s["id"] for s in d if s["ativo"])')"

echo "==> Abrindo a ordem de serviço"
os_id="$(api -X POST "${BASE_URL}/api/OrdensServico" \
  -d "{\"clienteId\":\"${cliente_id}\",\"veiculoId\":\"${veiculo_id}\",\"servicos\":[{\"servicoId\":\"${servico_id}\",\"quantidade\":1}]}" \
  | json 'd["ordemServicoId"]')"

os_servico_id="$(api "${BASE_URL}/api/OrdensServico/${os_id}" \
  | json 'd["servicos"][0]["id"]')"

echo "    OS ${os_id}"

for status in EmExecucao Finalizada; do
  echo "==> Alterando o serviço para ${status}"
  api -X PATCH "${BASE_URL}/api/OrdensServico/${os_id}/servicos/${os_servico_id}/status" \
    -d "\"${status}\"" >/dev/null
done

echo "==> Entregando a ordem de serviço"
api -X POST "${BASE_URL}/api/OrdensServico/${os_id}/entregar" >/dev/null

echo "==> E-mails recebidos no Mailpit (${MAILPIT_URL}) para a OS ${os_id}"
# O envio ocorre durante a requisição, mas aguarda um instante para o Mailpit indexar
sleep 1

recebidos="$(curl -sf --max-time 15 "${MAILPIT_URL}/api/v1/messages?limit=200" \
  | OS_ID="${os_id}" python3 -c '
import json, os, sys
mensagens = [m for m in json.load(sys.stdin)["messages"] if os.environ["OS_ID"] in m["Subject"]]
for m in reversed(mensagens):
    print("    " + ", ".join(t["Address"] for t in m["To"]) + " | " + m["Subject"], file=sys.stderr)
print(len(mensagens))
')"

if [[ "${recebidos}" -ne 3 ]]; then
  echo "FALHA: esperados 3 e-mails para a OS, recebidos ${recebidos}" >&2
  exit 1
fi

echo "==> OK: 3 notificações recebidas. Veja as mensagens em ${MAILPIT_URL}"
