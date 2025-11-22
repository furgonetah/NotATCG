using UnityEngine;

/// <summary>
/// Constantes centralizadas del juego para evitar números mágicos.
/// Facilita el balanceo y mantenimiento del juego.
/// </summary>
public static class GameConstants
{
    // ==========================================
    // CARD VISUAL - Configuración de animaciones y efectos visuales
    // ==========================================

    /// <summary>Distancia vertical (px) que sube una carta al seleccionarla</summary>
    public const float CARD_SELECTION_OFFSET = 50f;

    /// <summary>Escala multiplicadora cuando el cursor está sobre la carta</summary>
    public const float CARD_HOVER_SCALE = 1.1f;

    /// <summary>Duración estándar de animaciones de cartas (segundos)</summary>
    public const float CARD_ANIMATION_DURATION = 0.2f;

    /// <summary>Duración de animaciones rápidas como swap (segundos)</summary>
    public const float CARD_SWAP_ANIMATION_DURATION = 0.1f;

    /// <summary>Velocidad de suavizado de rotación durante drag</summary>
    public const float CARD_ROTATION_SMOOTH_SPEED = 10f;

    /// <summary>Ángulo máximo de rotación al arrastrar una carta (grados)</summary>
    public const float CARD_MAX_ROTATION_ANGLE = 40f;

    /// <summary>Sensibilidad de rotación basada en velocidad de drag</summary>
    public const float CARD_ROTATION_SENSITIVITY = 1f;

    /// <summary>Transparencia de la carta mientras se arrastra (0-1)</summary>
    public const float CARD_DRAG_ALPHA = 0.7f;

    /// <summary>Velocidad de decaimiento de rotación cuando no hay movimiento</summary>
    public const float CARD_ROTATION_DECAY_SPEED = 5f;

    /// <summary>Offset horizontal para animación de swap entre cartas</summary>
    public const float CARD_SWAP_OFFSET = 10f;

    // ==========================================
    // PLAYER - Configuración de jugadores
    // ==========================================

    /// <summary>Puntos de vida máximos por ronda</summary>
    public const int PLAYER_MAX_HP = 100;

    /// <summary>HP mínimo requerido para robar cartas con penalización</summary>
    public const int PLAYER_PENALTY_THRESHOLD = 11;

    /// <summary>Daño recibido al robar una carta cuando la mano está vacía</summary>
    public const int PLAYER_DRAW_PENALTY = 10;

    /// <summary>Daño por segundo cuando se agota el tiempo del jugador</summary>
    public const int TIMEOUT_DAMAGE_PER_SECOND = 2;

    // ==========================================
    // HAND & DECK - Configuración de mano y mazo
    // ==========================================

    /// <summary>Tamaño máximo de la mano</summary>
    public const int HAND_MAX_SIZE = 10;

    /// <summary>Número de cartas que se pueden jugar por turno (por defecto)</summary>
    public const int CARDS_PER_TURN = 2;

    /// <summary>Cartas iniciales que roba cada jugador al inicio de la partida</summary>
    public const int INITIAL_DRAW_COUNT = 10;

    /// <summary>Tamaño del mazo de cada jugador</summary>
    public const int DECK_SIZE = 25;

    // ==========================================
    // GAME - Configuración general del juego
    // ==========================================

    /// <summary>Número máximo de rondas (mejor de 3)</summary>
    public const int MAX_ROUNDS = 3;

    /// <summary>Rondas necesarias para ganar la partida</summary>
    public const int ROUNDS_TO_WIN = 2;

    /// <summary>Tiempo total de juego en segundos (configurable)</summary>
    public const float TOTAL_GAME_TIME = 600f; // 10 minutos
}
