using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

public class test_chess_tab : BaseBehaviour {
    public ShopChessTab chess_tab;
    public ChessType chessType;

    // [InspectorButton("ReBuild")]
    public bool reBuild;

    private void Start() {
        var xchess = GameData.chess_dict[chessType];
        Debug.Log(xchess);
        foreach (var xskill in xchess.skills) {
            Debug.Log(xskill.name());
        }
        chess_tab.Init(xchess);
    }
    public void ReBuild() {
        Debug.Log(chess_tab);
        Debug.Log(GameData);
        Debug.Log(GameData.chess_dict);

        chess_tab = GetComponent<ShopChessTab>();
        chess_tab.Init(GameData.chess_dict[chessType]);
    }
}