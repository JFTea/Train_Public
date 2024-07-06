using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreen : MonoBehaviour
{
    /// <summary>
    /// Switches scene to the scene with the index number
    /// </summary>
    /// <param name="index">The index of the screen to switch to</param>
    public void SwitchScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
