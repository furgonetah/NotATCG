# Instrucciones de Configuración - Multijugador PUN2

## Scripts Creados
Todos los scripts de red han sido creados en `Assets/Scripts/Network/`:
- ✅ NetworkManager.cs
- ✅ RoomUI.cs
- ✅ PhotonGameManager.cs
- ✅ PhotonCardQueue.cs
- ✅ PhotonPlayer.cs

## Scripts Modificados
- ✅ HandDisplayUI.cs - Control de input local
- ✅ TurnManager.cs - Integración con PhotonCardQueue

---

## PASO 1: Configurar Escena de Lobby (Nueva Escena)

### 1.1 Crear Nueva Escena
1. File → New Scene → Crear escena llamada "Lobby"
2. Guardar en `Assets/Scenes/Lobby.unity`

### 1.2 Crear NetworkManager GameObject
1. Create Empty GameObject → Nombre: "NetworkManager"
2. Add Component → NetworkManager script
3. Add Component → Photon View (se agregará automáticamente)

### 1.3 Crear UI de Lobby

**Jerarquía necesaria:**
```
Canvas (modo Screen Space - Overlay)
├── Panel_MainMenu
│   ├── InputField_RoomName (placeholder: "Nombre de sala")
│   ├── Button_CreateRoom (texto: "Crear Sala")
│   ├── InputField_JoinCode (placeholder: "Código de sala")
│   ├── Button_JoinRoom (texto: "Unirse")
│   └── Text_Status (texto inicial: "Conectando...")
└── Panel_WaitingRoom (desactivado por defecto)
    ├── Text_RoomName
    └── Text_Players
```

**Pasos:**
1. Click derecho en Hierarchy → UI → Canvas
2. Crear los Panels y elementos UI según la estructura de arriba
3. Asignar TextMeshPro a todos los textos
4. Desactivar Panel_WaitingRoom en Inspector

### 1.4 Configurar RoomUI Script
1. Create Empty GameObject en Canvas → Nombre: "RoomUIManager"
2. Add Component → RoomUI script
3. Arrastrar referencias en Inspector:
   - Main Menu Panel → Panel_MainMenu
   - Room Name Input → InputField_RoomName
   - Create Room Button → Button_CreateRoom
   - Join Code Input → InputField_JoinCode
   - Join Room Button → Button_JoinRoom
   - Status Text → Text_Status
   - Waiting Room Panel → Panel_WaitingRoom
   - Room Name Text → Text_RoomName (del Panel_WaitingRoom)
   - Players Count Text → Text_Players (del Panel_WaitingRoom)

---

## PASO 2: Configurar Escena de Juego (SampleScene)

### 2.1 Modificar GameManager Existente
**OPCIÓN A - Reemplazar GameManager con PhotonGameManager (RECOMENDADO):**
1. Seleccionar GameObject que tiene GameManager
2. Remove Component "GameManager"
3. Add Component → PhotonGameManager script
4. Add Component → Photon View
5. Configurar referencias (igual que GameManager anterior):
   - player1
   - player2
   - turnManager
   - cardQueue
   - victoryUI
   - maxRounds, totalGameTime

**OPCIÓN B - Mantener ambos (para testing):**
1. Desactivar GameManager cuando estés en modo multijugador
2. Crear nuevo GameObject "PhotonGameManager" con el script

### 2.2 Agregar PhotonCardQueue al GameObject de TurnManager
1. Seleccionar GameObject que tiene TurnManager
2. Add Component → PhotonCardQueue script
3. Add Component → Photon View
4. En Inspector de PhotonCardQueue:
   - Arrastrar el componente CardQueue a la referencia "Card Queue"

### 2.3 Agregar PhotonPlayer a los GameObjects de Player
1. Seleccionar GameObject de Player1
2. Add Component → PhotonPlayer script
3. Add Component → Photon View
4. En Inspector de PhotonPlayer:
   - La referencia "Player" debería auto-asignarse
   - Si no, arrastrar el componente Player

5. Repetir para Player2

### 2.4 Verificar HandDisplayUI
- No requiere cambios manuales, ya está modificado en código
- Automáticamente detectará si es jugador local

---

## PASO 3: Configurar Build Settings

1. File → Build Settings
2. Agregar escenas en este orden:
   - [0] Lobby
   - [1] SampleScene
3. Click "Add Open Scenes" para cada una

---

## PASO 4: Configurar Photon Settings

