using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using LitJson;
using Utils;
using UnityEngine.SceneManagement;
using UltimateDH;
using Tools;
public class GameManagerhttp : MonoBehaviour
{
    public static GameManagerhttp GetGameManager;

    public void Awake()
    {
        GetGameManager = this;
        //清除对象池缓存
        ObjectPool.GetInstance().ClearAll();
        ListCreatTools.Clear();   
    }

    private void Start()
    {
        WinParent = GameObject.Find("CanvasTop").GetComponent<TransformData>();
        UpdateWin.Send(WinParent.Init());
        DontDestroyOnLoad(WinParent.gameObject);
        //StartCoroutine("TimeDoAction");

    }

    private void Update()
    {
        Utils.Timer.Instance.DoUpdate();
    }

    IEnumerator TimeDoAction()
    {
        while (true)
        {
            Utils.Timer.Instance.DoUpdate();
            yield return new WaitForSeconds(1);
        }
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


    public string FriendID = null;

    [SerializeField]
    public TransformData WinParent;
    public Dic<string, Transform> WinTransfrom = new Dic<string, Transform>();

    public Message<TransformData> UpdateWin = new Message<TransformData>();

}

