using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Maneja el botón de salir del juego
/// Cierra la aplicación cuando se presiona
/// </summary>
public class QuitGameButton : MonoBehaviour
{
    [Header("Referencias")]
    public Button quitButton;

    void Start()
    {
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }
        else
        {
            Debug.LogWarning("[QuitGameButton] No se asignó el botón en el Inspector");
        }
    }

    void OnQuitClicked()
    {
        Debug.Log("[QuitGameButton] Cerrando aplicación...");

        #if UNITY_EDITOR
        // En el editor de Unity, detener el modo de juego
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // En build, cerrar la aplicación
        Application.Quit();
        #endif
    }

    void OnDestroy()
    {
        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }
    }
}
