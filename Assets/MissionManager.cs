using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // Para cambiar de escena y usar corutinas

public class MissionManager : MonoBehaviour
{
    public Text instructionText;

    private bool gallinaPhotoTaken = false;
    private bool caballoPhotoTaken = false;
    private bool ciervoPhotoTaken = false;
    private bool osoPhotoTaken = false;

    private bool waitingForDelivery = false;
    private bool missionStarted = false;
    private bool isNearVan = false;

    void Start()
    {
        if (instructionText == null)
        {
            Debug.LogError("No se ha asignado un objeto Text en la UI.");
        }

        UpdateInstructions("Pulsa clic izquierdo para sacar una foto.\nPulsa E y usa las flechas para ver el álbum de fotos.\nAcércate a la furgoneta para empezar tu misión.");
    }

    void Update()
    {
        if (isNearVan && waitingForDelivery)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (gallinaPhotoTaken)
                {
                    DeliverGallinaPhoto();
                }
                else if (caballoPhotoTaken)
                {
                    DeliverCaballoPhoto();
                }
                else if (ciervoPhotoTaken)
                {
                    DeliverCiervoPhoto();
                }
                else if (osoPhotoTaken)
                {
                    DeliverOsoPhoto();
                }
            }
        }
    }

    public void OnGallinaPhotoCaptured()
    {
        if (!missionStarted) return;

        Debug.Log("¡La gallina apareció en la foto!");
        gallinaPhotoTaken = true;
        UpdateInstructions("¡Foto de la gallina tomada! Vuelve a la furgoneta y pulsa 'T' para entregarla.");
        waitingForDelivery = true;
    }

    public void OnCaballoPhotoCaptured()
    {
        Debug.Log("¡El caballo apareció en la foto!");
        caballoPhotoTaken = true;
        UpdateInstructions("¡Foto del caballo tomada! Vuelve a la furgoneta y pulsa 'T' para entregarla.");
        waitingForDelivery = true;
    }

    public void OnCiervoPhotoCaptured()
    {
        Debug.Log("¡El ciervo apareció en la foto!");
        ciervoPhotoTaken = true;
        UpdateInstructions("¡Foto del ciervo tomada! Vuelve a la furgoneta y pulsa 'T' para entregarla.");
        waitingForDelivery = true;
    }

    public void OnOsoPhotoCaptured()
    {
        Debug.Log("¡El oso apareció en la foto!");
        osoPhotoTaken = true;
        UpdateInstructions("¡Foto del oso tomada! Vuelve a la furgoneta y pulsa 'T' para entregarla.");
        waitingForDelivery = true;

        // 🐻 Hacer que el oso se vuelva tu amigo al tomar la foto
        GameObject oso = GameObject.FindWithTag("Oso");
        if (oso != null)
        {
            MovimentOso comportamientoOso = oso.GetComponent<MovimentOso>();
            if (comportamientoOso != null)
            {
                comportamientoOso.HacerseAmigo();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearVan = true;

            if (!missionStarted)
            {
                missionStarted = true;
                UpdateInstructions("Objetivo: ¡Captura una foto de una gallina junto a la casa!");
            }
            else if (waitingForDelivery)
            {
                UpdateInstructions("Estás cerca de la furgoneta. Pulsa 'T' para entregar la foto.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearVan = false;

            if (waitingForDelivery)
            {
                UpdateInstructions("Vuelve a la furgoneta para entregar la foto.");
            }
        }
    }

    private void DeliverGallinaPhoto()
    {
        Debug.Log("Foto de la gallina entregada.");
        gallinaPhotoTaken = false;
        waitingForDelivery = false;
        UpdateInstructions("Objetivo: Más allá encontrarás unos caballos llenos de energía.\r\n\r\n¡A ver si puedes conseguir hacerles una buena foto!");
        StartNextMission("caballo");
    }

    private void DeliverCaballoPhoto()
    {
        Debug.Log("Foto del caballo entregada.");
        caballoPhotoTaken = false;
        waitingForDelivery = false;
        UpdateInstructions("Objetivo: Ahora busca unos ciervos a lo lejos, vigila que corren mucho!");
        StartNextMission("ciervo");
    }

    private void DeliverCiervoPhoto()
    {
        Debug.Log("Foto del ciervo entregada.");
        ciervoPhotoTaken = false;
        waitingForDelivery = false;
        UpdateInstructions("Objetivo: Saca una foto a Sombra y conseguiras hacerte su amigo!");
        StartNextMission("oso");
    }

    private void DeliverOsoPhoto()
    {
        Debug.Log("Foto del oso entregada.");
        osoPhotoTaken = false;
        waitingForDelivery = false;
        UpdateInstructions("Misión del oso completada. ¡Felicidades, has terminado!");

        // 🎉 Cambiar de escena después de una breve pausa
        StartCoroutine(ChangeToWinScene());
    }

    private IEnumerator ChangeToWinScene()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("cambio Scenewin");
        SceneManager.LoadScene("SceneWin");
    }

    private void UpdateInstructions(string message)
    {
        if (instructionText != null)
        {
            instructionText.text = message;
        }
    }

    private void StartNextMission(string animal)
    {
        if (animal == "caballo")
        {
            UpdateInstructions("Objetivo: Más allá encontrarás unos caballos llenos de energía.\r\n\r\n¡A ver si puedes conseguir hacerles una buena foto!");
        }
        else if (animal == "ciervo")
        {
            UpdateInstructions("Objetivo: Ahora busca unos ciervos a lo lejos, vigila que corren mucho!");
        }
        else if (animal == "oso")
        {
            UpdateInstructions("Objetivo: Saca una foto a Sombra y conseguiras hacerte su amigo!");
        }
    }
}
