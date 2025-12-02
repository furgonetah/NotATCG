# TCG Project - Trading Card Game

Un juego de cartas coleccionables tactico desarrollado en Unity 2D, donde dos jugadores compiten en partidas al mejor de 3 rondas utilizando cartas de ataque, defensa, especiales y trampas con un sistema de modificadores dinamicos.

## Tabla de Contenidos

- [Caracteristicas](#caracteristicas)
- [Reglas del Juego](#reglas-del-juego)
- [Stack Tecnologico](#stack-tecnologico)
- [Arquitectura del Proyecto](#arquitectura-del-proyecto)
- [Instalacion](#instalacion)
- [Como Jugar](#como-jugar)
- [Estructura del Proyecto](#estructura-del-proyecto)

---

## Caracteristicas

### Modos de Juego
- **Multijugador Online**: Conectate con otro jugador a traves de Photon PUN 2

### Sistema de Cartas
- **4 tipos de cartas**:
  - **Ataque**: Infligen dano plano o porcentual
  - **Defensa**: Curan PV de forma plana o porcentual
  - **Especiales**: Efectos unicos (robar cartas, modificadores, aumentar limite de cartas por turno)
  - **Trampas**: Se activan segun condiciones del juego

### Mecanicas Avanzadas
- **Sistema de modificadores**: Las cartas pueden aplicar efectos temporales que potencian las siguientes cartas (duplicar valores, anadir dano, multiplicar curacion, etc.)
- **Descripciones dinamicas**: Las cartas muestran valores modificados en tiempo real
- **Drag & Drop con reordenamiento**: Organiza tus cartas visualmente con animaciones fluidas
- **Seleccion tactica**: Elige hasta 2 cartas por turno (configurable) y define su orden de ejecucion

### UI/UX
- Animaciones suaves con DOTween
- Barras de vida con gradientes de color dinamicos
- Indicadores visuales de rondas ganadas
- Rotacion dinamica de cartas durante el drag
- Feedback visual claro para seleccion de cartas

---

## Reglas del Juego

### Objetivo
- Partida al **mejor de 3 rondas**
- Gana reduciendo los **PV del rival a 0**
- Cada jugador empieza cada ronda con **100 PV**

### Mazos y Cartas
- Cada jugador tiene **25 cartas**
- Al inicio de la **partida** (no cada ronda), cada jugador roba **10 cartas**
- No se roba carta al inicio del turno ni al empezar rondas
- El mazo se **baraja** al inicio
- No existe descarte manual
- Las cartas no se reponen entre rondas

#### Tipos de Cartas
| Tipo | Descripcion |
|------|-------------|
| **Ataque** | Infligen dano al oponente (plano o porcentual) |
| **Defensa** | Curan PV propios (plano o porcentual) |
| **Especiales** | Modifican reglas o estados (robar cartas, aplicar modificadores) |
| **Trampas** | Esperan una condicion especifica y se activan automaticamente |

### Sistema de Tiempo
- Temporizador independiente por jugador (estilo ajedrez)
- Al llegar a 0 tiempo: **pierde 2 PV/segundo**
- Duracion total configurable (por defecto 10 minutos)

### Rondas
- PV vuelven a **100** al empezar cada ronda
- Las cartas en mano **se conservan** entre rondas
- Si empiezas una ronda sin cartas, puedes robar **pagando PV**

### Robo de Cartas
- **No se roba automaticamente**
- Si te quedas sin cartas en mano:
  - Puedes robar hasta 9 cartas pagando **10 PV por carta**
  - Debes tener **minimo 11 PV** para poder robar
- Las cartas especiales que digan "robar" solo consumen PV si lo especifican

### Turnos
- Puedes jugar **0, 1 o 2 cartas** por turno (configurable)
- El **orden de las cartas importa** y determina el resultado
- Tras elegir cartas y orden, se ejecutan en secuencia

#### Carta Basica Regenerativa
- Cada turno recibes una carta basica de ataque
- Si la usas, **cuenta como 1 carta jugada**
- Desaparece al final del turno
- No se acumula

#### Pasar Turno
- Puedes pasar turno sin jugar cartas
- Usar carta basica != pasar (cuenta como 1 jugada)

### Resolucion
- Se ejecutan primero las cartas del jugador activo
- Tras **cada carta**, se comprueban trampas del rival
- Dano siempre secuencial: **nunca hay empate simultaneo**

### Trampas
- Colocar trampa cuenta como jugar 1 carta
- Son visibles para quien las coloca
- El oponente sabe que hay trampa, pero no cual es
- Sin limite de trampas activas

### Dano / Curacion
- Dano porcentual y fijo siguen el orden jugado
- Redondeo **hacia abajo** (12.5 -> 12)
- Limite de PV = **100**
- Puede haber **Puntos de Escudo** independientes de PV (futuro)

### Empates
- **No existen empates**: el dano es secuencial, alguien llega a 0 antes

### Anti-bloqueo
- Existe carta basica de ataque para evitar bloqueos totales
- Si no tienes PV para robar y no tienes cartas, juegas hasta perder (es parte del juego)

---

## Stack Tecnologico

### Core
- **Motor**: Unity 6000.0.35f1 (Unity 2023+)
- **Rendering**: Universal Render Pipeline (URP) 17.0.4
- **Lenguaje**: C# (.NET Standard 2.1)

### Librerias y Paquetes
| Paquete | Version | Uso |
|---------|---------|-----|
| Unity Input System | 1.14.2 | Sistema de input moderno |
| Unity UI (uGUI) | 2.0.0 | Interfaz de usuario |
| TextMesh Pro | 3.0.6 | Renderizado de texto |
| DOTween | 1.2.765 | Animaciones fluidas (Demigiant) |
| Photon PUN 2 | 2.x | Multijugador en linea |

---

## Arquitectura del Proyecto

### Patron de Diseno
El proyecto sigue una arquitectura **Manager-driven** con **Singleton patterns** para managers globales.

### Componentes Principales

#### Game Managers
- **GameManager**: Singleton que orquesta el juego completo (single-player)
- **PhotonGameManager**: Extiende GameManager para multijugador con sincronizacion de red
- **TurnManager**: Controla el flujo de turnos y cambio de jugadores
- **GameState**: Contenedor centralizado de estado del juego

#### Sistema de Cartas
```
Card (abstract)
├── AttackCard (dano plano/porcentual)
├── DefenseCard (curacion plana/porcentual)
├── SpecialCard (abstract)
│   ├── DrawCardsSpecial
│   ├── IncreaseCardLimitSpecial
│   ├── DoubleNextCardSpecial
│   └── AddDamageNextCardSpecial
└── TrapCard (efectos condicionales - futuro)
```

#### Modificadores
- **CardModifier**: Sistema de efectos temporales que potencian cartas
- **ModifierApplicationHelper**: Helper estatico para aplicar modificadores sin duplicacion de codigo
- Tipos: MultiplyAllValues, MultiplyDamage, MultiplyHealing, AddFlatDamage, AddFlatHealing, MultiplyCardDraw

#### Sistema de Jugadores
- **Player**: Gestiona HP, mano, mazo y modificadores activos
- **Hand**: Administra cartas en mano (maximo 10)
- **Deck**: Gestiona mazo de 25 cartas (barajado, robo)

#### UI
- **HandDisplayUI**: Visualizacion de mano con drag & drop y seleccion
- **CardVisual**: Representacion visual e interaccion de una carta (separado de logica)
- **HealthBarUI**: Barra de vida animada con gradientes
- **RoundIndicatorUI**: Indicadores visuales de rondas ganadas
- **VictoryUI**: Pantalla de victoria
- **EndTurnButton**: Boton de fin de turno con control de disponibilidad

#### Sistema de Red (Photon)
- **NetworkManager**: Gestiona conexion a Photon (solo en Lobby)
- **PhotonGameManager**: Manager del juego en red con RPCs
- **PhotonCardQueue**: Sincronizacion de ejecucion de cartas en red
- **PhotonPlayer**: Wrapper para sincronizacion de jugadores (opcional)
- **RoomUI**: UI de creacion/union a salas

---

## Instalacion

### Requisitos Previos
- Unity 2023.x o superior (recomendado: Unity 6000.0.35f1)
- Git (para clonar el repositorio)
- Cuenta de Photon (opcional, solo para multijugador)

### Pasos de Instalacion

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/tu-usuario/tcg-project.git
   cd tcg-project
   ```

2. **Abrir en Unity Hub**
   - Abre Unity Hub
   - Click en "Add" -> "Add project from disk"
   - Selecciona la carpeta del proyecto
   - Unity instalara las dependencias automaticamente

3. **Configurar Photon (opcional, solo para multijugador)**
   - Crea una cuenta en [Photon Engine](https://www.photonengine.com/)
   - Obten tu App ID de PUN 2
   - En Unity: `Window` -> `Photon Unity Networking` -> `PUN Wizard`
   - Pega tu App ID

4. **Abrir escena principal**
   - Abre `Assets/Scenes/Lobby.unity` (para multijugador)
   - O `Assets/Scenes/GameOnline.unity` (para single-player en desarrollo)

5. **Configurar Input System**
   - Si Unity pregunta sobre el Input System, selecciona "Yes" para reiniciar
   - El proyecto usa el nuevo Input System de Unity

---

## Como Jugar

### Controles
- **Hover sobre carta**: Previsualizacion ampliada
- **Click en carta**: Seleccionar/deseleccionar carta para jugar
- **Drag & Drop**: Reordenar cartas en la mano
- **Boton "End Turn"**: Finalizar turno y ejecutar cartas seleccionadas

### Flujo de Juego

1. **Inicio de Partida**
   - Ambos jugadores roban 10 cartas
   - Se inicia el temporizador global
   - Player 1 comienza

2. **Durante tu Turno**
   - Selecciona hasta 2 cartas de tu mano (por defecto)
   - El orden de seleccion importa
   - Click en "End Turn" para ejecutar
   - Las cartas se ejecutan secuencialmente

3. **Fin de Ronda**
   - Cuando un jugador llega a 0 PV, el otro gana la ronda
   - PV se resetean a 100
   - Las cartas en mano se conservan
   - Cambia el jugador inicial

4. **Victoria**
   - El primer jugador en ganar 2 rondas gana la partida

---

## Estructura del Proyecto

```
Assets/
├── Plugins/
│   └── Demigiant/
│       └── DOTween/              # Libreria de animaciones
├── Prefabs/
│   ├── Cards/                    # Prefabs de cartas individuales
│   └── UI/                       # Prefabs de UI
├── Resources/
│   ├── Icons/                    # Iconos de tipos de carta
│   └── DOTweenSettings.asset     # Configuracion de DOTween
├── Scenes/
│   ├── Lobby.unity               # Escena de lobby multijugador
│   └── GameOnline.unity          # Escena de juego principal
├── Scripts/
│   ├── Ambient/
│   │   └── CloudMovement.cs      # Animacion de nubes de fondo
│   ├── Cards/
│   │   ├── Card.cs               # Clase base abstracta de cartas
│   │   ├── AttackCard.cs         # Cartas de ataque
│   │   ├── DefenseCard.cs        # Cartas de defensa
│   │   ├── SpecialCard.cs        # Cartas especiales (base + 4 tipos)
│   │   ├── CardModifier.cs       # Sistema de modificadores
│   │   ├── ModifierApplicationHelper.cs  # Helper para aplicar modificadores
│   │   ├── CardVisual.cs         # Representacion visual de carta
│   │   ├── CardQueue.cs          # Cola de cartas a ejecutar
│   │   └── CardSlot.cs           # Contenedor de carta en UI
│   ├── Managers/
│   │   ├── GameManager.cs        # Manager principal (single-player)
│   │   ├── TurnManager.cs        # Gestion de turnos
│   │   ├── GameState.cs          # Estado centralizado del juego
│   │   ├── HandManager.cs        # Gestion de visibilidad de manos
│   │   └── UIManager.cs          # Coordinacion general de UI
│   ├── Network/
│   │   ├── NetworkManager.cs     # Gestion de conexion Photon
│   │   ├── PhotonGameManager.cs  # Manager del juego en red
│   │   ├── PhotonCardQueue.cs    # Sincronizacion de cartas en red
│   │   ├── PhotonPlayer.cs       # Wrapper de Player para red
│   │   └── RoomUI.cs             # UI de salas multijugador
│   ├── Player/
│   │   ├── Player.cs             # Jugador (HP, mano, mazo, modificadores)
│   │   ├── Hand.cs               # Mano del jugador
│   │   └── Deck.cs               # Mazo del jugador
│   ├── UI/
│   │   ├── HandDisplayUI.cs      # Visualizacion de mano con drag & drop
│   │   ├── CardVisual.cs         # Visual de carta individual
│   │   ├── HealthBarUI.cs        # Barra de vida animada
│   │   ├── RoundIndicatorUI.cs   # Indicadores de rondas
│   │   ├── VictoryUI.cs          # Pantalla de victoria
│   │   └── EndTurnButton.cs      # Boton de fin de turno
│   └── GameConstants.cs          # Constantes centralizadas del juego
├── Sprites/                      # Assets de sprites 2D
│   └── UI/                       # Sprites de interfaz
└── Settings/                     # Configuracion de Unity (URP, Input, etc.)
```

---

## Documentacion para Desarrolladores

### Constantes del Juego
Todas las constantes estan centralizadas en `GameConstants.cs` para facilitar el balanceo:
- Valores de animaciones de cartas
- HP maximo, penalizaciones, limites
- Configuracion de turnos y rondas
- Timings del juego

---

## Futuras Mejoras

- [ ] Sistema de trampas completo (TrapCard + TrapManager)
- [ ] Tutorial interactivo (flags `isTutorialActive` ya presentes)
- [ ] Sistema de escudos independiente de PV
- [ ] Feedback visual y audio para dano/curacion
- [ ] Mas tipos de cartas especiales
- [ ] Sistema de progresion y desbloqueo de cartas
- [ ] Modo torneo/ranking en linea
- [ ] Replays y estadisticas de partidas

---

## Notas de Desarrollo

### Conocidos TODOs en el Codigo
- Timer system refinement (valores de tiempo necesitan revision)
- UI feedback para queueing de cartas (highlights/animaciones)
- Player damage/healing visual y audio feedback
- TrapManager implementation (actualmente trampas se verifican en CardQueue)

---

## Creditos

**Desarrollador Principal**: César Fuentes Ayuso

**Librerias de Terceros**:
- DOTween (c) Demigiant
- Photon PUN 2 (c) Exit Games
- Unity Engine (c) Unity Technologies

---

