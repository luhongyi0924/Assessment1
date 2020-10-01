using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    //提示文本
    private Text text;
    //关闭按钮
    private Button closeBtn;

    //初始化
    public override void OnInit()
    {
        prefabPath = "Panels/TipPanel";
        layer = PanelManager.Layer.Tip;
    }
    //显示
    public override void OnShow(params object[] args)
    {
        //按钮组件
        text = prefabIns.transform.Find("ShowText").GetComponent<Text>();
        closeBtn = prefabIns.transform.Find("CloseBtn").GetComponent<Button>();
        //监听
        closeBtn.onClick.AddListener(OnCloseBtnClick);
        //提示语
        if (args.Length == 1)
        {
            text.text = (string)args[0];
        }
    }

    //关闭
    public override void OnClose()
    {

    }

    //当按下退出按钮
    public void OnCloseBtnClick()
    {
        Close();
    }
}