using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class _Unit : MonoBehaviour
{
    [HideInInspector] public List<_Subunit> subunits;
    [HideInInspector] public List<_Subunit> alive_Subunits;
    [SerializeField] int subunitCount;
    [SerializeField] UnitData unitData;

    public List<_Unit> targeters = new List<_Unit>();
    public _Unit target;

    int unitWidth = 50;
    int unitDepth;
    int unitSpacing = 2;
    [SerializeField] float noise;
    float direction = 0;

    [SerializeField] GameObject previewPrefab;
    [SerializeField] GameObject arrowPrefab;
    List<GameObject> previews = new List<GameObject>();

    int minWidth = 8;
    int maxWidth;
    LayerMask layerMask;
    Vector3 startVector;
    Camera cam;
    public bool isSelected;

    public int morale;

    public GameObject healthTemplate;

    void Awake()
    {
        isSelected = false;
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Terrain", "Hitbox");
         
        for(int i = 0; i < subunitCount; i++)
        {
            // GameObject sub_Unit = unitData.Instantiate(this);
            //subunits.Add(sub_Unit.GetComponent<_Subunit>());
            //sub_Unit.transform.SetParent(this.transform);
            //sub_Unit.GetComponent<_Subunit>().unit = this;
        }

        alive_Subunits = new List<_Subunit>(subunits);
        UpdateDimensions();
        List<Vector3> startingOffsets = GetOffsets(this.transform.position);

        for(int i = 0; i < alive_Subunits.Count; i++)
        {
            alive_Subunits[i].transform.position = startingOffsets[i];
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
                    Attack_Unit(hit.transform.parent.GetComponent<_Subunit>().unit);
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
        }

        if(Input.GetMouseButtonUp(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {  
                if(hit.collider.gameObject.layer == 9)
                {
                    Move_Unit(startVector);
                }
            }
        }
    }

    void Attack_Unit(_Unit unit)
    {
        target = unit;
        unit.targeters.Add(this);
    }

    void UpdateDimensions()
    {
        unitDepth = alive_Subunits.Count / unitWidth;
        maxWidth = alive_Subunits.Count / 6;
    }

    public void Move_Unit(Vector3 position)
    {
        List<Vector3> offsets = GetOffsets(position);
        Move_Unit(offsets);
    }

    void Move_Unit(List<Vector3> offsets)
    {
        for(int i = 0; i < offsets.Count; i++)
        {
            _Subunit subunit = subunits[i];
            Vector3 noiseVector = new Vector3(Random.Range(-noise, noise), 0 ,Random.Range(-noise, noise));
            subunit.gameObject.GetComponent<NavMeshAgent>().SetDestination(offsets[i] + noiseVector);
        }
    }

    List<Vector3> GetOffsets(Vector3 position)
    {
        List<Vector3> offsets = new List<Vector3>();
        int x = 0;
        int z = unitDepth;

        for(int i = 0; i < alive_Subunits.Count; i++)
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
