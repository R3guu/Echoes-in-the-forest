using UnityEngine;
using System.IO;
using System.Collections; // �Esta l�nea es necesaria para usar IEnumerator!

public class CapturePhoto : MonoBehaviour
{
    public Renderer photoPlane; // Plano donde se mostrar� la �ltima foto tomada
    public Camera photoCamera; // C�mara utilizada para tomar las fotos
    private string photoPath; // Ruta donde se guardar� la foto

    void Start()
    {
        // Crear un directorio para las fotos si no existe
        string directoryPath = Path.Combine(Application.dataPath, "Screenshots");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        photoPath = Path.Combine(directoryPath, "lastPhoto.png"); // Ruta para la �ltima foto
    }

    void Update()
    {
        // Detectar clic izquierdo para tomar una foto
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(TakeScreenshot());
        }
    }

    private IEnumerator TakeScreenshot()
    {
        // Guardar la m�scara de culling original de la c�mara
        int originalCullingMask = photoCamera.cullingMask;

        // Excluir la capa "UIVisor" de la c�mara
        int uiVisorLayer = LayerMask.NameToLayer("UIVisor");
        photoCamera.cullingMask &= ~(1 << uiVisorLayer);

        // Esperar al final del fotograma para capturar correctamente
        yield return new WaitForEndOfFrame();

        // Capturar la pantalla y guardarla como PNG
        ScreenCapture.CaptureScreenshot(photoPath);

        Debug.Log("�Foto tomada! Guardada en: " + photoPath);

        // Restaurar la m�scara de culling original
        photoCamera.cullingMask = originalCullingMask;

        // Cargar la foto en el plano
        yield return new WaitForSeconds(0.5f); // Esperar un momento para asegurarse de que se guarda
        UpdatePhotoPlane();
    }

    private void UpdatePhotoPlane()
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
            Debug.LogWarning("No se encontr� la foto para cargarla en el plano.");
        }
    }
}
