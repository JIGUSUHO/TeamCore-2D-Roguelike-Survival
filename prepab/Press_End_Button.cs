using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Press_End_Button : MonoBehaviour
{
    [Header("설정할 이미지들")]
    public Image targetButtonImage;
    public Sprite clickedSprite;

    public void ClickEnd()
    {
        if (targetButtonImage != null && clickedSprite != null)
        {
            targetButtonImage.sprite = clickedSprite;
            SceneManager.LoadScene("Main");
        }
    }
}