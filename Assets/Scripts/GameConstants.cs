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

    //Distancia vertical (px) que sube una carta al seleccionarla
    public const float CARD_SELECTION_OFFSET = 50f;

    //Escala multiplicadora cuando el cursor está sobre la carta
    public const float CARD_HOVER_SCALE = 1.1f;

    //Duración estándar de animaciones de cartas (segundos)
    public const float CARD_ANIMATION_DURATION = 0.2f;

    //Duración de animaciones rápidas como swap (segundos)
    public const float CARD_SWAP_ANIMATION_DURATION = 0.1f;

    //Velocidad de suavizado de rotación durante drag
    public const float CARD_ROTATION_SMOOTH_SPEED = 10f;

    //Ángulo máximo de rotación al arrastrar una carta (grados)
    public const float CARD_MAX_ROTATION_ANGLE = 40f;

    //Sensibilidad de rotación basada en velocidad de drag
    public const float CARD_ROTATION_SENSITIVITY = 1f;

    //Transparencia de la carta mientras se arrastra (0-1)
    public const float CARD_DRAG_ALPHA = 0.7f;

    //Velocidad de decaimiento de rotación cuando no hay movimiento
    public const float CARD_ROTATION_DECAY_SPEED = 5f;

    //Offset horizontal para animación de swap entre cartas
    public const float CARD_SWAP_OFFSET = 10f;

    // ==========================================
    // PLAYER - Configuración de jugadores
    // ==========================================

    //Puntos de vida máximos por ronda
    public const int PLAYER_MAX_HP = 100;

    ///HP mínimo requerido para robar cartas con penalización
    public const int PLAYER_PENALTY_THRESHOLD = 11;

    //Daño recibido al robar una carta cuando la mano está vacía
    public const int PLAYER_DRAW_PENALTY = 10;

    //Daño por segundo cuando se agota el tiempo del jugador
    public const int TIMEOUT_DAMAGE_PER_SECOND = 2;

    // ==========================================
    // HAND & DECK - Configuración de mano y mazo
    // ==========================================

    //Tamaño máximo de la mano
    public const int HAND_MAX_SIZE = 10;

    //Número de cartas que se pueden jugar por turno (por defecto)
    public const int CARDS_PER_TURN = 2;

    //Cartas iniciales que roba cada jugador al inicio de la partida
    public const int INITIAL_DRAW_COUNT = 10;

    //Tamaño del mazo de cada jugador
    public const int DECK_SIZE = 25;

    // ==========================================
    // GAME - Configuración general del juego
    // ==========================================

    //Número máximo de rondas (mejor de 3)
    public const int MAX_ROUNDS = 3;

    //Rondas necesarias para ganar la partida
    public const int ROUNDS_TO_WIN = 2;

    //Tiempo total de juego en segundos (configurable)
    public const float TOTAL_GAME_TIME = 600f; // 10 minutos
}
