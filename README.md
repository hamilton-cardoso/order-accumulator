# 📘 order-accumulator

**OrderAccumulator** é uma aplicação .NET que implementa um servidor (acceptor) FIX 4.4. Sua principal responsabilidade é receber ordens via protocolo FIX e responder com mensagens de `ExecutionReport`, mantendo controle da exposição financeira por símbolo.

---

## ⚙️ Requisitos

- [.NET 8.0 SDK ou superior](https://dotnet.microsoft.com/en-us/download)
- [QuickFIX/n](https://www.nuget.org/packages/QuickFIXn.FIX4.4/1.13.0?_src=template)

---

## 📦 Instalação

```bash
git clone https://github.com/hamilton-cardoso/order-accumulator.git
cd order-accumulator
dotnet restore
dotnet build
```

---

## 🚀 Execução

```bash
dotnet run
```

A aplicação iniciará o servidor FIX e ficará aguardando conexões e ordens do initiator.

---

## 📁 Estrutura do Projeto

```text
OrderAccumulator/
├── Fix/
│   ├── FixServerApp.cs         # Implementação principal do servidor FIX
│   ├── acceptor.cfg            # Configuração do QuickFIX/n
├── Program.cs                  # Entrada principal da aplicação
├── OrderAccumulator.csproj     # Projeto .NET
```

---

## 🔒 Limite de Exposição

A aplicação possui um controle de risco simples baseado em um limite de exposição (R$ 100.000.000 por símbolo). Ordens que ultrapassam esse limite são rejeitadas com um `ExecutionReport` de tipo `REJECTED`.

---

## 🔄 Comunicação

A comunicação é feita exclusivamente via protocolo FIX. As mensagens `NewOrderSingle` são processadas e respondidas com `ExecutionReport` com status `NEW` ou `REJECTED`.

---

## ✅ Exemplo de Execução

```bash
[LOGON] FIX.4.4:ORDER_GENERATOR->ORDER_ACCUMULATOR
[OrderID: 123abc] [VALE11] 1000 @ 50,00 (BUY) | EXP: 50000 | STATUS: ACEITA
```

---

## 🛠️ Personalização

Você pode editar o arquivo `acceptor.cfg` para ajustar a porta, timeouts e identificadores da sessão FIX.

---

## 📄 Licença

Este projeto é de uso interno e acadêmico.
