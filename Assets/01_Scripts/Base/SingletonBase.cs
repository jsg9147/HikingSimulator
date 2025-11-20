using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
            if(SingletonManager.instance != null)
                SingletonManager.instance.RegisterSingleton(this); // ½Ì±ÛÅæ °ü¸®ÀÚ¿¡ µî·Ï
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            if(SingletonManager.instance != null)
                SingletonManager.instance.UnregisterSingleton(this); // ½Ì±ÛÅæ °ü¸®ÀÚ¿¡¼­ Á¦°Å
            instance = null;
        }
    }
}
