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


    private void Start()
    {
        wait = new WaitForSeconds(1/speed);
        PigEffectBody.LoadAssets("pig_walk", "pig_walk");
        AllSprite = PigEffectBody.GetValueGroup("pig_walk");
        image = GetComponent<Image>();
        PlayerAnimator();
    }

    public void PlayerAnimator()
    {
        StartCoroutine("Play");
    }

    public void PlayerLoopAnimator()
    {

    }

    IEnumerator Play()
    {
        int i = 0;
        while (true)
        {
            image.sprite = AllSprite[i];
            i++;
            if (i >= AllSprite.Length)
                i = 0;

            yield return wait;
        }
    }
}
