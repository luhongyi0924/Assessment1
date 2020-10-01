using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    //电子邮箱账号输入框
    private InputField emailAccountIF;
    //密码输入框
    private InputField passwordIF;
    //验证码输入框
    private InputField verificationCodeIF;
    //登陆按钮
    private Button loginBtn;
    //注册按钮
    private Button registBtn;
    //刷新验证码按钮
    private Button refreshBtn;
    //生成的验证码
    private string verificationCode;
    //获取Panel中的VerificationCodeImage组件
    private Image verificationCodeImage;

    public override void OnInit()
    {
        prefabPath = "Panels/LoginPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args)
    {
        ////刷新验证码
        //verificationCodeImage =
        //    GameObject.Find("VerificationCodeImage").GetComponent<Image>();
        //OnRefreshBtnClick();

        //输入框组件
        emailAccountIF = prefabIns.transform.Find("EmailAccountIF").GetComponent<InputField>();
        passwordIF = prefabIns.transform.Find("PasswordIF").GetComponent<InputField>();
        verificationCodeIF = prefabIns.transform.Find("VerificationCodeIF").GetComponent<InputField>();
        
        //按钮组件
        loginBtn = prefabIns.transform.Find("LoginBtn").GetComponent<Button>();
        registBtn = prefabIns.transform.Find("RegistBtn").GetComponent<Button>();
        refreshBtn = prefabIns.transform.Find("RefreshBtn").GetComponent<Button>();

        //监听
        loginBtn.onClick.AddListener(OnLoginBtnClick);
        registBtn.onClick.AddListener(OnRegistBtnClick);
        refreshBtn.onClick.AddListener(OnRefreshBtnClick);
    }

    //当按下登录按钮
    private void OnLoginBtnClick()
    {
        //用户名密码为空
        if (emailAccountIF.text == "" || passwordIF.text == "")
        {
            PanelManager.Open<TipPanel>("用户名或密码不能为空！");
            //刷新封装验证码
            OnRefreshBtnClick();
        }
        //else if ((!verificationCodeIF.text.ToUpper().Equals(verificationCode.ToUpper()))|| verificationCodeIF.text==null)
        //{
        //    PanelManager.Open<TipPanel>("验证码错误或不能为空！");
        //    //刷新封装验证码
        //    OnRefreshBtnClick();
        //}
        else if (DBManager.CheckPassword(emailAccountIF.text, passwordIF.text))
        {
            DBManager.GetUserData(emailAccountIF.text);
            PanelManager.Open<TipPanel>("登录成功！");

            string contentPath ="Canvas/Panel/MainPanel(Clone)/FormPanel/Scroll View/Viewport/Content";
            PanelManager.Open<MainPanel>(contentPath);

            DBManager.GetAllCols();
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("用户名或密码输入不正确！");
            //刷新封装验证码
            OnRefreshBtnClick();
        }
    }

    //当按下注册按钮
    public void OnRegistBtnClick()
    {
        PanelManager.Open<RegistPanel>();
        verificationCodeImage =
            GameObject.Find("VerificationCodeImage").GetComponent<Image>();
        Close();
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