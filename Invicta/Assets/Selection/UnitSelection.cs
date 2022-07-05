using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    LayerMask hitboxLayer;
    

    private List<GameObject> selectedObjects = new List<GameObject>();
    [HideInInspector] public List<GameObject> selectableObjects = new List<GameObject>();
    
    Vector3 mousePos1;
    Vector3 mousePos2;

    void Awake()
    {
        hitboxLayer = LayerMask.GetMask("Hitbox");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            RaycastHit selectionHit;  
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out selectionHit, Mathf.Infinity, hitboxLayer))
            {
                Unit unit = selectionHit.transform.parent.GetComponent<Subunit>().unit;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (unit.isSelected == false)
                    {
                        selectedObjects.Add(unit.gameObject);
                        unit.isSelected = true;
                    }
                    else
                    {
                        selectedObjects.Remove(unit.gameObject);
                        unit.isSelected = false;
                    }
                }
                else
                {
                   ClearSelection();
                }

                selectedObjects.Add(unit.gameObject);
                unit.isSelected = true;
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    ClearSelection();
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            mousePos2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            if (mousePos1 != mousePos2)
            {
                SelectObjects();
            }
        }
    }
    void SelectObjects()
    {
        List<GameObject> remObjects = new List<GameObject>();
        if (Input.GetKey(KeyCode.LeftShift) == false)
        {
            ClearSelection();
        }
        
        Rect selectRect = new Rect(mousePos1.x, mousePos1.y, mousePos2.x - mousePos1.x, mousePos2.y - mousePos1.y);
        foreach (GameObject hitbox in selectableObjects)
        {
            if (hitbox != null)
            {
                if (selectRect.Contains(Camera.main.WorldToViewportPoint(hitbox.transform.position), true))
                {
                    Unit unit = hitbox.transform.parent.GetComponent<Subunit>().unit;
                    selectedObjects.Add(unit.gameObject);
                    unit.GetComponent<Unit>().isSelected = true;
                }
            }
            else
            {
                remObjects.Add(hitbox);
            }
            
        }
        if (remObjects.Count > 0)
        {
            foreach (GameObject rem in remObjects)
            {
                selectableObjects.Remove(rem);
            }
            remObjects.Clear();
        }
    }
    void ClearSelection()
    {
       if (selectedObjects.Count > 0)
        {
            foreach (GameObject obj in selectedObjects)
            {
                obj.GetComponent<Unit>().isSelected = false;
            }
            selectedObjects.Clear();        
        }
    }
}
