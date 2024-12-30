using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class BigSkillQueue : BaseBehaviour {
    public RectTransform content_trans;
    private List<BigSkillCard> cards = new List<BigSkillCard>();
    public InteractCard selected_card;
    public XSkill selected_skill {
        get {
            if (selected_card == null) return null;
            return selected_card.skill;
        }
    }
    private void Awake() {
        EM.actors_change.AddListener(Refresh);
    }
    public void AddActor(XActor xactor) {
        if (xactor.big_skill != null && xactor.camp == XCamp.SELF) {
            var card = FM.LoadBigSkillCard(xactor);
            card.transform.SetParent(content_trans);
            card.rect_trans.localScale = Vector3.one;
            card.MoveCard(new(-500f, 0f), 0f);
            cards.Add(card);
        }
        Refresh();
    }
    public void RemoveActor(XActor xactor) {
        for (int i = cards.Count - 1; i >= 0; --i) {
            if (cards[i].actor == xactor) {
                cards[i].Kill();
                cards.RemoveAt(i);
            }
        }
        Refresh();
    }
    public void Refresh() {
        for (int i = 0; i < cards.Count; ++i) {
            cards[i].SetInteractId(i + 1);
            cards[i].MoveCard(new(50f + 100 * i, 0f));
        }

    }
}