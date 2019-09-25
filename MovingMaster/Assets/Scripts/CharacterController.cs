using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public GameObject mainWorldManager = null;

    public GameObject operatableTarget = null;

    public GameObject headAnchor = null;

    public float movingSpeed = 6.0f;

    WorldController mainWorldController = null;

    Vector2Int[] orientationArray = new Vector2Int[4];

    // 1 向上
    // 2 向右
    // 3 向下
    // 4 向左
    int characterOrientation = 0;

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

    bool isMoving = false;

    float movingTime = 0.0f;
    float movingNeedTime = 0.0f;
    Vector3 endPosition = new Vector3();
    Vector3 direction = new Vector3();

    FurnitureController operatingFurnitureController = null;

    public Animation anim;

    public void CharacterMoveUpdate(int moveOrientation)
    {
        if (isPushingOrPull)
        {
            // 如果正在推或者拉

            if (characterOrientation == moveOrientation || characterOrientation == (moveOrientation + 2) % 4)
            {
                if (operatingFurnitureController.TryToMoveFurnitureOffset(currentCoordinate, orientationArray[moveOrientation]))
                {
                    // 如果家具可以向上推动
                    //this.transform.position = this.transform.position + new Vector3( orientationArray[moveOrientation].x * 2.5f, 0.0f, orientationArray[moveOrientation].y * 2.5f );
                    isMoving = true;                    
                    currentCoordinate += orientationArray[moveOrientation];
                    endPosition = mainWorldController.GetTilePosition(currentCoordinate);
                    direction = Vector3.Normalize(endPosition - this.transform.position);

                    if (characterOrientation == moveOrientation)
                    {
                        targetCoordinate = currentCoordinate + orientationArray[moveOrientation];
                    }
                    else
                    {
                        targetCoordinate = currentCoordinate - orientationArray[moveOrientation];
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
            if (characterOrientation != moveOrientation)
            {
                this.transform.rotation = Quaternion.Euler(0, moveOrientation * 90.0f, 0);
                characterOrientation = moveOrientation;
                targetCoordinate = currentCoordinate + orientationArray[moveOrientation];

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

            if (mainWorldController.CheckTilePassable(currentCoordinate + orientationArray[moveOrientation]))
            {
                //this.transform.position = this.transform.position + new Vector3(orientationArray[moveOrientation].x * 2.5f, 0.0f, orientationArray[moveOrientation].y * 2.5f);
                isMoving = true;
                currentCoordinate += orientationArray[moveOrientation];
                endPosition = mainWorldController.GetTilePosition(currentCoordinate);
                direction = Vector3.Normalize(endPosition - this.transform.position);

                targetCoordinate += orientationArray[moveOrientation];

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

    // Start is called before the first frame update
    void Start()
    {
        orientationArray[0] = new Vector2Int(0, 1);
        orientationArray[1] = new Vector2Int(1, 0);
        orientationArray[2] = new Vector2Int(0, -1);
        orientationArray[3] = new Vector2Int(-1, 0);

        mainWorldController = mainWorldManager.GetComponent<WorldController>();

        
        //anim["Run"].speed = 1;

        movingNeedTime = mainWorldManager.GetComponent<WorldController>().tileSize / movingSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            if(movingTime + Time.deltaTime >= movingNeedTime)
            {
                anim.Play("Idle");
                this.transform.position = endPosition;
                movingTime = 0.0f;
                isMoving = false;
            }
            else
            {
                anim.Play("Run");
                this.transform.position += direction * movingSpeed * Time.deltaTime;
                movingTime += Time.deltaTime;
            }
        }

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

        if(!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                CharacterMoveUpdate(0);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                CharacterMoveUpdate(2);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                CharacterMoveUpdate(3);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                CharacterMoveUpdate(1);
            }
        }        
    }
}
