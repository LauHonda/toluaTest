using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowsManager : MonoBehaviour
{

    public static WindowsManager GetWindowsManager;

    private void Awake()
    {
        GetWindowsManager = this;
    }

    /// <summary>
    /// 打开窗口
    /// </summary>
    /// <param name="key">窗口组件</param>
    /// <param name="WinType">打开方式</param>
    /// <param name="IsNoMrak">是否显示背景遮罩</param>
    public void OpenWindow(Transform key, SizeType WinType = SizeType.FullSize, bool IsNoMrak = false)
    {
        winNum++;
        OpenByDotween(key.gameObject, WinType, IsNoMrak);
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    /// <param name="key">窗口组件</param>
    /// <param name="WinType">打开方式</param>
    public void CloseWindow(Transform key, SizeType WinType = SizeType.FullSize)
    {
        winNum--;
        CloseByDotween(key.gameObject, WinType);
    }

    public int winNum = 0;

    public bool CanOpen()
    {
        return winNum <= 0;
    }

    private bool _lock;

    public void OpenByDotween(GameObject go, SizeType Ani_Type, bool IsMrak = false)
    {
        if (_lock)
            return;
        _lock = true;


        switch (Ani_Type)
        {
            case SizeType.FullSize:
                go.transform.localScale = Vector3.one;
                go.SetActive(true);
                if (!IsMrak)
                    go.GetComponent<Image>().enabled = true;
                _lock = false;
                break;
            case SizeType.Scale:
                go.SetActive(true);
                go.GetComponent<Image>().enabled = false;
                go.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .15f).OnComplete(delegate ()
                {

                    go.transform.DOScale(Vector3.one, .15f).OnComplete(delegate ()
                    {
                        go.GetComponent<Image>().enabled = true;
                        _lock = false;
                    });
                });
                break;
            case SizeType.FilledByVerticalAndTop:
                go.transform.localScale = Vector3.one;
                go.SetActive(true);
                go.GetComponent<Image>().enabled = true;
                go.transform.Find("board").GetComponent<Image>().DOFillAmount(1, .35f);
                _lock = false;
                break;
        }
    }

    public void CloseByDotween(GameObject go, SizeType Ani_Type)
    {
        go.GetComponent<Image>().enabled = false;
        switch (Ani_Type)
        {
            case SizeType.FullSize:
                go.SetActive(false);
                break;
            case SizeType.Scale:
                go.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), .15f).OnComplete(delegate ()
                {
                    go.transform.DOScale(Vector3.zero, .15f).OnComplete(delegate ()
                    {
                        go.SetActive(false);
                    });
                });
                break;
            case SizeType.FilledByVerticalAndTop:
                go.transform.Find("board").GetComponent<Image>().fillAmount = 0;
                go.SetActive(false);
                break;
        }
    }
}
