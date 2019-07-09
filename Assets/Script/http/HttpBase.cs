using System.Collections;
using System.Collections.Generic;
using UltimateDH;
using UnityEngine;

public abstract class HttpBase : MonoBehaviour
{

    protected DicBase<Http> EntityHttp;

    protected DicBase<HttpBase> EntityHttpModel;

    protected TransformData EntityWin;

    [HideInInspector]
    protected TransformData Win;

    private void Awake()
    {
        if (GameManagerhttp.GetGameManager)
            GameManagerhttp.GetGameManager.UpdateWin.AddListener(delegate (TransformData winbody)
            {
                if (WinID() != null)
                    Win = winbody.Get<TransformData>(WinID()).Init();
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

    /// <summary>
    ///获取窗口组件 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    protected Transform GetWinTransrorm(string key)
    {
        return EntityWin.GetTransform(key);
    }

    /// <summary>
    /// 获取UI组件
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    protected TransformData GetWinTransrormData(string key)
    {
        TransformData win = EntityWin.Get<TransformData>(key);
        return win == null ? null : win;
    }

    /// <summary>
    /// 打开窗口
    /// </summary>
    /// <param name="win"></param>
    protected void OpenWin(Transform win)
    {
        WindowsManager.GetWindowsManager.OpenWindow(win);
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    /// <param name="win"></param>
    protected void CloseWin(Transform win)
    {
        WindowsManager.GetWindowsManager.CloseWindow(win);
    }

}
