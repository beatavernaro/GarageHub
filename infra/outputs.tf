output "cluster_name" {
  description = "Nome do cluster kind."
  value       = kind_cluster.this.name
}

output "kubectl_context" {
  description = "Contexto adicionado ao kubeconfig (~/.kube/config)."
  value       = "kind-${kind_cluster.this.name}"
}

output "cluster_endpoint" {
  description = "Endpoint da API do Kubernetes."
  value       = kind_cluster.this.endpoint
}

output "namespace" {
  description = "Namespace da aplicação e do banco."
  value       = kubernetes_namespace_v1.garagehub.metadata[0].name
}

output "db_storage_class" {
  description = "StorageClass usada pelo volume do PostgreSQL."
  value       = kubernetes_storage_class_v1.database.metadata[0].name
}

output "db_init_config_map" {
  description = "ConfigMap com os scripts de inicialização do banco."
  value       = kubernetes_config_map_v1.database_init.metadata[0].name
}

output "api_url" {
  description = "URL da API após o deploy dos manifestos de k8s/."
  value       = "http://localhost:${var.api_node_port}"
}
