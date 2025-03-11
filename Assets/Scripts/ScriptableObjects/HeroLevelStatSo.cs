using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Hero/LevelData", fileName = "New Hero Level Data")]
public class HeroLevelStatSo : ScriptableObject
{
    public Stats[] stats;
}

[Serializable]
public class Stats
{
    public float Attack;
    public float Health;
}
