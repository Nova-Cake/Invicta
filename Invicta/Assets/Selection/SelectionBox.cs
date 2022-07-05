using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBox : MonoBehaviour
{
    [SerializeField] private RectTransform selectSquareImage;
    [SerializeField] private Canvas canvas;
    Vector3 startPos;
    Vector3 endPos;
    Vector2 scaleFactor = new Vector2();
        
    void Start()
    {
        selectSquareImage.gameObject.SetActive(false); // Box is invisible by default
        scaleFactor = Vector2.one / canvas.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // When player left clicks down
        {
            startPos = Input.mousePosition; // Set the startpoint of the rectangle where the mouse is
        }
        if (Input.GetMouseButtonUp(0)) // When the player stops holding left click
        {
            selectSquareImage.gameObject.SetActive(false); // Make the box invisible
        }
        if (Input.GetMouseButton(0)) // While left click is being held down
        {
            if (!selectSquareImage.gameObject.activeInHierarchy) // If the gameObject currently isn't visible
            {
                selectSquareImage.gameObject.SetActive(true); //Make it
            }
            endPos = Input.mousePosition; // Set the end corner of the rectangle to wherever the cursor is
            Vector3 center = (startPos + endPos) / 2f; // Set the center of the rectangle to the average between the start and end point

            float sizeX = Mathf.Abs((startPos.x - endPos.x) * scaleFactor.x); // Set the width to be the distance between the start and the end point
            float sizeY = Mathf.Abs((startPos.y - endPos.y) * scaleFactor.y); // Set the hieght to be the distance between the start and end point

            selectSquareImage.sizeDelta = new Vector2(sizeX, sizeY);
            selectSquareImage.position = center;

        }
    }
}