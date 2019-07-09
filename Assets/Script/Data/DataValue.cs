using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TypeClass;
using System;

namespace TypeClass
{
    public enum ReceiveType
    {
        Update,
        Add,
        Remove
    }

    public enum SaveNameType
    {
        None,
        SaveMySelfName,
        SaveOtherName,
        ActionEvent,
        Chosetype

    }

    //判断类型
    public enum CheckType
    {
        none,
        isEqual,
        rule
    }

    public enum GetTypeValue
    {
        GetFromValue,
        GetFormText,
        GetFromInputField,
        GetFromList,
        GetFromListOther,
        GetFromDropDown,
        GetFromToggleGroup,
        GetFromToggle
    }

    public enum GetBackTypeValue
    {
        GetValue,
        NoGetValue,
    }


    public enum Valuetype
    {
        GetInt,
        GetString
    }
}

public class EmptyItem
{
    public EmptyItem()
    {
        //空值过滤器
        Items.Add("tuijianren");
    }

    public bool Calculate(string ValueA, string ValueB, string key)
    {
        int A = 0;
        int B = 0;
        bool isSuccessedA = int.TryParse(ValueA, out A);
        bool isSuccessedB = int.TryParse(ValueB, out B);
        Debug.Log(A + "**" + B);
        if (!isSuccessedA || !isSuccessedB)
        {
            MessageManager.GetMessageManager.ShowBar("输入的数值无法解析");
        }
        bool IsGone = false;
        switch (key)
        {
            case "%":
                IsGone = A % B == 0 ? true : false;
                break;
            case "<":
                IsGone = A < B ? true : false;
                break;
            case ">":
                IsGone = A >= B ? true : false;
                break;
        }
        return IsGone;
    }

    private List<string> Items = new List<string>();

    public bool IsGone(string itemename)
    {
        if (Items.Contains(itemename))
            return true;
        return false;
    }

    public bool RuleValue(string key, string GetValue)
    {
        string[] str = key.Split(new char[] { ',' });
        bool isGone = false;
        foreach (string child in str)
        {
            if (child == string.Empty)
                continue;
            string cutForamt = child.Replace("(", string.Empty).Replace(")", string.Empty);
            isGone = Calculate(GetValue, cutForamt.Substring(1, cutForamt.Length - 1), cutForamt.Substring(0, 1));
            if (!isGone)
                break;
        }
        return isGone;
    }
}

[System.Serializable]
public class DataValue
{
    public DataValue(NewMessageInfo data = null)
    {
        CurrentData = data;
    }
    NewMessageInfo CurrentData;
    public bool IsSave;
    public string Name;
    public string OtherName;

    public GetTypeValue MyType;
    public CheckType Rule;
    public Text SetText;
    public InputField SetInputField;
    public string SetValue;
    public Dropdown DropDown;
    public ToggleGroup toggleGroup;
    public Toggle toggle;
    //public TagerType MyTager;
    public string MakeValue;

    public bool IsCheck;
    public InputField InputText;
    public string WarmMessage;
    public string RuleFromat;

    public Func<string, string, bool> CompareAri;
    public Func<string, bool> FiltrationAri;
    public string CompareKey;
    public string CompareWarm;

    public void SetData(object Value)
    {
        System.Type type = Value.GetType();
        if (type == typeof(string))
        {
            SetValue = Value.ToString();
            MyType = GetTypeValue.GetFromValue;
        }
        else if (type == typeof(Text))
        {
            SetText = (Text)Value;
            MyType = GetTypeValue.GetFormText;
        }
        else if (type == typeof(InputField))
        {
            SetInputField = (InputField)Value;
            MyType = GetTypeValue.GetFromInputField;
        }
        else if (type == typeof(Dropdown))
        {
            DropDown = (Dropdown)Value;
            MyType = GetTypeValue.GetFromDropDown;
        }
        else if (type == typeof(ToggleGroup))
        {
            toggleGroup = (ToggleGroup)Value;
            MyType = GetTypeValue.GetFromToggleGroup;
        }
        else if (type == typeof(Toggle))
        {
            toggle = (Toggle)Value;
            MyType = GetTypeValue.GetFromToggle;
        }
    }

    //可为空值的过滤器
    private EmptyItem EmptyBody = new EmptyItem();

    public string GetString()
    {
        string ValueData = null;
        switch (MyType)
        {
            case GetTypeValue.GetFromValue:
                ValueData = SetValue;
                break;
            case GetTypeValue.GetFormText:
                ValueData = SetText.text;
                break;
            case GetTypeValue.GetFromInputField:
                ValueData = SetInputField.text;
                break;
            case GetTypeValue.GetFromList:
                ValueData = CachingRegion.Get(Name);
                break;
            case GetTypeValue.GetFromListOther:
                ValueData = CachingRegion.Get(OtherName);
                break;
            case GetTypeValue.GetFromDropDown:
                ValueData = DropDown.value.ToString();
                break;
            case GetTypeValue.GetFromToggleGroup:
                IEnumerable<Toggle> Group = toggleGroup.ActiveToggles();
                List<Toggle> currentToggel = new List<Toggle>(Group);
                if (currentToggel.Count <= 0)
                {
                    Debug.LogError(toggleGroup.name + ":请至少选择一个toogleGroup标签");
                    return "Error";
                }
                ValueData = currentToggel[0].transform.GetSiblingIndex().ToString();
                break;
            case GetTypeValue.GetFromToggle:
                ValueData = toggle.isOn ? "1" : "0";
                break;
        }

        if (CompareAri != null)
        {
            string data = CurrentData.GetData(CompareKey);
            if (!CompareAri(ValueData, data))
                return "Error";
        }

        if (FiltrationAri != null)
        {
            if (!FiltrationAri(ValueData))
                return "Error";
        }

        if (ValueData == string.Empty)
        {
            if (!EmptyBody.IsGone(Name))
            {
                MessageManager.GetMessageManager.WindowShowMessage("参数不能为空");
                Debug.Log("参数不能为空" + "--输出接口字段--" + Name);
                return "Error";
            }
            else
                return "Gone";
        }

        switch (Rule)
        {
            case CheckType.isEqual:
                if (ValueData != InputText.text)
                {
                    MessageManager.GetMessageManager.WindowShowMessage(WarmMessage);
                    return "Error";
                }
                break;
            case CheckType.rule:
                ValueData = ValueData == string.Empty ? "0" : ValueData;
                if (!EmptyBody.RuleValue(RuleFromat, ValueData))
                {
                    MessageManager.GetMessageManager.ShowBar(WarmMessage);
                    return "Error";
                }
                break;
        }


        if (IsSave)
            CachingRegion.Add(Name, ValueData);
        return ValueData;

    }
}

[System.Serializable]
public class BackDataValue
{
    public GetBackTypeValue MyType;
    public bool IsSave;
    public string Name;
    public Text SetText;


    public void SetString(string BackValue)
    {
        switch (MyType)
        {
            case GetBackTypeValue.GetValue:
                SetText.text = BackValue;
                break;
        }
        if (IsSave)
            CachingRegion.Add(Name, BackValue);
    }
}

public class EventClass
{
    public UnityEvent Fuc;
    public UnityEvent Fal;
}

public class HttpCallBackMessage
{
    public LitJson.JsonData Data;
    public HttpCode Code;
    public string ErroMsg;
}

public enum HttpCode
{
    SUCCESS,
    FAILED,
    UPDATE,
    ERROR
}