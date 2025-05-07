using UnityEngine;
using UnityEngine.UI;

public class MissionGallina : MonoBehaviour
{
    public GameObject missionUI;
    public Text missionText;
    private bool missionActive = false;
    private bool gallinaPhotoTaken = false;
    private bool canDeliverPhoto = false;

    private MissionCaballo missionCaballo;

    void Start()
    {
        missionUI.SetActive(false);
        missionText.text = "";
        missionCaballo = FindObjectOfType<MissionCaballo>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (missionActive && gallinaPhotoTaken && !canDeliverPhoto)
            {
                canDeliverPhoto = true;
                missionText.text = "Pulsa T para entregar la foto de la gallina.";
            }
            else if (canDeliverPhoto && Input.GetKeyDown(KeyCode.T))
            {
                DeliverGallinaPhoto();
            }
            else if (!missionActive)
            {
                missionActive = true;
                missionUI.SetActive(true);
                missionText.text = "Misión: Captura una foto de la gallina.";
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
        missionText.text = "¡Foto entregada hola!";
        canDeliverPhoto = false;

        // Iniciar la misión del caballo
        if (missionCaballo != null)
        {
            missionCaballo.StartMission();
        }
    }
}
