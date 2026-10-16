# Cluster Kubernetes local executado em containers Docker (kind).
# O NodePort da API é publicado no host para permitir acesso via localhost.
resource "kind_cluster" "this" {
  name           = var.cluster_name
  node_image     = var.node_image
  wait_for_ready = true

  # Registra o contexto kind-<nome> no kubeconfig padrão do usuário. Sem este
  # valor, o provider exporta uma cópia com credenciais para o diretório atual.
  kubeconfig_path = pathexpand("~/.kube/config")

  kind_config {
    kind        = "Cluster"
    api_version = "kind.x-k8s.io/v1alpha4"

    node {
      role = "control-plane"

      extra_port_mappings {
        container_port = var.api_node_port
        host_port      = var.api_node_port
        protocol       = "TCP"
      }
    }

    dynamic "node" {
      for_each = range(var.worker_count)

      content {
        role = "worker"
      }
    }
  }
}
