using Unity.Netcode;
using UnityEngine;

namespace _game.Scripts.Utils
{
    public class NetworkSingleton<T> : NetworkBehaviour where T : Component
    {
        // create a private reference to T instance
        private static T instance;

        public static T Instance
        {
            get
            {
                // if instance is null
                if (instance == null)
                {
                    // find the generic instance
                    instance = FindObjectOfType<T>();

                    // if it's null again create a new object
                    // and attach the generic instance
                    if (instance == null)
                    {
                        GameObject obj = new GameObject
                        {
                            name = typeof(T).Name
                        };
                        instance = obj.AddComponent<T>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return instance;
            }
        }

        public void Awake()
        {
            // create the instance
            if (instance == null)
            {
                instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
