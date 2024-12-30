using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ----- In Game -----


// ----- Game Data -----

[Serializable]
public enum ChessType {
    LONG = 0, // 龙
    FENG = 1, // 凤
    SI = 2, // 司
    KUN = 3, // 鲲
    XIA = 4, // 侠
    GONG = 5, // 工
    HU = 6, // 狐
    XIANG = 7, // 相
    JIANG = 8, // 将
    BING_YUAN = 9, // 兵（远程）
    BING_JIN = 10, // 兵（近战）
    YI = 11, // 医
    SHI = 12, // 士
    NONG = 13, // 农
    YAO = 14, // 妖
    SHI1 = 15, // 狮
    YING = 16, // 鹰
    WEI = 17, // 尉
    PAO = 18, // 炮
    ZHA = 19, // 炸(Item)
    MO = 20, // 魔
}

[Serializable]
public class ChessSkill {

}
public class ChessDescribe {
    public string name;
    public string role;
}


[Serializable]
public class ChessData : BaseGame {
    public string server_id;
    public ChessPositionType position_type;
    public string position_info;
    public ChessType type;
    public int level;
    public int camp;
    public XCamp local_camp {
        get {
            return GameInfo.GetLocalCamp(camp);
        }
    }
    public ChessData(string serverId, ChessType chessType, int chessLevel, XCamp xcamp, ChessPositionType chessPositionType, string positionInfo = "") {
        server_id = serverId;
        type = chessType;
        level = chessLevel;
        camp = GameInfo.GetRealCamp(xcamp);
        position_type = chessPositionType;
        position_info = positionInfo;
    }
}

[Serializable]
public enum ChessPositionType {
    none = 0,
    grid_server_id = 1,
    random_prepare_grid = 2,
}