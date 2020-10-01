using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MyColPanel : BasePanel
{
    //搜索输入框
    private InputField searchIF;
    //顶部“搜索”按钮
    private Button searchBtn;
    //顶部“回到首页”按钮
    private Button returnBtn;
    //底部“回顶部”按钮
    private Button goTopBtn;
    //底部“查看”按钮
    private Button viewBtn;
    //底部“去底部”按钮
    private Button goBottomBtn;
    //当前Panel要取到所有问题
    public static List<QueData> allMyColsInServer;
    //当前Panel检索的关键字Toggler列表
    public List<QueData> allKeyQuesInServer;

    public override void OnInit()
    {
        prefabPath = "Panels/MyColPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        DBManager.GetAllCols();
        //获取所收藏的问题的问题数据
        allMyColsInServer = DBManager.GetAllColQues();

        //顶部昵称文本
        accountText = prefabIns.transform.Find("Top/AccountText").GetComponent<Text>();
        accountText.text = DBManager.currentUser.nickName;

        //输入框组件
        searchIF = prefabIns.transform.Find("Top/SearchIF").GetComponent<InputField>();

        //按钮组件
        searchBtn = prefabIns.transform.Find("Top/SearchBtn").GetComponent<Button>();
        returnBtn = prefabIns.transform.Find("Top/ReturnBtn").GetComponent<Button>();
        goTopBtn = prefabIns.transform.Find("Bottom/GoTopBtn").GetComponent<Button>();
        goBottomBtn = prefabIns.transform.Find("Bottom/GoBottomBtn").GetComponent<Button>();
        viewBtn = prefabIns.transform.Find("Bottom/ViewBtn").GetComponent<Button>();
        cancellationBtn = prefabIns.transform.Find("Bottom/CancellationBtn").GetComponent<Button>();

        //监听
        searchBtn.onClick.AddListener(OnSearchBtnBtnClick);
        returnBtn.onClick.AddListener(OnReturnBtnClick);
        goTopBtn.onClick.AddListener(OnGoTopBtnClick);
        goBottomBtn.onClick.AddListener(OnGoBottomBtnClick);
        viewBtn.onClick.AddListener(OnViewBtnClick);
        cancellationBtn.onClick.AddListener(OnCancellationBtnClick);

        //Content部分的Toggler列表
        foreach (var queItem in allMyColsInServer)
        {
            MsgTogglerHelper.AddToggler<BigMsgToggler>(queItem.id, queItem.title,para[0]);
        }
    }

    //注销按钮
    private void OnCancellationBtnClick()
    {
        PanelManager.Open<LoginPanel>();
        DBManager.currentUser = null;
        Close();
    }

    //当按下关键词搜索按钮
    private void OnSearchBtnBtnClick()
    {
        allKeyQuesInServer = DBManager.GetAllKeyQues(searchIF.text);

        MsgTogglerHelper.RemoveAllToggler();

        //Content部分的Toggler列表
        foreach (var queItem in allKeyQuesInServer)
        {
            MsgTogglerHelper.AddToggler<BigMsgToggler>(queItem.id, queItem.title);
        }
    }

    private void OnReturnBtnClick()
    {
        string contentPath = "Canvas/Panel/MainPanel(Clone)/FormPanel/Scroll View/Viewport/Content";
        PanelManager.Open<MainPanel>(contentPath);
        Close();
    }

    private void OnGoTopBtnClick()
    {
        throw new NotImplementedException();
    }

    private void OnGoBottomBtnClick()
    {
        throw new NotImplementedException();
    }

    private void OnViewBtnClick()
    {
        //PanelManager.Open<MsgDetailPanel>();
    }
}
