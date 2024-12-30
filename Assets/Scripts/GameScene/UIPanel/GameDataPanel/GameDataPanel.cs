using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using Unity.VisualScripting;
using System.IO;

// TODO
public class GameDataPanel : BaseBehaviour {
    public ButtonGroup button_group;
    public Transform content_trans;
    public TMP_Text detail_text;

    public int panel_id = -1;

    public void Init(int default_id = 0) {
        if (panel_id < 0) panel_id = default_id;
        if (panel_id == 0) LoadChess();
    }
    public void LoadChess() {
        List<XChess> xchess_list = new List<XChess>();
        foreach (var xchess in GameData.chess_dict.Values) {
            xchess_list.Add(xchess);
        }
        xchess_list.Sort((x, y) => {
            if (x.rarity == y.rarity) {
                return x.type.CompareTo(y.type);
            }
            return x.rarity.CompareTo(y.rarity);
        });
        foreach (var xchess in xchess_list) {
            var xchess_ui = FM.LoadNormalActorUI(xchess);
            xchess_ui.transform.SetParent(content_trans);
            xchess_ui.rect_trans.localScale = Vector3.one;
        }
    }

}