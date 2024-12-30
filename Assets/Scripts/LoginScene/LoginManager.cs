using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour {
    private static LoginManager _instance;
    public static LoginManager Ins {
        get {
            return _instance;
        }
    }
    public TMP_InputField login_account_input_field;
    public TMP_InputField login_password_input_field;
    public TMP_InputField register_account_input_field;
    public TMP_InputField register_password_input_field;
    public TMP_InputField register_confirm_password_input_field;
    public GameObject login_panel;
    public GameObject register_panel;
    public GameObject register_success_panel;
    public Toggle is_remember_password_toggle;
    public TMP_Text login_account_tip_text;
    public TMP_Text login_password_tip_text;
    public TMP_Text register_account_tip_text;
    public TMP_Text register_password_tip_text;
    public TMP_Text register_confirm_password_tip_text;
    private void Awake() {
        _instance = this;
    }
    private void Start() {
        login_account_tip_text.text = "";
        login_password_tip_text.text = "";
        register_account_tip_text.text = "";
        register_password_tip_text.text = "";
        register_confirm_password_tip_text.text = "";
        login_account_input_field.text = PlayerPrefs.GetString("account", "");
        login_password_input_field.text = PlayerPrefs.GetString("password", "");
        LoginPanel();
    }
    public void Login() {
        if (CheckLoginAccount() && CheckLoginPassword()) {
            var account = login_account_input_field.text;
            var password = login_password_input_field.text;
            Debug.Log("[Login] " + account + ": " + password);
            var result = GrpcService.Ins.Login(account, password);
            var content = JsonUtility.FromJson<GrpcLoginContent>(result.content);
            if (result.is_success) {
                Debug.Log("Login Success");
                // GrpcService.Ins.user_id = result.user_id;
                GrpcService.Ins.user_info = new UserInfo(content);
                if (is_remember_password_toggle.isOn) {
                    PlayerPrefs.SetString("account", account);
                    PlayerPrefs.SetString("password", password);
                }
                else {
                    PlayerPrefs.SetString("account", account);
                    PlayerPrefs.SetString("password", "");
                }
                SceneManager.LoadScene(1);
            }
            else {
                if (content.code == (int)EnumGrpcCode.Account_Or_Password_Error) {
                    login_password_tip_text.text = "账户或密码错误";
                }
            }
        }
    }
    public void Register() {
        if (CheckRegisterAccount() && CheckRegisterPassword() && CheckRegisterConfirmPassword()) {
            var account = register_account_input_field.text;
            var password = register_password_input_field.text;
            Debug.Log("[Register] " + account + ": " + password);
            var result = GrpcService.Ins.Register(account, password);
            if (result.is_success) {
                register_success_panel.SetActive(true);
            }
            else {
                var content = JsonUtility.FromJson<GrpcLoginContent>(result.content);
                if (content.code == (int)EnumGrpcCode.Account_Already_Exists) {
                    register_account_tip_text.text = "账号已存在，请使用其他账号名";
                }
            }
        }
    }
    public void RegisterPanel() {
        register_panel.SetActive(true);
        login_panel.SetActive(false);
    }
    public void LoginPanel() {
        register_panel.SetActive(false);
        register_success_panel.SetActive(false);
        login_panel.SetActive(true);
    }


    public bool CheckLoginAccount() {
        if (login_account_input_field.text == "") {
            login_account_tip_text.text = "请输入账号";
            return false;
        }
        login_account_tip_text.text = "";
        return true;
    }
    public void EndEditLoginAccount() {
        CheckLoginAccount();
    }
    public bool CheckLoginPassword() {
        if (login_password_input_field.text == "") {
            login_password_tip_text.text = "请输入密码";
            return false;
        }
        login_password_tip_text.text = "";
        return true;
    }
    public void EndEditLoginPassword() {
        CheckLoginPassword();
    }
    public bool CheckRegisterAccount() {
        if (register_account_input_field.text == "") {
            register_account_tip_text.text = "请输入账号";
            return false;
        }
        register_account_tip_text.text = "";
        return true;
    }
    public void EndEditRegisterAccount() {
        CheckRegisterAccount();
    }
    public bool CheckRegisterPassword() {
        if (register_password_input_field.text == "") {
            register_password_tip_text.text = "请输入密码";
            return false;
        }
        if (register_password_input_field.text.Length < 6) {
            register_password_tip_text.text = "密码至少6位以上";
            return false;
        }
        register_password_tip_text.text = "";
        return true;
    }
    public void EndEditRegisterPassword() {
        if (CheckRegisterPassword()) {
            register_confirm_password_input_field.text = "";
        }
    }
    public bool CheckRegisterConfirmPassword() {
        if (register_confirm_password_input_field.text == "") {
            register_confirm_password_tip_text.text = "请重复你的密码";
            return false;
        }
        if (register_confirm_password_input_field.text != register_password_input_field.text) {
            register_confirm_password_tip_text.text = "请确保两次密码输入一致";
            return false;
        }
        register_confirm_password_tip_text.text = "";
        return true;
    }
    public void EndEditRegisterConfirmPassword() {
        CheckRegisterConfirmPassword();
    }
}