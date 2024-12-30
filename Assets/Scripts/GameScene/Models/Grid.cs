using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ----- In Game -----

[Serializable]
public enum GridState {
    None = 0,
    EMPTY = 1,
    HAVING = 2,
    FORBIDDEN = 3,
}


// ----- Game Data -----

[Serializable]
public enum GridType {
    None = 0, // None
    AREA = 1, // 地
    TIAN = 10, // 田
    QIANG = 100, // 墙
    LOU = 101, // 楼
    FU = 102, // 府
    CHENG = 103, // 城
    GONG = 104, // 宫
    TA = 105, // 塔

    prepare_chess = 1001,
    prepare_item = 1002,
    delete_chess = 1003,

}

[Serializable]
public class GridDescribe {
    public string role;
}

// [Serializable]
// public class GridData {
//     public int id;
//     public GridType type;
//     public string name;
//     public GridDescribe describe;
//     public GridData(int grid_id, GridType grid_type) {
//         id = grid_id;
//         type = grid_type;
//     }
// }


[Serializable]
public class GridData : BaseGame {
    public string server_id;
    public Vector3Int position;
    public GridType type;
    public int level;
    public int camp;
    public XCamp local_camp {
        get {
            return GameInfo.GetLocalCamp(camp);
        }
    }
    public GridData(string xserver_id, Vector3Int xposition, GridType grid_type, int xlevel, XCamp xcamp) {
        server_id = xserver_id;
        type = grid_type;
        level = xlevel;
        position = xposition;
        camp = GameInfo.GetRealCamp(xcamp);
    }
}