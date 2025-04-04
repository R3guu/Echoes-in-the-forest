using UnityEngine;
using UnityEngine.UI;

public class MissionCaballo : MonoBehaviour
{
    public GameObject missionUI;
    public Text missionText;
    private bool missionActive = false;
    private bool caballoPhotoTaken = false;
    private bool canDeliverCaballoPhoto = false;

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

    public void CompleteMission()
    {
        missionText.text = "¡Misión completada! ¡Foto del caballo entregada!";
        missionActive = false;
        Invoke("HideMissionUI", 3f);
    }

    void HideMissionUI()
    {
        missionUI.SetActive(false);
    }
}
