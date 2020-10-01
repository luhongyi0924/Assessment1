using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    //搜索输入框
    private InputField searchIF;
    //顶部“搜索”按钮
    private Button searchBtn;
    //顶部“我的问题”按钮
    private Button myQueBtn;
    //顶部“我的收藏”按钮
    private Button myColBtn;
    //底部“回顶部”按钮
    private Button goTopBtn;
    //底部“查看”按钮
    private Button viewBtn;
    //底部“去底部”按钮
    private Button goBottomBtn;
    //当前Panel要取到所有问题
    public List<QueData> allQuesInServer;
    //当前Panel检索的关键字Toggler列表
    public List<QueData> allKeyQuesInServer;

    public override void OnInit()
    {
        prefabPath = "Panels/MainPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        allQuesInServer = DBManager.GetAllQues();
        DBManager.GetAllCols();

        //顶部昵称文本
        accountText = prefabIns.transform.Find("Top/AccountText").GetComponent<Text>();
        accountText.text = DBManager.currentUser.nickName;

        //输入框组件
        searchIF = prefabIns.transform.Find("Top/SearchIF").GetComponent<InputField>();

        //按钮组件
        searchBtn = prefabIns.transform.Find("Top/SearchBtn").GetComponent<Button>();
        myQueBtn = prefabIns.transform.Find("Top/MyQueBtn").GetComponent<Button>();
        myColBtn = prefabIns.transform.Find("Top/MyColBtn").GetComponent<Button>();
        goTopBtn = prefabIns.transform.Find("Bottom/GoTopBtn").GetComponent<Button>();
        goBottomBtn = prefabIns.transform.Find("Bottom/GoBottomBtn").GetComponent<Button>();
        viewBtn = prefabIns.transform.Find("Bottom/ViewBtn").GetComponent<Button>();
        cancellationBtn = prefabIns.transform.Find("Bottom/CancellationBtn").GetComponent<Button>();

        //监听
        searchBtn.onClick.AddListener(OnSearchBtnBtnClick);
        myQueBtn.onClick.AddListener(OnMyQueBtnClick);
        myColBtn.onClick.AddListener(OnMyColBtnClick);
        goTopBtn.onClick.AddListener(OnGoTopBtnClick);
        goBottomBtn.onClick.AddListener(OnGoBottomBtnClick);
        viewBtn.onClick.AddListener(OnViewBtnClick);
        cancellationBtn.onClick.AddListener(OnCancellationBtnClick);

        //Content部分的Toggler列表
        foreach (var queItem in allQuesInServer)
        {
            MsgTogglerHelper.AddToggler<BigMsgToggler>(queItem.id, queItem.title,para[0]);
        }
    }

    private void OnCancellationBtnClick()
    {
        PanelManager.Open<LoginPanel>();
        DBManager.currentUser = null;
        Close();
    }

    //按下查看按钮
    private void OnViewBtnClick()
    {
        //PanelManager.Open<MsgDetailPanel>();  
    }

    private void OnGoBottomBtnClick()
    {
        throw new NotImplementedException();
    }

    private void OnGoTopBtnClick()
    {
        throw new NotImplementedException();
    }

    private void OnMyColBtnClick()
    {
        string contentPath =
            "Canvas/Panel/MyColPanel(Clone)/FormPanel/Scroll View/Viewport/Content";
        PanelManager.Open<MyColPanel>(contentPath);
        Close();
    }

    private void OnMyQueBtnClick()
    {
        string contentPath =
            "Canvas/Panel/MyQuePanel(Clone)/FormPanel/Scroll View/Viewport/Content";
        PanelManager.Open<MyQuePanel>(contentPath);
        Close();
    }

    //当按下关键词搜索按钮
    private void OnSearchBtnBtnClick()
    {
        MsgTogglerHelper.RemoveAllToggler();

        allKeyQuesInServer = DBManager.GetAllKeyQues(searchIF.text);

        //Content部分的Toggler列表
        foreach (var queItem in allKeyQuesInServer)
        {
            MsgTogglerHelper.AddToggler<BigMsgToggler>(queItem.id, queItem.title);
        }
    }
}