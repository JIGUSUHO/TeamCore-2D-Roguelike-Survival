using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class WeaponData
{
    public string weaponName;
    public GameObject weaponPrefab;
    public float cooldown;
    public int spawnCount = 1;

    [Header("레벨 시스템")]
    public int level = 0;
    public int maxLevel = 5;

    [Header("무기 스탯")]
    public float damage;
    public float slowRatio;

    [HideInInspector] public float currentTimer;
}

public class WeaponManager : MonoBehaviour
{

    // ★ [추가됨] 어디서든 WeaponManager.Instance 로 부를 수 있게 만듭니다!
    public static WeaponManager Instance; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    [Header("장착된 무기 목록")]
    public List<WeaponData> equippedWeapons = new List<WeaponData>();

    private void Update()
    {
        HandleWeaponCooldowns();

        // 테스트용 키보드 입력 (숫자 1번: 공전형 검, 숫자 2번: 음파 무기 레벨업)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) LevelUpWeapon("공전형 검");
            if (Keyboard.current.digit2Key.wasPressedThisFrame) LevelUpWeapon("음파 무기");
            if (Keyboard.current.digit3Key.wasPressedThisFrame) LevelUpWeapon("투사체 무기"); // [추가] 3번 키 누르면 투사체 레벨업!
        }
    }

    private void HandleWeaponCooldowns()
    {
        foreach (WeaponData weapon in equippedWeapons)
        {
            if (weapon.level <= 0 || weapon.cooldown <= 0 || weapon.weaponPrefab == null) continue;

            weapon.currentTimer += Time.deltaTime;

            if (weapon.currentTimer >= weapon.cooldown)
            {
                FireWeapon(weapon);
                weapon.currentTimer = 0f;
            }
        }
    }

    private void FireWeapon(WeaponData weapon)
    {
        int count = Mathf.Max(1, weapon.spawnCount);
        float angleStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            GameObject spawnedObj = Instantiate(weapon.weaponPrefab, transform.position, Quaternion.identity);

            // 1. 공전형 무기 세팅
            OrbitWeapon orbitScript = spawnedObj.GetComponent<OrbitWeapon>();
            if (orbitScript != null)
            {
                float currentStartAngle = angleStep * i;
                // [수정됨] 무기의 데미지를 OrbitWeapon으로 전달합니다.
                orbitScript.Setup(transform, weapon.damage, currentStartAngle);
            }

            // 2. 음파 무기 세팅 
            SonicWaveWeapon sonicScript = spawnedObj.GetComponent<SonicWaveWeapon>();
            if (sonicScript != null)
            {
                sonicScript.Setup(weapon.damage, weapon.slowRatio);
            }

            // 3. 투사체 무기 세팅
            ProjectileWeapon projScript = spawnedObj.GetComponent<ProjectileWeapon>();
            if (projScript != null)
            {
                projScript.Setup(weapon.damage, weapon.level);
            }
        }
    }

    public void LevelUpWeapon(string targetName)
    {
        foreach (WeaponData weapon in equippedWeapons)
        {
            if (weapon.weaponName == targetName)
            {
                if (weapon.level < weapon.maxLevel)
                {
                    weapon.level++;

                    // --- [1] 공전형 검 레벨업 로직 ---
                    if (weapon.weaponName == "공전형 검")
                    {
                        weapon.spawnCount = weapon.level;
                        
                        // [수정됨] 레벨에 따른 데미지 증가 로직 추가
                        switch (weapon.level)
                        {
                            case 1: weapon.damage = 10f; break;
                            case 2: weapon.damage = 15f; break;
                            case 3: weapon.damage = 20f; break;
                            case 4: weapon.damage = 25f; break;
                            case 5: weapon.damage = 30f; break;
                        }
                    }
                    // --- [2] 음파 무기 레벨업 로직 ---
                    else if (weapon.weaponName == "음파 무기")
                    {
                        switch (weapon.level)
                        {
                            case 1:
                                weapon.cooldown = 8f;      
                                weapon.slowRatio = 0.1f;   
                                weapon.damage = 10f;       
                                break;
                            case 2:
                                weapon.cooldown -= 2f;     
                                break;
                            case 3:
                                weapon.slowRatio += 0.1f;  
                                break;
                            case 4:
                                weapon.damage += 5f;       
                                break;
                            case 5:
                                weapon.slowRatio += 0.1f;  
                                break;
                        }
                    }
                    // --- [3] 투사체 무기 레벨업 로직 ---
                    else if (weapon.weaponName == "투사체 무기")
                    {
                        switch (weapon.level)
                        {
                            case 1:
                                weapon.cooldown = 1.0f; 
                                weapon.damage = 15f;    
                                break;
                            case 2:
                                weapon.cooldown = 0.9f;
                                break;
                            case 3:
                                weapon.cooldown = 0.8f;
                                weapon.damage += 2f;    
                                break;
                            case 4:
                                weapon.cooldown = 0.7f;
                                break;
                            case 5:
                                weapon.cooldown = 0.6f;
                                weapon.damage += 2f;    
                                break;
                        }
                    }

                    Debug.Log($"{weapon.weaponName} 레벨업! 현재 레벨: {weapon.level} | 쿨타임: {weapon.cooldown}초 | 데미지: {weapon.damage}");
                }
                else
                {
                    Debug.Log($"{weapon.weaponName}은(는) 이미 최고 레벨(5)입니다!");
                }
                break;
            }
        }
    }
}