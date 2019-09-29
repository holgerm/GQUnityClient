using System;
using GQ.Client.Conf;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollViewLayout : MonoBehaviour
{
    public bool suppressMargin;
    private ScrollRect _scrollRect;
    private ScrollRect scrollRect
    {
        get
        {
            if (_scrollRect == null)
            {
                _scrollRect = GetComponent<ScrollRect>();
            }
            return _scrollRect;
        }
    }

    void Start()
    {
        Init();
    }

    void Reset()
    {
        Init();
    }

    void Init()
    {
        if (scrollRect.horizontalScrollbar != null)
        {
            InitScrollBar(scrollRect.horizontalScrollbar);
            scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.horizontalScrollbarSpacing =
                ConfigurationManager.Current.margin4Scrollbar && ! suppressMargin ?
                10 : -ConfigurationManager.Current.scrollbarWidth;
        }
        if (scrollRect.verticalScrollbar != null)
        {
            InitScrollBar(scrollRect.verticalScrollbar);
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarSpacing =
                ConfigurationManager.Current.margin4Scrollbar && !suppressMargin ?
                10 : -ConfigurationManager.Current.scrollbarWidth;

        }
    }

    private void InitScrollBar(Scrollbar scrollbar)
    {
        Image scrImg = scrollbar.GetComponent<Image>();
        scrImg.sprite = null;
        scrImg.color = ConfigurationManager.Current.scrollbarBGColor;
        scrollbar.GetComponent<RectTransform>().sizeDelta =
            new Vector2(ConfigurationManager.Current.scrollbarWidth, 0f);
        Image handleImg = scrollbar.handleRect.GetComponent<Image>();
        handleImg.sprite = null;
        handleImg.color = ConfigurationManager.Current.scrollbarHandleColor;
    }

}
