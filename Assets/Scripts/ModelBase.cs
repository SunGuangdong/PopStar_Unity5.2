using UnityEngine;
using System;

namespace DajiaGame.Px
{
    /// <summary>
    /// 作为 Model单例的 基类 【和之前的 MonoBehaviour 的单例基类 Singleton不同】
    /// author ： 孙广东
    /// </summary>
    public abstract class ModelBase<T> :IDisposable where T : new()  
    {
        #region 单例
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }

        protected ModelBase() 
        {
        }

        public void Dispose()
        {
            instance = default(T);
        }
        #endregion
    }
}
