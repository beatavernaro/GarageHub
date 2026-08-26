# Linguagem Ubíqua — GarageHub

A Linguagem Ubíqua define o vocabulário compartilhado utilizado no GarageHub entre negócio, documentação e código. Os termos abaixo devem possuir o mesmo significado em todos os contextos do sistema.

---

## Papéis

| Termo | Definição |
| --- | --- |
| **Cliente** | Pessoa física ou jurídica que possui ou é responsável por um veículo atendido pela oficina e que decide sobre a aprovação ou rejeição do orçamento. |
| **Atendente** | Profissional responsável pela identificação e cadastro de clientes e veículos, abertura e acompanhamento do atendimento, elaboração administrativa do orçamento e entrega do veículo. |
| **Mecânico** | Profissional responsável pela análise do veículo e pela execução e finalização dos serviços autorizados. |
| **Usuário** | Pessoa autorizada a acessar as funcionalidades administrativas do GarageHub mediante autenticação. |
| **Sistema** | Responsável pela execução automática das regras de negócio, cálculos, transições automáticas de status, geração da Ordem de Serviço e demais operações automatizadas. |

---

## Entidades e conceitos de negócio

| Termo | Definição |
| --- | --- |
| **Cliente** | Cadastro que representa uma pessoa física ou jurídica atendida pela oficina. |
| **Veículo** | Automóvel vinculado a um Cliente e que pode ser submetido aos serviços da oficina. |
| **Serviço** | Atividade oferecida pela oficina e que pode ser incluída em um Orçamento, como troca de óleo, alinhamento ou manutenção. |
| **Item de Estoque** | Termo utilizado para representar qualquer Peça ou Insumo controlado pelo estoque da oficina. |
| **Peça** | Item de Estoque que representa um componente físico utilizado ou substituído durante a execução de um serviço. |
| **Insumo** | Item de Estoque consumível utilizado durante a execução de um serviço, como óleo, fluidos ou outros materiais. |
| **Estoque** | Controle das quantidades disponíveis de Peças e Insumos da oficina. |
| **Orçamento** | Proposta elaborada para um Cliente e Veículo contendo os Serviços, Peças, Insumos, quantidades e valores previstos para o atendimento. |
| **Item do Orçamento** | Registro pertencente ao Orçamento que preserva as informações e valores do Serviço ou Item de Estoque no momento da elaboração. |
| **Ordem de Serviço (OS)** | Registro gerado após a aprovação de um Orçamento e utilizado para controlar a execução dos serviços autorizados pelo Cliente. |
| **Item da Ordem de Serviço** | Registro pertencente à OS que preserva as informações dos serviços e valores provenientes do Orçamento aprovado. |
| **Documento** | Identificação fiscal do Cliente, representada por CPF para Pessoa Física ou CNPJ para Pessoa Jurídica. |
| **Código Interno** | Identificador de negócio utilizado pela oficina para localizar Serviços e Itens de Estoque, distinto do identificador técnico do sistema. |
| **Preço** | Valor atualmente cadastrado para um Serviço ou Item de Estoque. |
| **Desconto** | Valor aplicado para redução do Valor Total de um Orçamento ou Ordem de Serviço. |
| **Quantidade** | Número de unidades de um item considerado em uma operação. |
| **Valor Unitário** | Valor registrado para uma unidade de determinado item no momento da operação. |
| **Valor Total** | Valor consolidado dos itens considerando quantidades, valores unitários e desconto aplicável. |
| **Snapshot** | Cópia dos dados relevantes de um item no momento de uma operação, utilizada para preservar o histórico mesmo que o cadastro original seja alterado posteriormente. |

---

## Identificação e cadastro

| Termo | Definição |
| --- | --- |
| **Identificar Cliente** | Localizar um Cliente já cadastrado utilizando seu CPF ou CNPJ. |
| **Cadastrar Cliente** | Registrar um novo Cliente no sistema. |
| **Identificar Veículo** | Localizar um Veículo já cadastrado utilizando seus dados de identificação. |
| **Cadastrar Veículo** | Registrar um novo Veículo no sistema. |
| **Vincular Veículo** | Associar um Veículo ao Cliente responsável por ele. |
| **Inativar Cadastro** | Tornar um Cliente, Serviço ou Item de Estoque indisponível para novas operações sem remover seu histórico do sistema. |

---

## Orçamento

