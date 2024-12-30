using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ToolI;
using LitJson;
using System.Linq;

public class test_tilemap : MonoBehaviour {
    Dictionary<string, (GridType, int, XCamp)> grid_dict = new Dictionary<string, (GridType, int, XCamp)> {
        {"chess_board_1",(GridType.AREA, 0, XCamp.NEUTRAL)},
        {"chess_board_2",(GridType.TIAN, 0, XCamp.NEUTRAL)},
        {"chess_board_3",(GridType.TIAN, 1, XCamp.NEUTRAL)},
        {"chess_board_4",(GridType.TIAN, 2, XCamp.NEUTRAL)},
        {"chess_board_5",(GridType.QIANG, 0, XCamp.SELF)},
        {"chess_board_6",(GridType.LOU, 0, XCamp.SELF)},
        {"chess_board_7",(GridType.FU, 0, XCamp.SELF)},
        {"chess_board_8",(GridType.CHENG, 0, XCamp.SELF)},
        {"chess_board_9",(GridType.GONG, 0, XCamp.SELF)},
        {"chess_board_11",(GridType.TA, 0, XCamp.PUBLIC_ENEMY)},
        {"chess_board_1001",(GridType.prepare_chess, 0, XCamp.SELF)},
        {"chess_board_1002",(GridType.prepare_item, 0, XCamp.SELF)},
        {"chess_board_1003",(GridType.delete_chess, 0, XCamp.SELF)},
    };
    public Tilemap tilemap;

    public Vector2Int center_position;
    public string chess_board_data_json_name;
    public int board_id;
    public int max_n;
    public int camp;

    void Start() {
        int server_id = 1;
        ChessBoardData chess_board_data = new ChessBoardData();
        chess_board_data.board_id = board_id;

        XActorData shop = new(server_id.ToString(), 0);
        Debug.Log(JsonUtility.ToJson(shop));
        chess_board_data.shops.Add(shop);
        server_id++;

        // XActorData tian_actor = new(server_id.ToString(), 0);
        // Debug.Log(JsonUtility.ToJson(tian_actor));
        // chess_board_data.tian_actors.Add(tian_actor);
        // server_id++;

        server_id = 1000;

        if (camp == 1) {
            for (int j = -max_n; j <= max_n; ++j) {
                for (int i = -max_n; i <= max_n; ++i) {
                    Sprite sprite = tilemap.GetSprite(new Vector3Int(i, j, 0));
                    if (sprite) {
                        var (xtype, xlevel, xcamp) = grid_dict[sprite.name];
                        if (HaveCamp(xtype)) {
                            if (j > 0) xcamp = XCamp.ENEMY;
                        }
                        // FIXME
                        var x = j % 2 == 0 ? center_position.x - i : -i;
                        var y = center_position.y - j;
                        GridData grid = new(server_id.ToString(), new Vector3Int(x, y, 0), xtype, xlevel, xcamp);
                        Debug.Log(JsonUtility.ToJson(grid));
                        chess_board_data.grids.Add(grid);
                        server_id++;
                    }
                }
            }
        }
        else {
            for (int j = -max_n; j <= max_n; ++j) {
                for (int i = -max_n; i <= max_n; ++i) {
                    Sprite sprite = tilemap.GetSprite(new Vector3Int(i, j, 0));
                    if (sprite) {
                        var (xtype, xlevel, xcamp) = grid_dict[sprite.name];
                        if (HaveCamp(xtype)) {
                            if (j > 0) xcamp = XCamp.ENEMY;
                        }
                        GridData grid = new(server_id.ToString(), new Vector3Int(i, j, 0), xtype, xlevel, xcamp);
                        Debug.Log(JsonUtility.ToJson(grid));
                        chess_board_data.grids.Add(grid);
                        server_id++;
                    }
                }
            }

        }
        JsonI.WriteToJson(chess_board_data, chess_board_data_json_name);
    }
    bool HaveCamp(GridType xtype) {
        if (xtype == GridType.QIANG || xtype == GridType.LOU || xtype == GridType.FU || xtype == GridType.CHENG || xtype == GridType.GONG) return true;
        if (xtype == GridType.prepare_chess || xtype == GridType.prepare_item || xtype == GridType.delete_chess) return true;
        return false;
    }

    // Update is called once per frame
    void Update() {

    }
}
