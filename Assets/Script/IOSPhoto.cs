using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class IOSPhoto : MonoBehaviour
{
    [SerializeField]
    public RawImage a;
    //public HttpImage GetRaw;
    byte[] SaveHeadImg = null;

    void Start()
    {
#if UNITY_IPHONE
        ios.Initialization();
#endif
    }
    // Use this for initialization
    public void Initialization()
    {
        if (IOSAlbumCamera.Instance)
        {
            IOSAlbumCamera.Instance.CallBack_PickImage_With_Base64 += callback_PickImage_With_Base64;
            IOSAlbumCamera.Instance.CallBack_ImageSavedToAlbum += callback_imageSavedToAlbum;
        }
    }

    public void DestroyFuntion()
    {
        if (IOSAlbumCamera.Instance)
        {
            IOSAlbumCamera.Instance.CallBack_PickImage_With_Base64 -= callback_PickImage_With_Base64;
            IOSAlbumCamera.Instance.CallBack_ImageSavedToAlbum -= callback_imageSavedToAlbum;
        }
    }

    void OnDisable()
    {
#if UNITY_IPHONE
        ios.DestroyFuntion();
#endif

    }

    public void OpenPhoto()
    {
        IOSAlbumCamera.iosOpenPhotoLibrary(true);
    }

    void onclick_album()
    {
        IOSAlbumCamera.iosOpenPhotoAlbums(true);
    }

    public void OpenCamera()
    {
        IOSAlbumCamera.iosOpenCamera(true);
    }

    //void onclick_saveToAlbum()
    //{
    //	string path = Application.persistentDataPath + "/lzhscreenshot.png";
    //	Debug.Log (path);

    //	byte[] bytes = (rawImage.texture as Texture2D).EncodeToPNG ();
    //	System.IO.File.WriteAllBytes (path, bytes);

    //	IOSAlbumCamera.iosSaveImageToPhotosAlbum (path);

    //}

    void callback_PickImage_With_Base64(string base64)
    {
        Texture2D tex = IOSAlbumCamera.Base64StringToTexture2D(base64);
        //        byte[] bytes = System.Convert.FromBase64String(base64);
        //        SaveHeadImg = bytes;
        //
        //        Sprite sprites = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        a.texture = tex;
        LoadImage.GetLoadIamge.SendImage(tex);
        //GetRaw.image[0].sprite = sprites;
        //GetRaw.SavePath = null;
    }

    void callback_imageSavedToAlbum(string msg)
    {
        //txt_saveTip.text = msg;
    }


    public void CamReset()
    {
        StopAllCoroutines();
        SaveHeadImg = null;
    }
}
