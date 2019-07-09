using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CreatImgVerCode : MonoBehaviour
{

    private Texture2D TextureMap;

    public RawImage show;

    public Color[] vColor;

    [SerializeField]
    private float Heigh = 0.5f;


    [SerializeField]
    private int MinFontSize = 30, MaxFontSize = 50;

    // Use this for initialization
    public void Creat(string Num)
    {
        Text[] aa = show.GetComponentsInChildren<Text>();

        int j = 0;
        foreach (Text child in aa)
        {
            child.text = Num[j].ToString();
            child.fontSize = Random.Range(MinFontSize, MaxFontSize);
            child.rectTransform.position += new Vector3(0, Random.Range(-Heigh, Heigh), 0);
            child.rectTransform.eulerAngles = new Vector3(0, 0, 0);
            child.rectTransform.eulerAngles += new Vector3(0, 0, Random.Range(-20, 20));
            child.color = vColor[Random.Range(0, vColor.Length - 1)];
            j++;
        }
        TextureMap = new Texture2D(40, 20);
        for (int i = 0; i < 60; i++)
            TextureMap.SetPixel(Random.Range(0, 40), Random.Range(0, 20), new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1));
        TextureMap.Apply();
        show.texture = TextureMap;
    }

}
