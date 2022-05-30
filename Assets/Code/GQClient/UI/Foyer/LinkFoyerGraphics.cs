using System;
using System.Collections;
using System.Collections.Generic;
using Code.GQClient.UI.Foyer.header;
using Code.GQClient.UI.layout;
using UnityEngine;
using UnityEngine.UI;

public class LinkFoyerGraphics : LayoutConfig
{
    public HeaderButtonPanel listHeader;
    public HeaderButtonPanel topicHeader;
    public HeaderButtonPanel mapHeader;

    private static Sprite _menuSprite;
    private static Sprite _topLogoSprite;
    private static Sprite _infoSprite;

    // Start is called before the first frame update
    public override void layout()
    {
        _menuSprite = Resources.Load<Sprite>("icons/menu");
        _topLogoSprite = Resources.Load<Sprite>("TopLogo");
        _infoSprite = Resources.Load<Sprite>("icons/info");
        
        listHeader?.SetGraphics(_menuSprite, _topLogoSprite, _infoSprite);
        topicHeader?.SetGraphics(_menuSprite, _topLogoSprite, _infoSprite);
        mapHeader?.SetGraphics(_menuSprite, _topLogoSprite, _infoSprite);
    }

    private void OnValidate()
    {
        layout();
    }
}