using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public BuildableObjectDatabase database; //Database of all the objects you can build
    public KeyCode placeBtn = KeyCode.X; //How to start building TESTING PURPOSES will be replaced with ui buttons
    public float maxDis = 10f; //Max distance you can build
    public LayerMask layerMask;
    public Material greenMat; //Will show if you can build the object
    public Material redMat; //Will show if you can't build the object and there is something in the way

    //Raycasting Varibles
    private Camera cam;
    private Ray ray;
    private Vector3 camRay;
    private RaycastHit hit;
    RaycastHit hit2;

    //Building Variables
    private BuildableObject currentBuilding; //The prefab that will be built UI will change/control this variable
    private bool holoBuild = false;
    private GameObject placedObj = null; //The object you are going to place
    private Material placedObjMat = null; //PlacedObj default material
    private LayerMask placedObjLayer; //PlacedObj default Layer
    private PreviewBuild previewBuild; //This script will tell if the placedObj is colliding with something and it can be placed
    private bool isBuildable;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        currentBuilding = database.buildableObjects[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(placeBtn) && !holoBuild)
        {
            holoBuild = true;
            placedObj = Instantiate(currentBuilding.prefab);
            placedObjMat = placedObj.GetComponent<MeshRenderer>().material;
            previewBuild = placedObj.AddComponent<PreviewBuild>();
            placedObjLayer = placedObj.layer;
            placedObj.layer = LayerMask.NameToLayer("Preview");
        }

        if (holoBuild)
        {
            GoingToBuild(placedObj, currentBuilding);

            if (previewBuild.colliders.Count > 0)
            {
                isBuildable = false;
            }
            else
            {
                isBuildable = true;
            }

            if (isBuildable)
            {
                placedObj.GetComponent<MeshRenderer>().material = greenMat;
                if (Input.GetMouseButtonDown(0))
                {
                    DeleteHoloBuild(placedObj);
                }
            }
            else
            {
                placedObj.GetComponent<MeshRenderer>().material = redMat;
            }
        }
    }

    void GoingToBuild(GameObject placedObj, BuildableObject buildableObj)
    {
        Collider col = placedObj.GetComponent<Collider>();
        col.isTrigger = true;

        camRay = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2)) + cam.transform.forward * maxDis;
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, maxDis, layerMask))
        {
            MoveObjectToMousePos(placedObj, buildableObj, col);
        }
        else
        {
            if (Physics.Raycast(camRay, Vector3.down, out hit, maxDis, layerMask))
            {
                MoveObjectToMousePos(placedObj, buildableObj, col);
            }
        }
    }

    void DeleteHoloBuild(GameObject placedObj)
    {
        holoBuild = false;
        placedObj.GetComponent<Collider>().isTrigger = false;
        placedObj.layer = placedObjLayer;
        placedObj.GetComponent<MeshRenderer>().material = placedObjMat;
        Destroy(previewBuild);
        previewBuild = null;
        placedObj = null;
        isBuildable = false;
    }

    void MoveObjectToMousePos(GameObject placedObj, BuildableObject buildableObj, Collider col)
    {
        if (buildableObj.type == BuildingType.Wall)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                placedObj.transform.position = hit.point;
                placedObj.transform.Translate(new Vector3(-col.bounds.extents.x, 0, 0), Space.Self);
            }
        }
        else if (buildableObj.type == BuildingType.Ground)
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                if (Physics.Raycast(cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2)) + cam.transform.forward * hit.distance, Vector2.down, out hit2, maxDis, layerMask))
                {
                    placedObj.transform.position = hit2.point;
                    placedObj.transform.Translate(new Vector3(0, col.bounds.extents.y, 0), Space.Self);
                    placedObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit2.normal);
                }
            }
            else
            {
                placedObj.transform.position = hit.point;
                placedObj.transform.Translate(new Vector3(0, col.bounds.extents.y, 0), Space.Self);
                placedObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }
        }
    }

    public void ChangeCurrentBuildingObject(BuildableObject obj)
    {
        currentBuilding = obj;
        //layerMask = LayerMask.NameToLayer(currentBuilding.type.ToString());
    }
}
