using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CapturePhoto : MonoBehaviour
{
    public Renderer photoPlane;
    public Camera photoCamera;
    public GameObject camera;
    public GameObject visor;
    public AudioSource cameraSound; // 🔊 Efecto de sonido

    private string directoryPath;
    private List<string> photoPaths = new List<string>();
    private int currentPhotoIndex = 0;
    private bool isAlbumMode = false;

    private MissionManager missionManager;

    public float detectionRange = 50f;
    private bool gallinaDetectedThisFrame = false;
    private bool caballoDetectedThisFrame = false;
    private bool ciervoDetectedThisFrame = false;
    private bool osoDetectedThisFrame = false;

    void Start()
    {
        missionManager = FindObjectOfType<MissionManager>();
        if (missionManager == null)
        {
            Debug.LogError("No se encontró el MissionManager.");
        }

        directoryPath = Path.Combine(Application.persistentDataPath, "Screenshots");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        SetMode(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetMode(!isAlbumMode);
        }

        if (isAlbumMode)
        {
            if (photoPaths.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    currentPhotoIndex = (currentPhotoIndex + 1) % photoPaths.Count;
                    UpdatePhotoPlane(photoPaths[currentPhotoIndex]);
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    currentPhotoIndex = (currentPhotoIndex - 1 + photoPaths.Count) % photoPaths.Count;
                    UpdatePhotoPlane(photoPaths[currentPhotoIndex]);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(TakeScreenshot());
            }

            DetectGallina();
            DetectCaballo();
            DetectCiervo();
            DetectOso();
        }
    }

    private void SetMode(bool albumMode)
    {
        isAlbumMode = albumMode;
        camera.SetActive(!albumMode);
        visor.SetActive(albumMode);

        if (isAlbumMode && photoPaths.Count > 0)
        {
            currentPhotoIndex = 0;
            UpdatePhotoPlane(photoPaths[currentPhotoIndex]);
        }
    }

    private void DetectGallina() => gallinaDetectedThisFrame = RaycastTag("Gallina");
    private void DetectCaballo() => caballoDetectedThisFrame = RaycastTag("Caballo");
    private void DetectCiervo() => ciervoDetectedThisFrame = RaycastTag("Ciervo");
    private void DetectOso() => osoDetectedThisFrame = RaycastTag("Oso");

    private bool RaycastTag(string tag)
    {
        Ray ray = photoCamera.ScreenPointToRay(new Vector3(photoCamera.pixelWidth / 2, photoCamera.pixelHeight / 2, 0));
        return Physics.Raycast(ray, out RaycastHit hit, detectionRange) && hit.collider.CompareTag(tag);
    }

    private IEnumerator TakeScreenshot()
    {
        int originalCullingMask = photoCamera.cullingMask;
        int uiVisorLayer = LayerMask.NameToLayer("UIVisor");
        photoCamera.cullingMask &= ~(1 << uiVisorLayer);

        // 🔊 Reproducir sonido de cámara
        if (cameraSound != null)
        {
            cameraSound.Play();
        }

        yield return new WaitForEndOfFrame();

        string photoName = "Photo_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string photoPath = Path.Combine(directoryPath, photoName);

        ScreenCapture.CaptureScreenshot(photoPath);
        Debug.Log("¡Foto tomada! Guardada en: " + photoPath);

        photoPaths.Add(photoPath);
        currentPhotoIndex = photoPaths.Count - 1;

        photoCamera.cullingMask = originalCullingMask;

        yield return new WaitForSeconds(0.5f);
        UpdatePhotoPlane(photoPath);

        // Llamar al MissionManager para procesar la foto tomada
        if (gallinaDetectedThisFrame)
            missionManager.OnGallinaPhotoCaptured();
        if (caballoDetectedThisFrame)
            missionManager.OnCaballoPhotoCaptured();
        if (ciervoDetectedThisFrame)
            missionManager.OnCiervoPhotoCaptured();
        if (osoDetectedThisFrame)
            missionManager.OnOsoPhotoCaptured();
    }

    private void UpdatePhotoPlane(string photoPath)
    {
        if (File.Exists(photoPath))
        {
            byte[] photoBytes = File.ReadAllBytes(photoPath);
            Texture2D photoTexture = new Texture2D(2, 2);
            photoTexture.LoadImage(photoBytes);
            photoPlane.material.mainTexture = photoTexture;
            Debug.Log("Foto cargada en el visor.");
        }
        else
        {
            Debug.LogWarning("No se encontró la foto para cargarla en el visor.");
        }
    }
}
