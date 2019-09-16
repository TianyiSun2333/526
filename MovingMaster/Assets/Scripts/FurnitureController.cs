using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureController : MonoBehaviour
{
    WorldController MainWorldController = null;

    // 家具占用地砖的地砖坐标数组
    public Vector2Int[] occupyTiles = new Vector2Int[8];

    //List<Vector2Int>

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(occupyTiles.GetLength(0));

        MainWorldController = GameObject.Find("WorldManager").GetComponent<WorldController>();

        Vector3 initialPosition = MainWorldController.GetTilePosition(occupyTiles[0]);

        initialPosition.y = 1.25f;

        this.transform.position = initialPosition;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
