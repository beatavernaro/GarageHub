# GarageHub
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-13.0-239120?logo=csharp)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-4169E1?logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?logo=docker&logoColor=white)
![Tests](https://img.shields.io/badge/Tests-326%20passing-brightgreen)
![Coverage](https://img.shields.io/badge/Coverage-96.7%25-brightgreen)
![Quality Gate](https://img.shields.io/badge/Quality%20Gate-Passed-brightgreen)
![Security](https://img.shields.io/badge/Security-A-brightgreen)

GarageHub é um MVP de back-end para gestão de oficinas mecânicas, desenvolvido como parte do Tech Challenge da Pós Tech em Software Architecture da FIAP.

GarageHub é um MVP de back-end para gestão de oficinas mecânicas, desenvolvido como parte do Tech Challenge da Pós Tech em Software Architecture da FIAP.

O sistema foi criado para centralizar e organizar processos que normalmente poderiam depender de anotações manuais ou planilhas, como cadastro de clientes e veículos, controle de peças e insumos, elaboração de orçamentos e acompanhamento da execução dos serviços.

O objetivo principal do projeto é oferecer uma base estruturada para o gerenciamento do fluxo de atendimento da oficina, desde a identificação do cliente e do veículo até a aprovação do orçamento, geração da ordem de serviço, execução dos serviços e entrega do veículo.

## 📑Sumário

- [Como executar o projeto](#como-executar-o-projeto)
- [Autenticação e acesso à API](#autenticação-e-acesso-à-api)
- [Funcionalidades e fluxos de negócio](#funcionalidades-e-fluxos-de-negócio)
- [Arquitetura e Domain-Driven Design](#arquitetura-e-domain-driven-design)
- [Estrutura do projeto](#estrutura-do-projeto)
- [Banco de dados e persistência](#banco-de-dados-e-persistência)
- [Tecnologias utilizadas](#tecnologias-utilizadas)
- [API REST e documentação](#api-rest-e-documentação)
- [Segurança e validações](#segurança-e-validações)
- [Docker](#docker)
- [Testes e cobertura](#testes-e-cobertura)
- [Qualidade de código e SonarQube](#qualidade-de-código-e-sonarqube)
- [CI com GitHub Actions](#ci-com-github-actions)
- [Decisões técnicas, premissas e limitações do MVP](#decisões-técnicas-premissas-e-limitações-do-mvp)
- [Autora](#autora)

## Principais funcionalidades

- Cadastro e gerenciamento de clientes
- Cadastro e gerenciamento de veículos
- Cadastro e gerenciamento de serviços
- Cadastro de peças e insumos
- Controle de estoque
- Criação e gerenciamento de orçamentos
- Inclusão de serviços, peças e insumos no orçamento
- Aprovação, rejeição, cancelamento e expiração de orçamentos
- Geração automática da Ordem de Serviço após a aprovação do orçamento
- Controle do status dos serviços executados
- Atualização automática do status da Ordem de Serviço
- Consulta do andamento da Ordem de Serviço
- Monitoramento do tempo médio de execução dos serviços
- Autenticação administrativa utilizando JWT
- Validação de CPF, CNPJ e placa de veículos
- Documentação REST por meio de Swagger/OpenAPI
- Testes unitários e de integração
- Análise estática de código e segurança com SonarQube Cloud

## Objetivos técnicos

Além das funcionalidades de negócio, o projeto busca aplicar conceitos e práticas relacionados a:

- Domain-Driven Design (DDD)
- Clean Architecture
- Separação de responsabilidades
- Regras de domínio centralizadas
- APIs RESTful
- Persistência utilizando PostgreSQL e Dapper
- Autenticação e autorização
- Testes automatizados
- Conteinerização com Docker
- Integração contínua com GitHub Actions
- Qualidade e segurança de código

## 🚀 Como executar o projeto

O GarageHub foi configurado para que todo o ambiente necessário seja inicializado utilizando Docker Compose. A aplicação e o banco de dados PostgreSQL são executados em containers, e os scripts de criação e carga inicial do banco são executados automaticamente.

### Pré-requisitos

Para executar o projeto é necessário ter instalado:

- Git
- Docker
- Docker Compose

Não é necessário instalar localmente o .NET ou o PostgreSQL para executar a aplicação utilizando Docker.

### 1. Clonar o repositório

```bash
git clone https://github.com/beatavernaro/GarageHub.git
```

Acesse a pasta do projeto:

```bash
cd GarageHub
```

### 2. Subir o ambiente

Execute:

```bash
docker compose up --build
```

O Docker Compose será responsável por:

- Construir a imagem da API;
- Inicializar o PostgreSQL;
- Criar o banco de dados;
- Executar os scripts de criação das tabelas;
- Executar os dados iniciais (seed);
- Iniciar a API;
- Configurar a comunicação entre a API e o banco de dados.

Após a inicialização, o ambiente estará pronto para utilização sem necessidade de configurações adicionais.

### 3. Acessar a API

A documentação Swagger/OpenAPI pode ser acessada em:

```text
http://localhost:8080/swagger
```

### 4. Encerrar o ambiente

Para interromper os containers e remover também os volumes, incluindo os dados armazenados pelo PostgreSQL:

```bash
docker compose down -v
```

## 🔐 Autenticação e acesso à API

O GarageHub utiliza autenticação baseada em **JWT (JSON Web Token)** para proteger as operações administrativas da API.

Os endpoints administrativos exigem que o usuário esteja autenticado. Após o login, a API retorna um token JWT que deve ser enviado nas requisições protegidas utilizando o esquema `Bearer`.

### Login

A autenticação é realizada através do endpoint:

```http
POST /api/Auth/login
```

Para facilitar a execução e avaliação do projeto, o banco de dados é inicializado com usuários de desenvolvimento através do script de seed.

### Usuário administrador

```text
E-mail: admin@garagehub.com
Senha: Admin@123
```

> As credenciais acima são destinadas exclusivamente ao ambiente acadêmico e de desenvolvimento. Em um ambiente de produção, credenciais e secrets não devem ser armazenados diretamente em arquivos versionados no repositório.

### Autenticação pelo Swagger

Para acessar os endpoints protegidos através do Swagger:

1. Acesse `http://localhost:8080/swagger`.
2. Execute o endpoint `POST /api/Auth/login` utilizando as credenciais de desenvolvimento.
3. Copie o token JWT retornado pela API.
4. Clique no botão **Authorize** disponível no Swagger.
5. Informe o token no campo de autenticação.
6. Após a autorização, os endpoints administrativos poderão ser executados normalmente.

### Proteção dos endpoints

A API diferencia operações administrativas de operações que precisam ser acessíveis pelo cliente.

As funcionalidades administrativas, como gerenciamento de clientes, veículos, serviços, estoque e ordens de serviço, são protegidas por autenticação JWT.

Operações que fazem parte da interação do cliente com a oficina podem ser disponibilizadas sem autenticação administrativa quando necessário ao fluxo de negócio, como o acompanhamento ou a resposta a um orçamento.

A configuração de autenticação também realiza a validação do token, incluindo:

- Assinatura do token;
- Emissor (`Issuer`);
- Destinatário (`Audience`);
- Tempo de validade;
- Identificação do usuário autenticado.

Essa abordagem permite manter as operações internas protegidas enquanto preserva os endpoints necessários para interação externa com o sistema.

## 🔧 Funcionalidades e fluxos de negócio

O GarageHub centraliza o fluxo de atendimento de uma oficina mecânica, desde o cadastro do cliente e do veículo até a elaboração do orçamento, execução dos serviços e conclusão da Ordem de Serviço.

### Clientes

O sistema permite o gerenciamento dos clientes da oficina, incluindo:

- Cadastro de pessoa física ou jurídica;
- Identificação por CPF ou CNPJ;
- Consulta por identificador ou documento;
- Atualização dos dados cadastrais;
- Inativação de clientes;
- Registro de informações de contato e endereço.

### Veículos

Os veículos são vinculados aos seus respectivos clientes e possuem as informações necessárias para identificação e acompanhamento dos atendimentos.

O sistema permite:

- Cadastro de veículos;
- Associação do veículo ao cliente;
- Identificação por placa;
- Registro de marca, modelo, ano, cor, chassi e quilometragem;
- Atualização dos dados do veículo;
- Consulta dos veículos cadastrados.

### Serviços

Os serviços representam os trabalhos que podem ser executados pela oficina.

Entre as funcionalidades disponíveis estão:

- Cadastro e atualização de serviços;
- Definição e alteração de preço;
- Ativação e inativação;
- Consulta dos serviços disponíveis;
- Acompanhamento do tempo médio de execução.

### Peças, insumos e estoque

O sistema possui controle dos itens utilizados durante a execução dos serviços, diferenciando peças e insumos.

A gestão de estoque contempla:

- Cadastro de peças e insumos;
- Código interno para identificação;
- Definição e alteração de preço;
- Controle da quantidade disponível;
- Entrada manual de estoque;
- Saída manual de estoque;
- Ativação e inativação de itens;
- Validação para impedir estoque negativo.

### Orçamentos

O orçamento concentra os serviços, peças e insumos previstos para o atendimento do veículo.

O fluxo permite:

- Criação de orçamento para um cliente e veículo;
- Inclusão e remoção de serviços, peças e insumos;
- Alteração de quantidade e valor unitário dos itens;
- Aplicação de desconto;
- Cálculo do valor total;
- Envio do orçamento para aprovação do cliente;
- Aprovação;
- Rejeição;
- Cancelamento;
- Expiração do orçamento.

Os itens adicionados ao orçamento mantêm um **snapshot das informações relevantes**, preservando os dados utilizados naquele momento mesmo que o cadastro original seja posteriormente alterado.

### Aprovação e geração da Ordem de Serviço

Quando um orçamento é aprovado, o sistema gera automaticamente a **Ordem de Serviço (OS)** correspondente.

A Ordem de Serviço mantém as informações necessárias para execução do atendimento, incluindo cliente, veículo, serviços e valores definidos no orçamento aprovado.

Durante a aprovação, o sistema também verifica a disponibilidade dos itens de estoque necessários e informa quando existem itens com quantidade insuficiente.

### Execução dos serviços

Os serviços pertencentes à Ordem de Serviço possuem controle individual de execução.

O fluxo considera os estados:

- Aguardando execução;
- Em execução;
- Finalizado.

O sistema registra os momentos de início e finalização, permitindo posteriormente calcular o tempo gasto na execução dos serviços.

As transições de status respeitam as regras do domínio, impedindo, por exemplo, a finalização de um serviço que ainda não tenha sido iniciado.

### Acompanhamento da Ordem de Serviço

A Ordem de Serviço representa o acompanhamento do atendimento após a aprovação do orçamento.

Seu status é atualizado de acordo com a evolução do fluxo de execução, permitindo identificar o estágio atual do atendimento e consultar seu progresso através da API.

Ao final da execução de todos os serviços, a Ordem de Serviço pode ser finalizada e posteriormente marcada como entregue ao cliente.

### Monitoramento de tempo

O sistema registra informações temporais durante a execução dos serviços e das Ordens de Serviço.

Esses dados permitem consultar indicadores de tempo de execução, incluindo o **tempo médio dos serviços**, oferecendo informações que podem auxiliar a oficina no acompanhamento da eficiência operacional.

## 🏗️ Arquitetura e Domain-Driven Design

O GarageHub foi desenvolvido como um **monólito modular**, utilizando princípios de **Clean Architecture** e **Domain-Driven Design (DDD)** para organizar as responsabilidades da aplicação e manter as regras de negócio independentes de detalhes de infraestrutura.

A solução está dividida em quatro projetos principais: **Domain, Application, Infrastructure e API**.

### Domain

A camada de domínio concentra as regras de negócio e os conceitos centrais do GarageHub.

Nela estão localizados:

- Entidades do domínio;
- Regras e invariantes de negócio;
- Value Objects;
- Enums relacionados aos estados do sistema;
- Exceções de domínio;
- Validações relacionadas aos objetos de negócio.

As entidades são responsáveis por proteger seu próprio estado. Operações como aprovação de orçamento, alteração de status, movimentação de estoque e execução de serviços são realizadas através de comportamentos do domínio, evitando que regras importantes fiquem espalhadas pelos controllers ou pela infraestrutura.

### Application

A camada de aplicação coordena os casos de uso do sistema e faz a comunicação entre o domínio e as abstrações necessárias para execução das operações.

Ela contém:

- Application Services;
- DTOs de entrada e saída;
- Interfaces dos serviços;
- Interfaces dos repositórios;
- Abstrações utilizadas pelos casos de uso.

Essa camada organiza fluxos como criação de clientes, elaboração e aprovação de orçamentos, geração de Ordens de Serviço e movimentação de estoque, delegando às entidades as regras que pertencem ao domínio.

### Infrastructure

A camada de infraestrutura contém as implementações relacionadas ao acesso a recursos externos e persistência dos dados.

Entre suas responsabilidades estão:

- Implementação dos repositórios;
- Acesso ao PostgreSQL;
- Execução das consultas utilizando Dapper;
- Scripts SQL;
- Criação do schema do banco;
- Seed dos dados iniciais;
- Configuração das dependências de infraestrutura.

As abstrações dos repositórios permanecem na camada de aplicação, enquanto suas implementações ficam na infraestrutura, reduzindo o acoplamento dos casos de uso com a tecnologia de persistência.

### API

A camada de API representa o ponto de entrada HTTP da aplicação.

Ela é responsável por:

- Controllers REST;
- Configuração da aplicação;
- Autenticação e autorização JWT;
- Identificação do usuário autenticado;
- Middleware para tratamento global de exceções;
- Configuração do Swagger/OpenAPI;
- Injeção das dependências necessárias para execução da aplicação.

Os controllers permanecem responsáveis principalmente por receber as requisições e encaminhá-las para os serviços da aplicação, evitando concentrar regras de negócio na camada HTTP.

### Aplicação de Domain-Driven Design

O DDD foi utilizado para modelar o sistema a partir dos conceitos e processos existentes no domínio de uma oficina mecânica.

Entre os principais conceitos representados no modelo estão:

- Cliente;
- Veículo;
- Serviço;
- Item de Estoque;
- Orçamento;
- Ordem de Serviço;
- Aprovação e rejeição de orçamento;
- Execução de serviço;
- Movimentação de estoque;
- Entrega do veículo.

As regras relacionadas a esses conceitos são mantidas próximas às entidades responsáveis por elas, buscando evitar modelos compostos apenas por propriedades sem comportamento.

### Principais regras de domínio

Entre as regras implementadas estão:

- Um orçamento deve seguir transições de status válidas;
- A aprovação de um orçamento gera uma Ordem de Serviço;
- Orçamentos possuem controle de expiração;
- Serviços devem ser iniciados antes de serem finalizados;
- Serviços finalizados não podem retornar para estados anteriores;
- O estoque de uma peça ou insumo não pode ficar negativo;
- A disponibilidade de estoque é verificada durante o fluxo do orçamento;
- Alterações de preço não modificam os valores registrados anteriormente em orçamentos e Ordens de Serviço;
- Os itens mantêm snapshots das informações relevantes para preservar o histórico;
- A Ordem de Serviço acompanha a evolução dos serviços associados;
- Datas de início, finalização e entrega são registradas de acordo com as ações realizadas no domínio.

### Linguagem Ubíqua

O projeto utiliza uma **Linguagem Ubíqua** para manter os mesmos termos de negócio na documentação, no código e nas APIs.

Termos como `Orcamento`, `OrdemServico`, `Servico`, `ItemEstoque`, `Cliente` e `Veiculo` representam diretamente conceitos utilizados no domínio da oficina, reduzindo diferenças entre a linguagem técnica e a linguagem de negócio.

A modelagem completa do domínio, incluindo **Event Storming, agregados, eventos, comandos, políticas, diagramas e Linguagem Ubíqua**, está disponível na documentação DDD do projeto.

**Documentação DDD:**  
https://github.com/beatavernaro/GarageHub/tree/main/Documentacao/DDD

## 📁 Estrutura do projeto

A solução foi organizada de forma a separar as responsabilidades de domínio, aplicação, infraestrutura, exposição da API e testes automatizados.

A estrutura principal do GarageHub é:

```text
GarageHub/
├── src/
│   ├── Api/
│   ├── Application/
│   ├── Domain/
│   └── Infrastructure/
│
├── tests/
│   └── Tests/
│
├── Dockerfile
├── docker-compose.yml
└── README.md
```

### `src/Domain`

Contém o núcleo do negócio e não depende das camadas externas da aplicação.

Principais elementos:

```text
Domain/
├── Entities/
├── Enums/
├── Exceptions/
└── Validators/
```

Nesta camada estão entidades como `Cliente`, `Veiculo`, `Servico`, `ItemEstoque`, `Orcamento` e `OrdemServico`, juntamente com suas regras e comportamentos de domínio.

### `src/Application`

Contém os casos de uso e a coordenação das operações da aplicação.

Principais elementos:

```text
Application/
├── DTOs/
├── Interfaces/
│   ├── Repositories/
│   └── Services/
└── Services/
```

Os DTOs definem os dados utilizados na entrada e saída dos casos de uso. As interfaces estabelecem os contratos necessários para a aplicação, enquanto os Application Services coordenam os fluxos utilizando as entidades e os repositórios.

### `src/Infrastructure`

Contém as implementações relacionadas à persistência e ao acesso ao banco de dados.

Principais elementos:

```text
Infrastructure/
├── Database/
├── Repositories/
├── SQL/
└── DependencyInjection/
```

Os repositórios implementam os contratos definidos pela aplicação e utilizam **Dapper** para comunicação com o PostgreSQL.

As consultas SQL são mantidas separadamente dos repositórios, facilitando sua leitura e manutenção.

A pasta de banco de dados também contém os scripts responsáveis pela criação da estrutura e pelos dados iniciais utilizados no ambiente de desenvolvimento.

### `src/Api`

Representa a camada HTTP e o ponto de inicialização da aplicação.

Principais elementos:

```text
Api/
├── Controllers/
├── Middleware/
├── Security/
└── Program.cs
```

Os controllers disponibilizam os recursos através da API REST, enquanto os componentes de segurança são responsáveis pela integração com a autenticação JWT.

O middleware centraliza o tratamento das exceções geradas durante as requisições.

O `Program.cs` realiza a configuração da aplicação, incluindo injeção de dependências, autenticação, autorização, Swagger/OpenAPI e pipeline HTTP.

### `tests/Tests`

Concentra os testes automatizados do projeto.

Os testes são divididos entre:

- **Testes unitários**, responsáveis por validar isoladamente regras de domínio e serviços da aplicação;
- **Testes de integração**, responsáveis por validar a interação entre diferentes componentes, incluindo API, autenticação, aplicação e persistência no PostgreSQL.

Essa separação permite testar tanto as regras individualmente quanto os principais fluxos da aplicação executados de forma integrada.

### Dependências entre as camadas

A organização das dependências busca manter o domínio independente das tecnologias utilizadas nas camadas externas.

De forma simplificada, a relação entre os projetos segue:

```text
API → Application → Domain
 ↓
Infrastructure → Application / Domain
```

A camada de domínio permanece no centro da solução, enquanto detalhes como HTTP, PostgreSQL, Dapper e autenticação são tratados nas camadas externas.

## 🗄️ Banco de dados e persistência

O GarageHub utiliza **PostgreSQL** como banco de dados relacional e **Dapper** para realizar o acesso e a persistência dos dados.

A camada de persistência foi implementada na Infrastructure, mantendo os detalhes de acesso ao banco separados das regras de negócio e dos casos de uso da aplicação.

### PostgreSQL

O PostgreSQL foi escolhido por ser um banco de dados relacional robusto, open source e amplamente utilizado em aplicações modernas.

A escolha também está relacionada às características do domínio do GarageHub, que possui dados naturalmente relacionais e necessita preservar a consistência entre informações como:

- Clientes e veículos;
- Orçamentos e seus itens;
- Ordens de Serviço e seus serviços;
- Serviços, peças e insumos;
- Usuários e registros de auditoria.

O uso de um banco relacional permite representar essas relações de forma explícita através de chaves primárias, chaves estrangeiras, constraints e transações.

Além disso, o PostgreSQL possui suporte adequado aos tipos utilizados pela aplicação, incluindo UUIDs, valores monetários, datas e valores booleanos.

### Persistência com Dapper

O acesso ao PostgreSQL é realizado utilizando **Dapper**, um micro ORM para .NET.

A escolha pelo Dapper permite manter maior controle sobre as consultas executadas pela aplicação, utilizando SQL explícito e evitando abstrações adicionais sobre operações que possuem comportamento bem definido.

Os repositórios são responsáveis por executar as operações de persistência e transformar os resultados das consultas em objetos utilizados pela aplicação.

Essa abordagem mantém uma separação clara entre:

- Regras de domínio;
- Casos de uso;
- Contratos de persistência;
- Implementação dos repositórios;
- Consultas SQL.

### Repositórios

A aplicação utiliza o padrão **Repository** para abstrair o acesso aos dados.

Os contratos dos repositórios são definidos através de interfaces na camada Application, enquanto suas implementações ficam na camada Infrastructure.

Dessa forma, os serviços da aplicação dependem das abstrações necessárias para executar seus casos de uso, sem depender diretamente do Dapper ou do PostgreSQL.

Entre os repositórios utilizados estão os responsáveis por:

- Clientes;
- Veículos;
- Serviços;
- Itens de estoque;
- Orçamentos;
- Ordens de Serviço;
- Usuários.

### Consultas SQL

As consultas utilizadas pelos repositórios são mantidas em arquivos `.sql` separados. Essa organização evita concentrar grandes consultas diretamente nas classes C# e facilita:

- Leitura das queries;
- Manutenção;
- Identificação das operações realizadas no banco;
- Evolução das consultas sem misturá-las com a lógica dos repositórios.

O Dapper é utilizado para executar essas consultas e realizar o mapeamento dos resultados.

### Criação e inicialização do banco

A estrutura inicial do banco de dados é definida através de scripts SQL versionados junto ao projeto.

Os principais scripts são:

```text
src/Infrastructure/Database/01-schema.sql
src/Infrastructure/Database/02-seed.sql
```

O `01-schema.sql` é responsável pela criação da estrutura necessária para a aplicação, incluindo tabelas, relacionamentos e constraints.

O `02-seed.sql` adiciona os dados iniciais necessários para utilização do ambiente de desenvolvimento, incluindo os usuários utilizados para autenticação.

Quando o ambiente é iniciado através do Docker Compose, esses scripts são executados automaticamente durante a inicialização do PostgreSQL.

Isso permite que um novo ambiente seja criado de maneira reproduzível sem necessidade de criação manual das tabelas ou inserção manual dos dados iniciais.

### Integridade dos dados

Além das validações realizadas pela aplicação e pelo domínio, o banco de dados também possui responsabilidades relacionadas à integridade dos dados persistidos.

A modelagem utiliza recursos como:

- Chaves primárias;
- Chaves estrangeiras;
- Campos obrigatórios;
- Restrições de unicidade;
- Constraints;
- Tipos de dados adequados para cada informação.

Dessa forma, as regras de negócio permanecem concentradas na aplicação e no domínio, enquanto o PostgreSQL atua também como uma camada adicional de proteção da consistência estrutural dos dados.

## 🛠️ Tecnologias utilizadas

O GarageHub foi desenvolvido utilizando tecnologias voltadas ao ecossistema .NET, persistência relacional, conteinerização, testes automatizados e análise contínua da qualidade do código.

### Back-end

- **C#** — linguagem principal utilizada no desenvolvimento da aplicação.
- **.NET 10** — plataforma utilizada para construção e execução do projeto.
- **ASP.NET Core** — framework utilizado para disponibilização da API REST.
- **Swagger / OpenAPI** — documentação e interface para exploração dos endpoints da API.

### Banco de dados e persistência

- **PostgreSQL** — banco de dados relacional utilizado para persistência das informações.
- **Dapper** — micro ORM utilizado para execução das consultas SQL e mapeamento dos dados.

### Segurança

- **JWT (JSON Web Token)** — utilizado para autenticação e proteção dos endpoints administrativos.
- **BCrypt** — utilizado para armazenamento e validação segura das senhas dos usuários.

### Testes e cobertura

- **xUnit** — framework utilizado para implementação dos testes automatizados.
- **Moq** — biblioteca utilizada para criação de mocks e isolamento das dependências nos testes unitários.
- **FluentAssertions** — utilizada para tornar as asserções dos testes mais expressivas e legíveis.
- **Microsoft.AspNetCore.Mvc.Testing** — utilizada nos testes de integração para inicializar a aplicação em ambiente de teste.
- **Coverlet** — responsável pela coleta e geração das métricas de cobertura de código.

O projeto possui testes unitários e de integração, com cobertura superior ao requisito mínimo de **80% dos domínios críticos**.

### Containers

- **Docker** — utilizado para criar uma imagem reproduzível da aplicação.
- **Docker Compose** — responsável por orquestrar a API e o PostgreSQL, permitindo inicializar todo o ambiente com um único comando.

### Qualidade e análise estática

- **SonarQube Cloud** — utilizado para análise estática do código, identificação de vulnerabilidades, problemas de confiabilidade, Code Smells, duplicações e acompanhamento da cobertura dos testes.

A análise final do projeto apresentou **Quality Gate aprovado**, classificação **A em Security, Reliability e Maintainability** e **96,7% de cobertura de código**.

### Integração contínua

- **GitHub Actions** — utilizado para automatizar o processo de build, execução dos testes, geração da cobertura e análise pelo SonarQube Cloud.

O pipeline executado no GitHub realiza a validação automática do projeto a cada alteração configurada no workflow, permitindo verificar continuamente se o código continua compilando, se os testes permanecem válidos e se os critérios de qualidade estabelecidos continuam sendo atendidos.

### Resumo da stack

| Categoria | Tecnologias |
| --- | --- |
| Linguagem | C# |
| Plataforma | .NET 10 |
| API | ASP.NET Core |
| Banco de dados | PostgreSQL |
| Persistência | Dapper |
| Autenticação | JWT |
| Senhas | BCrypt |
| Documentação da API | Swagger / OpenAPI |
| Testes | xUnit, Moq, FluentAssertions |
| Testes de integração | Microsoft.AspNetCore.Mvc.Testing |
| Cobertura | Coverlet |
| Containers | Docker, Docker Compose |
| Qualidade | SonarQube Cloud |
| CI | GitHub Actions |

## 🌐 API REST e documentação

O GarageHub disponibiliza suas funcionalidades através de uma **API REST**, utilizando os métodos HTTP de acordo com a operação realizada sobre cada recurso.

A API está documentada utilizando **OpenAPI** e pode ser explorada através da interface do **Swagger**, disponível após a inicialização do projeto em:

```text
http://localhost:8080/swagger
```

A documentação apresenta as rotas disponíveis, parâmetros, modelos de entrada e saída, códigos de resposta e informações sobre autenticação.

### Clientes

Responsável pelo gerenciamento dos clientes da oficina.

| Método | Endpoint | Descrição | Autenticação |
| --- | --- | --- | --- |
| `GET` | `/api/Clientes` | Lista os clientes | JWT |
| `GET` | `/api/Clientes/{id}` | Consulta um cliente pelo ID | JWT |
| `GET` | `/api/Clientes/documento/{documento}` | Consulta um cliente pelo CPF/CNPJ | JWT |
| `POST` | `/api/Clientes` | Cadastra um cliente | JWT |
| `PUT` | `/api/Clientes/{id}` | Atualiza um cliente | JWT |
| `PATCH` | `/api/Clientes/{id}/inativar` | Inativa um cliente | JWT |

### Veículos

Responsável pelo cadastro e gerenciamento dos veículos associados aos clientes.

| Método | Endpoint | Descrição | Autenticação |
| --- | --- | --- | --- |
| `GET` | `/api/Veiculos` | Lista os veículos | JWT |
| `GET` | `/api/Veiculos/{id}` | Consulta um veículo pelo ID | JWT |
| `GET` | `/api/Veiculos/placa/{placa}` | Consulta um veículo pela placa | JWT |
| `POST` | `/api/Veiculos` | Cadastra um veículo | JWT |
| `PUT` | `/api/Veiculos/{id}` | Atualiza os dados do veículo | JWT |
| `PATCH` | `/api/Veiculos/{id}/inativar` | Inativa um veículo | JWT |

### Serviços

Responsável pelo catálogo de serviços oferecidos pela oficina.

| Método | Endpoint | Descrição | Autenticação |
| --- | --- | --- | --- |
| `GET` | `/api/Servicos` | Lista os serviços | JWT |
| `GET` | `/api/Servicos/{id}` | Consulta um serviço pelo ID | JWT |
| `GET` | `/api/Servicos/codigo/{codigoInterno}` | Consulta pelo código interno | JWT |
| `POST` | `/api/Servicos` | Cadastra um serviço | JWT |
| `PUT` | `/api/Servicos/{id}` | Atualiza um serviço | JWT |
| `PATCH` | `/api/Servicos/{id}/preco` | Altera o preço do serviço | JWT |
| `PATCH` | `/api/Servicos/{id}/inativar` | Inativa um serviço | JWT |
| `PATCH` | `/api/Servicos/{id}/ativar` | Reativa um serviço | JWT |
| `GET` | `/api/Servicos/tempo-medio` | Consulta o tempo médio de execução dos serviços | JWT |

### Peças, insumos e estoque

Os itens de estoque representam as peças e os insumos utilizados pela oficina.

| Método | Endpoint | Descrição | Autenticação |
| --- | --- | --- | --- |
| `GET` | `/api/ItensEstoque` | Lista os itens de estoque | JWT |
| `GET` | `/api/ItensEstoque/{id}` | Consulta um item pelo ID | JWT |
| `GET` | `/api/ItensEstoque/codigo/{codigoInterno}` | Consulta pelo código interno | JWT |
| `POST` | `/api/ItensEstoque` | Cadastra uma peça ou insumo | JWT |
| `PUT` | `/api/ItensEstoque/{id}` | Atualiza um item | JWT |
| `PATCH` | `/api/ItensEstoque/{id}/adicionar-estoque` | Adiciona quantidade ao estoque | JWT |
| `PATCH` | `/api/ItensEstoque/{id}/remover-estoque` | Remove quantidade do estoque | JWT |
| `PATCH` | `/api/ItensEstoque/{id}/preco` | Altera o preço do item | JWT |
| `PATCH` | `/api/ItensEstoque/{id}/inativar` | Inativa um item | JWT |

### Orçamentos

Responsável pelo fluxo de elaboração, envio e decisão sobre os orçamentos.

| Método | Endpoint | Descrição | Autenticação |
| --- | --- | --- | --- |
| `GET` | `/api/Orcamentos` | Lista os orçamentos | JWT |
| `GET` | `/api/Orcamentos/{id}` | Consulta os detalhes de um orçamento | Conforme operação |
| `GET` | `/api/Orcamentos/cliente/{clienteId}` | Consulta os orçamentos de um cliente | Conforme operação |
| `POST` | `/api/Orcamentos` | Cria um orçamento | JWT |
| `POST` | `/api/Orcamentos/{id}/itens` | Adiciona um item ao orçamento | JWT |
| `DELETE` | `/api/Orcamentos/{id}/itens/{itemId}` | Remove um item do orçamento | JWT |
| `PATCH` | `/api/Orcamentos/{id}/itens/{itemId}/quantidade` | Altera a quantidade de um item | JWT |
| `PATCH` | `/api/Orcamentos/{id}/itens/{itemId}/valor` | Altera o valor unitário de um item | JWT |
| `PATCH` | `/api/Orcamentos/{id}/desconto` | Aplica desconto ao orçamento | JWT |
| `POST` | `/api/Orcamentos/{id}/aguardando-cliente` | Envia o orçamento para decisão do cliente | JWT |
| `POST` | `/api/Orcamentos/{id}/aprovar` | Aprova o orçamento | Acesso do cliente |
| `POST` | `/api/Orcamentos/{id}/rejeitar` | Rejeita o orçamento | Acesso do cliente |
| `POST` | `/api/Orcamentos/{id}/cancelar` | Cancela o orçamento | JWT |

A aprovação do orçamento gera automaticamente a Ordem de Serviço correspondente.

### Ordens de Serviço

Responsável pelo acompanhamento da execução dos serviços após a aprovação do orçamento.

| Método | Endpoint | Descrição | Autenticação |
| --- | --- | --- | --- |
| `GET` | `/api/OrdensServico` | Lista as Ordens de Serviço | JWT |
| `GET` | `/api/OrdensServico/{id}` | Consulta uma Ordem de Serviço | Conforme operação |
| `GET` | `/api/OrdensServico/placa/{placa}` | Consulta a OS atual de um veículo pela placa | Conforme operação |
| `GET` | `/api/OrdensServico/tempos` | Consulta informações de tempo das Ordens de Serviço | JWT |

A Ordem de Serviço também disponibiliza operações responsáveis pela evolução dos serviços durante a execução, respeitando as transições de estado definidas pelo domínio.

### Autenticação

A autenticação administrativa é realizada pelo endpoint:

| Método | Endpoint | Descrição | Autenticação |
| --- | --- | --- | --- |
| `POST` | `/api/Auth/login` | Autentica o usuário e retorna um token JWT | Não |

O token retornado deve ser utilizado nas operações administrativas protegidas.

### Códigos HTTP

A API utiliza códigos HTTP para representar o resultado das operações. Entre os principais estão:

| Código | Significado |
| --- | --- |
| `200 OK` | Operação realizada com sucesso |
| `201 Created` | Recurso criado com sucesso |
| `204 No Content` | Operação realizada sem conteúdo de retorno |
| `400 Bad Request` | Requisição inválida ou violação de uma regra de negócio |
| `401 Unauthorized` | Autenticação ausente ou inválida |
| `404 Not Found` | Recurso não encontrado |
| `500 Internal Server Error` | Erro inesperado durante o processamento |

Os contratos completos, schemas dos DTOs, parâmetros, respostas e operações disponíveis podem ser consultados diretamente através do Swagger/OpenAPI da aplicação.

## 🛡️ Segurança e validações

A segurança do GarageHub foi considerada tanto no controle de acesso à API quanto na validação dos dados recebidos e na proteção das regras de negócio.

### Autenticação JWT

As operações administrativas são protegidas utilizando **JSON Web Token (JWT)**.

Após a autenticação, o token identifica o usuário responsável pela requisição e permite o acesso aos recursos protegidos da aplicação.

A validação do JWT considera:

- Assinatura do token;
- Emissor (`Issuer`);
- Destinatário (`Audience`);
- Tempo de validade;
- Identificação do usuário autenticado.

A aplicação utiliza o usuário identificado pelo token também para registrar informações de criação e alteração das entidades.

### Validação dos dados

Os dados recebidos pela aplicação são validados antes de serem utilizados nos fluxos de negócio.

Entre as validações implementadas estão:

- Validação de CPF;
- Validação de CNPJ;
- Validação de placa de veículo;
- Validação dos dados de endereço;
- Campos obrigatórios;
- Valores monetários válidos;
- Quantidades válidas;
- Consistência dos dados das entidades.

Além da validação estrutural dos dados, as próprias entidades protegem suas invariantes e impedem operações incompatíveis com seu estado atual.

### Proteção das regras de negócio

As regras críticas não dependem apenas dos dados enviados pelo cliente da API.

O domínio é responsável por impedir operações inválidas, como:

- Estoque negativo;
- Finalização de um serviço que ainda não foi iniciado;
- Alterações inválidas de status;
- Operações incompatíveis com um orçamento já finalizado;
- Inclusão de itens inválidos nos fluxos;
- Transições inconsistentes durante a execução da Ordem de Serviço.

Essa abordagem evita que uma regra importante possa ser ignorada simplesmente utilizando diretamente outro endpoint da aplicação.

### Tratamento global de exceções

A API possui um middleware responsável por centralizar o tratamento das exceções geradas durante as requisições.

Com isso, erros de domínio e erros inesperados podem ser tratados de maneira padronizada, evitando a duplicação de tratamento em cada controller e reduzindo a exposição desnecessária de detalhes internos da aplicação.

### Análise de segurança

O código também é submetido à análise estática utilizando **SonarQube Cloud**.

Durante o desenvolvimento, os apontamentos de segurança foram revisados e, quando aplicável, corrigidos.

Alguns apontamentos relacionados a credenciais utilizadas exclusivamente no ambiente acadêmico e local foram classificados como aceitos após análise. Essas decisões são específicas para este MVP e não representam a abordagem recomendada para produção.

Em um ambiente produtivo, secrets, senhas, chaves JWT e strings de conexão seriam armazenados utilizando mecanismos apropriados de gerenciamento de secrets e variáveis de ambiente.

## 🐳 Docker

O GarageHub utiliza **Docker** para fornecer um ambiente reproduzível e simplificar a execução local da aplicação.

Toda a infraestrutura necessária para executar o MVP pode ser inicializada através do Docker Compose, sem necessidade de instalação local do PostgreSQL ou do .NET.

### Dockerfile

O `Dockerfile` é responsável pela construção da imagem da API.

A construção utiliza múltiplos estágios, separando o ambiente utilizado para compilação do ambiente final responsável pela execução da aplicação.

De forma geral, o processo realiza:

1. Restauração das dependências;
2. Compilação e publicação da aplicação;
3. Criação da imagem final utilizando o runtime do ASP.NET Core;
4. Configuração do diretório da aplicação;
5. Inicialização da API.

Como melhoria de segurança identificada durante a análise do SonarQube Cloud, a aplicação no container final é executada utilizando um **usuário não privilegiado**, evitando a execução da API como `root`.

### Docker Compose

O `docker-compose.yml` é responsável pela orquestração do ambiente completo.

Ele inicializa os serviços necessários para execução do projeto e configura a comunicação entre eles.

O ambiente contém:

- Container da API GarageHub;
- Container PostgreSQL;
- Rede utilizada para comunicação entre os serviços;
- Volume para persistência dos dados;
- Configurações necessárias para conexão da API com o banco.

### Inicialização do banco

Durante a primeira criação do container PostgreSQL, os scripts versionados no projeto são utilizados para preparar automaticamente o ambiente.

```text
01-schema.sql
02-seed.sql
```

O primeiro cria a estrutura do banco e o segundo adiciona os dados iniciais necessários para utilização da aplicação.

Isso permite iniciar um ambiente novo através de:

```bash
docker compose up --build
```

sem necessidade de executar scripts manualmente.

### Persistência

O PostgreSQL utiliza volume Docker para que os dados sejam preservados mesmo quando os containers forem interrompidos.

O comando:

```bash
docker compose down
```

remove os containers, mas mantém os dados persistidos.

Para recriar completamente o ambiente, incluindo o banco:

```bash
docker compose down -v
docker compose up --build
```

O uso de `-v` remove os volumes e, consequentemente, os dados armazenados anteriormente.

## 🧪 Testes e cobertura

O GarageHub possui uma estratégia de testes automatizados que combina **testes unitários e testes de integração**, permitindo validar tanto regras isoladas quanto os principais fluxos executados pela aplicação completa.

Os testes foram desenvolvidos utilizando **xUnit**, **Moq** e **FluentAssertions**.

### Testes unitários

Os testes unitários concentram-se principalmente nas regras de domínio e nos serviços da camada Application.

Eles validam comportamentos como:

- Criação e atualização das entidades;
- Validações de domínio;
- Transições de status;
- Aprovação e rejeição de orçamentos;
- Aplicação de descontos;
- Manipulação dos itens do orçamento;
- Controle de estoque;
- Execução e finalização de serviços;
- Comportamento das Ordens de Serviço;
- Casos de uso da camada Application;
- Tratamento de cenários inválidos e exceções esperadas.

As dependências externas dos Application Services são isoladas utilizando mocks, permitindo validar o comportamento de cada caso de uso sem depender do banco de dados.

### Testes de integração

Além dos testes unitários, foram implementados testes de integração para validar a comunicação entre diferentes componentes da aplicação.

Esses testes inicializam a API utilizando `Microsoft.AspNetCore.Mvc.Testing` e executam requisições HTTP reais contra a aplicação de teste.

Entre os fluxos cobertos estão:

- Autenticação administrativa;
- Validação do acesso a endpoints protegidos;
- Cadastro e consulta de clientes;
- Cadastro e consulta de veículos;
- Gestão de itens de estoque;
- Criação e consulta de orçamentos;
- Aprovação de orçamento;
- Geração automática da Ordem de Serviço;
- Integração entre API, Application, Domain, Infrastructure e PostgreSQL.

Dessa forma, os testes de integração complementam os testes unitários verificando se os componentes funcionam corretamente quando utilizados em conjunto.

### Executando os testes

Para executar toda a suíte:

```bash
dotnet test tests/Tests/Tests.csproj
```

Para executar somente os testes de integração:

```bash
dotnet test tests/Tests/Tests.csproj --filter FullyQualifiedName~Integration /p:Threshold=0
```

O parâmetro `Threshold=0` no segundo comando permite executar isoladamente os testes de integração sem aplicar sobre esse subconjunto a regra global de cobertura mínima.

### Cobertura

A cobertura de código é coletada utilizando **Coverlet**.

O projeto estabelece um limite mínimo de:

```text
80% de cobertura de linhas nos domínios críticos
```

A configuração concentra a medição principalmente no **Domain** e nos **Application Services**, evitando que classes essencialmente estruturais, como DTOs, interfaces, enums e detalhes de infraestrutura, distorçam a métrica utilizada para avaliar as regras críticas do sistema.

Na análise final integrada ao SonarQube Cloud, o projeto apresentou:

```text
Coverage: 96,7%
```

O resultado supera o requisito mínimo de 80% definido para o projeto.

## 🔍 Qualidade de código e SonarQube

O GarageHub utiliza **SonarQube Cloud** para análise estática e acompanhamento contínuo da qualidade do código.

A análise está integrada ao repositório e é executada através do pipeline do GitHub Actions.

### Aspectos analisados

O SonarQube é utilizado para acompanhar aspectos como:

- Segurança;
- Confiabilidade;
- Manutenibilidade;
- Vulnerabilidades;
- Code Smells;
- Security Hotspots;
- Duplicação de código;
- Cobertura dos testes.

Durante o desenvolvimento, os apontamentos identificados foram analisados individualmente e diversas melhorias foram realizadas no código.

### Melhorias realizadas

Entre os tipos de melhoria realizados a partir dos apontamentos da análise estão:

- Execução do container com usuário não privilegiado;
- Melhor utilização de operações assíncronas;
- Propagação de `CancellationToken` quando aplicável;
- Redução de duplicação de código;
- Simplificação de estruturas utilizando LINQ;
- Melhor tratamento de nulabilidade;
- Refatoração de propriedades que lançavam exceções;
- Revisão de complexidade e quantidade de parâmetros;
- Melhorias gerais de legibilidade e manutenibilidade.

Também foram analisados apontamentos relacionados aos arquivos SQL. Regras incompatíveis com o PostgreSQL ou direcionadas especificamente a outros dialetos foram avaliadas considerando a tecnologia efetivamente utilizada pelo projeto.

### Accepted Issues

Alguns apontamentos de segurança relacionados a credenciais de desenvolvimento foram mantidos como **Accepted Issues**.

Esses casos envolvem exclusivamente dados utilizados no ambiente acadêmico e local, sem acesso a sistemas, usuários ou informações reais.

A aceitação desses itens foi uma decisão consciente para o contexto deste MVP. Em um ambiente produtivo, credenciais, hashes, chaves JWT e outros secrets não deveriam permanecer diretamente em arquivos versionados.

### Resultado da análise

Após as correções e análises realizadas, o projeto obteve:

| Métrica | Resultado |
| --- | --- |
| Quality Gate | **Passed** |
| Security | **A** |
| Reliability | **A** |
| Maintainability | **A** |
| Security Issues abertas | **0** |
| Security Hotspots | **0** |
| Coverage | **96,7%** |
| Duplications | **1,3%** |

O **Quality Gate aprovado** indica que os critérios de qualidade configurados para o projeto foram atendidos após a análise do código.

## ⚙️ CI com GitHub Actions

O GarageHub utiliza **GitHub Actions** para automatizar a validação do projeto e a integração com o SonarQube Cloud.

O workflow é executado automaticamente para alterações na branch principal e nos eventos de Pull Request configurados no repositório.

### Pipeline

O processo automatizado executa as principais etapas necessárias para validar a aplicação:

```text
Checkout do código
        ↓
Configuração do .NET
        ↓
Configuração do Java
        ↓
Inicialização do PostgreSQL
        ↓
Criação do schema e execução do seed
        ↓
Inicialização da análise SonarQube
        ↓
Restore
        ↓
Build
        ↓
Testes unitários e de integração
        ↓
Geração do relatório de cobertura
        ↓
Envio da análise e cobertura ao SonarQube Cloud
```

### Banco de dados no CI

Como os testes de integração dependem do PostgreSQL, o próprio workflow inicializa uma instância isolada do banco durante sua execução.

Antes dos testes são executados:

```text
01-schema.sql
02-seed.sql
```

Isso permite que os testes de integração sejam executados em um ambiente novo e reproduzível, sem depender do banco de dados utilizado durante o desenvolvimento local.

### Validação automatizada

O pipeline verifica automaticamente:

- Restauração das dependências;
- Compilação da solução;
- Execução dos testes unitários;
- Execução dos testes de integração;
- Geração da cobertura;
- Análise estática;
- Critérios de qualidade do SonarQube Cloud.

Essa automação reduz a possibilidade de alterações incompatíveis serem incorporadas ao projeto sem que problemas de compilação, testes ou qualidade sejam identificados.

## 📝 Decisões técnicas, premissas e limitações do MVP

O GarageHub foi desenvolvido como um MVP acadêmico. Algumas decisões foram tomadas buscando equilibrar simplicidade, clareza arquitetural e atendimento aos requisitos definidos para esta fase.

### Monólito

A aplicação foi implementada como um **monólito**, conforme proposto para o MVP.

A separação interna em Domain, Application, Infrastructure e API permite manter responsabilidades bem definidas sem introduzir a complexidade operacional de uma arquitetura distribuída.

### Dapper e SQL explícito

O Dapper foi escolhido como mecanismo de persistência para manter controle explícito sobre as consultas executadas no PostgreSQL.

Essa decisão também permite que os repositórios permaneçam relativamente simples e que as consultas SQL possam ser analisadas diretamente.

### Snapshot de informações

Itens de orçamento e informações transferidas para a Ordem de Serviço preservam dados relevantes do momento em que a operação ocorreu.

Essa decisão evita que alterações futuras no cadastro de um serviço ou item de estoque modifiquem retroativamente informações pertencentes a um atendimento anterior.

### Expiração de orçamento

Os orçamentos possuem regra de expiração para evitar que propostas permaneçam indefinidamente disponíveis para aprovação.

A expiração faz parte das regras de domínio e é verificada durante os fluxos relacionados ao orçamento.

### Credenciais de desenvolvimento

O projeto possui credenciais destinadas exclusivamente à execução local e avaliação acadêmica.

Essa abordagem facilita a inicialização do ambiente através do Docker sem etapas adicionais de configuração.

Em produção, essa estratégia seria substituída por gerenciamento adequado de secrets, separação de configurações por ambiente e rotação de credenciais.

### Escopo do MVP

Por se tratar da primeira versão do sistema, o objetivo é atender aos fluxos centrais da oficina e demonstrar a aplicação dos conceitos arquiteturais, de domínio, qualidade e segurança definidos para a fase.

Funcionalidades de uma solução comercial completa, como interfaces web/mobile, notificações externas, integrações com sistemas de pagamento, infraestrutura de produção e mecanismos avançados de observabilidade não fazem parte do escopo desta versão.

A arquitetura e a separação das responsabilidades foram estruturadas para permitir que novas funcionalidades sejam incorporadas conforme a evolução do sistema.

## 👩‍💻 Autora

[**Beatriz Tavernaro**](https://www.linkedin.com/in/beatriz-tavernaro)

Projeto desenvolvido para o **Tech Challenge – Fase 1 da Pós Tech em Software Architecture da FIAP**.

Este repositório possui finalidade acadêmica e foi desenvolvido como parte da aplicação prática dos conceitos abordados durante a fase.