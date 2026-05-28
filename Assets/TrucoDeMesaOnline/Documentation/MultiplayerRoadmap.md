# Multiplayer roadmap

O MVP local nao referencia diretamente Netcode for GameObjects ainda. Isso mantem o projeto compilando mesmo antes dos pacotes online estarem instalados.

## Pacotes Unity a instalar na proxima fase

- Netcode for GameObjects
- Unity Transport
- Authentication
- Lobby
- Relay

Instale pelo Package Manager da sua versao do Unity. Depois disso, os scripts em `Scripts/Networking` devem virar adaptadores reais para:

- criar sala privada e codigo de entrada em `LobbyManager`;
- criar alocacao Relay para host;
- conectar cliente pelo codigo;
- iniciar `NetworkManager` como host/client;
- sincronizar assentos, cartas jogadas, sinais, Truco, Correr e placar;
- manter cartas escondidas usando dados server-authoritative.

## Contrato de sincronizacao planejado

- O host/servidor distribui cartas e envia para cada cliente apenas a propria mao.
- `SeatManager` mapeia `clientId -> SeatId`.
- `RoundManager` e `TurnManager` rodam de forma autoritativa no host.
- Jogar carta vira `ServerRpc`, validado pelo turno e pela mao do jogador.
- Sinal corporal vira evento curto replicado por `ClientRpc`.
- Placar e valor da rodada ficam em estado de rede.
- UI local reage a eventos de estado, nao decide regra.

