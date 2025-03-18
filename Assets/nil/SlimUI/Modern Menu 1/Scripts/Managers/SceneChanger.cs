using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void LoadNewScene(string nameScene)
    {
       SceneManager.LoadScene (nameScene);
    }
}
