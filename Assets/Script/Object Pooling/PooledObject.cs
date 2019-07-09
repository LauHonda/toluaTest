using System.Collections;
using UnityEngine;

namespace UltimateDH
{
	/// <summary>
	/// 
	/// </summary>
    public class PooledObject : MonoBehaviour
    {
        /// <summary>对象释放后引发此事件.</summary>
        public Message<PooledObject> Released = new Message<PooledObject>();

		[SerializeField]
		private bool m_ReleaseOnTimer = true;
		
		[SerializeField]
		private float m_ReleaseTimer = 20f;

		[SerializeField]
		private ParticleSystem[] m_ToResetParticles;

		//private Transform m_Container;
		private WaitForSeconds m_WaitInterval;


		/// <summary>
		/// 
		/// </summary>
		public virtual void OnUse(Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), Transform parent = null)
		{
			try
			{
				gameObject.SetActive(true);

				transform.position = position;
				transform.rotation = rotation;

				if(transform.parent)
					transform.SetParent(parent);

				for(int i = 0;i < m_ToResetParticles.Length;i ++)
					m_ToResetParticles[i].Play(true);

				if(m_ReleaseOnTimer)
				{
					StopAllCoroutines();
					StartCoroutine(ReleaseWithDelay());
				}
			}
            //黑客：当一个集合的对象（如一个弹孔贴花被删除，同时是一个对象的孩子）时的bug。
            catch { }
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnRelease()
		{
			gameObject.SetActive(false);
			//transform.SetParent(m_Container);

			Released.Send(this);
		}

		private void Awake()
		{
            // todo用waitforsecondsraltime进行bug。
            if (m_ReleaseOnTimer)
				m_WaitInterval = new WaitForSeconds(m_ReleaseTimer);
		}

		private IEnumerator ReleaseWithDelay()
		{
			yield return m_WaitInterval;
			OnRelease();
		}
    }
}