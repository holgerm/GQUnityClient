using Code.GQClient.Conf;
using Code.GQClient.UI;
using Code.GQClient.UI.Foyer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TopicButtonCtrl : PrefabController
{
    private const string PREFAB = "TopicButton";

    public static TopicButtonCtrl Create (GameObject root, Topic topic)
    {
        // Create the view object for this controller:
        var go = PrefabController.Create ("prefabs", PREFAB, root);
        go.name = PREFAB + " (" + topic.Name + ")";

        var topicCtrl = go.GetComponent<TopicButtonCtrl> ();
        topicCtrl._topic = topic;
        topicCtrl.text.text = topic.Name;
        var topicImage = go.GetComponent<Image>();
        topicImage.color = Config.Current.NextPaletteColor;
        
        topicCtrl.gameObject.SetActive(true);

        return topicCtrl;
    }
    
    public TMP_Text text;

    private Topic _topic;
    
    public void OnTopicSelected()
    {
        Topic.CursorMoveDown(_topic.Name);
    }
}
