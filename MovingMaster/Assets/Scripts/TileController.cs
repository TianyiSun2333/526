﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    List<GameObject> containFurniture = new List<GameObject>();



    public GameObject GetTopFurniture()
    {
        if(containFurniture.Count == 0)
        {
            return null;
        }
        else
        {
            return containFurniture[containFurniture.Count - 1];
        }
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
