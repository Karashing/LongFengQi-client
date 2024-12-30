// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;
// using ToolI;
// using LitJson;
// using System.Linq;

// public class test_grids : MonoBehaviour {
//     Dictionary<string, int> grid_dict = new Dictionary<string, int> {
//         {"chess_board_1",1},
//         {"chess_board_2",2},
//         {"chess_board_3",3},
//         {"chess_board_4",4},
//         {"chess_board_5",5},
//         {"chess_board_6",6},
//         {"chess_board_7",7},
//         {"chess_board_8",8},
//         {"chess_board_9",9},
//         {"chess_board_x",10},
//         {"chess_board_1001",1001},
//         {"chess_board_1002",1002},
//     };
//     public Tilemap tilemap;
//     public Tile[] tiles;

//     public string chess_board_data_json_name;
//     public int board_id;
//     public int max_n;

//     void Start() {
//         int grid_id = 1;
//         ChessBoardData chess_board_data = new ChessBoardData();
//         chess_board_data.board_id = board_id;
//         for (int i = -max_n; i <= max_n; ++i) {
//             for (int j = -max_n; j <= max_n; ++j) {
//                 Sprite sprite = tilemap.GetSprite(new Vector3Int(i, j, 0));
//                 if (sprite) {
//                     GridForChessBoardData grid = new GridForChessBoardData(grid_id, new Vector3Int(i, j, 0), grid_dict[sprite.name]);
//                     Debug.Log(JsonUtility.ToJson(grid));
//                     grid_id++;
//                     chess_board_data.grids.Add(grid);
//                 }
//             }
//         }
//         JsonI.WriteToJson(chess_board_data, chess_board_data_json_name);
//     }

//     // Update is called once per frame
//     void Update() {

//     }
// }
