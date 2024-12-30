using System;
using System.Collections;
using System.Collections.Generic;

enum EnumGrpcCode {
    Server_Error = 40001,

    Account_Already_Exists = 50001,
    Account_Or_Password_Error = 50002,
}

[Serializable]
public class UserInfo {
    public string user_id;
    public string nick_name;
    public int game_score;
    public UserInfo(GrpcLoginContent content) {
        user_id = content.user_id;
        nick_name = content.user_name;
        game_score = content.user_score;
    }
}


[Serializable]
public class GrpcConnectContent {
    public int code;
    public string reason;
    public int vera0 = -1;
    public int verb0 = -1;
    public int verc0 = -1;
    public int vera1 = -1;
    public int verb1 = -1;
    public int verc1 = -1;
}
[Serializable]
public class GrpcLoginContent {
    public int code;
    public string reason;
    public string user_id;
    public string user_name;
    public int user_score;
}

[Serializable]
class GrpcMatchContent {
    public int code;
    public string reason;
    public string match_result;
    public List<string> player_name_list;
    public List<GrpcUserInfoContent> player_info;
}

[Serializable]
class GrpcUserInfoContent {
    public string user_id;
    public string nick_name;
    public string rating;
}

[Serializable]
public class GrpcVersion {
    public int va, vb, vc;
    public bool Validate(GrpcConnectContent content) {
        if (content.vera0 <= va && va <= content.vera1)
            if (content.verb0 <= vb && vb <= content.verb1)
                if (content.verc0 <= vc && vc <= content.verc1)
                    return true;
        return false;
    }
    public GrpcVersion() {
        va = -1;
        vb = -1;
        vc = -1;
    }
    public GrpcVersion(GrpcConnectContent content, bool is_new = true) {
        if (is_new) {
            va = content.vera1;
            vb = content.verb1;
            vc = content.verc1;
        }
        else {
            va = content.vera0;
            vb = content.verb0;
            vc = content.verc0;
        }
    }
    public override string ToString() {
        return $"{va}.{vb}.{vc}";
    }
}