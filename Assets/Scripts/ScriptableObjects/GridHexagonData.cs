using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Grid/Hexagon", fileName = "New Hero Hexagon")]
public class GridHexagonData : ScriptableObject
{
    public Sprite HeroSprite;
    public Color HexagonColor;
    public Material HexagonMaterial;
    public HeroTypes HeroType;
}
