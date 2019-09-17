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

        if(tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile()!=null)
        {
            res = false;
        }

        return res;
    }

    public FurnitureController CheckTileLiftablle(Vector2Int paraCoordinate)
    {
        if (tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile() != null)
        {
            return tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile();
        }
        else
        {
            return null;
        }
    }

    // 检查目标地砖可以卸下家具
    public bool CheckTileDisposable(Vector2Int paraCoordinate)
    {
        bool res = true;

        if (paraCoordinate.x < 0 || paraCoordinate.y < 0 || paraCoordinate.x >= worldSize || paraCoordinate.y >= worldSize)
        {
            res = false;
        }

        if (tileArray[paraCoordinate.x, paraCoordinate.y].GetFurnitureOnThisTile() != null)
        {
            res = false;
        }

        return res;
    }

    public void SetFurniture(FurnitureController paraFuFurnitureController, Vector2Int paraCoordinate)
    {
        tileArray[paraCoordinate.x, paraCoordinate.y].SetFurnitureOnThisTile(paraFuFurnitureController);
    }

    public void RemoveFurniture(FurnitureController paraFuFurnitureController, Vector2Int paraCoordinate)
    {
        tileArray[paraCoordinate.x, paraCoordinate.y].SetFurnitureOnThisTile(null);
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
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
