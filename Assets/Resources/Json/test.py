import json;

data = {
    "chess":[
        {
            "id":0,
            "name":"龙",
            "attack":[6,9],
            "hp":[9,14],
            "skill":{},
            "describe":{
                "role":"毁灭",
            }
        },
        {
            "id":1,
            "name":"凤",
            "attack":[8,13],
            "hp":[6,9],
            "skill":{},
            "describe":{
                "role":"巡猎",
            }
        }
    ]
}
print(json.dumps(data, ensure_ascii=False))