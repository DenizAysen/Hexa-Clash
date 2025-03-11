using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class StackController : MonoBehaviour
{
    #region Variables
    [SerializeField] private LayerMask hexagonLayerMask;
    [SerializeField] private LayerMask gridHexagonLayerMask;
    [SerializeField] private LayerMask groundLayerMask;

    private HexStack currentStack;
    private GridCell previousSelectedCell,targetCell;

    private Vector3 currentStackInitialPos;

    private Camera mainCam;
    #endregion

    public static Action<GridCell> onStackPlaced;
    [SerializeField] private Transform gridParent;
    bool _canRotate;
    float xRotation, _gridYRotation;
    #region Unity Methods
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        ManageControl();
    } 
    #endregion

    private void ManageControl()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ManageMouseDown();
        }
        if(currentStack == null)
        {
            if (_canRotate)
            {
                if (Input.GetMouseButton(0))
                {
                    xRotation = Input.GetAxis("Mouse X") * 30f * Time.deltaTime;

                    gridParent.Rotate(Vector3.up, -xRotation);
                    //Debug.Log("Grid rotation : " + gridParent.rotation.y);
                }

                else if (Input.GetMouseButtonUp(0))
                {

                    _gridYRotation = MathF.Ceiling(gridParent.eulerAngles.y);
                    Debug.Log("Grid rotation : " + _gridYRotation);
                    if (_gridYRotation > 0 && _gridYRotation <= 45)
                    {
                        LeanTween.rotateY(gridParent.gameObject, GetClosestYAngle(_gridYRotation, 1f, 45f), .1f);
                    } 

                    else if (_gridYRotation > 45 && _gridYRotation <= 90)
                    {
                        LeanTween.rotateY(gridParent.gameObject, GetClosestYAngle(_gridYRotation, 46f, 90f), .1f);
                    }

                    else if (_gridYRotation > 90 && _gridYRotation <= 135)
                    {
                        LeanTween.rotateY(gridParent.gameObject, GetClosestYAngle(_gridYRotation, 91f, 135f), .1f);
                    }

                    else if (_gridYRotation > 135 && _gridYRotation <= 180)
                    {
                        LeanTween.rotateY(gridParent.gameObject, GetClosestYAngle(_gridYRotation, 136f, 180f), .1f);
                    }

                    else if (_gridYRotation > 180 && _gridYRotation <= 225)
                    {
                        LeanTween.rotateY(gridParent.gameObject, GetClosestYAngle(_gridYRotation , 181f, 225f), .1f);
                    }

                    else if (_gridYRotation > 225 && _gridYRotation <= 270)
                    {
                        LeanTween.rotateY(gridParent.gameObject, GetClosestYAngle(_gridYRotation, 226f, 270f), .1f);
                    }

                    else if (_gridYRotation > 270 && _gridYRotation <= 315)
                    {
                        LeanTween.rotateY(gridParent.gameObject, GetClosestYAngle(_gridYRotation, 271f, 315f), .1f);
                    }
                    else if (_gridYRotation > 315 && _gridYRotation <= 360)
                    {
                        LeanTween.rotateY(gridParent.gameObject, GetClosestYAngle(_gridYRotation, 315f, 0f), .1f);
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                ManageMouseDrag();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                ManageMouseUp();
            }
        }
        //else if (Input.GetMouseButton(0) && currentStack != null) 
        //{
        //    ManageMouseDrag();
        //}
        //else if (Input.GetMouseButtonUp(0) && currentStack != null) 
        //{
        //    ManageMouseUp();
        //}
    }
    private void ManageMouseDown()
    {
        RaycastHit hit;
        //Physics.Raycast(GetClickedRay(), out hit, 500, hexagonLayerMask);
        Physics.Raycast(GetClickedRay(), out hit, 500);

        if (hit.collider == null)
            return;

        Debug.Log(hit.collider.gameObject.name);

        if (hit.collider.gameObject.name == "Ground")
        {
            Debug.Log("Can rotate object");
            _canRotate = true;
        }

        else if (hit.collider.gameObject.layer == 7)
        {
            currentStack = hit.collider.GetComponent<Hexagon>().HexStack;
            currentStackInitialPos = currentStack.transform.position;
            _canRotate = false;
        }

        else
        {
            Debug.Log("Cannot rotate object");
            _canRotate = false;
        }

        //currentStack = hit.collider.GetComponent<Hexagon>().HexStack;
        //currentStackInitialPos = currentStack.transform.position;
    }
    private void ManageMouseUp()
    {
        if(targetCell == null)
        {
            currentStack.transform.position = currentStackInitialPos;
            currentStack = null;
            return;
        }

        currentStack.transform.position = targetCell.transform.position.With(y: .2f);
        currentStack.transform.SetParent(targetCell.transform);
        currentStack.Place();

        targetCell.AssignStack(currentStack);
        onStackPlaced?.Invoke(targetCell);

        targetCell.ChangeColor(false);
        targetCell = null;
        previousSelectedCell = null;
        currentStack = null;
    }

    private void ManageMouseDrag()
    {
        RaycastHit hit;
        Physics.Raycast(GetClickedRay(), out hit, 500, gridHexagonLayerMask);

        if (hit.collider == null)
        {
            DraggingAboveGround();
        }
        else
            DraggingAboveGridCell(hit);
    }

    private void DraggingAboveGridCell(RaycastHit hit)
    {
       GridCell gridCell = hit.collider.GetComponent<GridCell>();

        if (gridCell.IsOccupied)
            DraggingAboveGround();
        else
        {
            DraggingAboveNonOcupiedGridCell(gridCell);
        }
    }

    private void DraggingAboveNonOcupiedGridCell(GridCell gridCell)
    {
        //Vector3 currentStackTargetPos = gridCell.transform.position.With(y: 2);

        //currentStack.transform.position = Vector3.MoveTowards(currentStack.transform.position,
        //    currentStackTargetPos,
        //    Time.deltaTime * 30f);
        RaycastHit hit;
        Physics.Raycast(GetClickedRay(), out hit, 500, groundLayerMask);

        if (hit.collider == null)
        {
            Debug.LogError("no ground detected");
            return;
        }

        Vector3 currentStackTargetPos = hit.point.With(y: 2);
        currentStack.transform.position = Vector3.MoveTowards(currentStack.transform.position,
            currentStackTargetPos,
            Time.deltaTime * 30f);

        if (previousSelectedCell != null && previousSelectedCell != gridCell)
        {
            previousSelectedCell.ChangeColor(false);
            previousSelectedCell = targetCell;
        }
        else
            previousSelectedCell = gridCell;

        targetCell = gridCell;

        targetCell.ChangeColor(true);
    }

    private void DraggingAboveGround()
    {
        RaycastHit hit;
        Physics.Raycast(GetClickedRay(), out hit, 500, groundLayerMask);

        if(hit.collider == null)
        {
            Debug.LogError("no ground detected");
            return;
        }

        Vector3 currentStackTargetPos = hit.point.With(y: 2);
        currentStack.transform.position = Vector3.MoveTowards(currentStack.transform.position,
            currentStackTargetPos, 
            Time.deltaTime * 30f);

        if(targetCell != null)
            targetCell.ChangeColor(false);

        targetCell = null;
        previousSelectedCell = null;
    }
    
    private float GetClosestYAngle(float currentAngle, float minAngle, float maxAngle)
    {
        if(maxAngle == 0f)
        {
            if (currentAngle - minAngle > 23f)
                return maxAngle;

            else
                return minAngle;
        }
        else
        {
            float firstDistance = currentAngle - minAngle;
            float secondDistance = maxAngle - currentAngle;

            Debug.Log("Min angle distance : " + firstDistance);
            Debug.Log("Max angle distance : " + secondDistance);

            float finalAngle = firstDistance < secondDistance ? minAngle : maxAngle;

            return finalAngle;
        }      
    }
    private Ray GetClickedRay() => mainCam.ScreenPointToRay(Input.mousePosition);
}
