using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureController : MonoBehaviour
{
    WorldController MainWorldController = null;

    // 当前家具是否位于地面上
    public bool isOnTheGround = false;

    // 家具占用地砖的地砖坐标数组
    public Vector2Int[] occupyTiles = new Vector2Int[8];

    //List<Vector2Int>

    public void SetOccupyTiles(Vector2Int newCoordinate)
    {
        occupyTiles[0] = newCoordinate;
    }

    //检查当前家具移动目标位置是否合法
    public bool CheckTargetOffset(Vector2Int characterCoordinate, Vector2Int paraOffset)
    {
        bool res = true;

        // 1.先移除
        for (int i = 0; i < occupyTiles.GetLength(0); i++)
        {
            MainWorldController.RemoveFurniture(this, occupyTiles[i]);
        }

        // 检查主角是否可以移动
        if(!MainWorldController.CheckTilePassable(characterCoordinate + paraOffset))
        {
            res = false;
        }

        // 2.检查当前家具是否能够移动
        for (int i = 0; i < occupyTiles.GetLength(0); i++)
        {
            if(!MainWorldController.CheckTileDisposable(occupyTiles[i] + paraOffset))
            {
                res = false;
            }
        }

        // 3.恢复初始移除
        for (int i = 0; i < occupyTiles.GetLength(0); i++)
        {
            MainWorldController.SetFurniture(this, occupyTiles[i]);
        }

        return res;
    }

    public void MoveFurnitureOffset(Vector2Int paraOffset)
    {
        for (int i = 0; i < occupyTiles.GetLength(0); i++)
        {
            MainWorldController.RemoveFurniture(this, occupyTiles[i]);
        }

        // 3.恢复初始移除
        for (int i = 0; i < occupyTiles.GetLength(0); i++)
        {
            occupyTiles[i] += paraOffset;
            MainWorldController.SetFurniture(this, occupyTiles[i]);
        }

        this.transform.position = this.transform.position + new Vector3(2.5f * paraOffset.x, 0.0f, 2.5f * paraOffset.y);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(occupyTiles.GetLength(0));

        MainWorldController = GameObject.Find("WorldManager").GetComponent<WorldController>();

        Vector3 initialPosition = MainWorldController.GetTilePosition(occupyTiles[0]);

        initialPosition.y = 1.25f;

        this.transform.position = initialPosition;

        if(isOnTheGround)
        {
            for(int i=0;i< occupyTiles.GetLength(0); i++)
            {
                MainWorldController.SetFurniture(this, occupyTiles[i]);
            }           
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
