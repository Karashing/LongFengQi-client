using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Unity.VisualScripting.FullSerializer;
using ToolI;

[Serializable]
public class GameData : BaseGame {
    private ChessBoardData chess_board;
    // ----- 功能性Actor -----
    [SerializeField]
    private ChessShop shop;
    // ----- 棋子 -----
    [SerializeField]
    private List<XChess> chess_list;
    public ParasiticDict<ChessType, XChess> chess_dict = new ParasiticDict<ChessType, XChess>();
    public int chess_num {
        get {
            return chess_dict.Count;
        }
    }
    // ----- 格子 -----
    [SerializeField]
    private List<XGrid> grid_list;
    public ParasiticDict<GridType, XGrid> grid_dict = new ParasiticDict<GridType, XGrid>();


    public void Init() {
        grid_dict.LoadBy(grid_list, (x) => x.type);
        chess_dict.LoadBy(chess_list, (x) => x.type);
    }
    public void LoadData(ChessBoardData data) {
        chess_board = data;
        foreach (var shop_data in GameData.chess_board.shops) {
            FM.LoadShop(shop_data);
        }
        // foreach (var chess_data in GameData.chess_board.chesses) {
        //     FM.LoadChess(chess_data);
        // }
        foreach (var grid_data in GameData.chess_board.grids) {
            FM.LoadGrid(grid_data);
        }
    }
    // ----- Emotion -----
    public int emotion_count;
}


