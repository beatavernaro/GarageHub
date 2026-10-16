variable "cluster_name" {
  description = "Nome do cluster kind. O contexto do kubectl será kind-<cluster_name>."
  type        = string
  default     = "garagehub"
}

variable "node_image" {
  description = "Imagem dos nós do kind (versão do Kubernetes), fixada por digest para reprodutibilidade. Deve ser compatível com a versão do kind embutida no provider tehcyx/kind 0.11.x (kind v0.31.0)."
  type        = string
  default     = "kindest/node:v1.35.0@sha256:452d707d4862f52530247495d180205e029056831160e22870e37e3f6c1ac31f"
}

variable "worker_count" {
  description = "Quantidade de nós worker além do control-plane."
  type        = number
  default     = 1
}

variable "api_node_port" {
  description = "NodePort do Service da API, publicado no host (localhost:<porta>). Deve ser o mesmo de k8s/api-service.yaml."
  type        = number
  default     = 30080
}

variable "namespace" {
  description = "Namespace onde a aplicação e o banco são implantados. Deve ser o mesmo de k8s/kustomization.yaml."
  type        = string
  default     = "garagehub"
}

variable "metrics_server_chart_version" {
  description = "Versão do chart Helm do metrics-server (necessário para o HPA)."
  type        = string
  default     = "3.14.0"
}
