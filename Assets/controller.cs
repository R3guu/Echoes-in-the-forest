using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para reiniciar la escena

public class Controller : MonoBehaviour
{
    public GameObject gameOverCanvas; // Arrástralo desde el Inspector

    void Start()
    {
        gameOverCanvas.SetActive(false); // Asegurar que GameOver está oculto al iniciar
    }

    public void MatarJugador()
    {
        Debug.Log("¡Has muerto! Game Over.");
        gameOverCanvas.SetActive(true); // Mostrar Game Over
        Time.timeScale = 0f; // Pausar el juego
    }

    public void ReiniciarPartida()
    {
        Time.timeScale = 1f; // Reactivar el tiempo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recargar la escena actual
    }
}
