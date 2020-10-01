using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistPanel : BasePanel
{
    //电子邮箱账号输入框
    private InputField registEmailIF;
    //密码输入框
    private InputField passwordIF;
    //重复密码输入框
    private InputField rePasswordIF;
    //昵称输入框
    private InputField nicknameIF;
    //验证码输入框
    private InputField verificationCodeIF;
    //注册按钮
    private Button registBtn;
    //重置按钮
    private Button resetBtn;
    //登陆按钮
    private Button loginBtn;
    //刷新验证码按钮
    private Button refreshBtn;
    //生成的验证码
    private string verificationCode;
    //获取Panel中的VerificationCodeImage组件
    private Image verificationCodeImage;

    //初始化
    public override void OnInit()
    {
        prefabPath = "Panels/RegistPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args)
    {
        verificationCodeImage =
            GameObject.Find("VerificationCodeImage").GetComponent<Image>();
        //刷新验证码
        OnRefreshBtnClick();

        //输入框组件
        registEmailIF = prefabIns.transform.Find("RegistEmailIF").GetComponent<InputField>();
        passwordIF = prefabIns.transform.Find("PasswordIF").GetComponent<InputField>();
        rePasswordIF = prefabIns.transform.Find("RePasswordIF").GetComponent<InputField>();
        verificationCodeIF = prefabIns.transform.Find("VerificationCodeIF").GetComponent<InputField>();
        nicknameIF = prefabIns.transform.Find("NicknameIF").GetComponent<InputField>();

        //按钮组件
        registBtn = prefabIns.transform.Find("RegistBtn").GetComponent<Button>();
        resetBtn = prefabIns.transform.Find("ResetBtn").GetComponent<Button>();
        loginBtn = prefabIns.transform.Find("LoginBtn").GetComponent<Button>();
        refreshBtn = prefabIns.transform.Find("RefreshBtn").GetComponent<Button>();

        //监听
        registBtn.onClick.AddListener(OnRegistBtnClick);
        resetBtn.onClick.AddListener(OnResetBtnClick);
        loginBtn.onClick.AddListener(OnLoginBtnClick);
        refreshBtn.onClick.AddListener(OnRefreshBtnClick);
    }

    private void OnResetBtnClick()
    {
        registEmailIF.text = null;
        passwordIF.text = null;
        rePasswordIF.text = null;
        verificationCodeIF.text = null;
        nicknameIF.text = null;
    }

    //当按下登录按钮
    private void OnLoginBtnClick()
    {
        PanelManager.Open<LoginPanel>();
        verificationCodeImage =
            GameObject.Find("VerificationCodeImage").GetComponent<Image>();
        Close();
    }

    //当按下注册按钮
    public void OnRegistBtnClick()
    {
        //用户名密码为空
        if (registEmailIF.text == "" || passwordIF.text == "" || nicknameIF.text == "")
        {
            PanelManager.Open<TipPanel>("用户名、密码或昵称不能为空！");
            //刷新封装验证码
            OnRefreshBtnClick();
        }
        else if (passwordIF.text != rePasswordIF.text)
        {
            PanelManager.Open<TipPanel>
                ("两次输入的密码不一致哦！请重新输入");
            //刷新封装验证码
            OnRefreshBtnClick();
        }
        else if ((!verificationCodeIF.text.ToUpper().Equals(verificationCode.ToUpper())) || verificationCodeIF.text == null)
        {
            PanelManager.Open<TipPanel>("验证码错误或不能为空！");
            //刷新封装验证码
            OnRefreshBtnClick();
        }
        else if (DBManager.Regist(registEmailIF.text, passwordIF.text, nicknameIF.text))
        {
            PanelManager.Open<TipPanel>
                ("恭喜你，已成功注册！点击登录按钮返回登录界面登录吧！");
        }
    }

    //当按下刷新验证码的按钮
    private void OnRefreshBtnClick()
    {
        verificationCodeImage =
            GameObject.Find("VerificationCodeImage").GetComponent<Image>();

        GetValidateCode(4);
        GetValidateGraphic(verificationCode);
    }

    //关闭
    public override void OnClose()
    {
    }

    private void GetValidateCode(int length)
    {
        verificationCode = VerificationCodeHelper.CreateRandomCode(length);
    }

    private void GetValidateGraphic(string verificationCode)
    {
        Texture2D t = new Texture2D(8, 8);
        t.LoadImage(VerificationCodeHelper.CreateValidateGraphic(verificationCode));
        Sprite sprite = Sprite.Create(t, new Rect(0, 0, 64, 27), Vector2.zero);
        verificationCodeImage.sprite = sprite;
    }
}
