using UnityEngine;

namespace RedHoney.Utilities
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(this);
        }
    }
}