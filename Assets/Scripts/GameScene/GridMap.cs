using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridMap : BaseBehaviour {
    public Tilemap tilemap;

    private void Awake() {
    }
    public Vector3 GetCellCenterWorld(XGrid grid) {
        return tilemap.GetCellCenterWorld(grid.grid_position);
    }
    public Vector3 GetCellCenterWorld(Vector3Int grid_position) {
        return tilemap.GetCellCenterWorld(grid_position);
    }

}
