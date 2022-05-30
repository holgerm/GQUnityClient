using Code.GQClient.Conf;
using Code.GQClient.UI.layout;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CanvasWithHeaderBg : LayoutConfig
{
    private Image _image;
    public void Awake()
    {
        _image = GetComponent<Image>();
    }
    // Start is called before the first frame update
    public override void layout()
    {
        if (_image) _image.color = Config.Current.headerBgColor;
    }
}
