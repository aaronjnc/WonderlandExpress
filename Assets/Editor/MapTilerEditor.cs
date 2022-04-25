using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(MapTiler))]
public class MapTilerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapTiler tiler = (MapTiler)target;
        if (GUILayout.Button("Spawn Tiles"))
        {
            Tilemap map = tiler.map;
            if (tiler.clearMap)
                map.ClearAllTiles();
            for (int x = tiler.min.x; x < tiler.max.x; x += tiler.spacing)
            {
                for (int y = tiler.min.y; y < tiler.max.y; y += tiler.spacing)
                {
                    map.SetTile(new Vector3Int(x, y, 0), tiler.useTile);
                }
            }
        }
    }
}
