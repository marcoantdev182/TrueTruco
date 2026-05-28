# Truco de Mesa Online - MVP local

## Pastas

- `Assets/Scenes`: cena inicial `TrucoTableLocal.unity`.
- `Assets/TrucoDeMesaOnline/Scripts/Bootstrap`: raiz da cena local.
- `Assets/TrucoDeMesaOnline/Scripts/Core`: `GameManager`, constantes e utilitarios compartilhados.
- `Assets/TrucoDeMesaOnline/Scripts/Table`: mesa, assentos e posicionamento.
- `Assets/TrucoDeMesaOnline/Scripts/Player`: camera em primeira pessoa e controle local.
- `Assets/TrucoDeMesaOnline/Scripts/Cards`: carta, baralho, mao e visual placeholder.
- `Assets/TrucoDeMesaOnline/Scripts/Gameplay`: turno, rodada, vaza e placar.
- `Assets/TrucoDeMesaOnline/Scripts/Signals`: sinais corporais e gestos placeholder.
- `Assets/TrucoDeMesaOnline/Scripts/UI`: HUD criada em runtime.
- `Assets/TrucoDeMesaOnline/Scripts/Networking`: adaptadores iniciais para Netcode/Lobby/Relay.
- `Assets/TrucoDeMesaOnline/Art`: placeholders futuros de cartas, avatares e ambiente.
- `Assets/TrucoDeMesaOnline/Prefabs`: prefabs futuros quando os placeholders virarem assets.
- `Assets/TrucoDeMesaOnline/Materials`: materiais futuros editaveis.

## Como rodar agora

1. Abra a pasta `TrueTruco` como projeto Unity.
2. Abra `Assets/Scenes/TrucoTableLocal.unity`.
3. Aperte Play.
4. Clique em uma carta da mao para jogar.
5. Use mouse ou setas/WASD para olhar parceiro, rivais e mesa.
6. Clique nos botoes de sinais para emitir eventos no console/HUD.

## Cena local

A cena tem apenas um `LocalOfflineSceneRoot`. Em runtime ele cria o `GameManager`, e o `GameManager` monta:

- mesa redonda simples;
- quatro assentos fixos;
- camera no assento `Voce`;
- avatares placeholder para parceiro e rivais;
- rostos placeholder com olhos, sobrancelhas, boca, nariz, bochechas e lingua;
- baralho de Truco Paulista com vira e manilha;
- mao local visivel somente na HUD;
- bots simples que jogam a primeira carta disponivel;
- placar e valor da rodada;
- sinais como eventos de blefe livres;
- bots que podem fazer sinais antes de jogar; a HUD so mostra que voce viu se a camera estiver apontada para o avatar.

## Atalhos no Editor

- `Truco de Mesa > Create Local MVP Scene`: recria a cena local em `Assets/Scenes`.
- `Truco de Mesa > Add Local MVP To Current Scene`: adiciona o objeto raiz do MVP na cena atual, util quando o Unity abriu em `Untitled`.
