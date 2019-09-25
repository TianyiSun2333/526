using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureController : MonoBehaviour
{
    WorldController MainWorldController = null;

    //Attribute 
    //=================================================================

    // Flag of whether player can put something on this furniture.
    public bool canPutSomethingOn = false;

    // Flag of whether player can pull this furniture.
    public bool canBePulled = true;

    // Flag of whether player can push this furniture.
    public bool canBePushed = true;

    // Flag of whether player can lift this furniture.
    public bool canBeLifted = true;
   
    // 当前家具是否位于地面上
    public bool isOnTheGround = false;

    //=================================================================

    // 家具占用地砖的地砖坐标数组
    public Vector2Int[] tilesOffset = new Vector2Int[8];
    public Vector2Int baseTile;
    private Vector2Int[] occupyTiles;

    // 当前家具每个占用地砖位置上面放置的家具
    public GameObject[] itemOnOccupyTile = new GameObject[8];

    // 当前家具每个占用地砖位置的上方锚点
    public GameObject[] anchorOfOccupyTile = new GameObject[8];

    // 当前家具下方的家具引用
    public GameObject furnitureUnderCF = null;

    // Flag of wether this furniture is moving
    bool isMoving = false;

    // The moving speed of this furniture.
    float movingSpeed = 6.0f;

    Vector3 initialPosition = new Vector3();
    Vector3 targetPosition = new Vector3();

    Vector3 direction = new Vector3();

    public void SetFurnitureMove(Vector3 newTargetPosition)
    {
        if(!isMoving)
        {
            initialPosition = this.transform.position;
            targetPosition = newTargetPosition;

            direction = Vector3.Normalize(targetPosition - initialPosition);
            isMoving = true;
        }
    }

    public void SetIsOTheGround(bool newIsOnTheGround)
    {
        isOnTheGround = newIsOnTheGround;
    }

    public bool GetBearable() { return canPutSomethingOn; }

    public bool GetPullable() { return canBePulled; }

    public bool GetPushable() { return canBePushed; }

    public bool GetLiftable() { return canBeLifted; }

    public void SetFurnitureUnderCF(GameObject newFurnitureUnderCF)
    {
        furnitureUnderCF = newFurnitureUnderCF;
    }

    public FurnitureController GetFurnitureOnTargetCoordinateOfCF(Vector2Int paraCoordinate)
    {
        for(int i=0;i< occupyTiles.GetLength(0); i++)
        {
            if(occupyTiles[i].x == paraCoordinate.x && occupyTiles[i].y == paraCoordinate.y)
            {
                if(itemOnOccupyTile[i] != null)
                {
                    return itemOnOccupyTile[i].GetComponent<FurnitureController>().GetFurnitureOnTargetCoordinateOfCF(paraCoordinate);
                }
                else
                {
                    return this;
                }
            }
        }

        return null;
    }

    public Vector3 GetAnchorPositionOnTargetCoordinateOfCF(Vector2Int paraCoordinate)
    {
        for (int i = 0; i < occupyTiles.GetLength(0); i++)
        {
            if (occupyTiles[i].x == paraCoordinate.x && occupyTiles[i].y == paraCoordinate.y)
            {
                return anchorOfOccupyTile[i].transform.position;
            }
        }

        return new Vector3();
    }

    // 由下方家具调用，在paraCoordinate的坐标处注册位于当前家具上的家具控制器paraFurnitureController
    public void RegisterFurnitureOnCF(FurnitureController paraFurnitureController, Vector2Int paraCoordinate)
    {
        for (int i = 0; i < occupyTiles.GetLength(0); i++)
        {
            if (occupyTiles[i].x == paraCoordinate.x && occupyTiles[i].y == paraCoordinate.y)
            {
                itemOnOccupyTile[i] = paraFurnitureController.gameObject;
            }
        }
    }

    // 由下方家具调用，在paraCoordinate的坐标处注销位于当前家具上的家具控制器paraFurnitureController
    public void DeregisterFurnitureOnCF(FurnitureController paraFurnitureController, Vector2Int paraCoordinate)
    {
        for (int i = 0; i < occupyTiles.GetLength(0); i++)
        {
            if (occupyTiles[i].x == paraCoordinate.x && occupyTiles[i].y == paraCoordinate.y)
            {
                itemOnOccupyTile[i] = null;
            }
        }
    }

    public void RegisterFurniture()
    {
        if (isOnTheGround)
        {
            for (int i = 0; i < occupyTiles.GetLength(0); i++)
            {
                MainWorldController.RegisterFurnitureOnSingleTile(this, occupyTiles[i]);
            }
        }
        else
        {
            furnitureUnderCF.GetComponent<FurnitureController>().RegisterFurnitureOnCF(this, occupyTiles[0]);
        }
    }

    public void DeregisterFurniture()
    {
        if(isOnTheGround)
        {
            for (int i = 0; i < occupyTiles.GetLength(0); i++)
            {
                MainWorldController.DeregisterFurnitureOnSingleTile(this, occupyTiles[i]);
            }
        }
        else
        {
            furnitureUnderCF.GetComponent<FurnitureController>().DeregisterFurnitureOnCF(this, occupyTiles[0]);
        }

        furnitureUnderCF = null;
    }

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
            MainWorldController.DeregisterFurnitureOnSingleTile(this, occupyTiles[i]);
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
            MainWorldController.RegisterFurnitureOnSingleTile(this, occupyTiles[i]);
        }

        return res;
    }

    public void MoveFurnitureOffset(Vector2Int paraOffset)
    {
        for (int i = 0; i < occupyTiles.GetLength(0); i++)
        {
            MainWorldController.DeregisterFurnitureOnSingleTile(this, occupyTiles[i]);
        }

        // 3.恢复初始移除
        for (int i = 0; i < occupyTiles.GetLength(0); i++)
        {
            occupyTiles[i] += paraOffset;
            MainWorldController.RegisterFurnitureOnSingleTile(this, occupyTiles[i]);
        }

        this.transform.position = this.transform.position + new Vector3(2.5f * paraOffset.x, 0.0f, 2.5f * paraOffset.y);
    }

    //===============================================================================================

    public bool TryToMoveFurnitureOffset(Vector2Int characterCoordinate, Vector2Int paraOffset)
    {
        bool res = true;

        // 1.先移除
        for (int i = 0; i < occupyTiles.GetLength(0); i++)
        {
            MainWorldController.DeregisterFurnitureOnSingleTile(this, occupyTiles[i]);
        }

        // 检查主角是否可以移动
        if (!MainWorldController.CheckTilePassable(characterCoordinate + paraOffset))
        {
            res = false;
        }

        // 2.检查当前家具是否能够移动
        for (int i = 0; i < occupyTiles.GetLength(0); i++)
        {
            if (!MainWorldController.CheckTileHaveNoFurniture(occupyTiles[i] + paraOffset))
            {
                res = false;
            }
        }

        if(res)
        {
            for (int i = 0; i < occupyTiles.GetLength(0); i++)
            {
                occupyTiles[i] += paraOffset;
                MainWorldController.RegisterFurnitureOnSingleTile(this, occupyTiles[i]);

                if (itemOnOccupyTile[i] != null)
                {
                    itemOnOccupyTile[i].GetComponent<FurnitureController>().MoveFurnitureOnCFByOffset(paraOffset);
                }
            }

            SetFurnitureMove(this.transform.position + new Vector3(2.5f * paraOffset.x, 0.0f, 2.5f * paraOffset.y));

            //this.transform.position = this.transform.position + new Vector3(2.5f * paraOffset.x, 0.0f, 2.5f * paraOffset.y);
        }
        else
        {
            for (int i = 0; i < occupyTiles.GetLength(0); i++)
            {
                MainWorldController.RegisterFurnitureOnSingleTile(this, occupyTiles[i]);
            }
        }

        return res;
    }

    //===============================================================================================

    public void MoveFurnitureOnCFByOffset(Vector2Int paraOffset)
    {
        //this.transform.position = this.transform.position + new Vector3(2.5f * paraOffset.x, 0.0f, 2.5f * paraOffset.y);

        SetFurnitureMove(this.transform.position + new Vector3(2.5f * paraOffset.x, 0.0f, 2.5f * paraOffset.y));

        for (int i = 0; i < occupyTiles.GetLength(0); i++)
        {
            occupyTiles[i] += paraOffset;

            if (itemOnOccupyTile[i]!=null)
            {
                itemOnOccupyTile[i].GetComponent<FurnitureController>().MoveFurnitureOnCFByOffset(paraOffset);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        occupyTiles = new Vector2Int[tilesOffset.Length];
        for(int i = 0; i < tilesOffset.Length; i++)
        {
            occupyTiles[i] = baseTile + tilesOffset[i];
        }
        Debug.Log(occupyTiles.GetLength(0));

        MainWorldController = GameObject.Find("WorldManager").GetComponent<WorldController>();

        Vector3 initialPosition = MainWorldController.GetTilePosition(occupyTiles[0]);

        initialPosition.y = 1.25f;

        this.transform.position = initialPosition;

        if(isOnTheGround)
        {
            for(int i=0;i< occupyTiles.GetLength(0); i++)
            {
                MainWorldController.RegisterFurnitureOnSingleTile(this, occupyTiles[i]);
            }           
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            Vector3 currentFramePosition = this.transform.position;

            Vector3 nextFramePosition = this.transform.position + direction * Time.deltaTime * movingSpeed;

            float dot = Vector3.Dot(targetPosition - currentFramePosition, nextFramePosition - targetPosition);

            if(dot >= 0)
            {
                this.transform.position = targetPosition;

                isMoving = false;
            }
            else
            {
                this.transform.position = nextFramePosition;
            }

        }
    }
}
