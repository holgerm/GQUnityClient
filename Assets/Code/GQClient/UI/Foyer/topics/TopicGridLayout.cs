using System;
using System.Collections;
using System.Collections.Generic;
using Code.GQClient.Conf;
using Code.GQClient.UI.layout;
using Code.GQClient.Util;
using Code.QM.Util;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class TopicGridLayout : LayoutConfig
{
    private GridLayoutGroup _gridLayoutGroup;
    
    // Start is called before the first frame update
    public override void layout()
    {
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        
        // set topic buttons size:
        var padding = _gridLayoutGroup.padding;
        var emptyWidth = padding.left + padding.right + _gridLayoutGroup.spacing.x * (NumberOfColumns -1);
        var cellWidth = (Device.width - emptyWidth) / NumberOfColumns;
        var cellHeight = cellWidth / ConfigurationManager.Current.topicButtonAspectRatio;
        _gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = NumberOfColumns;
    }

    private static int NumberOfColumns => Device.DisplaySize >= Device.Size.Medium ? 3 : 2;
    


}
