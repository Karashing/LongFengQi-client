using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ToolI;
using System.Linq;

public class ShopIncomePanel : BaseBehaviour {
    public TMP_Text income_title_text;
    public RectTransform income_detail_rect_trans;
    public void Init() {
        ShopIncomeDetail xincome_detail;
        for (int i = 0; i < income_detail_rect_trans.childCount; ++i) {
            Destroy(income_detail_rect_trans.GetChild(i).gameObject);
        }

        var tian_income = 0;
        foreach (var xgrid in GameInfo.GetGrids(GridType.TIAN)) {
            if (xgrid.state == GridState.HAVING && xgrid.bind_chess.camp == XCamp.SELF) tian_income++;
        }
        if (tian_income > 0) {
            xincome_detail = FM.LoadShopIncomeDetail(tian_income, "田间收益");
            xincome_detail.transform.SetParent(income_detail_rect_trans);
            xincome_detail.rect_trans.localScale = Vector3.one;
        }

        var building_income = 0;
        foreach (var xgrid in GameInfo.GetSelfBuildingGrids()) {
            if (xgrid.state == GridState.HAVING && xgrid.bind_chess.camp == XCamp.SELF) building_income++;
        }
        if (building_income > 0) {
            xincome_detail = FM.LoadShopIncomeDetail(building_income, "建筑收益");
            xincome_detail.transform.SetParent(income_detail_rect_trans);
            xincome_detail.rect_trans.localScale = Vector3.one;
        }

        var interest_income = Mathf.Min(GameInfo.coin / 10 * 2, 6);
        if (interest_income > 0) {
            xincome_detail = FM.LoadShopIncomeDetail(interest_income, "利息");
            xincome_detail.transform.SetParent(income_detail_rect_trans);
            xincome_detail.rect_trans.localScale = Vector3.one;
        }

        var base_income = (int)(GameInfo.cur_act_time / 400) + 6;
        xincome_detail = FM.LoadShopIncomeDetail(base_income, "基础收益");
        xincome_detail.transform.SetParent(income_detail_rect_trans);
        xincome_detail.rect_trans.localScale = Vector3.one;

        var delta_coin = base_income + interest_income + tian_income + building_income;
        GameInfo.coin += delta_coin;
        Debug.Log("coin += " + delta_coin);
        income_title_text.text = $"本轮总收益：{delta_coin}";

        gameObject.SetActive(true);
    }
    public void Close() {
        gameObject.SetActive(false);
    }
}