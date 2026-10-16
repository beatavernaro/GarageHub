# O metrics-server fornece as métricas de CPU e memória dos pods,
# necessárias para o HorizontalPodAutoscaler da API.
resource "helm_release" "metrics_server" {
  name       = "metrics-server"
  repository = "https://kubernetes-sigs.github.io/metrics-server/"
  chart      = "metrics-server"
  version    = var.metrics_server_chart_version
  namespace  = "kube-system"

  wait    = true
  timeout = 300

  # Os kubelets do kind usam certificados autoassinados.
  # Aceitável apenas em ambiente local.
  values = [
    yamlencode({
      args = ["--kubelet-insecure-tls"]
    })
  ]
}
