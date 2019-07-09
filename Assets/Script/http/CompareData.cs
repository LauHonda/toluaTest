using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareTools
{

    /// <summary>
    /// 比较2次数值是否相等
    /// </summary>
    /// <param name="KeyA"></param>
    /// <param name="KeyB"></param>
    /// <returns></returns>
    public static bool CompareEqual(string KeyA, string KeyB)
    {
        if (KeyA != KeyB)
        {
            MessageManager.GetMessageManager.WindowShowMessage("2次输入的密码不一致");
            return false;
        }
        return true;
    }
}
