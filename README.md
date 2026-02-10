# Projeto Hackathon para Monitoramento de Propriedades Rurais

## Sumário

- [Visão Geral](#visão-geral)
- [Componentes](#componentes)
- [Fluxo Geral de Dados](#fluxo-geral-de-dados)
- [Infraestrutura como Código (IaC)](#infraestrutura-como-código-iac)
  - [Como iniciar todos os serviços](#como-iniciar-todos-os-serviços)
  - [Como iniciar apenas serviços específicos](#como-iniciar-apenas-serviços-específicos)
  - [Como parar e remover os containers](#como-parar-e-remover-os-containers)
- [Kubernetes & Minikube](#kubernetes--minikube)
- [Autores](#autores)

## Visão Geral

O sistema é composto por uma arquitetura baseada em microsserviços, orientada a eventos e preparada para ingestão e análise de dados de sensores em propriedades rurais. O diagrama abaixo representa os principais componentes e seus fluxos de comunicação.

A proposta é permitir que dados coletados em campo (sensores) sejam ingeridos, armazenados, processados e posteriormente consumidos por produtores rurais através de uma API centralizada.

![Diagrama da Arquitetura](architecture-diagram.drawio.png)

## Componentes

### 👤 Rural Producer

Representa o usuário final (produtor rural), que consome os dados e insights por meio da **Analytics API**.

---

### 🔷 Analytics API

Serviço responsável por consolidar e disponibilizar dados analíticos ao usuário final.

Funções principais:

* Receber requisições HTTP do produtor rural
* Consultar dados em batch em outros serviços
* Utilizar o **Redis** como cache para melhorar performance
* Orquestrar chamadas para:

  * **Properties API** (dados cadastrais)
  * **Ingress API** (dados de sensores)

---

### 🟥 Redis

Utilizado como camada de cache para a **Analytics API**, reduzindo latência e evitando consultas repetidas aos serviços dependentes.

---

### 🔷 Properties API

Responsável pelo gerenciamento de dados relacionados às propriedades rurais.

Funções principais:

* Cadastro e consulta de propriedades
* Persistência dos dados no **MongoDB**

---

### 🟪 MongoDB

Banco de dados NoSQL utilizado para armazenar os dados estruturais e cadastrais das propriedades.

---

### 🔷 Ingress API

Serviço responsável pela ingestão e processamento dos dados vindos dos sensores.

Funções principais:

* Consumir eventos provenientes do **Kafka**
* Processar dados de telemetria
* Persistir séries temporais no **InfluxDB**
* Disponibilizar dados para consumo da **Analytics API**

---

### 🟦 InfluxDB

Banco de dados de séries temporais utilizado para armazenar os dados dos sensores processados pela **Ingress API**.

Funções principais:

* Armazenamento otimizado para dados de séries temporais
* Consultas eficientes baseadas em tempo
* Retenção automática de dados
* Interface web para visualização e consulta (http://localhost:8086)

Dados armazenados:
* Umidade do solo
* Temperatura
* Precipitação
* Timestamp dos sensores

---

### 🟧 Kafka

Plataforma de mensageria utilizada como broker de eventos entre os sensores e a **Ingress API**.

Benefícios:

* Comunicação assíncrona
* Maior resiliência
* Escalabilidade no processamento de eventos

---

### 📟 Sensors

Representam dispositivos de campo responsáveis pela coleta de dados (ex: temperatura, umaidade, localização, etc.).

Esses dados são enviados para o Kafka, simulando um cenário real de IoT.

---

### 🛡️ Keycloak

Responsável pela autenticação e autorização dos usuários e serviços.

Funções principais:

* Gerenciamento de identidade (IAM)
* Emissão e validação de tokens (OAuth2 / OpenID Connect)
* Integração com banco PostgreSQL

---

### 🐘 PostgreSQL

Banco de dados utilizado pelo **Keycloak** para persistência de usuários, credenciais e configurações de segurança.

---

## Fluxo Geral de Dados

1. Sensores enviam dados → **Kafka**
2. **Ingress API** consome os eventos e armazena no **InfluxDB**
3. **Properties API** gerencia dados cadastrais no **MongoDB**
4. **Analytics API** consulta os dois serviços em batch
5. Resultados são cacheados no **Redis**
6. O produtor rural consome os dados via requisições HTTP

## Infraestrutura como Código (IaC)

O projeto utiliza Docker Compose para orquestrar todos os serviços. O arquivo principal [`docker-compose.yml`](docker-compose.yml) está na raiz do projeto e inclui os arquivos de definição de cada serviço localizados na pasta [`iac`](iac/):

- [`iac/analytics-docker-compose.yml`](iac/analytics-docker-compose.yml)
- [`iac/ingress-docker-compose.yml`](iac/ingress-docker-compose.yml)
- [`iac/keycloak-docker-compose.yml`](iac/keycloak-docker-compose.yml)
- [`iac/properties-docker-compose.yml`](iac/properties-docker-compose.yml)
- [`iac/sensors-docker-compose.yml`](iac/sensors-docker-compose.yml)

### Como iniciar todos os serviços

No terminal, acesse a raiz do projeto (onde está o arquivo `docker-compose.yml`) e execute:

```sh
docker compose up -d
```

Isso irá iniciar todos os serviços definidos nos arquivos de compose incluídos.

### Como iniciar apenas serviços específicos

Você pode subir apenas um serviço (e suas dependências) usando:

```sh
docker compose up -d <nome-do-serviço>
```

Por exemplo, para subir apenas o serviço de sensores:

```sh
docker compose up -d sensors
```

> Certifique-se de que os arquivos de compose individuais estejam devidamente configurados com os serviços necessários.

### Como parar e remover os containers

Para parar todos os containers:

```sh
docker compose stop
```

Para parar e remover todos os containers, redes e volumes criados:

```sh
docker compose down
```

Você também pode usar essas opções com arquivos de compose personalizados usando a opção `-f`.

## Kubernetes & Minikube

Esta seção orienta como executar e orquestrar todos os microsserviços do projeto em um cluster Kubernetes local utilizando o Minikube. O uso do Kubernetes permite simular um ambiente de produção real, facilitando testes de escalabilidade, resiliência, deploy contínuo e integração entre os serviços. Com Minikube, você pode experimentar práticas modernas de DevOps, validar a infraestrutura como código e garantir que sua aplicação está pronta para ambientes cloud-native.

### O que é Minikube?
Minikube é uma ferramenta que executa clusters Kubernetes localmente, ideal para desenvolvimento e testes.

### Por que usar Kubernetes neste projeto?
- Orquestração e automação de deploys, escalonamento e gerenciamento dos serviços.
- Simulação de ambiente produtivo, com pods, serviços, volumes persistentes e variáveis de ambiente.
- Facilidade para testar cenários de falha, escalabilidade e atualização contínua.
- Separação clara dos recursos de infraestrutura (Kafka, InfluxDB, Zookeeper, etc) e dos microsserviços da aplicação.

### Organização dos Manifests

Os manifests do Kubernetes estão organizados em subpastas dentro de `k8s/`:

- `k8s/influxdb/` - Deployment, Service, PVCs do InfluxDB
- `k8s/kafka/` - Deployment & Service do Kafka
- `k8s/zookeeper/` - Deployment & Service do Zookeeper
- `k8s/ingress/` - Deployment & Service da API Ingress

Cada subpasta contém arquivos YAML separados para Deployment, Service e, quando necessário, PersistentVolumeClaim.

### Como rodar no Minikube

1. **Inicie o Minikube:**
   ```sh
   minikube start
   ```
2. **Construa as imagens Docker dentro do Minikube:**
   - **Opção 1: Build de todas as imagens com Docker Compose:**
     ```sh
     eval $(minikube docker-env)
     docker compose build
     ```
   - **Opção 2: Build manual de cada imagem:**
     ```sh
     eval $(minikube docker-env)
     docker build -t ingress:latest ./src/Ingress
     # Construa outras imagens conforme necessário
     ```
3. **Aplique os manifests do Kubernetes:**
   - **Opção 1: Aplicar todos os manifests de uma vez:**
     ```sh
     kubectl apply -f k8s/
     ```
   - **Opção 2: Aplicar manualmente cada manifest:**
     ```sh
     kubectl apply -f k8s/zookeeper/zookeeper-deployment.yaml
     kubectl apply -f k8s/zookeeper/zookeeper-service.yaml
     kubectl apply -f k8s/kafka/kafka-deployment.yaml
     kubectl apply -f k8s/kafka/kafka-service.yaml
     kubectl apply -f k8s/influxdb/influxdb-pvc.yaml
     kubectl apply -f k8s/influxdb/influxdb-deployment.yaml
     kubectl apply -f k8s/influxdb/influxdb-service.yaml
     kubectl apply -f k8s/ingress/ingress-api-deployment.yaml
     kubectl apply -f k8s/ingress/ingress-api-service.yaml
     ```
4. **Verifique os pods e serviços:**
   ```sh
   kubectl get pods
   kubectl get svc
   ```
5. **Acesse a API ingress:**
   ```sh
   minikube service ingress-api
   ```

### Dicas e Boas Práticas

- Use `kubectl delete -f <arquivo.yaml>` para remover recursos.
- Use `kubectl logs <nome-do-pod>` para debugar problemas em pods.
- Para acessar o InfluxDB, crie um port-forward:
  ```sh
  kubectl port-forward svc/influxdb 8086:8086
  ```
- Para testar escalabilidade, altere o campo `replicas` nos arquivos de Deployment.
- As variáveis de ambiente definidas nos Deployments sobrescrevem as configurações do `appsettings.json`.
- Certifique-se de que o valor de `InfluxDbConfig__Url` seja `http://influxdb:8086` no ambiente Kubernetes.

## Autores

- [Paulo](https://github.com/paulobusch)
- [Geovanne](https://github.com/gehcosta)
- [Letícia](https://github.com/leticia-kojima)
- [Matheus](https://github.com/M4theusVieir4)
- [Marcelo](https://github.com/marceloalvees)
