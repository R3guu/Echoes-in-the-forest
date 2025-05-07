using UnityEngine;
using UnityEngine.UI;

public class MissionCaballo : MonoBehaviour
{
    public GameObject missionUI;
    public Text missionText;
    public MissionCiervo missionCiervo;

    private bool missionActive = false;
    private bool caballoPhotoTaken = false;

    void Start()
    {
        missionUI.SetActive(false);
        missionText.text = "";
    }

    public void StartMission()
    {
        if (!missionActive)
        {
            missionActive = true;
            missionUI.SetActive(true);
            missionText.text = "Misión: Captura una foto del caballo.";
        }
    }

    public void CaballoPhotoCaptured()
    {
        if (missionActive && !caballoPhotoTaken)
        {
            caballoPhotoTaken = true;
            missionText.text = "¡Foto del caballo tomada! Vuelve a la furgoneta para entregarla.";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (missionActive && caballoPhotoTaken)
            {
                missionText.text = "Pulsa T para entregar la foto del caballo.";
            }
            else if (Input.GetKeyDown(KeyCode.T) && caballoPhotoTaken)
            {
                // Ahora aseguramos que solo se llame a CompleteMission
                CompleteMission();
            }
        }
    }

    public void CompleteMission()
    {
        missionText.text = "¡Misión completada! ¡Foto del caballo entregada!";
        missionActive = false;

        // Inicia automáticamente la misión del ciervo
        if (missionCiervo != null)
        {
            missionCiervo.StartMission();
        }
    }
}
