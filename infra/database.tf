# Infraestrutura necessária para o PostgreSQL no cluster.
# O StatefulSet do banco (k8s/postgres-statefulset.yaml) consome estes recursos pelos nomes abaixo.

locals {
  db_storage_class_name   = "garagehub-db"
  db_init_config_map_name = "garagehub-db-init"

  # Os scripts versionados em src/ são a única fonte do schema e do seed,
  # compartilhados com o docker-compose e com o CI.
  db_scripts_dir = "${path.module}/../src/Infrastructure/Database"
}

resource "kubernetes_namespace_v1" "garagehub" {
  metadata {
    name = var.namespace

    labels = {
      "app.kubernetes.io/part-of" = "garagehub"
    }
  }
}

# StorageClass dedicada ao banco: usa o provisionador local do kind e mantém
# o volume (Retain) mesmo que o PersistentVolumeClaim seja removido por engano.
resource "kubernetes_storage_class_v1" "database" {
  metadata {
    name = local.db_storage_class_name
  }

  storage_provisioner = "rancher.io/local-path"
  reclaim_policy      = "Retain"
  volume_binding_mode = "WaitForFirstConsumer"
}

# Scripts de criação e carga inicial, montados em /docker-entrypoint-initdb.d.
# O PostgreSQL só os executa quando o volume de dados está vazio.
resource "kubernetes_config_map_v1" "database_init" {
  metadata {
    name      = local.db_init_config_map_name
    namespace = kubernetes_namespace_v1.garagehub.metadata[0].name

    labels = {
      "app.kubernetes.io/name"    = "garagehub-db"
      "app.kubernetes.io/part-of" = "garagehub"
    }
  }

  data = {
    for script in fileset(local.db_scripts_dir, "*.sql") :
    script => file("${local.db_scripts_dir}/${script}")
  }
}
