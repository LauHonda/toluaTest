using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotate : MonoBehaviour {

    [SerializeField]
    float speed = 1;

    private void Update()
    {
        transform.Rotate(0,0,speed * Time.deltaTime);
    }
}
