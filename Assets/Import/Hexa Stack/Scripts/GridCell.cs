using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class GridCell : MonoBehaviour
{

    [OnValueChanged("GenerateInitialHexagons")]
    [SerializeField] private Material[] gridMaterials;
    [SerializeField] private MeshRenderer meshRenderer;

    public HexStack Stack { get; private set; }
    public bool IsOccupied {  
        get => Stack != null;
        private set { } }

    public void AssignStack(HexStack stack)
    {
        Stack = stack;
    }
    private void Start()
    {
        if(transform.childCount > 1)
        {
            Stack = transform.GetChild(1).GetComponent<HexStack>();
            Stack.Initiliaze();
        }
    }
    public void ChangeColor(bool selected)
    {
        if (selected)
        {
            meshRenderer.material = gridMaterials[1];
        }
        else
            meshRenderer.material = gridMaterials[0];
    }
}
