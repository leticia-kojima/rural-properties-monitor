# rural-properties-monitor
Projeto Hackathon para Monitoramento de Propriedades Rurais

![Diagrama da Arquitetura](architecture-diagram.drawio.png)

## Infraestrutura como Código (iac)

A pasta `iac` contém os arquivos docker-compose responsáveis por orquestrar os serviços do projeto:

- [analytics-docker-compose.yml](iac/analytics-docker-compose.yml)
- [ingress-docker-compose.yml](iac/ingress-docker-compose.yml)
- [keycloak-docker-compose.yml](iac/keycloak-docker-compose.yml)
- [properties-docker-compose.yml](iac/properties-docker-compose.yml)
- [sensors-docker-compose.yml](iac/sensors-docker-compose.yml)
- [docker-compose.yml](iac/docker-compose.yml) (arquivo principal que referencia os demais)

### Como iniciar todos os serviços

No diretório `iac`, execute:

```sh
docker compose up -d
```

Isso irá iniciar todos os serviços definidos nos arquivos de compose.

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