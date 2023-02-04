using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveSettings", menuName = "ScriptableObjects/WaveSettings", order = 1)]
public class WaveSettings : ScriptableObject {
    public List<Wave> waves;
}

[Serializable]
public class Wave {
    public List<SpawnGroup> groups;
}


[Serializable]
public class SpawnGroup {
    public EnemyVariantID enemyVariant;
    public int count;
}