| Termo | Definição |
| --- | --- |
| **Iniciar Orçamento** | Criar um novo Orçamento para um Cliente e Veículo. |
| **Elaborar Orçamento** | Definir os Serviços, Peças, Insumos, quantidades e valores necessários para o atendimento. |
| **Adicionar Serviço** | Incluir um Serviço no Orçamento. |
| **Adicionar Item de Estoque** | Incluir uma Peça ou Insumo no Orçamento. |
| **Alterar Quantidade** | Modificar a quantidade de determinado Item do Orçamento enquanto a alteração for permitida. |
| **Aplicar Desconto** | Registrar um desconto sobre o valor do Orçamento. |
| **Calcular Orçamento** | Determinar o Valor Total do Orçamento com base nos itens, quantidades, valores e desconto. |
| **Enviar Orçamento** | Finalizar sua elaboração e disponibilizá-lo para decisão do Cliente, passando-o para Aguardando Cliente. |
| **Aprovar Orçamento** | Registrar a decisão do Cliente de aceitar o Orçamento apresentado. |
| **Rejeitar Orçamento** | Registrar a decisão do Cliente de recusar o Orçamento apresentado. |
| **Cancelar Orçamento** | Interromper administrativamente um Orçamento antes da conclusão normal do seu fluxo. |
| **Expirar Orçamento** | Encerrar um Orçamento que permaneceu sem decisão do Cliente além do prazo permitido. |
| **Prazo do Orçamento** | Período durante o qual um Orçamento em Aguardando Cliente permanece disponível para decisão. |
| **Gerar Ordem de Serviço** | Criar automaticamente uma OS a partir de um Orçamento aprovado. |

---

## Estoque

| Termo | Definição |
| --- | --- |
| **Adicionar Estoque** | Aumentar manualmente a quantidade disponível de um Item de Estoque. |
| **Remover Estoque** | Reduzir manualmente a quantidade disponível de um Item de Estoque. |
| **Baixar Estoque** | Reduzir a quantidade disponível em decorrência da utilização de uma Peça ou Insumo no atendimento. |
| **Quantidade em Estoque** | Quantidade atualmente disponível de determinado Item de Estoque. |
| **Estoque Suficiente** | Situação em que existe quantidade disponível suficiente para atender à necessidade da operação. |
| **Estoque Insuficiente** | Situação em que a quantidade disponível é menor que a quantidade necessária. |
| **Atualizar Estoque** | Registrar a nova quantidade disponível após uma movimentação de estoque. |

---

## Ordem de Serviço e execução

| Termo | Definição |
| --- | --- |
| **Gerar Ordem de Serviço** | Criar automaticamente uma OS utilizando os dados de um Orçamento aprovado. |
| **Iniciar Serviço** | Registrar o início da execução de um Serviço pertencente à Ordem de Serviço. |
| **Executar Serviço** | Realizar efetivamente o trabalho autorizado no Veículo. |
| **Finalizar Serviço** | Registrar que a execução de determinado Serviço foi concluída. |
| **Iniciar Ordem de Serviço** | Transição automática da OS para Em Execução quando sua execução efetivamente começa. |
| **Finalizar Ordem de Serviço** | Alterar automaticamente a OS para Finalizada após a conclusão dos serviços necessários. |
| **Entregar Veículo** | Registrar a devolução do Veículo ao Cliente após a conclusão da Ordem de Serviço. |
| **Tempo de Execução** | Intervalo entre o início e a finalização da execução de um serviço. |
| **Tempo Médio de Execução** | Indicador calculado a partir do histórico de execuções para representar o tempo médio necessário para realização dos serviços. |

---

## Status do Orçamento

| Status | Definição |
| --- | --- |
| **Em Elaboração** | O Orçamento está sendo preparado e seus itens, quantidades e valores ainda podem ser definidos. |
| **Aguardando Cliente** | O Orçamento foi concluído e está aguardando a decisão do Cliente. |
| **Aprovado** | O Cliente aceitou o Orçamento, permitindo a geração da Ordem de Serviço. |
| **Rejeitado** | O Cliente recusou o Orçamento apresentado. |
| **Cancelado** | O Orçamento foi interrompido administrativamente antes da conclusão normal do fluxo. |
| **Expirado** | O prazo para decisão do Cliente terminou sem uma aprovação válida. |

---

## Status da Ordem de Serviço

| Status | Definição |
| --- | --- |
| **Aguardando Execução** | A Ordem de Serviço foi gerada a partir do Orçamento aprovado e ainda não teve sua execução iniciada. |
| **Em Execução** | A execução da Ordem de Serviço foi iniciada e existem serviços sendo realizados ou aguardando conclusão. |
| **Finalizada** | Todos os serviços necessários foram concluídos e o Veículo está pronto para entrega. |
| **Entregue** | O Veículo foi entregue ao Cliente, encerrando o ciclo da Ordem de Serviço. |

---

## Status do Serviço

