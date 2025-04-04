using UnityEngine;
using UnityEngine.UI;

public class MissionGallina : MonoBehaviour
{
    public GameObject missionUI; // Panel de la lista de misiones
    public Text missionText; // Texto de la misión
    private bool missionActive = false;
    private bool gallinaPhotoTaken = false;
    private bool canDeliverPhoto = false;

    private MissionCaballo missionCaballo; // Referencia a la misión del caballo

    void Start()
    {
        missionUI.SetActive(false);
        missionText.text = "";
        missionCaballo = FindObjectOfType<MissionCaballo>(); // Encuentra la misión del caballo
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!missionActive)
            {
                missionActive = true;
                missionUI.SetActive(true);
                missionText.text = "Misión: Captura una foto de la gallina.";
            }
            else if (gallinaPhotoTaken && !canDeliverPhoto)
            {
                canDeliverPhoto = true;
                missionText.text = "Pulsa T para entregar la foto.";
            }
            else if (canDeliverPhoto && !gallinaPhotoTaken)
            {
                missionText.text = "¡Esperando la foto del caballo!";
            }
        }
    }

    void Update()
    {
        if (canDeliverPhoto && Input.GetKeyDown(KeyCode.T))
        {
            DeliverGallinaPhoto();
        }
    }

    public void GallinaPhotoCaptured()
    {
        if (missionActive && !gallinaPhotoTaken)
        {
            gallinaPhotoTaken = true;
            missionText.text = "¡Foto de la gallina tomada! Vuelve a la furgoneta para entregarla.";
        }
    }

    void DeliverGallinaPhoto()
    {
        missionText.text = "¡Foto entregada!";
        canDeliverPhoto = false;

        // Activar la misión del caballo
        missionCaballo.StartMission(); // Inicia la misión del caballo
        Invoke("HideMissionUI", 3f);
    }

    void HideMissionUI()
    {
        missionUI.SetActive(true);
    }
}
