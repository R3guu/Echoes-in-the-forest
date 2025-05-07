using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class TextoWin : MonoBehaviour
{
    string frase = "Enhorabuena, Alex.\r\n\r\nHas logrado lo que muy pocos siquiera han imaginado: capturar un retrato del legendario oso Sombra… y más aún, ganarte su confianza.\r\n\r\nEl Bosque Espectral te reconoce ahora como uno de los suyos.\r\n\r\nEres más que un fotógrafo: eres parte del misterio.\r\n\r\nTe conviertes en el primer explorador conocido en forjar un vínculo con el guardián del bosque.\r\n\r\nGracias por tu valentía, tu paciencia y tu mirada única.\r\n\r\nFin del juego.\r\n\r\nGracias por jugar.";
    public TextMeshProUGUI texto;
    public AudioSource audioSource; // Arrastra el audio desde el inspector
    private bool textoTerminado = false;

    void Start()
    {
        // Mostrar y desbloquear el cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(Reloj());
    }

    void Update()
    {
        if (textoTerminado && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    IEnumerator Reloj()
    {
        audioSource.Play(); // Empieza el audio al iniciar el tipeo

        foreach (char character in frase)
        {
            texto.text += character;
            yield return new WaitForSeconds(0.02f);
        }

        audioSource.Stop(); // Detiene el audio cuando termina
        textoTerminado = true;
    }
}
