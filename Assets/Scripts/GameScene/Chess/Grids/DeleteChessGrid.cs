using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeleteChessGrid : XGrid {
    public TMP_Text coin_text;

    public override void Init(GridData grid_data) {
        base.Init(grid_data);
        Hide();
    }
    public void Show(int coin) {
        coin_text.text = coin.ToString();
        gameObject.SetActive(true);
    }
    public void Hide() {
        gameObject.SetActive(false);
    }
}