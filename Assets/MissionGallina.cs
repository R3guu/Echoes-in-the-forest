using UnityEngine;
using UnityEngine.UI;

public class MissionGallina : MonoBehaviour
{
    public GameObject missionUI; // Panel de la lista de misiones
    public Text missionText; // Texto de la misión
    public GameObject furgonetaTrigger; // Trigger de la furgoneta
    private bool missionActive = false;
    private bool gallinaPhotoTaken = false;
    private bool canDeliverPhoto = false;

    void Start()
    {
        missionUI.SetActive(false);
        missionText.text = "";
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
                missionText.text = "¡Vuelve a la furgoneta para entregar la foto!";
            }
            else if (canDeliverPhoto)
            {
                missionText.text = "Pulsa T para entregar la foto.";
            }
        }
    }

    void Update()
    {
        if (canDeliverPhoto && Input.GetKeyDown(KeyCode.T))
        {
            CompleteMission();
        }
    }

    public void GallinaPhotoCaptured()
    {
        if (missionActive)
        {
            gallinaPhotoTaken = true;
            missionText.text = "¡Foto tomada! Vuelve a la furgoneta para entregarla.";
        }
    }

    void CompleteMission()
    {
        missionText.text = "¡Misión completada!";
        canDeliverPhoto = false;
        Invoke("HideMissionUI", 3f);
    }

    void HideMissionUI()
    {
        missionUI.SetActive(false);
    }
}
