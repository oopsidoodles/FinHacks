using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchScene(string newScene)
    {
        SceneManager.LoadScene(newScene);
    }

    public void SwitchScene(int newScene)
    {
        SceneManager.LoadScene(newScene);
    }
}
