using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MsgTogglerHelper : MonoBehaviour
{
    //结构
    public static Transform canvas = PanelManager.canvas;
    //Toggler列表
    public static Dictionary<string, MsgTogglerBase> togglers =
        new Dictionary<string, MsgTogglerBase>();

    //列出一条Toggler
    public static void AddToggler<T>(params object[] para) where T : MsgTogglerBase
    {
        GameObject contentObj;

        if (typeof(T).ToString() == "SmallMsgToggler")
            contentObj = GameObject.FindGameObjectWithTag("SmallMsgTogglerContent");
        else
            contentObj = GameObject.Find(para[2].ToString());

        //将预制体加载到内存，为实例化到场景中做准备
        GameObject memoryPfb = ResManager.LoadPrefab("Panels/" + typeof(T).ToString());
        GameObject prefabIns = Instantiate(memoryPfb);

        //设置Content为父容器
        prefabIns.transform.SetParent(contentObj.transform);

        MsgTogglerBase mtb = prefabIns.AddComponent<T>();

        //string queID;

        //if (typeof(T).ToString() == "SmallMsgToggler")
        //    queID = para[2].ToString();
        //else
        //    queID = para[0].ToString();

        mtb.OnInit();
        mtb.OnShow(para);

        //togglers.Add(queID, mtb);
    }

    //移除一条Toggler
    public static void RemoveToggler(string queID)
    {
        //如果这条问题不存在
        if (!togglers.ContainsKey(queID))
        {
            return;
        }

        MsgTogglerBase toggler = togglers[queID];

        toggler.OnClose();

        //字典
        togglers.Remove(queID);
        //销毁
        Destroy(toggler);
    }

    //移除当前Content所有Toggler
    public static void RemoveAllToggler()
    {
        GameObject content = GameObject.Find("Content");

        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
    }
}

public class MsgTogglerBase : MonoBehaviour
{
    public void Init()
    {
    }

    //关闭
    public void Close()
    {
        string name = this.GetType().ToString();
        PanelManager.Close(name);
    }

    //初始化时
    public virtual void OnInit()
    {
    }

    //显示时
    public virtual void OnShow()
    {
    }
    //有参重载
    public virtual void OnShow(params object[] para)
    {
    }

    //关闭时
    public virtual void OnClose()
    {
        Close();
    }
}

//问题消息条类
public class BigMsgToggler : MsgTogglerBase
{
    public Button button;
    //问题序号
    private Text numText;
    //问题内容
    private Text msgText;
    //当前问题
    private QueData qd;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();
        numText = transform.Find("NumText").GetComponent<Text>();
        msgText = transform.Find("MsgText").GetComponent<Text>();
        button.onClick.AddListener(OnButtonDown);
    }

    ////初始化时
    //public override void OnInit()
    //{
    //    numText = GameObject.Find("NumText").GetComponent<Text>();
    //    msgText = GameObject.Find("MsgText").GetComponent<Text>();
    //}

    public override void OnShow(params object[] para)
    {
        //文本组件
        numText.text = para[0].ToString();
        msgText.text = para[1].ToString();
        qd = DBManager.GetQueDataByQueID(numText.text);
    }

    private void OnButtonDown()
    {
        PanelManager.Open<MsgDetailPanel>(qd);
    }
}

//回复点赞条类
public class SmallMsgToggler : MsgTogglerBase
{
    public Button button1;
    //回复内容
    private Text replyText;
    //点赞次数
    private Text goodText;
    //当前回复数据
    private ReplyData rd;

    private void Awake()
    {
        button1 = GetComponentInChildren<Button>();
        replyText = transform.Find("ReplyText").GetComponent<Text>();
        goodText = transform.Find("GoodText").GetComponent<Text>();
        button1.onClick.AddListener(OnButtonDown);
    }

    ////初始化时
    //public override void OnInit()
    //{
    //    replyText = GameObject.Find("ReplyText").GetComponent<Text>();
    //    goodText = GameObject.Find("GoodText").GetComponent<Text>();
    //}

    public override void OnShow(params object[] para)
    {
        rd = DBManager.GetReplyByID(para[2].ToString());
        replyText.text = rd.content;
        goodText.text = rd.goodcount;
    }

    private void OnButtonDown()
    {
        DBManager.SendGoodReply(rd.id);
        goodText.text = (int.Parse(goodText.text) + 1).ToString();
    }
}