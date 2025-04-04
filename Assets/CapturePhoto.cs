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
    private string directoryPath;
    private List<string> photoPaths = new List<string>();
    private int currentPhotoIndex = 0;
    private bool isAlbumMode = false;
    private MissionGallina missionGallina;
    private MissionCaballo missionCaballo;

    public float detectionRange = 50f;
    private bool gallinaDetectedThisFrame = false;
    private bool caballoDetectedThisFrame = false; // <-- nuevo flag para el caballo

    void Start()
    {
        missionGallina = FindObjectOfType<MissionGallina>();
        missionCaballo = FindObjectOfType<MissionCaballo>(); // <-- buscar misión del caballo

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

        if (!isAlbumMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(TakeScreenshot());
            }

            DetectGallina();
            DetectCaballo(); // <-- detectar caballo también
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ShowNextPhoto();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ShowPreviousPhoto();
            }
        }
    }

    private void SetMode(bool albumMode)
    {
        isAlbumMode = albumMode;
        camera.SetActive(!albumMode);
        visor.SetActive(albumMode);
    }

    private void DetectGallina()
    {
        RaycastHit hit;
        Ray ray = photoCamera.ScreenPointToRay(new Vector3(photoCamera.pixelWidth / 2, photoCamera.pixelHeight / 2, 0));

        if (Physics.Raycast(ray, out hit, detectionRange))
        {
            gallinaDetectedThisFrame = hit.collider.CompareTag("Gallina");
        }
        else
        {
            gallinaDetectedThisFrame = false;
        }
    }

    // NUEVA FUNCIÓN: Detectar Caballo
    private void DetectCaballo()
    {
        RaycastHit hit;
        Ray ray = photoCamera.ScreenPointToRay(new Vector3(photoCamera.pixelWidth / 2, photoCamera.pixelHeight / 2, 0));

        if (Physics.Raycast(ray, out hit, detectionRange))
        {
            caballoDetectedThisFrame = hit.collider.CompareTag("Caballo");
        }
        else
        {
            caballoDetectedThisFrame = false;
        }
    }

    private IEnumerator TakeScreenshot()
    {
        if (isAlbumMode) yield break;

        int originalCullingMask = photoCamera.cullingMask;
        int uiVisorLayer = LayerMask.NameToLayer("UIVisor");
        photoCamera.cullingMask &= ~(1 << uiVisorLayer);

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

        if (gallinaDetectedThisFrame)
        {
            Debug.Log("¡La gallina apareció en la foto!");
            missionGallina?.GallinaPhotoCaptured();
        }

        if (caballoDetectedThisFrame)
        {
            Debug.Log("¡El caballo apareció en la foto!");
            missionCaballo?.CaballoPhotoCaptured(); // <-- nueva notificación
        }
    }

    private void UpdatePhotoPlane(string photoPath)
    {
        if (File.Exists(photoPath))
        {
            byte[] photoBytes = File.ReadAllBytes(photoPath);
            Texture2D photoTexture = new Texture2D(2, 2);
            photoTexture.LoadImage(photoBytes);
            photoPlane.material.mainTexture = photoTexture;
            Debug.Log("Foto cargada en el plano.");
        }
        else
        {
            Debug.LogWarning("No se encontró la foto para cargarla en el plano.");
        }
    }

    private void ShowNextPhoto()
    {
        if (photoPaths.Count > 0)
        {
            currentPhotoIndex = (currentPhotoIndex + 1) % photoPaths.Count;
            UpdatePhotoPlane(photoPaths[currentPhotoIndex]);
        }
    }

    private void ShowPreviousPhoto()
    {
        if (photoPaths.Count > 0)
        {
            currentPhotoIndex = (currentPhotoIndex - 1 + photoPaths.Count) % photoPaths.Count;
            UpdatePhotoPlane(photoPaths[currentPhotoIndex]);
        }
    }
}
