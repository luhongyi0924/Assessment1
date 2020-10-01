using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public static class PanelManager
{
    public enum Layer
    {
        Panel,
        Tip,
    }

    //层级列表
    private static Dictionary<Layer, Transform> layers = new Dictionary<Layer, Transform>();
    //面板列表
    public static Dictionary<string, BasePanel> panels = new Dictionary<string, BasePanel>();
    //结构
    public static Transform canvas;
    //初始化
    public static void Init()
    {
        canvas = GameObject.Find("Canvas").transform;
        Transform panel = canvas.Find("Panel");
        Transform tip = canvas.Find("Tip");
        layers.Add(Layer.Panel, panel);
        layers.Add(Layer.Tip, tip);
    }

    //打开面板
    public static void Open<T>(params object[] para) where T : BasePanel
    {
        string name = typeof(T).ToString();
        //已经打开
        if (panels.ContainsKey(name))
        {
            return;
        }
        //组件
        BasePanel panel = canvas.gameObject.AddComponent<T>();
        panel.OnInit();
        panel.Init();
        //父容器
        Transform layer = layers[panel.layer];
        panel.prefabIns.transform.SetParent(layer, false);
        //列表
        panels.Add(name, panel);
        //OnShow
        panel.OnShow();
        panel.OnShow(para);
    }

    //关闭面板
    public static void Close(string name)
    {
        //没有打开
        if (!panels.ContainsKey(name))
        {
            return;
        }
        BasePanel panel = panels[name];

        //OnClose
        panel.OnClose();
        //列表
        panels.Remove(name);
        //销毁
        Object.Destroy(panel.prefabIns);
        Object.Destroy(panel);
    }
}