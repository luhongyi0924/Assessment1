using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MsgDetailPanel : BasePanel
{
    //问题标题文本
    private Text titleText;
    //问题内容文本
    private Text contentText;
    //关闭按钮
    private Button closeBtn;
    //收藏按钮
    private Button collectBtn;
    //点赞按钮
    private Button goodBtn;
    //回复输入框
    private InputField replyIF;
    //发送按钮
    private Button sendBtn;
    //当前Panel所对应的问题信息
    private QueData currentQue ;                        //null
    //当前Panel要取到所有回复
    public List<ReplyData> allReplyInServer;

    public override void OnInit()
    {
        prefabPath = "Panels/MsgDetailPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        currentQue = para[0] as QueData;

        //当前问题的所有回复
        allReplyInServer = DBManager.GetAllRepliesByQueID(currentQue.id);

        //文本组件
        titleText = prefabIns.transform.Find("TitleText").GetComponent<Text>();
        contentText=prefabIns.transform.Find("ContentText").GetComponent<Text>();

        string title = currentQue.title;
        if (title.Length <= 8)
            titleText.text = title;
        else
            titleText.text = title.Substring(0, 8) + "......";

        contentText.text = currentQue.content;

        //输入框组件
        replyIF = prefabIns.transform.Find("Bottom/ReplyIF").GetComponent<InputField>();

        //按钮组件
        closeBtn = prefabIns.transform.Find("CloseBtn").GetComponent<Button>();
        collectBtn = prefabIns.transform.Find("CollectBtn").GetComponent<Button>();
        goodBtn = prefabIns.transform.Find("GoodBtn").GetComponent<Button>();
        sendBtn = prefabIns.transform.Find("Bottom/SendBtn").GetComponent<Button>();

        //监听
        closeBtn.onClick.AddListener(OnCloseBtnClick);
        collectBtn.onClick.AddListener(OnCollectBtnClick);
        goodBtn.onClick.AddListener(OnGoodBtnClick);
        sendBtn.onClick.AddListener(OnSendBtnClick);

        //Content部分的Toggler列表
        foreach (var replyItem in allReplyInServer)
        {
            MsgTogglerHelper.AddToggler<SmallMsgToggler>
                (replyItem.content, replyItem.goodcount, replyItem.id, replyItem.user_id, replyItem.question_id);
        }
    }

    private void OnCloseBtnClick()
    {
        Close();
    }

    private void OnCollectBtnClick()
    {
        DBManager.SendCol(currentQue.id);
        PanelManager.Open<TipPanel>("收藏成功！");
    }

    private void OnGoodBtnClick()
    {
        //DBManager.SendGoodReply();
    }

    private void OnSendBtnClick()
    {
        string reply_id = DBManager.SendReply(replyIF.text, currentQue.id).id;
        MsgTogglerHelper.AddToggler<SmallMsgToggler>(replyIF.text, "0", reply_id);
    }
}
