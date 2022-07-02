using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    
    [HideInInspector] public List<GameObject> subunits;
    [HideInInspector] public List<GameObject> aliveSubunits;
    [SerializeField] int subunitCount;
    [SerializeField] SubunitObject subunitObject;
    
    

    int unitWidth = 16;
    int unitDepth;
    int unitSpacing = 2;
    [SerializeField] float noise;
    float direction = 0;

    [SerializeField] GameObject testPrefab;
    [SerializeField] GameObject previewPrefab;
    [SerializeField] GameObject arrowPrefab;
    List<GameObject> previews = new List<GameObject>();

    int minWidth = 8;
    int maxWidth;
    LayerMask terrain;
    Vector3 startVector;

    



    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        terrain = LayerMask.GetMask("Terrain");
         
        for(int i = 0; i < subunitCount; i++)
        {
            GameObject subUnit = Instantiate<GameObject>(testPrefab);
            subunits.Add(subUnit);
            subUnit.transform.SetParent(this.transform);
        }

        aliveSubunits = new List<GameObject>(subunits);
        UpdateDimensions();
        List<Vector3> startingOffsets = GetOffsets(Vector3.zero);

        for(int i = 0; i < aliveSubunits.Count; i++)
        {
            aliveSubunits[i].transform.position = startingOffsets[i];
        }
    }

    void Update()
    {
        foreach(GameObject preview in previews)
        {
            Destroy(preview);
        }

        if(Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, terrain))
            {
                startVector = hit.point;
            }
        }

        if(Input.GetMouseButton(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, terrain))
            {
                Vector3 start2D = new Vector3(startVector.x, 0, startVector.z);
                Vector3 hit2D = new Vector3(hit.point.x, 0, hit.point.z);

                float spread =Vector3.Distance(hit2D, start2D);
                float angle = hit2D.x > start2D.x ? -Vector3.Angle(hit2D - start2D, transform.forward) + 90 :
                Vector3.Angle(hit2D - start2D, transform.forward) + 90;
                angle = angle < 0 ? angle + 360 : angle; // I FUCKING LOVE TERNARY OPERATORS

                direction = -angle;
                unitWidth = Mathf.Clamp((int)spread, minWidth, maxWidth);
                UpdateDimensions();
                
                previews.Add(Instantiate(arrowPrefab, GetOffset(startVector, unitWidth / 2, unitDepth + 2), Quaternion.Euler(0, 90 + direction, 0)));
                List<Vector3> offsets = GetOffsets(startVector);
                foreach(Vector3 offset in offsets)
                {
                    previews.Add(Instantiate(previewPrefab, offset, Quaternion.Euler(0, 0, 0)));
                }
            }
        }

        if(Input.GetMouseButtonUp(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, terrain))
            {  
                MoveUnit(startVector);
            }
        }
        
    }

    void UpdateDimensions()
    {
        unitDepth = aliveSubunits.Count / unitWidth;
        maxWidth = aliveSubunits.Count / 6;
    }

    public void MoveUnit(Vector3 position)
    {
        List<Vector3> offsets = GetOffsets(position);
        MoveUnit(offsets);
    }

    void MoveUnit(List<Vector3> offsets)
    {
        for(int i = 0; i < offsets.Count; i++)
        {
            GameObject subunit = subunits[i];
            Vector3 noiseVector = new Vector3(Random.Range(-noise, noise), 0 ,Random.Range(-noise, noise));
            subunit.GetComponent<NavMeshAgent>().SetDestination(offsets[i] + noiseVector);
        }
    }

    List<Vector3> GetOffsets(Vector3 position)
    {
        List<Vector3> offsets = new List<Vector3>();
        int x = 0;
        int z = unitDepth;

        for(int i = 0; i < aliveSubunits.Count; i++)
        {
            Vector3 offset = GetOffset(position, x, z);
            offsets.Add(offset);
            x++;
            if(x >= unitWidth) {z--; x = 0;}
        }

        return offsets;
    }

    public Vector3 GetOffset(Vector3 position, int x, int z)
    {
        Vector3 middle = new Vector3(Mathf.RoundToInt(unitWidth / 2), 0, Mathf.RoundToInt(unitDepth / 2));
        Vector3 offset = new Vector3(x, 0, z) - middle;
        offset *= unitSpacing;
        offset += position;
        offset = Quaternion.Euler(0, direction, 0) * (offset - position) + position;
        return offset;
    }
}
