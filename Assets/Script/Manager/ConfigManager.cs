using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using UltimateDH;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager GetConfigManager;
    public bool IsLoaded = false;
    public void Awake()
    {
        GetConfigManager = this;
    }

    public DicResources<Sprite> PigEffectBody = new DicResources<Sprite>();

    public string[] Body_ConfigTag;
    public void LoadConfig()
    {
        //添加需要配置信息的消息模块
        AddConfig();
        IsLoaded = true;

        PigEffectBody.LoadAssets("Car01left", "Car01_left");

    }

    public void AddConfig()
    {

    }


    //道具颜色组件配置
    public Sprite[] ColorGroup;

    //头像配置信息
    [SerializeField]
    private Texture2D[] HeadIamgeGroup;
    [SerializeField]
    private Sprite[] HeadSmallIamgeGroup;

    public void SetRawIamge(RawImage headiamge, int nub)
    {
        headiamge.texture = HeadIamgeGroup[nub];
    }
    public void SetIamge(Image headiamge, int nub)
    {
        Texture2D texture = HeadIamgeGroup[nub];
        Sprite sprites = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        headiamge.sprite = sprites;
    }

    public void SetSmallIamge(Image headiamge, int nub)
    {
        headiamge.sprite = HeadSmallIamgeGroup[nub];
    }

    //当前加载装备的矿工id;
    public string kd_id = "-1";
    //当前加载装备的位置信息;
    public string position_id = "-1";
    //记录当前是否为否买状态
    public bool IsBuy = false;
    public void CloseBuy()
    {
        IsBuy = false;
    }

    public Message<bool> SyncState = new Message<bool>();

    public void Update()
    {
        SyncState.Send(IsBuy);
    }


    //道具图标配置
    public Sprite[] PropSprite;

    [System.Serializable]
    public class ImageMessage
    {
        public Sprite[] all;
    }

    public ImageMessage[] BodyIamgeProp;


    public void SetImagePropColor(Image GetImage, string id, string color)
    {
        GetImage.sprite = BodyIamgeProp[int.Parse(color) - 1].all[int.Parse(id) - 1];
    }

    public Color[] configcolor;
    //颜色信息配置
    public Color GetColor(string colorvalue)
    {
        return configcolor[int.Parse(colorvalue) - 1];
    }


}



