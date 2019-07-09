using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField]
    Sprite[] AllSprite;

    Image image;

    [SerializeField]
    float speed = 1;

    WaitForSeconds wait;

    public DicResources<Sprite> PigEffectBody = new DicResources<Sprite>();

    [System.Serializable]
    public class AssetsData
    {
        public string assetsName;
        public string assetsPath;
    }

    [SerializeField]
    AssetsData[] Group;
    [SerializeField]
    bool isChange = false;
    private void Start()
    {
        wait = new WaitForSeconds(1 / speed);

        foreach (AssetsData child in Group)
        {
            PigEffectBody.LoadAssets(child.assetsName, child.assetsPath);
        }

        image = GetComponent<Image>();

        PalyAniamtor(2);

        int num = Random.Range(8, 12);
        if (isChange)
            InvokeRepeating("Change", 0, num);

    }


    private void Change()
    {
        if (!this.gameObject.activeInHierarchy)
            return;
        int nun = Random.Range(0, Group.Length);
        PalyAniamtor(nun);
    }

    Coroutine C;

    private void OnEnable()
    {
        if (C != null)
            StopCoroutine(C);
        if (AllSprite == null || AllSprite.Length <= 0)
            AllSprite = PigEffectBody.GetValueGroup(Group[0].assetsName);
        C = StartCoroutine("Play");

        //Debug.Log("ASDSADSADSADSADAS:" + gameObject.name);
    }

    public void PalyAniamtor(string key)
    {
        if (C != null)
            StopCoroutine(C);
        AllSprite = PigEffectBody.GetValueGroup(key);
        C = StartCoroutine("Play");
    }

    public void PalyAniamtor(int key)
    {
        if (C != null)
            StopCoroutine(C);
       // Debug.Log(key);
        AllSprite = PigEffectBody.GetValueGroup(Group[key].assetsName);
        C = StartCoroutine("Play");
    }

    IEnumerator Play()
    {
        int i = 0;
        while (true)
        {
            if (AllSprite != null && AllSprite.Length - 1 >= i)
            {
                image.sprite = AllSprite[i];
                i++;
                if (i >= AllSprite.Length)
                    i = 0;
            }
            yield return wait;
        }
    }
}
