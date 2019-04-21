using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterCustomGrid : MonoBehaviour
{
    public bool displayGizmos = false;
    public int checkHeight = 80;
    public GameObject MainCamera;
    

    [Header("mineralPlacement")]
    public bool randomOn = false;
    [Range(2,100)]
    public int spawnMineralsOneIn = 2;
    public GameObject mineralsParent;

    [Header("Rest")]
    public GameObject previeuwsParent;
    public GameObject[] mineralPrefabs;
    public LayerMask mouseMask;
    public LayerMask mineralMask;
    public LayerMask groundMask;
    public LayerMask waterMask;
    public LayerMask closeToWaterMask;
    public LayerMask buildingMask;
    public LayerMask ghostBuildingMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public GameObject previeuwObject;
    private List<GameObject> previeuwObjects = new List<GameObject>();
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start(){
        nodeDiameter = nodeRadius*2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
        CreateGrid();
    }

    void CreateGrid(){
        grid = new Node[gridSizeX,gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

        for(int x = 0; x < gridSizeX; x++){
            for(int y = 0; y < gridSizeY; y++){
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                Vector3 checkBoxes = new Vector3(nodeRadius, checkHeight, nodeRadius);


                bool walkable = !(Physics.CheckBox(worldPoint, checkBoxes, Quaternion.identity,  waterMask));
                bool isWater = (Physics.CheckBox(worldPoint, checkBoxes, Quaternion.identity,  waterMask));
                bool isCloseToWater = (Physics.CheckBox(worldPoint, checkBoxes, Quaternion.identity, closeToWaterMask));
                bool theresBuilding = (Physics.CheckSphere(worldPoint, nodeRadius, buildingMask));

                grid[x,y] = new Node(walkable, worldPoint);
                grid[x,y].isWater = isWater;
                if(isWater){
                    grid[x,y].walkable = false;
                    grid[x,y].isCloseToWater = false;
                    grid[x,y].theresBuilding = false;
                } else {
                    grid[x,y].isCloseToWater = isCloseToWater;
                    grid[x,y].theresBuilding = theresBuilding;
                }
                

                if(randomOn){
                    if(Random.Range(0,spawnMineralsOneIn) == 1 && !grid[x,y].isWater){
                        grid[x,y].hasMineral = true;
                        RaycastHit hit;
                        if(Physics.Raycast(worldPoint, Vector3.down, out hit, 100, groundMask)){
                            GameObject mineral = Instantiate(mineralPrefabs[Random.Range(0,mineralPrefabs.Length)], hit.point, this.transform.rotation);
                            mineral.transform.parent = mineralsParent.transform;
                        }
                    }
                } else {
                    bool hasMineral = Physics.CheckBox(worldPoint, checkBoxes, Quaternion.identity,  mineralMask);
                    grid[x,y].hasMineral = hasMineral;
                    Collider[] minerals = Physics.OverlapBox(worldPoint, checkBoxes, Quaternion.identity, mineralMask);
                    if(minerals.Length >= 1){
                        minerals[0].transform.parent = mineralsParent.transform;
                    }
                    

                    // //Not Done yeat
                    // if(amountOfMineralsPlaced <= amountOfMinerals && Random.Range(x,gridSizeX) == amountOfMineralsPlaced && !grid[x,y].isWater){
                    //     grid[x,y].hasMineral = true;
                    //     RaycastHit hit;
                    //     if(Physics.Raycast(worldPoint, Vector3.down, out hit, 100, groundMask)){
                    //         GameObject mineral = Instantiate(mineralPrefabs[Random.Range(0,mineralPrefabs.Length)], hit.point, this.transform.rotation);
                    //         mineral.transform.parent = mineralsParent.transform;
                    //         amountOfMineralsPlaced++;
                    //     }
                    // }
                    // //Goota fix boi,, yeet
                }
                

            }
        }
    }

    void Update(){
        if(grid != null){
            foreach(Node n in grid){
                Vector3 checkBoxes = new Vector3(nodeRadius, checkHeight, nodeRadius);
                bool theresGhost = (Physics.CheckBox(n.worldPosition, checkBoxes, Quaternion.identity,  ghostBuildingMask));
                n.theresGhost = theresGhost;

                bool theresBuilding = (Physics.CheckBox(n.worldPosition, checkBoxes, Quaternion.identity,  buildingMask));
                n.theresBuilding = theresBuilding;

                bool hasMineral = Physics.CheckBox(n.worldPosition, checkBoxes, Quaternion.identity,  mineralMask);
                n.hasMineral = hasMineral;

                if((Physics.CheckBox(n.worldPosition, checkBoxes, Quaternion.identity,  mouseMask))){
                    n.walkable = false;
                    if(Input.GetMouseButtonDown(0) && MainCamera.GetComponent<MouseOnGrid>().CanPlace(n.worldPosition, nodeRadius) && !n.isWater && !n.theresBuilding){
                        if(previeuwObjects.Count >= 1){
                            foreach(GameObject g in previeuwObjects){
                                Destroy(g);
                            }
                            previeuwObjects.Clear();
                        }
                        previeuwObjects.Clear();
                        MainCamera.GetComponent<MouseOnGrid>().PlaceBuilding(n.worldPosition.x, n.worldPosition.z);
                        n.clickedOn = true;
                    }
                } else {
                    n.walkable = true;
                }
            }
        }
    }

    void OnDrawGizmos(){
        if(displayGizmos){
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            if(grid != null){
                foreach(Node n in grid){
                    Gizmos.color = Color.white;

                    if(n.hasMineral){
                        Gizmos.color = Color.blue;
                    }

                    if(n.isWater){
                        Gizmos.color = Color.cyan;
                    }

                    if(n.isCloseToWater){
                        Gizmos.color = Color.yellow;
                    }

                    if(n.clickedOn){
                        Gizmos.color = Color.green;
                    }

                    if(n.theresBuilding){
                        Gizmos.color = Color.black;
                    }

                    if(n.hasMineral){
                        Gizmos.color = Color.blue;
                    }

                    if(n.theresGhost){
                        Gizmos.color = Color.magenta;
                    }

                    if(!n.walkable){
                        Gizmos.color = Color.red;
                    }



                    Gizmos.DrawCube(n.worldPosition, new Vector3(nodeRadius, checkHeight, nodeRadius));
                }
            }
        }
    }

    public void WhereToPlace(int bType){
        switch (bType){
            //Energy
            case 1:
            if(previeuwObjects.Count >= 1){
                foreach(GameObject g in previeuwObjects){
                    Destroy(g);
                }
                previeuwObjects.Clear();
            }


            foreach(Node n in grid){
                if(n.isCloseToWater && !n.theresBuilding && !n.hasMineral){
                    RaycastHit hit;
                    if(Physics.Raycast(n.worldPosition, Vector3.down, out hit, 100, groundMask)){
                        GameObject newObj = Instantiate(previeuwObject, hit.point, transform.rotation);
                        newObj.transform.LookAt(hit.normal + newObj.transform.position);
                        newObj.transform.parent = previeuwsParent.transform;
                        previeuwObjects.Add(newObj);
                    }
                }
            }
            break;

            //Farm
            case 2:
            if(previeuwObjects.Count >= 1){
                foreach(GameObject g in previeuwObjects){
                    Destroy(g);
                }
                previeuwObjects.Clear();
            }


            foreach(Node n in grid){
                if(!n.isCloseToWater && !n.theresBuilding && !n.hasMineral && !n.isWater){
                    RaycastHit hit;
                    if(Physics.Raycast(n.worldPosition, Vector3.down, out hit, 100, groundMask)){
                        GameObject newObj = Instantiate(previeuwObject, hit.point, transform.rotation);
                        newObj.transform.LookAt(hit.normal + newObj.transform.position);
                        newObj.transform.parent = previeuwsParent.transform;
                        previeuwObjects.Add(newObj);
                    }
                }
            }
            break;

            //Pod
            case 3:
            if(previeuwObjects.Count >= 1){
                foreach(GameObject g in previeuwObjects){
                    Destroy(g);
                }
                previeuwObjects.Clear();
            }


            foreach(Node n in grid){
                if(!n.isCloseToWater && !n.theresBuilding && !n.hasMineral && !n.isWater){
                    RaycastHit hit;
                    if(Physics.Raycast(n.worldPosition, Vector3.down, out hit, 100, groundMask)){
                        GameObject newObj = Instantiate(previeuwObject, hit.point, transform.rotation);
                        newObj.transform.LookAt(hit.normal + newObj.transform.position);
                        newObj.transform.parent = previeuwsParent.transform;
                        previeuwObjects.Add(newObj);
                    }
                }
            }
            break;

            //Mine
            case 4:
            if(previeuwObjects.Count >= 1){
                foreach(GameObject g in previeuwObjects){
                    Destroy(g);
                }
                previeuwObjects.Clear();
            }


            foreach(Node n in grid){
                if(!n.theresBuilding && n.hasMineral){
                    RaycastHit hit;
                    if(Physics.Raycast(n.worldPosition, Vector3.down, out hit, 100, groundMask)){
                        GameObject newObj = Instantiate(previeuwObject, hit.point, transform.rotation);
                        newObj.transform.LookAt(hit.normal + newObj.transform.position);
                        newObj.transform.parent = previeuwsParent.transform;
                        previeuwObjects.Add(newObj);
                    }
                }
            }
            break;
        }
    }
}

//Shitty extra Code------------------------------------------------------------
//for(int x = 0; x < gridSizeX; x++){
//     for(int y = 0; y < gridSizeY; y++){
//         if((Physics.CheckSphere(grid[x,y].worldPosition, nodeRadius, mouseMask))){
//             grid[x,y].walkable = false;
//             if(Input.GetMouseButtonDown(0) && !grid[x,y].isWater){
//                 grid[x,y].clickedOn = true;
//             }
//         } else {
//             grid[x,y].walkable = true;
//         }
//     }
// }
