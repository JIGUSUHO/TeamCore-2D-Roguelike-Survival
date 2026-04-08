using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game State")]
    public bool isGameOver;   // 게임 오버 상태
    public float gameTime;    // 생존 시간 (타이머)
    public float maxGameTime = 1800f; // 최대 생존 시간 (예: 30분 = 1800초)

    void Awake()
    {
        if (instance == null)  
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 게임 오버가 아닐 때만 시간이 흘러가게 함
        if (!isGameOver)
        {
            gameTime += Time.deltaTime;
            
            // 최대 버티기 시간에 도달하면 게임 클리어 처리 
            if (gameTime >= maxGameTime)
            {
                gameTime = maxGameTime;
                // 나중에 여기에 "게임 클리어!" 띄우는 함수 연결
            }
        }
    }
}