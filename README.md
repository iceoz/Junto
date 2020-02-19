# Junto

Teste backent junto seguros

### Prerequisitos para executar a aplicação

dotnet sdk ou docker

### Executando a aplicação utilizando dotnet sdk

acesse a pasta Junto.Api e execute

```
dotnet run
```

### Executando a aplicação utilizando docker

acesse a pasta raiz do projeto onde se encontra o dockerfile e execute os comandos

```
docker build -t junto:1 .
docker run --name junto -p 5000:80 junto:1
```

### Calculando métricas

acesse a pasta de testes e execute

```
dotnet test /p:CollectCoverage=true
```

### Utilizando o swagger

O swagger se encontra no caminho http://localhost:5000/swagger

Existem 2 endpoints abertos

[GET]/api/auth
Serve para efetuar o login

[POST]/api/user
Serve para cadastrar usuários

Os outros endpoints precisam de token que é obtido pelo endpoint de **auth**, dentro do swagger tem um botão no top escrito **Authorize**, clique nele e cole o token lembrando de colocar *bearer* na frente


