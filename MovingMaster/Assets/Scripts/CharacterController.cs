using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public GameObject mainWorldManager = null;

    public GameObject operatableTarget = null;

    public GameObject headAnchor = null;

    WorldController mainWorldController = null;

    // 1 向上
    // 2 向右
    // 3 向下
    // 4 向左
    int characterOrientation = 1;

    // 主角当前所在坐标
    Vector2Int currentCoordinate = new Vector2Int(0, 0);

    // 主角的操作目标位置
    Vector2Int targetCoordinate = new Vector2Int(0, 1);

    // 正在执行动作标旗
    bool isOperating = false;
    // 正在推标旗
    bool isPushing = false;
    // 正在拉标旗
    bool isPulling = false;

    // 正在推或者拉标旗
    bool isPushingOrPull = false;

    // 正在举标旗
    bool isLifting = false;

    FurnitureController operatingFurnitureController = null;

    // Start is called before the first frame update
    void Start()
    {
        mainWorldController = mainWorldManager.GetComponent<WorldController>();
    }

    // Update is called once per frame
    void Update()
    {

        // 举起物体
        if (Input.GetKeyDown(KeyCode.O))
        {
            if(isOperating)
            {
                // 如果正在进行动作
                if(isLifting)
                {
                    // 正在举着物体
                    if(mainWorldController.TryToDisposeFurniture(operatingFurnitureController, targetCoordinate))
                    {                       
                        operatingFurnitureController = null;

                        isOperating = false;
                        isLifting = false;
                    }
                }
                else
                {
                    // 如果正在推
                }
            }
            else
            {
                operatingFurnitureController = mainWorldController.CheckTileLiftablle(targetCoordinate);

                if (operatingFurnitureController != null)
                {
                    operatingFurnitureController.transform.SetParent(this.gameObject.transform);
                    operatingFurnitureController.transform.position = headAnchor.transform.position;

                    operatingFurnitureController.DeregisterFurniture();

                    isOperating = true;
                    isLifting = true;
                }                
            }
        }

        // 准备推拉
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isOperating)
            {
                if(isPushingOrPull)
                {
                    operatingFurnitureController = null;

                    isOperating = false;
                    isPushingOrPull = false;
                }
            }
            else
            {
                operatingFurnitureController = mainWorldController.CheckTilePushableOrPullable(targetCoordinate);

                if (operatingFurnitureController != null)
                {
                    isOperating = true;
                    isPushingOrPull = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if(isPushingOrPull)
            {
                // 如果正在推或者拉

                if(characterOrientation == 1 || characterOrientation == 3)
                {
                    // 如果此时朝向为上或者下

                    // 
                    if(operatingFurnitureController.TryToMoveFurnitureOffset(currentCoordinate, PublicUtility.UpDirection))
                    {
                        // 如果家具可以向上推动
                        this.transform.position = this.transform.position + new Vector3(0.0f, 0.0f, 2.5f);

                        currentCoordinate += PublicUtility.UpDirection;



                        if(characterOrientation == 1)
                        {
                            targetCoordinate = currentCoordinate + new Vector2Int(0, 1);
                        }
                        else
                        {
                            targetCoordinate = currentCoordinate - new Vector2Int(0, 1);
                        }                        

                        if (mainWorldController.CheckTileOperatable(targetCoordinate))
                        {
                            operatableTarget.SetActive(true);
                            operatableTarget.transform.position = mainWorldController.GetTilePosition(targetCoordinate);
                        }
                        else
                        {
                            operatableTarget.SetActive(false);
                        }

                    }

                }
            }
            else
            {
                if (characterOrientation != 1)
                {
                    this.transform.rotation = Quaternion.Euler(0, 0, 0);
                    characterOrientation = 1;
                    targetCoordinate = currentCoordinate + new Vector2Int(0, 1);

                    if (mainWorldController.CheckTileOperatable(targetCoordinate))
                    {
                        operatableTarget.SetActive(true);
                        operatableTarget.transform.position = mainWorldController.GetTilePosition(targetCoordinate);
                    }
                    else
                    {
                        operatableTarget.SetActive(false);
                    }
                }

                if (mainWorldController.CheckTilePassable(currentCoordinate + new Vector2Int(0, 1)))
                {
                    this.transform.position = this.transform.position + new Vector3(0.0f, 0.0f, 2.5f);

                    currentCoordinate += new Vector2Int(0, 1);

                    targetCoordinate += new Vector2Int(0, 1);

                    if (mainWorldController.CheckTileOperatable(targetCoordinate))
                    {
                        operatableTarget.SetActive(true);
                        operatableTarget.transform.position = mainWorldController.GetTilePosition(targetCoordinate);
                    }
                    else
                    {
                        operatableTarget.SetActive(false);
                    }
                }
            }                        
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (isPushingOrPull)
            {
                // 如果正在推或者拉

                if (characterOrientation == 1 || characterOrientation == 3)
                {
                    // 如果此时朝向为上或者下

                    // 
                    if (operatingFurnitureController.TryToMoveFurnitureOffset(currentCoordinate, PublicUtility.DownDirection))
                    {
                        // 如果家具可以向上推动
                        this.transform.position = this.transform.position - new Vector3(0.0f, 0.0f, 2.5f);

                        currentCoordinate += PublicUtility.DownDirection;

                        if(characterOrientation == 3)
                        {
                            targetCoordinate = currentCoordinate - new Vector2Int(0, 1);
                        }
                        else
                        {
                            targetCoordinate = currentCoordinate + new Vector2Int(0, 1);
                        }                        

                        if (mainWorldController.CheckTileOperatable(targetCoordinate))
                        {
                            operatableTarget.SetActive(true);
                            operatableTarget.transform.position = mainWorldController.GetTilePosition(targetCoordinate);
                        }
                        else
                        {
                            operatableTarget.SetActive(false);
                        }

                    }

                }
            }
            else
            {
                if (characterOrientation != 3)
                {
                    this.transform.rotation = Quaternion.Euler(0, 180, 0);
                    characterOrientation = 3;
                    targetCoordinate = currentCoordinate + new Vector2Int(0, -1);

                    if (mainWorldController.CheckTileOperatable(targetCoordinate))
                    {
                        operatableTarget.SetActive(true);
                        operatableTarget.transform.position = mainWorldController.GetTilePosition(targetCoordinate);
                    }
                    else
                    {
                        operatableTarget.SetActive(false);
                    }
                }

                if (mainWorldController.CheckTilePassable(currentCoordinate + new Vector2Int(0, -1)))
                {
                    this.transform.position = this.transform.position - new Vector3(0.0f, 0.0f, 2.5f);

                    currentCoordinate += new Vector2Int(0, -1);

                    targetCoordinate += new Vector2Int(0, -1);

                    if (mainWorldController.CheckTileOperatable(targetCoordinate))
                    {
                        operatableTarget.SetActive(true);
                        operatableTarget.transform.position = mainWorldController.GetTilePosition(targetCoordinate);
                    }
                    else
                    {
                        operatableTarget.SetActive(false);
                    }
                }
            }                            
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if(isPushingOrPull)
            {
                // 如果正在推或者拉

                if (characterOrientation == 2 || characterOrientation == 4)
                {
                    // 如果此时朝向为上或者下

                    // 
                    if (operatingFurnitureController.TryToMoveFurnitureOffset(currentCoordinate, PublicUtility.LeftDirection))
                    {
                        // 如果家具可以向上推动
                        this.transform.position = this.transform.position - new Vector3(2.5f, 0.0f, 0.0f);

                        currentCoordinate += PublicUtility.LeftDirection;

                        if(characterOrientation == 4)
                        {
                            targetCoordinate = currentCoordinate - new Vector2Int(1, 0);
                        }
                        else
                        {
                            targetCoordinate = currentCoordinate + new Vector2Int(1, 0);
                        }                        

                        if (mainWorldController.CheckTileOperatable(targetCoordinate))
                        {
                            operatableTarget.SetActive(true);
                            operatableTarget.transform.position = mainWorldController.GetTilePosition(targetCoordinate);
                        }
                        else
                        {
                            operatableTarget.SetActive(false);
                        }
                    }

                }
            }
            else
            {
                if (characterOrientation != 4)
                {
                    this.transform.rotation = Quaternion.Euler(0, -90, 0);
                    characterOrientation = 4;
                    targetCoordinate = currentCoordinate + new Vector2Int(-1, 0);

                    if (mainWorldController.CheckTileOperatable(targetCoordinate))
                    {
                        operatableTarget.SetActive(true);
                        operatableTarget.transform.position = mainWorldController.GetTilePosition(targetCoordinate);
                    }
                    else
                    {
                        operatableTarget.SetActive(false);
                    }
                }

                if (mainWorldController.CheckTilePassable(currentCoordinate + new Vector2Int(-1, 0)))
                {
                    this.transform.position = this.transform.position - new Vector3(2.5f, 0.0f, 0.0f);

                    currentCoordinate += new Vector2Int(-1, 0);

                    targetCoordinate += new Vector2Int(-1, 0);

                    if (mainWorldController.CheckTileOperatable(targetCoordinate))
                    {
                        operatableTarget.SetActive(true);
                        operatableTarget.transform.position = mainWorldController.GetTilePosition(targetCoordinate);
                    }
                    else
                    {
                        operatableTarget.SetActive(false);
                    }
                }
            }                        
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (isPushingOrPull)
            {
                // 如果正在推或者拉

                if (characterOrientation == 2 || characterOrientation == 4)
                {
                    // 如果此时朝向为上或者下

                    // 
                    if (operatingFurnitureController.TryToMoveFurnitureOffset(currentCoordinate, PublicUtility.RightDirection))
                    {
                        // 如果家具可以向上推动
                        this.transform.position = this.transform.position + new Vector3(2.5f, 0.0f, 0.0f);

                        currentCoordinate += PublicUtility.RightDirection;

                        if(characterOrientation == 2)
                        {
                            targetCoordinate = currentCoordinate + new Vector2Int(1, 0);
                        }
                        else
                        {
                            targetCoordinate = currentCoordinate - new Vector2Int(1, 0);
                        }
                        

                        if (mainWorldController.CheckTileOperatable(targetCoordinate))
                        {
                            operatableTarget.SetActive(true);
                            operatableTarget.transform.position = mainWorldController.GetTilePosition(targetCoordinate);
                        }
                        else
                        {
                            operatableTarget.SetActive(false);
                        }
                    }

                }
            }
            else
            {
                if (characterOrientation != 2)
                {
                    this.transform.rotation = Quaternion.Euler(0, 90, 0);
                    characterOrientation = 2;
                    targetCoordinate = currentCoordinate + new Vector2Int(1, 0);

                    if (mainWorldController.CheckTileOperatable(targetCoordinate))
                    {
                        operatableTarget.SetActive(true);
                        operatableTarget.transform.position = mainWorldController.GetTilePosition(targetCoordinate);
                    }
                    else
                    {
                        operatableTarget.SetActive(false);
                    }
                }

                if (mainWorldController.CheckTilePassable(currentCoordinate + new Vector2Int(1, 0)))
                {
                    this.transform.position = this.transform.position + new Vector3(2.5f, 0.0f, 0.0f);

                    currentCoordinate += new Vector2Int(1, 0);

                    targetCoordinate += new Vector2Int(1, 0);

                    if (mainWorldController.CheckTileOperatable(targetCoordinate))
                    {
                        operatableTarget.SetActive(true);
                        operatableTarget.transform.position = mainWorldController.GetTilePosition(targetCoordinate);
                    }
                    else
                    {
                        operatableTarget.SetActive(false);
                    }
                }
            }

                          
        }
    }
}
