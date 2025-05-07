using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSampleScene : MonoBehaviour
{
    // Este método puede llamarse desde el botón
    public void GoToSampleScene()
    {
        SceneManager.LoadScene("Demo1");
    }
}