| Status | Definição |
| --- | --- |
| **Aguardando Execução** | O Serviço pertence à Ordem de Serviço, mas ainda não foi iniciado. |
| **Em Execução** | O Serviço teve sua execução iniciada e ainda não foi concluído. |
| **Finalizada** | A execução do Serviço foi concluída. |

---

## Tipos

| Termo | Definição |
| --- | --- |
| **Pessoa Física** | Cliente identificado por CPF. |
| **Pessoa Jurídica** | Cliente identificado por CNPJ. |
| **Peça** | Tipo de Item de Estoque que representa um componente físico. |
| **Insumo** | Tipo de Item de Estoque que representa um material consumível. |

---

## Auditoria

| Termo | Definição |
| --- | --- |
| **Data de Criação** | Data e hora em que um registro foi criado. |
| **Criado Por** | Usuário responsável pela criação do registro. |
| **Data de Alteração** | Data e hora da última alteração realizada no registro. |
| **Alterado Por** | Usuário responsável pela última alteração do registro. |
| **Ativo** | Indica que o cadastro pode ser utilizado em novas operações. |
| **Inativo** | Indica que o cadastro foi desativado para novas operações, permanecendo disponível para preservação do histórico. |

---

## Eventos de negócio

| Termo | Definição |
| --- | --- |
| **Cliente Identificado** | Um Cliente existente foi localizado pelo sistema. |
| **Cliente Cadastrado** | Um novo Cliente foi registrado. |
| **Veículo Identificado** | Um Veículo existente foi localizado. |
| **Veículo Cadastrado** | Um novo Veículo foi registrado. |
| **Orçamento Criado** | Um novo Orçamento foi iniciado. |
| **Orçamento Calculado** | O Valor Total do Orçamento foi determinado. |
| **Orçamento Enviado** | O Orçamento passou a aguardar a decisão do Cliente. |
| **Orçamento Aprovado** | O Cliente autorizou o Orçamento. |
| **Orçamento Rejeitado** | O Cliente recusou o Orçamento. |
| **Orçamento Cancelado** | O Orçamento foi interrompido administrativamente. |
| **Orçamento Expirado** | O prazo para decisão terminou sem aprovação. |
| **Ordem de Serviço Gerada** | Uma OS foi criada como consequência da aprovação do Orçamento. |
| **Estoque Insuficiente Identificado** | O sistema identificou que a quantidade disponível não atende à quantidade necessária. |
| **Serviço Iniciado** | A execução de um Serviço começou. |
| **Estoque Atualizado** | A quantidade disponível de um Item de Estoque foi modificada. |
| **Serviço Finalizado** | A execução de um Serviço foi concluída. |
| **Ordem de Serviço Iniciada** | A OS entrou em execução. |
| **Ordem de Serviço Finalizada** | Todos os serviços necessários foram concluídos. |
| **Veículo Pronto para Entrega** | A execução foi concluída e o Veículo pode ser devolvido ao Cliente. |
| **Veículo Entregue** | O Veículo foi devolvido ao Cliente. |
| **Ordem de Serviço Entregue** | A OS foi encerrada após a entrega do Veículo. |

---

## Termos que não são sinônimos

| Termos | Distinção |
| --- | --- |
| **Orçamento / Ordem de Serviço** | Orçamento é a proposta apresentada ao Cliente; Ordem de Serviço controla a execução após a aprovação. |
| **Serviço / Execução do Serviço** | Serviço é a atividade definida pela oficina; execução é a realização dessa atividade em uma OS. |
| **Peça / Insumo** | Ambos são Itens de Estoque, mas Peça é um componente físico e Insumo é um material consumível. |
| **Item de Estoque / Item do Orçamento** | Item de Estoque é um cadastro da oficina; Item do Orçamento registra sua utilização e valores em uma proposta específica. |
| **Preço / Valor Unitário** | Preço é o valor atual do cadastro; Valor Unitário é o valor preservado em uma operação específica. |
| **Rejeitar / Cancelar** | Rejeição representa decisão do Cliente; cancelamento representa interrupção administrativa. |
| **Cancelar / Expirar** | Cancelamento ocorre por uma ação; expiração ocorre pelo término do prazo. |
| **Finalizar OS / Entregar Veículo** | Finalizar indica que os serviços terminaram; entregar indica que o Veículo foi efetivamente devolvido ao Cliente. |
| **Remover Estoque / Baixar Estoque** | Remoção representa movimentação manual; baixa representa consumo decorrente do atendimento. |
| **Inativo / Excluído** | Um registro inativo permanece no histórico; exclusão representaria sua remoção. |
| **Status do Serviço / Status da OS** | O Serviço possui seu próprio ciclo de execução, independente do estado geral da Ordem de Serviço. |
| **Código Interno / Id** | Código Interno possui significado para a oficina; Id é o identificador técnico utilizado pelo sistema. |