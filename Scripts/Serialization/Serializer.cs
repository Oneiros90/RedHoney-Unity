using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace RedHoney.Serialization
{
    ///////////////////////////////////////////////////////////////////////////
    public static class Serializer
    {

        ///////////////////////////////////////////////////////////////////////////
        public static T LoadBinary<T>(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                try
                {
                    using (FileStream file = File.Open(fullPath, FileMode.Open))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        return (T)binaryFormatter.Deserialize(file);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Error deserializing the file {fullPath}: {e.ToString()}");
                }
            }

            return default;
        }

        ///////////////////////////////////////////////////////////////////////////
        public static void SaveBinary<T>(string fullPath, T data)
        {
            using (FileStream file = File.Create(fullPath))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(file, data);
            }
        }
    }
}