1. Window → Photon Unity Networking → Highlight Server Settings
2. Verificar que AppId esté configurado
3. En PhotonServerSettings:
   - App Settings → PUN → Verificar AppId
   - Auto-Join Lobby: ✅ TRUE
   - Enable Lobby Stats: ✅ TRUE

---

## PASO 5: Testing Local

### 5.1 Build para Testing
1. File → Build Settings
2. Platform: PC, Mac & Linux Standalone
3. Build → Guardar en carpeta "Builds/"
4. Nombre: "TCG_Multiplayer.exe" (o según tu OS)

### 5.2 Proceso de Testing
1. **Ejecutar Build:**
   - Abrir TCG_Multiplayer.exe
   - En InputField_RoomName escribir: "TestRoom"
   - Click "Crear Sala"
   - Esperar mensaje "Esperando segundo jugador..."

2. **Ejecutar Unity Editor:**
   - Play en Unity Editor
   - Debería ver escena Lobby
   - En InputField_JoinCode escribir: "TestRoom"
   - Click "Unirse"

3. **Verificar:**
   - Ambos deberían cambiar a "¡Iniciando partida...!"
   - Ambos deberían cargar SampleScene automáticamente
   - El juego debe iniciar con mazos barajados (diferentes manos)

---

## PASO 6: Debugging

### 6.1 Logs para Verificar
Buscar en Console estos logs:
```
[MASTER] Generando semillas: seed1=XXX, seed2=YYY
[CLIENT] Sincronizando mazos con semillas...
[MASTER] Mazo de Player1 barajado con seed XXX
[CLIENT] Mazo de Player1 barajado con seed XXX
[HandDisplayUI] Player1 es LOCAL
[HandDisplayUI] Player2 es REMOTO
```

### 6.2 Problemas Comunes

**Problema:** "PhotonView not found"
- Solución: Agregar componente PhotonView a GameObjects necesarios

**Problema:** "AppId not configured"
- Solución: Window → Photon Unity Networking → PUN Wizard → Re-configurar AppId

**Problema:** Ambos clientes no ven las mismas cartas
- Verificar logs de semillas - deben ser idénticos en ambos clientes
- Verificar que Random.InitState() se llama antes de ShuffleDeck()

**Problema:** No puedo hacer click en cartas
- Verificar logs de HandDisplayUI - debe decir "es LOCAL" para tu jugador
- Verificar que PhotonGameManager.Instance.localPlayer esté asignado correctamente

**Problema:** Cartas se ejecutan dos veces
- NO usar RPCs dentro de Card.Play() - ambos clientes ya ejecutan el mismo código

---

## PASO 7: Checklist de Testing

Casos de prueba según el plan:

- [ ] Ambos jugadores se conectan a sala
- [ ] Lobby muestra "2/2 jugadores"
- [ ] Escena de juego se carga automáticamente
- [ ] Mazos se barajan con semillas diferentes (manos distintas)
- [ ] Ambos clientes ven las mismas cartas en ambas manos
- [ ] Solo jugador local puede hacer click en sus cartas
- [ ] Jugador 1 juega carta → Jugador 2 ve la carta jugada
- [ ] HP sincronizado correctamente después de ejecutar cartas
- [ ] Fin de turno funciona correctamente
- [ ] Swap de jugador activo sincronizado
- [ ] Fin de ronda sincronizado (muerte, rounds won)
- [ ] Victoria sincronizada (mejor de 3)
- [ ] Desconexión otorga victoria al jugador restante

---

## Notas Finales

### Compatibilidad con Single-Player
- Todos los scripts son compatibles con modo single-player
- Si no estás conectado a Photon, el juego funciona normalmente
- HandDisplayUI automáticamente detecta modo local
- TurnManager usa CardQueue normal en single-player

### Próximos Pasos (Opcional)
- Agregar UI de "Esperando turno del oponente"
- Animaciones de cartas jugadas por el oponente
- Sistema de chat
- Matchmaking aleatorio (en lugar de código de sala)
- Reconnection (actualmente deshabilitado)

### Performance
- PUN2 Free: Límite de 20 CCU (usuarios concurrentes)
- Suficiente para testing y pequeña escala
- Para más usuarios: Actualizar a PUN+ (pago)

---

## Archivos de Referencia
- Plan completo: `C:\Users\Usuario\.claude\plans\crispy-popping-naur.md`
- Scripts de red: `Assets/Scripts/Network/`
