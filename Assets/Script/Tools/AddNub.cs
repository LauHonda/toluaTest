using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AddNub : MonoBehaviour {

    public Button Add_Button;
    public Button Dis_Button;

    public InputField Body;

    [SerializeField]
    private int AddCount;
    private int Nub;

    private void Start()
    {
        Body.text = "1";
        ; Add_Button.onClick.AddListener(delegate ()
         {
             Add(AddCount);
         });

        Dis_Button.onClick.AddListener(delegate ()
        {
            Add(-AddCount);
        });
    }

    public int Add(int addnub)
    {
        if (Body.text != string.Empty && Body.text != "0")
            Nub = int.Parse(Body.text);
        else
            Nub = 1;
        Nub = (Nub + addnub);
        Nub = Nub < 1 ? 1 : Nub;
        Body.text = Nub.ToString();
        return Nub;
    }

    public int Add(int addnub,out bool isGone)
	{
		if (Body.text != string.Empty&& Body.text!="0")
            Nub = int.Parse (Body.text);
		else
            Nub = 1;
        Nub = (Nub + addnub);
        isGone = Nub < 1 ? false: true;
        Nub = Nub < 1 ? 1 : Nub;
        Body.text = Nub.ToString() ;
        return Nub;
	}
}
