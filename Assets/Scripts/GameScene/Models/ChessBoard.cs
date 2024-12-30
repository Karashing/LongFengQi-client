using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// [Serializable]
// public class GridForChessBoardData : BaseGame {
//     public Vector3Int position;
//     public GridType type;
//     public int level;
//     public XCamp camp;
//     public GridForChessBoardData(Vector3Int xposition, GridType grid_type, int xlevel, XCamp xcamp) {
//         type = grid_type;
//         level = xlevel;
//         position = xposition;
//         camp = xcamp;
//     }
// }


[Serializable]
public class ChessBoardData {
    public int board_id;
    public List<ChessData> chesses = new List<ChessData>();
    public List<GridData> grids = new List<GridData>();
    public List<XActorData> shops = new List<XActorData>();
    public List<XActorData> tian_actors = new List<XActorData>();
}