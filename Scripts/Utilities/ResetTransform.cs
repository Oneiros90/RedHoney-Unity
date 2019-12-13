#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class ResetTransform
{
    [MenuItem("GameObject/Reset Transform #r")]
    static public void MoveSceneViewCamera()
    {
        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            Transform trx = selectedObject.transform;
            Undo.RegisterCompleteObjectUndo(trx, "Reset game object to origin");

            trx.localPosition = Vector3.zero;
            trx.localRotation = Quaternion.identity;
            trx.localScale = Vector3.one;
        }
    }
}
#endif
