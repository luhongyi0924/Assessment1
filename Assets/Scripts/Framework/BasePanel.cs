using UnityEngine;
using System.Collections;
using System.IO;
using System.Drawing;
using UnityEngine.UI;
using System.Drawing.Imaging;
using Image = UnityEngine.UI.Image;

public class BasePanel : MonoBehaviour
{
    //预制体路径
    public string prefabPath;
    //Panel中预制体的实例
    public GameObject prefabIns;
    //获取Panel中的注销按钮
    protected static Button cancellationBtn;
    //获取顶部的当前登录账号
    protected Text accountText;
    //被选中的Toggler
    public static Button chooseBtn;
    //当前界面最后一次点击的Toggler对象
    public static Button clickTogglerObj;                      //null
    //层级
    public PanelManager.Layer layer = PanelManager.Layer.Panel;

    //初始化
    public void Init()
    {
        //将预制体加载到内存，为实例化到场景中做准备
        GameObject memoryPfb = ResManager.LoadPrefab(prefabPath);
        prefabIns = Instantiate(memoryPfb);
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
    //带参重载
    public virtual void OnShow(params object[] para)
    {
    }

    //关闭时
    public virtual void OnClose()
    {
    }
}