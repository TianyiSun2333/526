using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class WorldController : MonoBehaviour
{
    public int worldSize = 10;

    public float tileSize = 2.5f;

    TileController[,] tileArray = null;

    GameObject tilePrefab = null;

    Material[] tileMaterial = null;

    Vector3 initialTilePosition = new Vector3();

    public Material DebugMaterial = null;



    public Vector3 GetTilePosition(Vector2Int paraCoordinate)
    {
        return initialTilePosition + new Vector3(paraCoordinate.x * tileSize, 0.0f, paraCoordinate.y * tileSize);
    }

    // 检查目标地砖是否为合法操作目标
    public bool CheckTileOperatable(Vector2Int paraCoordinate)
    {
        bool res = true;

        if (paraCoordinate.x < 0 || paraCoordinate.y < 0 || paraCoordinate.x >= worldSize || paraCoordinate.y >= worldSize)
        {
            res = false;
        }

        return res;
    }

    // 检查目标地点是否为可通行目标
    public bool CheckTilePassable(Vector2Int paraCoordinate)
    {
        bool res = true;

        if(paraCoordinate.x < 0 || paraCoordinate.y <0 || paraCoordinate.x >= worldSize || paraCoordinate.y >= worldSize)
        {
            res = false;
        }

        else if (tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile()!=null)
        {
            res = false;
        }

        return res;
    }

    // 获取目标地砖可以举起的家具的家具控制器
    public FurnitureController CheckTileLiftablle(Vector2Int paraCoordinate)
    {
        if (tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile() != null)
        {
            // If there is a furniture located on the target tile, ask it if something is on that furniture.
            FurnitureController topFurnitureController = tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile().GetFurnitureOnTargetCoordinateOfCF(paraCoordinate);

            if(topFurnitureController.GetLiftable())
            {
                return topFurnitureController;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public bool CheckTileHaveNoFurniture(Vector2Int paraCoordinate)
    {
        bool res = true;

        if (paraCoordinate.x < 0 || paraCoordinate.y < 0 || paraCoordinate.x >= worldSize || paraCoordinate.y >= worldSize)
        {
            return false;
        }

        if (tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile() != null)
        {
            return false;
        }


        return res;
    }

    // 检查目标地砖可以卸下家具
    public bool CheckTileDisposable(Vector2Int paraCoordinate)
    {
        bool res = true;

        

        if (paraCoordinate.x < 0 || paraCoordinate.y < 0 || paraCoordinate.x >= worldSize || paraCoordinate.y >= worldSize)
        {
            return false;
        }

        if (tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile() == null)
        {
            return true;
        }

        if (tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile() != null)
        {
            FurnitureController topFurnitureController = tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile().GetFurnitureOnTargetCoordinateOfCF(paraCoordinate);

            if (topFurnitureController.GetBearable())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return res;
    }

    public bool TryToDisposeFurniture(FurnitureController operatingFurnitureController, Vector2Int paraCoordinate)
    {
        if (paraCoordinate.x < 0 || paraCoordinate.y < 0 || paraCoordinate.x >= worldSize || paraCoordinate.y >= worldSize)
        {
            return false;
        }

        if (tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile() == null)
        {
            // Modify the position of the operating furniture.
            operatingFurnitureController.transform.SetParent(null);
            Vector3 newPosition = GetTilePosition(paraCoordinate);
            newPosition.y = 1.25f;
            operatingFurnitureController.transform.position = newPosition;

            operatingFurnitureController.SetOccupyTiles(paraCoordinate);

            operatingFurnitureController.SetIsOTheGround(true);

            operatingFurnitureController.SetFurnitureUnderCF(null);

            RegisterFurnitureOnSingleTile(operatingFurnitureController, paraCoordinate);

            return true;
        }

        if (tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile() != null)
        {
            FurnitureController topFurnitureController = tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile().GetFurnitureOnTargetCoordinateOfCF(paraCoordinate);

            if (topFurnitureController.GetBearable())
            {
                // Modify the position of the operating furniture.
                operatingFurnitureController.transform.SetParent(null);
                operatingFurnitureController.transform.position = topFurnitureController.GetAnchorPositionOnTargetCoordinateOfCF(paraCoordinate);

                operatingFurnitureController.SetOccupyTiles(paraCoordinate);

                operatingFurnitureController.SetIsOTheGround(false);

                operatingFurnitureController.SetFurnitureUnderCF(topFurnitureController.gameObject);

                topFurnitureController.RegisterFurnitureOnCF(operatingFurnitureController, paraCoordinate);

                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    // 获取目标地砖可以推或者拉的家具的家具控制器
    public FurnitureController CheckTilePushableOrPullable(Vector2Int paraCoordinate)
    {
        if(tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile() != null)
        {
            FurnitureController bottomFurnitureController = tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile();

            if (bottomFurnitureController.GetPullable() || bottomFurnitureController.GetPushable())
            {
                return bottomFurnitureController;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public void RegisterFurnitureOnSingleTile(FurnitureController paraFurnitureController, Vector2Int paraCoordinate)
    {
        //Debug.Log(paraFurnitureController.gameObject.name + " Register " + paraCoordinate);

        tileArray[paraCoordinate.x, paraCoordinate.y].SetFurnitureOnThisTile(paraFurnitureController);
        tileArray[paraCoordinate.x, paraCoordinate.y].gameObject.GetComponent<Renderer>().material = DebugMaterial;
    }

    public void DeregisterFurnitureOnSingleTile(FurnitureController paraFurnitureController, Vector2Int paraCoordinate)
    {
        //Debug.Log(paraFurnitureController.gameObject.name + " Deregister " + paraCoordinate);

        tileArray[paraCoordinate.x, paraCoordinate.y].SetFurnitureOnThisTile(null);
        tileArray[paraCoordinate.x, paraCoordinate.y].gameObject.GetComponent<Renderer>().material = tileMaterial[(paraCoordinate.x + paraCoordinate.y) % 2];
    }

        // Start is called before the first frame update
    void Start()
    {
        tileMaterial = new Material[2];
        tileMaterial[0] = (Material)Resources.Load("Materials/WhiteTile");
        tileMaterial[1] = (Material)Resources.Load("Materials/BlackTile");

        tilePrefab = (GameObject)Resources.Load("Prefabs/Tile");

        tileArray = new TileController[worldSize, worldSize];

        

        if(worldSize % 2==0)
        {
            initialTilePosition = new Vector3((float)-((worldSize / 2 - 0.5) * tileSize), 0.0f, (float)-((worldSize / 2 - 0.5) * tileSize));
        }
        else
        {
            initialTilePosition = new Vector3((float)-((worldSize / 2) * tileSize), 0.0f, (float)-((worldSize / 2) * tileSize));
        }

        for(int i=0;i< worldSize;i++)
        {
            for(int j=0;j< worldSize;j++)
            {
                Vector3 currentTilePosition = initialTilePosition + new Vector3(i * tileSize, 0.0f, j * tileSize);

                GameObject currentTile = GameObject.Instantiate(tilePrefab);

                currentTile.transform.SetParent(this.gameObject.transform);

                currentTile.transform.localPosition = currentTilePosition;

                currentTile.GetComponent<Renderer>().material = tileMaterial[(i + j) % 2];

                tileArray[i, j] = currentTile.GetComponent<TileController>();

                currentTile.GetComponent<TileController>().tileCordinate = new Vector2Int(i,j);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
