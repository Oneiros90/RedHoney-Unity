using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedHoney.ScriptableObjects
{
    ///////////////////////////////////////////////////////////////////////////
    public class AdvancedScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static readonly Dictionary<string, ScriptableObject> Instances = new Dictionary<string, ScriptableObject>();

        [SerializeField]
        [UniqueIdentifier]
        private string guid = null;
        public string GUID => guid;

        ///////////////////////////////////////////////////////////////////////////
        public static T GetInstance(string guid)
        {
            if (guid == null)
                return null;
            Instances.TryGetValue(guid, out ScriptableObject instance);
            return instance as T;
        }

        ///////////////////////////////////////////////////////////////////////////
        public static T[] GetAllInstances()
        {
            return Instances.Values.Select(scriptableObj => scriptableObj as T).ToArray();
        }

        ///////////////////////////////////////////////////////////////////////////
        public void OnEnable()
        {
            if (guid != null)
                Instances[guid] = this;
        }
    }
}