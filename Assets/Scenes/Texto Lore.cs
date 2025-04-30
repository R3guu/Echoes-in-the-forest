using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TextoLore : MonoBehaviour
{

    string frase = "Eres Alex, un fotógrafo de naturaleza que ha dejado atrás su vida rutinaria en la ciudad para adentrarse en el misterioso Bosque Espectral. Con tu furgoneta camper equipada con cámaras, objetivos y un dron, tu misión es capturar imágenes únicas de la fauna salvaje para revistas y clientes apasionados por la naturaleza.\r\n\r\nPero el bosque esconde secretos. Se habla de Sombra, un oso legendario que protege su territorio con astucia y fuerza. A medida que exploras el entorno, deberás utilizar tus herramientas para moverte con cautela y completar tus encargos fotográficos.\r\n\r\nTu objetivo final: ganarte la confianza de Sombra y capturar la fotografía más difícil de todas —un retrato del propio guardián del bosque. \r\n\r\nAl iniciar el juego acercate a la caravana para empezar el juego!!! \r\n\r\nPara empezar pulsa ENTER";
    public TextMeshProUGUI texto;
    private bool textoTerminado = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Reloj());
    }
    void Update()
    {
        if (textoTerminado && Input.GetKeyDown(KeyCode.Return)) // Enter
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
    IEnumerator Reloj()
    {
        foreach (char character in frase)
        {
            texto.text = texto.text + character;
            yield return new WaitForSeconds(0.02f);
        }
        textoTerminado = true; // Ahora ya se puede pulsar Enter
    }
}   
