using System;
using UnityEngine;


namespace RedHoney.Math
{

    ///////////////////////////////////////////////////////////////////////////
    public class CurveUniformResampler
    {
        private float[] tVals;
        private float[] cumDist;
        private int precision;

        public float curveTotalLength => cumDist[precision - 1];

        ///////////////////////////////////////////////////////////////////////////
        public CurveUniformResampler(Func<float, float> func, int prec, float ratio = 1.0f)
        {
            precision = prec;

            tVals = new float[precision];
            Vector2[] points = new Vector2[precision];
            for (int i = 0; i < precision; i++)
            {
                tVals[i] = i / (float)(precision - 1);
                points[i] = new Vector2(tVals[i], func(tVals[i]) * ratio);
            }

            cumDist = new float[precision];
            for (int i = 1; i < precision; i++)
            {
                float dist = (points[i] - points[i - 1]).magnitude;
                cumDist[i] = cumDist[i - 1] + dist;
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        public float Evaluate(float t)
        {
            float targetLen = t * curveTotalLength;
            int i = Array.BinarySearch(cumDist, targetLen);
            if (i < 0)
                i = ~i;
            if (i > 0)
                return Mathf.Lerp(tVals[i - 1], tVals[i], (targetLen - cumDist[i - 1]) / (cumDist[i] - cumDist[i - 1]));
            else
                return 0.0f;
        }


    }
}