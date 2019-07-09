using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ButtonDt
{
    public Color normal_clolor = Color.white, selen_color = Color.white;
    public Sprite normal, selen;
    public Button btn;
    public GameObject Obj;

}

public class ButtonChangeGroup : MonoBehaviour
{

    [SerializeField]
    private List<ButtonDt> all = new List<ButtonDt>();

    public void Start()
    {
        foreach (ButtonDt child in all)
        {
            child.btn.onClick.AddListener(delegate ()
           {
               SetState(all.IndexOf(child));
           });
        }
        SetFist();
    }


    //初始化状态
    public void SetFist()
    {
        SetState(0);
    }

    public Button GetButton(int num)
    {
        return all[num].btn;
    }


    //设置状态
    public void SetState(int index)
    {
        foreach (ButtonDt item in all)
        {
            if (item.Obj)
                item.Obj.SetActive(false);
            if (item.normal)
                item.btn.image.sprite = item.normal;
            item.btn.image.color = item.normal_clolor;
        }

        if ((all[index].Obj))
            all[index].Obj.SetActive(true);
        if (all[index].selen)
            all[index].btn.image.sprite = all[index].selen;
        all[index].btn.image.color = all[index].selen_color;
    }
}
