using UnityEngine;


namespace RedHoney.Math
{
    ///////////////////////////////////////////////////////////////////////////////
    public static class MatrixExtensions
    {

        ///////////////////////////////////////////////////////////////////////////////
        public static Matrix4x4 NewMatrix(Vector3 p, Quaternion q, Vector3 s)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(p, q, s);
            return matrix;
        }

        ///////////////////////////////////////////////////////////////////////////////
        public static Matrix4x4 Lerp(Matrix4x4 matrix, Matrix4x4 otherMatrix, float t)
        {
            Matrix4x4 lerp = new Matrix4x4();
            for (int i = 0; i < 16; i++)
                lerp[i] = Mathf.Lerp(matrix[i], otherMatrix[i], t);
            return lerp;
        }

        ///////////////////////////////////////////////////////////////////////////////
        public static Matrix4x4 SmoothStep(Matrix4x4 matrix, Matrix4x4 otherMatrix, float t)
        {
            Matrix4x4 lerp = new Matrix4x4();
            for (int i = 0; i < 16; i++)
                lerp[i] = Mathf.SmoothStep(matrix[i], otherMatrix[i], t);
            return lerp;
        }

        ///////////////////////////////////////////////////////////////////////////////
        public static void SetPosition(this Matrix4x4 matrix, Vector3 pos)
        {
            matrix.m03 = pos.x;
            matrix.m13 = pos.y;
            matrix.m23 = pos.z;
        }

        ///////////////////////////////////////////////////////////////////////////////
        public static Vector3 Position(this Matrix4x4 matrix)
        {
            return new Vector3(matrix.m03, matrix.m13, matrix.m23);
        }

        ///////////////////////////////////////////////////////////////////////////////
        public static void SetRotation(this Matrix4x4 matrix, Quaternion q)
        {
            matrix.SetTRS(matrix.Position(), q, matrix.lossyScale);
        }

        ///////////////////////////////////////////////////////////////////////////////
        public static Matrix4x4 ToMatrix(this Transform transform, bool local = true)
        {
            Matrix4x4 matrix = Matrix4x4.identity;
            matrix.SetTRS(
                local ? transform.localPosition : transform.position,
                local ? transform.localRotation : transform.rotation,
                transform.lossyScale
            );
            return matrix;
        }

        ///////////////////////////////////////////////////////////////////////////////
        public static void SetMatrix(this Transform transform, Matrix4x4 matrix, bool local = true)
        {
            if (local)
            {
                transform.localScale = matrix.lossyScale;
                transform.localRotation = matrix.rotation;
                transform.localPosition = matrix.Position();
            }
            else
            {
                transform.localScale = matrix.lossyScale;
                transform.rotation = matrix.rotation;
                transform.position = matrix.Position();
            }
        }
    }
}