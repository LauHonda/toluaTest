using UnityEngine;

namespace UltimateSurvival
{
    public class PoolableObject : MonoBehaviour
    {
        public virtual void OnSpawn() { }//启用对象时运行。

        public virtual void OnDespawn() { }//禁用对象时运行。

        public virtual void OnPoolableDestroy() { }//在对象被破坏时运行。
    }
}