using UnityEngine;
using System.Collections;

namespace LuaFramework {

    /// <summary>
    /// </summary>
    public class Main : MonoBehaviour {

        static bool isStart = false;

        void Start() {
            if (!isStart)
            {
                Transform GameManager = Instantiate(Resources.Load<GameObject>("GameManager")).transform;
                GameManager.position = Vector3.zero;
                GameManager.name = "GameManager";
                //Transform Canvas1 = Instantiate(Resources.Load<GameObject>("Canvas")).transform;
                //Canvas1.position = Vector3.zero;
                //Canvas1.name = "Canvas";

                Transform CanvasTop = Instantiate(Resources.Load<GameObject>("CanvasTop")).transform;
                CanvasTop.position = Vector3.zero;
                CanvasTop.name = "CanvasTop";
                Transform PlayButtonMusic = Instantiate(Resources.Load<GameObject>("PlayButtonMusic")).transform;
                PlayButtonMusic.position = Vector3.zero;
                PlayButtonMusic.name = "PlayButtonMusic";
                isStart = true;
            }

           
            AppFacade.Instance.StartUp();   //启动游戏
        }
    }
}