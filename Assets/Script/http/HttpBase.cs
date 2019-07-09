using System.Collections;
using System.Collections.Generic;
using UltimateDH;
using UnityEngine;

public abstract class HttpBase : MonoBehaviour
{
    protected DicBase<Http> EntityHttp;

    protected DicBase<HttpBase> EntityHttpModel;

    protected Dic<string, Transform> EntityWin;

    [HideInInspector]
    protected TransformData Win;

    private void Awake()
    {
        if (GameManagerHttp.GetGameManager)
            GameManagerHttp.GetGameManager.UpdateWin.AddListener(delegate (Dic<string, Transform> winbody)
            {
                if (WinID() != null)
                    Win = winbody.Get(WinID()).GetComponent<TransformData>().Init();
                EntityWin = winbody;
            });
    }

    protected virtual void Start()
    {
        EntityHttp = MessageManager.GetMessageManager.Http;
        EntityHttpModel = MessageManager.GetMessageManager.HttpModel;
        EntityHttpModel.Add(this.GetType().ToString(), this);
        Init();
    }

    protected abstract void Init();

    protected virtual string WinID()
    {
        return null;
    }

    protected Transform GetWinTransrorm(string key)
    {
        return EntityWin.Get(key);
    }

    protected TransformData GetWinTransrormData(string key)
    {
        Transform win = EntityWin.Get(key);
        return win == null ? null : win.GetComponent<TransformData>();
    }

    protected void OpenWin(Transform win)
    {
        WindowsManager.GetWindowsManager.OpenWindow(win);
    }

    protected void CloseWin(Transform win)
    {
        WindowsManager.GetWindowsManager.CloseWindow(win);
    }
}
