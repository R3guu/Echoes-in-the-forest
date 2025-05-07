using UnityEngine;
using UnityEngine.UI;

public class MissionCiervo : MonoBehaviour
{
    public GameObject missionUI;
    public Text missionText;
    public MissionOso missionOso;

    private bool missionActive = false;
    private bool ciervoPhotoTaken = false;

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
            missionText.text = "Misión: Captura una foto del ciervo.";
        }
    }

    public void CiervoPhotoCaptured()
    {
        if (missionActive && !ciervoPhotoTaken)
        {
            ciervoPhotoTaken = true;
            missionText.text = "¡Foto del ciervo tomada! Vuelve a la furgoneta para entregarla.";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (missionActive && ciervoPhotoTaken)
            {
                missionText.text = "Pulsa T para entregar la foto del ciervo.";
            }
            else if (Input.GetKeyDown(KeyCode.T) && ciervoPhotoTaken)
            {
                CompleteMission();
            }
        }
    }

    public void CompleteMission()
    {
        missionText.text = "¡Misión completada! ¡Foto del ciervo entregada!";
        missionActive = false;

        // Inicia automáticamente la misión del oso
        if (missionOso != null)
        {
            missionOso.StartMission();
        }
    }
}
