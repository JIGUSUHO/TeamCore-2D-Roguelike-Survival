using System.Collections.Generic;
using UnityEngine;

// 웨이브 안에 들어갈 개별 몬스터의 스폰 정보를 담는 구조체
[System.Serializable]
public class EnemySpawnInfo
{
    public string enemyName;      // 스포너에 등록해둔 몬스터 이름 (예: "Bat")
    public float spawnRate;       // 1초당 생성할 마리 수 (예: 2.0이면 1초에 2마리)
}

// 에디터에서 우클릭으로 쉽게 생성할 수 있도록 메뉴 추가
[CreateAssetMenu(fileName = "NewWave", menuName = "VampireSurvival/Wave Data")]
public class WaveData : ScriptableObject
{
    public float waveDuration; // 이 웨이브가 지속되는 시간 (초)
    public List<EnemySpawnInfo> spawnInfos; // 이 웨이브에 등장할 적 종류와 스폰율 리스트
}