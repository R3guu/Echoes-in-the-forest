using UnityEngine;
using UnityEngine.UI;

public class MissionOso : MonoBehaviour
{
    public GameObject missionUI;
    public Text missionText;
    private bool missionActive = false;
    private bool osoPhotoTaken = false;

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
            missionText.text = "Misión: Captura una foto del oso Sombra.";
        }
    }

    public void OsoPhotoCaptured()
    {
        if (missionActive && !osoPhotoTaken)
        {
            osoPhotoTaken = true;
            missionText.text = "¡Foto de Sombra tomada! Vuelve a la furgoneta para entregarla.";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (missionActive && osoPhotoTaken)
            {
                missionText.text = "Pulsa T para entregar la foto de Sombra.";
            }
            else if (Input.GetKeyDown(KeyCode.T) && osoPhotoTaken)
            {
                CompleteMission();
            }
        }
    }

    public void CompleteMission()
    {
        missionText.text = "¡Misión completada! ¡Foto del oso Sombra entregada!";
        missionActive = false;
        Invoke("HideMissionUI", 3f);
    }

    void HideMissionUI()
    {
        missionUI.SetActive(false);
    }
}
