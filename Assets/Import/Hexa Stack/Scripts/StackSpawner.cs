using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class StackSpawner : MonoBehaviour
{
    [SerializeField] private Transform stackPositionsParent;
    [SerializeField] private Hexagon hexagonPrefab;
    [SerializeField] private HexStack hexagonStackPrefab;

    [SerializeField] private Color[] colors;
    [SerializeField] private GridHexagonData[] gridHexagons;

    [NaughtyAttributes.MinMaxSlider(2,8)]
    [SerializeField] private Vector2Int minMaxHexCount;

    private int _stackCounter;
    private void Awake() => Application.targetFrameRate = 60;
    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        StackController.onStackPlaced += StackPlaced;
    }

    private void StackPlaced(GridCell cell)
    {
        _stackCounter++;

        if(_stackCounter >= 3)
        {
            _stackCounter = 0;
            GenerateStacks();
        }
    }
    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void UnsubscribeEvents()
    {
        StackController.onStackPlaced -= StackPlaced;
    }
    void Start()
    {
        GenerateStacks();
    }
    private void GenerateStacks()
    {
        for (int i = 0; i < stackPositionsParent.childCount; i++)
        {
            GenerateStacks(stackPositionsParent.GetChild(i));
        }
    }

    private void GenerateStacks(Transform parent)
    {
        HexStack hexStack = Instantiate(hexagonStackPrefab, parent.position, Quaternion.identity, parent);
        hexStack.name = $"Stack {parent.GetSiblingIndex() }";

        int amount = Random.Range(minMaxHexCount.x, minMaxHexCount.y);

        //Color[] colorArray = GetRandomColors();
        GridHexagonData[] heroHexagonArray = GetRandomHexagon();

        int firstColorHexagonCount = Random.Range(0, amount);

        for (int i = 0; i < amount;i++)
        {
            Vector3 hexagonLocalPos = Vector3.up * i * .2f;
            Vector3 spawnPosition = hexStack.transform.TransformPoint(hexagonLocalPos);
            Hexagon hexagonInstance = Instantiate(hexagonPrefab, spawnPosition, Quaternion.identity, hexStack.transform);

            if (heroHexagonArray.Length < 2)
            {
                hexagonInstance.HexagonData = heroHexagonArray[0];
            }
            else
                hexagonInstance.HexagonData = i < firstColorHexagonCount ? heroHexagonArray[0] : heroHexagonArray[1];
            hexagonInstance.Configure(hexStack);
            hexStack.Add(hexagonInstance);
        }
    }
    //private Color[] GetRandomColors()
    //{
    //    List<Color> colorList = new List<Color>();
    //    colorList.AddRange(colors);

    //    if(colorList.Count <= 0) 
    //    {
    //        Debug.LogError("No color found");
    //        return null;
    //    }

    //    Color firstColor = colorList.OrderBy(x => Random.value).First();
    //    colorList.Remove(firstColor);

    //    if (colorList.Count <= 0)
    //    {
    //        Debug.LogError("No color found");
    //        return null;
    //    }
    //    Color secondColor = colorList.OrderBy(x => Random.value).First();

    //    return new Color[] { firstColor, secondColor };
    //}

    private GridHexagonData[] GetRandomHexagon()
    {
        List<GridHexagonData> heroHexagonList = new List<GridHexagonData>();
        heroHexagonList.AddRange(gridHexagons);

        if (heroHexagonList.Count <= 0)
        {
            Debug.LogError("No hero found");
            return null;
        }

        int spawnChance = Random.Range(0, 4);

        if (spawnChance < 3)
        {
            GridHexagonData firstHero = heroHexagonList.OrderBy(x => Random.value).First();
            heroHexagonList.Remove(firstHero);

            if (heroHexagonList.Count <= 0)
            {
                Debug.LogError("No hero found");
                return null;
            }

            return new GridHexagonData[] { firstHero};
        }
        else
        {
            GridHexagonData firstHero = heroHexagonList.OrderBy(x => Random.value).First();
            heroHexagonList.Remove(firstHero);

            if (heroHexagonList.Count <= 0)
            {
                Debug.LogError("No hero found");
                return null;
            }
            GridHexagonData secondHero = heroHexagonList.OrderBy(x => Random.value).First();

            return new GridHexagonData[] { firstHero, secondHero };
        }
        
    }
}

