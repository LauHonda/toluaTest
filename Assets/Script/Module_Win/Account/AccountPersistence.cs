using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AccountPersistence
{
    //保存账号信息
    public void Write()
    {
        if (Tog.isOn)
        {
            ES2.Save(inputField.text, PersistenceName);
        }
    }

    //读取账号信息
    public void Read()
    {
        if (ES2.Exists(PersistenceName))
        {
            inputField.text = ES2.Load<string>(PersistenceName);
            Tog.isOn = true;
        }
    }

    public Toggle Tog;
    public InputField inputField;
    public string PersistenceName;

    /// <summary>
    /// 初始化存储信息
    /// </summary>
    /// <param name="Tog">控制是否保存的Toggle组件</param>
    /// <param name="inputField">InputField</param>
    /// <param name="PersistenceName">持久化保存的名称</param>
    public AccountPersistence(Toggle Tog, InputField inputField, string PersistenceName)
    {
        this.Tog = Tog;
        this.inputField = inputField;
        this.PersistenceName = PersistenceName;
        Tog.onValueChanged.AddListener(delegate (bool IsOn)
        {
            
            if (!IsOn && ES2.Exists(PersistenceName))
            {
                ES2.Delete(PersistenceName);
            }
        });
    }
}
