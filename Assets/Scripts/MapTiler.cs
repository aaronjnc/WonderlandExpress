using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapTiler : MonoBehaviour
{
    public Tilemap map;
    public Tile useTile;
    public Vector2Int min;
    public Vector2Int max;
    public int spacing;
    public bool clearMap;
}
