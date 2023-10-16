using UnityEngine;

namespace Common
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        
        private static T _instance;

        private static object _lock = new object();

        public static T instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            if (Application.isPlaying)
                                DontDestroyOnLoad(singleton);
                        }
                    }

                    return _instance;
                }
            }
        }

        private static bool applicationIsQuitting = false;
    
        public void OnDestroy()
        {
            applicationIsQuitting = true;
        }

        public static void OnEditorInit()
        {
            applicationIsQuitting = false;
        }

        public static bool CheckInstance()
        {
            return _instance != null;
        }
    }

//! Singleton
    public class SingletonN<T> where T : new()
    {
        private static T _instance;

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }

                return _instance;
            }
        }

        public static bool CheckInstance()
        {
            return _instance != null;
        }
    }
}