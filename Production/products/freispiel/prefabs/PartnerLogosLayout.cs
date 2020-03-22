using System.Collections;
using System.Collections.Generic;
using Code.QM.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Code.GQClient.Product
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class PartnerLogosLayout : MonoBehaviour
    {
        public GridLayoutGroup logoPanel;

        private void Reset()
        {
            Layout();
        }

        // Start is called before the first frame update
        private void Start()
        {
            Layout();
        }

        private void Layout()
        {
            var nrOfRows = 4;
            var padding = logoPanel.padding;
            var emptyPixels = (float) (padding.left + padding.right) +
                              (nrOfRows -1) * logoPanel.spacing.x;
            var cellWidth = (Device.width - emptyPixels) / nrOfRows;
            var cellHeight = cellWidth * (9f / 16f);
            logoPanel.cellSize = new Vector2(cellWidth, cellHeight);
        }
    }
}