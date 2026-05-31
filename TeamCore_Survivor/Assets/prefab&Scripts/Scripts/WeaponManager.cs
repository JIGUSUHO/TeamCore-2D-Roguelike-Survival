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

    // 레벨업을 통해 성장할 동적 스탯들을 추가합니다.
    [Header("동적 스탯")]
    public float damage;
    public float slowRatio;

    [HideInInspector] public float currentTimer;
}

public class WeaponManager : MonoBehaviour
{
    [Header("장착된 무기 목록")]
    public List<WeaponData> equippedWeapons = new List<WeaponData>();

    private void Update()
    {
        HandleWeaponCooldowns();

        // 테스트용 키보드 입력 (숫자 1번: 검 레벨업, 숫자 2번: 음파 레벨업)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) LevelUpWeapon("공전형 검");
            if (Keyboard.current.digit2Key.wasPressedThisFrame) LevelUpWeapon("음파 공격");
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
                orbitScript.Setup(transform, currentStartAngle);
            }

            // 2. 음파 무기 세팅 
            SonicWaveWeapon sonicScript = spawnedObj.GetComponent<SonicWaveWeapon>();
            if (sonicScript != null)
            {
                sonicScript.Setup(weapon.damage, weapon.slowRatio);
            }

            ProjectileWeapon projScript = spawnedObj.GetComponent<ProjectileWeapon>();
            if (projScript != null)
            {
                // 데미지와 레벨 정보를 넘겨주기

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

                    if (weapon.weaponName == "공전형 검")
                    {
                        weapon.spawnCount = weapon.level;
                    }
                    else if (weapon.weaponName == "음파 공격")
                    {
                        // 레벨업 로직
                        switch (weapon.level)
                        {
                            case 1:
                                weapon.cooldown = 8f;      // 1레벨: 쿨타임 8초
                                weapon.slowRatio = 0.1f;   // 1레벨: 둔화율 10%
                                weapon.damage = 10f;       // (기본 데미지 임의 설정)
                                break;
                            case 2:
                                weapon.cooldown -= 2f;     // 2레벨: 쿨타임 2초 감소
                                break;
                            case 3:
                                weapon.slowRatio += 0.1f;  // 3레벨: 둔화율 10% 증가
                                break;
                            case 4:
                                weapon.damage += 5f;       // 4레벨: 데미지 5 상승
                                break;
                            case 5:
                                weapon.slowRatio += 0.1f;  // 5레벨: 둔화율 10% 증가
                                break;
                        }
                    }

                    Debug.Log($"{weapon.weaponName} 레벨업! 현재 레벨: {weapon.level} | 쿨타임: {weapon.cooldown}초 | 둔화율: {weapon.slowRatio * 100}% | 데미지: {weapon.damage}");
                }
                else if (weapon.weaponName == "톱니바퀴 투척")
                {
                    switch (weapon.level)
                    {
                        case 1:
                            weapon.cooldown = 1.0f; // 1초에 한 발
                            weapon.damage = 15f;    // 1레벨 기본 데미지 설정
                            break;
                        case 2:
                            weapon.cooldown = 0.9f;
                            break;
                        case 3:
                            weapon.cooldown = 0.8f;
                            weapon.damage += 2f;    // 데미지 2 상승
                            break;
                        case 4:
                            weapon.cooldown = 0.7f;
                            break;
                        case 5:
                            weapon.cooldown = 0.6f;
                            weapon.damage += 2f;    // 데미지 2 상승
                            break;
                    }
                }
                else
                {
                    Debug.Log($"{weapon.weaponName}은(는) 이미 만렙(5)입니다!");
                }
                break;
            }
        }
    }
}