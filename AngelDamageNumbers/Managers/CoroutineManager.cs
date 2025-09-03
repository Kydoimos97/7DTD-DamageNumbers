using UnityEngine;

namespace AngelDamageNumbers.Managers
{
    public class CoroutineManager : MonoBehaviour
    {
        private static CoroutineManager _instance = null!;

        public static CoroutineManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CoroutineManager>();
                    if (_instance == null)
                    {
                        var gameObject = new GameObject(nameof(CoroutineManager));
                        _instance = gameObject.AddComponent<CoroutineManager>();
                        DontDestroyOnLoad(gameObject);
                    }
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}