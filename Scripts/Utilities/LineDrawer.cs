using UnityEngine;

namespace RedHoney.Utilities
{
    /// <summary>
    /// Component that adds a line drawer in Scene view that connects the owning GameObject to another Transform.
    /// </summary>
    public class LineDrawer : MonoBehaviour
    {
        #region Inspector infos
        /// <summary>
        /// Color of the line to draw.
        /// </summary>
        [SerializeField] private Color lineColor = Color.red;
        /// <summary>
        /// Reference to the transform to aim.
        /// </summary>
        [SerializeField] private Transform objectToAim = null;
        #endregion

        #region Unity Events
        /// <summary>
        /// Unity Update method.
        /// </summary>
        private void Update()
        {
            // If no transform to aim is specified, nothing to do.
            if (!objectToAim) return;

            Debug.DrawLine(transform.position, objectToAim.position, lineColor);
        }
        #endregion
    }
}