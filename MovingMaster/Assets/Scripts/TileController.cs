using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    //List<GameObject> containFurniture = new List<GameObject>();

    List<FurnitureController> containFurniture = new List<FurnitureController>();

    //public void InitilizeAFurniture(GameObject newFurnitureBase, int newFurenitureLayer)
    //{
    //    if
    //}

    // 位于当前地砖上方的家具
    FurnitureController furnitureOnThisTile = null;

    // 设置位于当前地砖上方的家具家具控制器引用
    public void SetFurnitureOnThisTile(FurnitureController newFurnitureOnThisTile)
    {
        furnitureOnThisTile = newFurnitureOnThisTile;
    }

    public FurnitureController GetFurnitureOnThisTile()
    {
        return furnitureOnThisTile;
    }

    // 清除位于当前地砖上方的家具家具控制器引用
    public void ClearFurnitureOnThisTile()
    {
        furnitureOnThisTile = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
