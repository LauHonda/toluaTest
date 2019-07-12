using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UltimateDH;

public class AndroidPhoto : MonoBehaviour
{
    public IOSPhoto ios;
    // Use this for initialization

    public static AndroidPhoto GetPhoto;

    public Value<Texture2D> headImg = new Value<Texture2D>(null);

    private void Awake()
    {
        GetPhoto = this;
    }
    public Text debug;
    //打开相册	
    public void OpenPhoto()
    {
        //Debug.Log("PHOTON");
        //debug.text=debug.text+"*";
#if UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("OpenGallery");
#elif UNITY_IPHONE
		ios.OpenPhoto();
#endif

    }

    //打开相机
    public void OpenCamera()
    {
#if UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        jo.Call("takephoto");
#elif UNITY_IPHONE
		ios.OpenCamera();
#endif
    }

    public void GetImagePath(string imagePath)
    {
        if (imagePath == null)
            return;
        StartCoroutine("Load", imagePath);
    }

    public void GetTakeImagePath(string imagePath)
    {
        if (imagePath == null)
            return;
        StartCoroutine("Load", imagePath);
    }

    private IEnumerator Load(string imagePath)
    {
        MessageManager.GetMessageManager.AddLockNub();
        WWW www = new WWW("file://" + imagePath);
        yield return www;
        MessageManager.GetMessageManager.DisLockNub();
        if (www.error == null)
        {
            //成功读取图片，写自己的逻辑
            //GetComponent<ChangePhoto>().LoadAndroidImageOK(www.texture);
            headImg.Set(www.texture);
            //MessageManager._Instantiate.Show("现车获取成功，等待上传");
            Texture2D tex = www.texture;
            LoadImage.GetLoadIamge.SendImage(tex);

        }
        else
        {
            Debug.LogError("LoadImage>>>www.error:" + www.error);
        }
    }
}
