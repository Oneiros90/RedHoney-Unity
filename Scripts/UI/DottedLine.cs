using System.Collections;
using System.Collections.Generic;
using RedHoney.Math;
using UnityEngine;
using UnityEngine.UI;


namespace RedHoney.UI
{
    ///////////////////////////////////////////////////////////////////////////
    [RequireComponent(typeof(RectTransform))]
    public partial class DottedLine : MonoBehaviour
    {
        // Inspector fields
        public Sprite Dot;
        [Range(0.01f, 1f)]
        public float Size = 0.01f;
        [Range(1f, 100f)]
        public float DotsDistance = 1.0f;
        public bool DrawOnStart = true;
        public Color DotsColor = Color.white;

        public AnimationCurve curve;
        private CurveUniformResampler resampler;


        //Utility fields
        private readonly List<RectTransform> dots = new List<RectTransform>();

        ///////////////////////////////////////////////////////////////////////////
        private IEnumerator Start()
        {
            if (DrawOnStart)
            {
                yield return null;
                Draw();
            }
        }


        ///////////////////////////////////////////////////////////////////////////
        // Custom in editor behaviour for redrawing dynamically on changes
#if UNITY_EDITOR
        private Vector2 resolution;
        private void OnValidate()
        {
            if (Application.isPlaying && gameObject.scene.rootCount != 0)
                Draw();
        }
        private void Update()
        {
            if (resolution.x != Screen.width || resolution.y != Screen.height)
            {
                resolution.x = Screen.width;
                resolution.y = Screen.height;
                Draw();
            }
        }
#endif

        ///////////////////////////////////////////////////////////////////////////
        public void Draw()
        {
            RectTransform rect = GetComponent<RectTransform>();
            if (!rect)
                return;

            foreach (var dot in dots)
                Destroy(dot.gameObject);
            dots.Clear();

            float rectRatio = rect.rect.height / rect.rect.width;
            resampler = new CurveUniformResampler(curve.Evaluate, 500, rectRatio);

            float curveTotalLength = resampler.curveTotalLength * rect.rect.width;
            int dotsCount = Mathf.FloorToInt(curveTotalLength / DotsDistance);
            if (dotsCount <= 0)
                return;

            Vector2[] positions = new Vector2[dotsCount];

            float step = 1.0f / (dotsCount - 1);
            for (int i = 0; i < positions.Length; i++)
            {
                float t = step * i;
                float t2 = resampler.Evaluate(t);
                positions[i].Set(t2, curve.Evaluate(t2));
            }

            RectTransform newDot()
            {
                var dot = new GameObject("dot");
                dot.transform.localScale = Vector2.one * Size;
                dot.transform.SetParent(transform, false);

                var sr = dot.AddComponent<Image>();
                sr.sprite = Dot;
                sr.color = DotsColor;

                var dotRect = dot.GetComponent<RectTransform>();
                dotRect.anchorMin = Vector2.zero;
                dotRect.anchorMax = Vector2.zero;

                return dotRect;
            }

            foreach (var position in positions)
            {
                var dot = newDot();
                dot.anchoredPosition = Vector2.Scale(position, rect.rect.size);
                dots.Add(dot);
            }
        }
    }
}