using UnityEngine;
using UnityEngine.SceneManagement;

public class Press_Start_Button : MonoBehaviour
{
    public void ClickStart()
    {
        SceneManager.LoadScene("InGame0");
    }
}