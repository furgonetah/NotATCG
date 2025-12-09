# 🎴 TCG Project - Trading Card Game

> Un juego de cartas táctico 2D multijugador desarrollado en Unity. Batalla 1v1 al mejor de 3 rondas con un sistema de modificadores dinámicos y combos estratégicos.

[![Unity](https://img.shields.io/badge/Unity-6000.0.35f1-black?logo=unity)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-.NET%20Standard%202.1-purple?logo=csharp)](https://docs.microsoft.com/dotnet/csharp/)
[![Photon](https://img.shields.io/badge/Photon-PUN%202-blue?logo=photon)](https://www.photonengine.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)



## 🚀 Quick Start

### Jugar Ahora (Build Precompilado)
```bash
# Descarga el último release
# Descomprime y ejecuta TCGProject.exe
# ¡Listo para jugar!
```

### Para Desarrolladores
```bash
# 1. Clonar el repositorio
git clone https://github.com/tu-usuario/tcg-project.git

# 2. Abrir en Unity Hub
# Unity 2023.x o superior
# El proyecto instala dependencias automáticamente

# 3. Ejecutar escena
# Abrir: Assets/Scenes/GameOffline.unity (single-player)
# O: Assets/Scenes/GameOnline.unity (multiplayer)
```

---

## ✨ Características Principales

### 🎮 Gameplay Táctico
- **Sistema de turnos estratégico** - Juega hasta 2 cartas por turno, el orden importa
- **Modificadores dinámicos** - Crea combos encadenando efectos (x2 daño, +10 daño, etc.)
- **4 tipos de cartas** - Ataque, Defensa, Especiales, Trampas
- **Mejor de 3 rondas** - Cartas se conservan entre rondas, HP se resetea a 100

### 🎨 UI/UX Pulida
- **Drag & Drop fluido** - Reordena cartas con animaciones suaves (DOTween)
- **Feedback visual claro** - Cartas seleccionadas se elevan, rotación durante drag
- **Descripciones dinámicas** - Los valores de cartas se actualizan en tiempo real con modificadores
- **Animaciones responsivas** - Barras de vida, indicadores de ronda, transiciones

### 🌐 Multijugador Online
- **Photon PUN 2** - Matchmaking y sincronización en tiempo real
- **Master Client Authority** - Lógica centralizada para evitar trampas
- **Regiones optimizadas** - Selección automática de mejor servidor (latencia)

### 🎯 Mecánicas Avanzadas
- **Sistema de penalizaciones** - Robar sin cartas cuesta 10 HP (requiere 11+ HP)
- **Timer individual** - Como ajedrez, al agotar tiempo pierdes 2 HP/segundo
- **Ejecución secuencial** - Sin empates, el orden de las cartas define el resultado
- **Carta básica regenerativa** - Siempre disponible para evitar bloqueos

---

## 🎲 Cómo Jugar

### Controles
| Acción | Control |
|--------|---------|
| **Seleccionar carta** | Click izquierdo |
| **Reordenar cartas** | Drag & Drop |
| **Ver detalles** | Hover sobre carta |
| **Finalizar turno** | Botón "Fin de Turno" |

### Flujo Básico
1. **Inicio** → Ambos jugadores roban 10 cartas
2. **Tu turno** → Selecciona hasta 2 cartas (el orden importa)
3. **Ejecución** → Las cartas se ejecutan secuencialmente
4. **Victoria** → Reduce el HP del rival a 0 → Gana la ronda → Primero en 2 rondas gana

### Tipos de Cartas

| Tipo | Descripción | Ejemplo |
|------|-------------|---------|
| ⚔️ **Ataque** | Inflige daño al oponente | "Ataque Rápido: 15 daño" |
| 🛡️ **Defensa** | Cura tus PV | "Curación Leve: +20 HP" |
| ✨ **Especial** | Efectos únicos y modificadores | "Duplicar: x2 próxima carta" |
| 🪤 **Trampa (a implementar)** | Se activa automáticamente | "Reflejo: Devuelve 50% daño" |


---

## 🛠️ Stack Tecnológico

### Core
- **Unity** 6000.0.35f1 (Unity 2023+)
- **C#** (.NET Standard 2.1)
- **URP** 17.0.4 (Universal Render Pipeline)

### Librerías Principales
| Librería | Versión | Uso |
|----------|---------|-----|
| **DOTween** | 1.2.765 | Animaciones fluidas |
| **Photon PUN 2** | 2.x | Networking multijugador |
| **Unity Input System** | 1.14.2 | Sistema de input moderno |
| **TextMesh Pro** | 3.0.6 | Renderizado de texto |

---

## 📖 Reglas del Juego (Completas)

<details>
<summary><b>Click para ver reglas detalladas</b></summary>

### 🎯 Objetivo
- Partida al **mejor de 3 rondas**
- Gana reduciendo los **PV del rival a 0**
- Cada ronda empieza con **100 PV**

### 🃏 Mazos y Cartas
- **25 cartas** por mazo
- **10 cartas** iniciales (solo al inicio de la partida)
- No se roba automáticamente cada turno
- El mazo se baraja al inicio y **no se rebaraja**
- Las cartas en mano **se conservan entre rondas**

### ⏱️ Sistema de Tiempo
- **Temporizador individual** por jugador (estilo ajedrez)
- Al llegar a **0 tiempo**: pierdes **2 HP/segundo**
- Duración total: **10 minutos** (configurable)

### 🔄 Turnos
- Juega **0, 1 o 2 cartas** por turno (configurable)
- **El orden importa**: las cartas se ejecutan secuencialmente
- Carta básica de ataque disponible cada turno

### 📥 Robo de Cartas (Penalizado)
Si te quedas sin cartas:
- Puedes robar **pagando 10 HP por carta**
- Requiere **mínimo 11 HP**
- Las cartas especiales que roban **no cuestan HP** (salvo indicación contraria)

### 🔢 Daño y Curación
- **Daño porcentual**: Redondeo hacia abajo (12.5 → 12)
- **Curación porcentual**: Redondeo hacia arriba (12.5 → 13)
- **Límite de HP**: 100 (no se puede superar)
- **Ejecución secuencial**: No hay empates

### 🪤 Trampas (WIP)
- Se colocan en tu turno (cuenta como 1 carta jugada)
- Se activan automáticamente al cumplir condición
- Visibles para ti, ocultas para el oponente
- Sin límite de trampas activas

### 🚫 Anti-Bloqueo
- **Carta básica siempre disponible** para evitar bloqueos
- Si no puedes robar (sin HP) y no tienes cartas: pierdes gradualmente

</details>

---

## 🏗️ Arquitectura del Proyecto

### Patrón de Diseño
**Manager-Driven Architecture** con **Singleton Pattern** para acceso global.

### Componentes Principales

```
🎮 Game Layer
├─ GameManager (Singleton) ── Orquestación principal
├─ TurnManager ────────────── Control de turnos
├─ GameState ──────────────── Estado centralizado
└─ HandManager ────────────── Visibilidad de manos

🃏 Card System
├─ Card (abstract)
│   ├─ AttackCard ─────────── Daño plano/porcentual
│   ├─ DefenseCard ────────── Curación plana/porcentual
│   ├─ SpecialCard (abstract)
│   │   ├─ DrawCardsSpecial
│   │   ├─ DoubleNextCardSpecial
│   │   └─ AddDamageNextCardSpecial
│   └─ TrapCard ───────────── Efectos condicionales (WIP)
│
├─ CardModifier ───────────── Sistema de efectos temporales
├─ ModifierApplicationHelper ─ Aplicación consistente de mods
└─ CardQueue ──────────────── Cola de ejecución de cartas

👥 Player System
├─ Player ─────────────────── HP, mano, mazo, modificadores
├─ Hand ───────────────────── Gestión de cartas en mano
└─ Deck ───────────────────── Barajado y robo de cartas

🎨 UI System
├─ HandDisplayUI ──────────── Drag & drop, selección
├─ CardVisual ─────────────── Visualización e interacción
├─ HealthBarUI ────────────── Barras de vida animadas
├─ RoundIndicatorUI ───────── Indicadores de rondas
└─ VictoryUI ──────────────── Pantalla de victoria

🌐 Network System (Photon)
├─ NetworkManager ─────────── Conexión a Photon
├─ PhotonGameManager ──────── Game manager con RPCs
├─ PhotonCardQueue ────────── Sincronización de cartas
└─ RoomUI ─────────────────── Lobby y matchmaking
```


---

## 📂 Estructura de Archivos

<details>
<summary><b>Click para ver estructura completa</b></summary>

```
Assets/
├── Plugins/Demigiant/DOTween/    # Librería de animaciones
├── Photon/                        # SDK de Photon PUN 2
├── Prefabs/
│   ├── Cards/                     # Prefabs de cartas (Attack, Defense, Special)
│   └── UI/                        # Prefabs de interfaz
├── Resources/
│   ├── Icons/                     # Iconos de tipos de carta
│   └── DOTweenSettings.asset      # Configuración de DOTween
├── Scenes/
│   ├── GameOffline.unity          # Modo single-player
│   └── GameOnline.unity           # Modo multijugador
├── Scripts/
│   ├── Cards/                     # Sistema de cartas completo
│   │   ├── Card.cs                # Clase base abstracta
│   │   ├── AttackCard.cs
│   │   ├── DefenseCard.cs
│   │   ├── SpecialCard.cs
│   │   ├── CardModifier.cs
│   │   ├── ModifierApplicationHelper.cs
│   │   ├── CardVisual.cs
│   │   ├── CardQueue.cs
│   │   └── CardSlot.cs
│   ├── Managers/                  # Managers del juego
│   │   ├── GameManager.cs
│   │   ├── TurnManager.cs
│   │   ├── GameState.cs
│   │   ├── HandManager.cs
│   │   └── UIManager.cs
│   ├── Network/                   # Sistema de red Photon
│   │   ├── NetworkManager.cs
│   │   ├── PhotonGameManager.cs
│   │   ├── PhotonCardQueue.cs
│   │   └── RoomUI.cs
│   ├── Player/                    # Sistema de jugadores
│   │   ├── Player.cs
│   │   ├── Hand.cs
│   │   └── Deck.cs
│   ├── UI/                        # Componentes de UI
│   │   ├── HandDisplayUI.cs
│   │   ├── CardVisual.cs
│   │   ├── HealthBarUI.cs
│   │   ├── RoundIndicatorUI.cs
│   │   ├── VictoryUI.cs
│   │   └── EndTurnButton.cs
│   ├── Ambient/
│   │   └── CloudMovement.cs       # Efectos de fondo
│   └── GameConstants.cs           # Constantes centralizadas
├── Sprites/                       # Assets 2D
└── Settings/                      # Configuración URP, Input System
```

</details>

---

## 🔧 Instalación y Desarrollo

### Requisitos
- **Unity** 2023.x o superior (recomendado: 6000.0.35f1)
- **Git** para clonar el repositorio
- **Cuenta Photon** (opcional, solo para multijugador)

### Instalación Paso a Paso

#### 1. Clonar Repositorio
```bash
git clone https://github.com/tu-usuario/tcg-project.git
cd tcg-project
```

#### 2. Abrir en Unity
- Abre **Unity Hub**
- Click en **"Add" → "Add project from disk"**
- Selecciona la carpeta `Project TCG`
- Unity instalará dependencias automáticamente (puede tardar 5-10 min)

#### 3. Configurar Photon (Solo para Multijugador)
1. Crea una cuenta gratuita en [Photon Engine](https://www.photonengine.com/)
2. Crea una nueva aplicación tipo **"Photon PUN"**
3. Copia tu **App ID**
4. En Unity: `Window → Photon Unity Networking → PUN Wizard`
5. Pega tu App ID y click en **"Setup Project"**


#### 4. Configurar Input System
Si Unity pregunta sobre el **Input System**, selecciona **"Yes"** para reiniciar.

#### 5. Ejecutar el Juego
- **Single-player**: Abre `Assets/Scenes/GameOffline.unity` → Play
- **Multijugador**: Abre `Assets/Scenes/GameOnline.unity` → Play (requiere 2 instancias)

### Testing Multiplayer Local
Para probar multijugador en local:
1. Haz un **Build** del proyecto (Ctrl+Shift+B)
2. Ejecuta el **build** en una ventana
3. Ejecuta el **Editor** en otra ventana
4. Crea sala en una, únete desde la otra

---

## 📚 Documentación

### Para Desarrolladores
- **[Manual del Programador](MANUAL_PROGRAMADOR.md)** - Arquitectura completa, estructuras de datos, flujos de código
- **[Guía de Contribución](CONTRIBUTING.md)** - Cómo contribuir al proyecto
- **[API Reference](docs/API.md)** - Documentación de clases y métodos

### Para Diseñadores
- **[Crear Nueva Carta](docs/CrearCarta.md)** - Guía paso a paso
- **[Balanceo de Juego](docs/Balanceo.md)** - Constantes y tweaking
- **[Assets y Arte](docs/Assets.md)** - Guía de estilo visual

### Documentación Técnica Clave
- **GameConstants.cs** - Todas las constantes del juego (HP, daño, timings)
- **Patrones de diseño** - Singleton, Template Method, Strategy, Observer
- **Sistema de modificadores** - Cómo funcionan los efectos temporales
- **Networking** - Arquitectura de sincronización con Photon

---

## 🗺️ Roadmap

### ✅ Completado (v1.0)
- [x] Sistema de cartas completo (Attack, Defense, Special)
- [x] Sistema de modificadores dinámicos
- [x] UI con drag & drop fluido
- [x] Multijugador online con Photon PUN 2
- [x] Sistema de turnos y rondas
- [x] Animaciones con DOTween
- [x] Timer individual y penalizaciones

### 🚧 En Progreso (v1.1)
- [ ] Sistema de trampas completo (TrapCard + TrapManager)
- [ ] Tutorial interactivo paso a paso
- [ ] Efectos visuales para daño/curación
- [ ] Sistema de audio (SFX + música)

### 🔮 Futuro (v2.0+)
- [ ] Sistema de escudos independiente de HP
- [ ] Deck builder in-game (editor de mazos)
- [ ] Modo torneo/ranking online
- [ ] Replays y estadísticas de partidas
- [ ] Más tipos de cartas especiales (15+ nuevas)
- [ ] Sistema de progresión y desbloqueo
- [ ] Matchmaking por ELO


---

## 🎓 Trabajo de Fin de Grado (TFG)

Este proyecto fue desarrollado como **Trabajo de Fin de Grado** para el grado de **DAM**.


### Objetivos del Proyecto
1. Diseñar e implementar un juego de cartas táctico multijugador
2. Aplicar patrones de diseño de software en videojuegos
3. Implementar networking en tiempo real con sincronización determinista
4. Crear una arquitectura escalable y mantenible
5. Desarrollar un sistema de modificadores dinámicos innovador

---

## 🤝 Contribuir

¡Las contribuciones son bienvenidas! Por favor, lee la [guía de contribución](CONTRIBUTING.md) antes de enviar un PR.

### Áreas donde Ayudar
- 🐛 **Reportar bugs** - Crea un issue con detalles
- ✨ **Nuevas cartas** - Diseña y propón nuevas mecánicas
- 🎨 **Assets** - Arte, iconos, sprites
- 📖 **Documentación** - Mejora los docs
- 🧪 **Testing** - Prueba y reporta problemas

### Proceso
1. Fork el proyecto
2. Crea una rama feature (`git checkout -b feature/NuevaCartas`)
3. Commit tus cambios (`git commit -m 'Add: Carta de Teletransporte'`)
4. Push a la rama (`git push origin feature/NuevaCartas`)
5. Abre un Pull Request

---

## 📄 Licencia

Este proyecto está bajo la licencia **MIT**. Ver [LICENSE](LICENSE) para más detalles.

**Librerías de Terceros:**
- **DOTween** © Demigiant - [Licencia](http://dotween.demigiant.com/license.php)
- **Photon PUN 2** © Exit Games - [Licencia](https://www.photonengine.com/en-US/sdks)
- **Unity** © Unity Technologies - [Licencia](https://unity.com/legal/terms-of-service)

---

## 👤 Autor

**César Fuentes Ayuso**
- 🐱 GitHub: [@furgonetah](https://github.com/futgonetah)

---

## 🙏 Agradecimientos

- **Profesores** del grado por su guía y feedback
- **Comunidad de Unity** por recursos y tutoriales
- **Demigiant** por la excelente librería DOTween
- **Exit Games** por Photon PUN 2
- **Beta testers** que ayudaron a balancear el juego

---

## 📞 Soporte

¿Necesitas ayuda o encontraste un bug?

- 📋 **Issues**: [GitHub Issues](https://github.com/tu-usuario/tcg-project/issues)
- 💬 **Discusiones**: [GitHub Discussions](https://github.com/tu-usuario/tcg-project/discussions)
- 📧 **Email**: cesar.fuentes@example.com

---

<div align="center">

**⭐ Si te gusta el proyecto, dale una estrella en GitHub ⭐**

[![Star on GitHub](https://img.shields.io/github/stars/furgonetah/not-a-tcg?style=social)](https://github.com/furgonetah/not-a-tcg)

Hecho con ❤️ usando Unity

</div>
