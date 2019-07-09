using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using LitJson;
using Utils;
using UnityEngine.SceneManagement;
using UltimateDH;

public class GameManagerHttp : MonoBehaviour
{
    public static GameManagerHttp GetGameManager;

    public void Awake()
    {
        GetGameManager = this;
        //清除对象池缓存
        ObjectPool.GetInstance().ClearAll();

        foreach (Transform child in WinParent)
        {
            WinTransfrom.Add(child.gameObject.name, child);
        }    
    }

    private void Start()
    {
        UpdateWin.Send(WinTransfrom);
    }

    public void Quite()
    {
        //清除数据池数据
        DataPool.GetInstance().ClearAll();
        //加载登录场景
        SceneManager.LoadScene("mainmeun");
    }
    public void QuiteAll()
    {
        //清除数据池数据
        DataPool.GetInstance().ClearAll();
        //加载登录场景
        Application.Quit();
    }

    [SerializeField]
    Transform WinParent;
    public Dic<string, Transform> WinTransfrom = new Dic<string, Transform>();

    public Message<Dic<string, Transform>> UpdateWin = new Message<Dic<string, Transform>>();

}

