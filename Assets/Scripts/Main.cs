using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        if (!DBManager.Connect
            ("qasysdb", "127.0.0.1", 3306, "root", "root"))
        {
            return;
        }

        PanelManager.Init();
        PanelManager.Open<LoginPanel>();
    }
}