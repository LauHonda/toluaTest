using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TransformData))]
public abstract class HttpTransformBLL : HttpBaseBLL
{
    protected TransformData transformData { get { return GetComponent<TransformData>() == null ? 
                gameObject.AddComponent<TransformData>() : GetComponent<TransformData>(); } }

}
