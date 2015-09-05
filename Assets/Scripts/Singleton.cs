using UnityEngine;
//-----------------------------------------------------------------------
// ��㶫
//-----------------------------------------------------------------------
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;                      // Ϊʲô�������е�ʱ������������Ҳ���ᱨ�� �Ӱ���

	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (T)FindObjectOfType(typeof(T));

				if (_instance == null)
				{
					Debug.LogError("An _instance of " + typeof(T) + " is needed in the scene, but there is none.");
				}
			}

			return _instance;
		}
	}

    protected virtual void OnDestroy()
    {
        _instance = null;
    }
}
