using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    public bool isSelected = false;
    [HideInInspector] public List<Subunit> subunits = new List<Subunit>();
    public int totalUnitCount;
    public UnitData unitData;

    int _unitWidth;
    int unitWidth
    {
        get {return _unitWidth;}
        set
        {
            _unitWidth = Mathf.Clamp(value, 
            Mathf.RoundToInt(Mathf.Sqrt(subunits.Count / 5)),
            Mathf.RoundToInt(Mathf.Sqrt(5 * subunits.Count)));
            unitDepth = subunits.Count / unitWidth;
        }
    }
    
    int unitDepth;
    float direction;
    Vector3 startVector;
    [SerializeField] float spacing;

    [SerializeField] GameObject previewPrefab;
    List<GameObject> previews = new List<GameObject>();
    Camera cam;
    LayerMask layerMask;

    void Awake()
    {
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Terrain");

        for(int i = 0; i < totalUnitCount; i++)
        {
            GameObject subunitObject = unitData.Instantiate(this);
            Subunit subunit = subunitObject.GetComponent<Subunit>();
            subunits.Add(subunit);
            subunit.transform.SetParent(this.transform);
            subunit.name = $"Subunit({i})";
        }

        unitWidth = 5;
        int j  = 0;
        List<Vector3> initialPos = GetDestinations(this.transform.position);
        foreach(Subunit subunit in subunits)
        {
            subunit.transform.position = initialPos[j];
            j++;
        }
    }

    void Update()
    {
        if(isSelected)
        {
            HandleMovement();
        }
    }

    void HandleMovement()
    {
        foreach(GameObject preview in previews)
        {
            Destroy(preview);
        }

        if(Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if(hit.collider.gameObject.layer == 9)
                {
                    startVector = hit.point;
                }
                else if(hit.collider.gameObject.layer == 11 && hit.collider.gameObject != this.gameObject)
                {
                    // Attack_Unit(hit.transform.parent.GetComponent<_Subunit>().unit);
                }
            }
        }

        if(Input.GetMouseButton(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if(hit.collider.gameObject.layer == 9)
                {    
                    Vector3 start2D = new Vector3(startVector.x, 0, startVector.z);
                    Vector3 hit2D = new Vector3(hit.point.x, 0, hit.point.z);

                    float spread = Vector3.Distance(hit2D, start2D);
                    float angle = hit2D.x > start2D.x ? -Vector3.Angle(hit2D - start2D, transform.forward) + 90 :
                    Vector3.Angle(hit2D - start2D, transform.forward) + 90;
                    angle = angle < 0 ? angle + 360 : angle; // I FUCKING LOVE TERNARY OPERATORS

                    direction = -angle;
                    unitWidth = Mathf.RoundToInt(spread / spacing);
                    
                    //previews.Add(Instantiate(arrowPrefab, GetOffset(startVector, unitWidth / 2, unitDepth + 2), Quaternion.Euler(0, 90 + direction, 0)));
                    List<Vector3> offsets = GetDestinations(startVector);
                    foreach(Vector3 offset in offsets)
                    {
                        previews.Add(Instantiate(previewPrefab, offset, Quaternion.Euler(0, 0, 0)));
                    }
                }
            }
        }

        if(Input.GetMouseButtonUp(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {  
                if(hit.collider.gameObject.layer == 9)
                {
                    MoveUnit(startVector);
                }
            }
        }
    }

    void MoveUnit(Vector3 pos)
    {
        List<Vector3> destinations = GetDestinations(pos);
        float noise = spacing / 7.5f;
        int i = 0;

        int[,] costs = new int[subunits.Count, destinations.Count];
        for(int x = 0; x < subunits.Count; x++)
        {
            for(int y = 0; y < subunits.Count; y++)
            {
                costs[x, y] = Mathf.RoundToInt(10 * Mathf.Abs(Vector3.Distance(
                destinations[y], subunits[x].transform.position)));
            }
        }

        HungarianAlgorithm hungAlgorithm = new HungarianAlgorithm(costs);
        int[] sorted = hungAlgorithm.Run();

        // int[] sorted = HungarianAlgorithm.FindAssignments(costs);

        foreach(Subunit subunit in subunits)
        {
            Vector3 noiseVector = new Vector3(Random.Range(-noise, noise), 0 ,Random.Range(-noise, noise));
            subunit.agent.SetDestination(destinations[sorted[i]] + noiseVector);
            i++;
        }
    }

    List<Vector3> GetDestinations(Vector3 pos)
    {
        return GetDestinations(pos, unitWidth);
    }

    List<Vector3> GetDestinations(Vector3 pos, int width)
    {
        int depth = subunits.Count / width;
        int x = 0;
        int z = depth;
        List<Vector3> destinations = new List<Vector3>();

        foreach(Subunit subunit in subunits)
        {
            Vector3 offset = new Vector3(x, 0, z - depth / 2); // Basic offset
            offset = offset * spacing; // Spaces them out
            offset = offset + pos; // Adds position
            offset = Quaternion.Euler(0, direction, 0) * (offset - pos) + pos; // Rotates them
            destinations.Add(offset);
            
            x++;
            if(x >= width)
            {
                x = 0; z--;
            }
        }
        return destinations;
    }
}
