using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CapturePhoto : MonoBehaviour
{
    public Renderer photoPlane; // Plano donde se mostrará la última foto tomada
    public Camera photoCamera; // Cámara utilizada para tomar las fotos
    public GameObject camera; // Objeto de la cámara en la mano
    public GameObject visor; // Objeto del visor (álbum de fotos)
    private string directoryPath; // Directorio donde se guardarán las fotos
    private List<string> photoPaths = new List<string>(); // Lista de rutas de las fotos
    private int currentPhotoIndex = 0; // Índice de la foto actual
    private bool isAlbumMode = false; // Indica si estamos en modo álbum o modo cámara

    void Start()
    {
        // Crear un directorio para las fotos si no existe
        directoryPath = Path.Combine(Application.persistentDataPath, "Screenshots");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Asegurar que comenzamos en modo cámara
        SetMode(false);
    }

    void Update()
    {
        // Alternar entre cámara y álbum con la tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            SetMode(!isAlbumMode);
        }

        if (!isAlbumMode)
        {
            // En modo cámara, permitir tomar fotos
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(TakeScreenshot());
            }
        }
        else
        {
            // En modo álbum, permitir navegar entre fotos
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

        // Mostrar u ocultar la cámara y el visor según el modo
        camera.SetActive(!albumMode);  // Activar/desactivar la cámara
        visor.SetActive(albumMode);    // Activar/desactivar el visor (álbum)

        Debug.Log("Modo cambiado. Cámara activo: " + !albumMode + ", Visor activo: " + albumMode);
    }

    private IEnumerator TakeScreenshot()
    {
        if (isAlbumMode) yield break; // No tomar fotos en modo álbum

        // Guardar la máscara de culling original de la cámara
        int originalCullingMask = photoCamera.cullingMask;

        // Excluir la capa "UIVisor" de la cámara
        int uiVisorLayer = LayerMask.NameToLayer("UIVisor");
        photoCamera.cullingMask &= ~(1 << uiVisorLayer);

        // Esperar al final del fotograma para capturar correctamente
        yield return new WaitForEndOfFrame();

        // Generar un nombre único para la foto
        string photoName = "Photo_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string photoPath = Path.Combine(directoryPath, photoName);

        // Capturar la pantalla y guardarla como PNG
        ScreenCapture.CaptureScreenshot(photoPath);

        Debug.Log("¡Foto tomada! Guardada en: " + photoPath);

        // Agregar la ruta a la lista
        photoPaths.Add(photoPath);
        currentPhotoIndex = photoPaths.Count - 1;

        // Restaurar la máscara de culling original
        photoCamera.cullingMask = originalCullingMask;

        // Esperar un momento para asegurarse de que se guarda y actualizar el plano
        yield return new WaitForSeconds(0.5f);
        UpdatePhotoPlane(photoPath);
    }

    private void UpdatePhotoPlane(string photoPath)
    {
        // Cargar la foto desde el archivo
        if (File.Exists(photoPath))
        {
            byte[] photoBytes = File.ReadAllBytes(photoPath);
            Texture2D photoTexture = new Texture2D(2, 2);
            photoTexture.LoadImage(photoBytes);

            // Asignar la textura al material del plano
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
