using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NewProPanel : BasePanel
{
    //问题输入框
    private InputField questionIF;
    //具体描述输入框
    private InputField contentIF;
    //发布按钮
    private Button releaseBtn;
    //重置按钮
    private Button resetBtn;
    //关闭按钮
    private Button closeBtn;

    public override void OnInit()
    {
        prefabPath = "Panels/NewProPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow()
    {
        //输入框组件
        questionIF = prefabIns.transform.Find("QuestionIF").GetComponent<InputField>();
        contentIF = prefabIns.transform.Find("ContentIF").GetComponent<InputField>();

        //按钮组件
        releaseBtn = prefabIns.transform.Find("ReleaseBtn").GetComponent<Button>();
        resetBtn = prefabIns.transform.Find("ResetBtn").GetComponent<Button>();
        closeBtn = prefabIns.transform.Find("CloseBtn").GetComponent<Button>();

        //监听
        releaseBtn.onClick.AddListener(OnReleaseBtnClick);
        resetBtn.onClick.AddListener(OnResetBtnClick);
        closeBtn.onClick.AddListener(OnCloseBtnClick);
    }

    private void OnReleaseBtnClick()
    {
        //问题框和说明框是空的
        if (questionIF.text == "" || contentIF.text == "")
        {
            PanelManager.Open<TipPanel>("问题框或内容框不能为空！");
            return;
        }
        else if (DBManager.SendQue(questionIF.text, contentIF.text))
        {
            PanelManager.Open<TipPanel>("发表成功！");
        }
    }

    private void OnResetBtnClick()
    {
        questionIF.text = null;
        contentIF.text = null;
    }

    private void OnCloseBtnClick()
    {
        Close();
    }
}