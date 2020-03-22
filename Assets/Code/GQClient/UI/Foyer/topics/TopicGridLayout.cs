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
        var emptyWidth = padding.left + padding.right + _gridLayoutGroup.spacing.x;
        var cellWidth = (Device.width - emptyWidth) / 2f;
        var cellHeight = cellWidth / ConfigurationManager.Current.topicButtonAspectRatio;
        _gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        _gridLayoutGroup.constraintCount = ConfigurationManager.Current.topicRowsNumber;
    }


}
