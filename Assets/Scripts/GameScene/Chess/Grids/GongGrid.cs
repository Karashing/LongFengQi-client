using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking.Types;

public class GongGrid : XGrid {
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new XMaintainSkill(this, 0),
            new XRecoverSkill(this, 1){
                recover_hp_rate = 1,
            },
            new XEnterBoardSkill(this, 2),
            new XQuitBoardSkill(this, 3),
        };
    }
    public override void Init(GridData grid_data) {
        base.Init(grid_data);
        EnterActionQueue();
    }



}