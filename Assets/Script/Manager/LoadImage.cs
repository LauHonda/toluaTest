// ==================================================================
// 作    者：A.R.I.P.风暴洋-宋杨
// 説明する：加载头像类
// 作成時間：2018-07-30
// 類を作る：LoadImage.cs
// 版    本：v 1.0
// 会    社：大连仟源科技
// QQと微信：731483140
// ==================================================================

using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadImage : MonoBehaviour
{
    public static LoadImage GetLoadIamge;

    void Awake()
    {
        GetLoadIamge = this;
    }

    private Dictionary<string, Texture2D> LoadedIamge = new Dictionary<string, Texture2D>();


    public void LoadNoSave(string url, RawImage[] image = null, bool IsSize = false)
    {
        StartCoroutine(GetMessage(url, image));
    }

    [SerializeField]
    Texture2D defTex;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="url">图片地址</param>
    /// <param name="image">赋值对象，RawImage类型的数组</param>
    /// <param name="IsSize"></param>
    public void Load(string url, RawImage[] image = null, bool IsSize = false)
    {
        if (url == "")
        {
            foreach (var item in image)
            {
                if (item != null)
                {
                    item.texture = defTex;
                }
            }
            return;
        }

        Texture2D cuuretimage = null;
        bool IsGet = LoadedIamge.TryGetValue(url, out cuuretimage);
        if (IsGet)
        {
            if (image != null)
            {
                foreach (var item in image)
                {
                    if (item != null)
                    {
                        //if(IsSize)
                        //item.GetComponent<RectTransform>().sizeDelta = new Vector2(cuuretimage.width, cuuretimage.height);
                        item.texture = cuuretimage;
                    }
                }
            }
        }
        else
            StartCoroutine(GetMessage(url, image));
    }


    private IEnumerator GetMessage(string url, RawImage[] image, bool IsSzie = false)
    {
        MessageManager.GetMessageManager.AddLockNub();
        WWW www = new WWW(url);
        yield return www;
        MessageManager.GetMessageManager.DisLockNub();
        if (www.error == null)
        {
            foreach (var item in image)
            {
                if (item != null)
                {
                    Debug.Log(www.texture.width + "----IMG-----" + www.texture.height);
                    //if (IsSzie)
                    //    item.GetComponent<RectTransform>().sizeDelta = new Vector2(www.texture.width, www.texture.height);
                    item.texture = www.texture;
                }
                else
                    Debug.Log("目标RawImage已被摧毁");
            }
            if (!LoadedIamge.ContainsKey(url))
                LoadedIamge.Add(url, www.texture);
        }
        else
        {
           // MessageManager.GetMessageManager.ShowBar("获取头像失败");
        }
    }


    public Texture2D texture2DTexture(Texture2D tex, int swidth, int sheght)
    {
        Texture2D res = new Texture2D(swidth, sheght, TextureFormat.ARGB32, false);
        for (int i = 0; i < res.height; i++)
        {
            for (int j = 0; j < res.width; j++)
            {
                Color newcolor = tex.GetPixelBilinear((float)j / (float)res.width, (float)i / (float)res.height);
                res.SetPixel(j, i, newcolor);
            }
        }
        res.Apply();
        return res;
    }

    [SerializeField]
    Texture2D current2D;

    public void SendImage(Texture2D img)
    {
        current2D = img;
        StartCoroutine(UploadTexture(Up()));
    }


    private string Up()
    {
        float X = 0;
        float Y = 0;
        if (current2D.width > current2D.height)
        {
            X = 1024;
            Y = ((float)(current2D.height) / (float)current2D.width) * 1024;
        }
        else
        {
            Y = 1024;
            X = ((float)(current2D.width) / (float)current2D.height) * 1024;
        }
        Texture2D newtext = texture2DTexture(current2D, 256, 256);
        string base64String = System.Convert.ToBase64String(newtext.EncodeToJPG());
        return base64String;
    }

    public Text MSG;
    public Texture2D tex;

    IEnumerator UploadTexture(string GetTex)
    {
        string url = Static.Instance.URL + "ajax_up_img.php";
        WWWForm form = new WWWForm();
        form.AddField("huiyuan_id", CachingRegion.Get("huiyuan_id"));
        form.AddField("img_url", GetTex);
        DtaMD5 data = EncryptDecipherTool.UserMd5Obj();
        form.AddField("token", data.token);
        form.AddField("time", data.time);
        Debug.Log(url);
        MessageManager.GetMessageManager.AddLockNub();
        WWW www = new WWW(url, form);
        yield return www;
        //MSG.text = string.Empty;
        MessageManager.GetMessageManager.DisLockNub();
        if (www.error != null)
        {
            // MSG.text = www.error;
            //MessageManager._Instantiate.Show("图片上传失败");
        }
        else
        {
            //  MSG.text = www.text;
            // Debug.Log(www.text);
            MessageManager.GetMessageManager.WindowShowMessage("图片上传成功");

        }
    }

    public void Laodtext()
    {
        float X = 0;
        float Y = 0;
        if (tex.width > tex.height)
        {
            X = 256.0f;
            Y = ((float)(tex.height) / (float)tex.width) * 256f;
        }
        else
        {
            Y = 256.0f;
            X = ((float)(tex.width) / (float)tex.height) * 256f;
        }
        //Color[] AA=tex.GetPixels();
        //tex.Resize(tex.width/4,tex.height/4,TextureFormat.ARGB32,false);
        //tex.SetPixels(AA);
        //tex.Apply();
        Texture2D newtext = texture2DTexture(tex, System.Convert.ToInt32(X), System.Convert.ToInt32(Y));
        Debug.Log(tex.width + "--" + tex.height);
        string base64String = System.Convert.ToBase64String(newtext.EncodeToPNG());
        StartCoroutine(UploadTexture(base64String));
    }

}
