using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempWeaponManager : MonoBehaviour
{
    public GameObject swordPrefab;    // 인스펙터에서 짠 검 프리팹을 끌어다 넣습니다.
    public Transform playerTransform; // 인스펙터에서 player 오브젝트를 끌어다 넣습니다.

    public int swordCount = 5;
    private void Start()
    {
        // 게임 시작 시 테스트용으로 검을 하나 생성합니다.
        SpawnOrbitingSword();
    }

    private void SpawnOrbitingSword()
    {
        float angleStep = 360f / swordCount;
        for (int i = 0; i < swordCount; i++)
        {
            // [임시 코드] 나중에 ObjectPool.Instance.Get() 등으로 교체될 부분
            GameObject swordObj = Instantiate(swordPrefab, playerTransform.position, Quaternion.identity);

            Weapon swordScript = swordObj.GetComponent<Weapon>();
            if (swordScript != null)
            {
                // i가 0, 1, 2, 3, 4로 늘어남에 따라
                // 시작 각도는 0도, 72도, 144도, 216도, 288도가 됩니다.
                float currentStartAngle = angleStep * i;

                // 각 검에게 계산된 시작 각도를 전달하여 세팅합니다.
                swordScript.Setup(playerTransform, currentStartAngle);
            }
        }
    }
}
