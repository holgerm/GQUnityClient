/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Xml;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class OnlineMapsUpdater : EditorWindow 
{
    private OnlineMapsUpdateChannel channel = OnlineMapsUpdateChannel.stable;
    private string invoiceNumber;
    private Vector2 scrollPosition;
    private List<OnlineMapsUpdateItem> updates;

    private void CheckNewVersions()
    {
        if (string.IsNullOrEmpty(invoiceNumber))
        {
            EditorUtility.DisplayDialog("Error", "Please enter the Invoice Number.", "OK");
            return;
        }

        int inum;

        if (!int.TryParse(invoiceNumber, out inum))
        {
            EditorUtility.DisplayDialog("Error", "Wrong Invoice Number.", "OK");
            return;
        }

        SavePrefs();

        string updateKey = GetUpdateKey();
        GetUpdateList(updateKey);
    }

    private string GetUpdateKey()
    {
        WebClient client = new WebClient();
        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
        string updateKey = client.UploadString("http://infinity-code.com/products_update/getupdatekey.php",
            "key=" + invoiceNumber + "&package=" + WWW.EscapeURL("Online Maps"));

        return updateKey;
    }

    private void GetUpdateList(string updateKey)
    {
        WebClient client = new WebClient();
        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

        string response;

        try
        {
            response = client.UploadString("http://infinity-code.com/products_update/checkupdates.php",
            "k=" + WWW.EscapeURL(updateKey) + "&v=" + OnlineMaps.version + "&c=" + (int)channel);
        }
        catch
        {
            return;
        }
        
        XmlDocument document = new XmlDocument();
        document.LoadXml(response);

        XmlNode firstChild = document.FirstChild;
        updates = new List<OnlineMapsUpdateItem>();

        foreach (XmlNode node in firstChild.ChildNodes) updates.Add(new OnlineMapsUpdateItem(node));
    }

    private void OnEnable()
    {
        if (EditorPrefs.HasKey("OnlineMapsInvoiceNumber"))
            invoiceNumber = EditorPrefs.GetString("OnlineMapsInvoiceNumber");
        else invoiceNumber = "";

        if (EditorPrefs.HasKey("OnlineMapsUpdateChannel")) 
            channel = (OnlineMapsUpdateChannel)EditorPrefs.GetInt("OnlineMapsUpdateChannel");
        else channel = OnlineMapsUpdateChannel.stable;
    }

    private void OnDestroy()
    {
        SavePrefs();
    }

    private void OnGUI()
    {
        invoiceNumber = EditorGUILayout.TextField("Invoice Number:", invoiceNumber).Trim(new[] { ' ' });
        channel = (OnlineMapsUpdateChannel) EditorGUILayout.EnumPopup("Channel:", channel);

        if (GUILayout.Button("Check new versions")) CheckNewVersions();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (updates != null)
        {
            foreach (OnlineMapsUpdateItem update in updates) update.Draw();
            if (updates.Count == 0) GUILayout.Label("No updates");
        }

        EditorGUILayout.EndScrollView();
    }

    [MenuItem("Component/Infinity Code/Online Maps/Check Updates")]
    public static void OpenWindow()
    {
        GetWindow<OnlineMapsUpdater>(false, "Online Maps Updater", true);
    }

    private void SavePrefs()
    {
        EditorPrefs.SetString("OnlineMapsInvoiceNumber", invoiceNumber);
        EditorPrefs.SetInt("OnlineMapsUpdateChannel", (int) channel);
    }
}

public class OnlineMapsUpdateItem
{
    private string version;
    private int type;
    private string changelog;
    private string download;
    private string date;

    private static GUIStyle _changelogStyle;
    private static GUIStyle _titleStyle;

    private static GUIStyle changelogStyle
    {
        get { return _changelogStyle ?? (_changelogStyle = new GUIStyle(EditorStyles.label) {wordWrap = true}); }
    }

    private static GUIStyle titleStyle
    {
        get
        {
            return _titleStyle ??
                   (_titleStyle = new GUIStyle(EditorStyles.boldLabel) {alignment = TextAnchor.MiddleCenter});
        }
    }

    public OnlineMapsUpdateItem(XmlNode node)
    {
        version = node.SelectSingleNode("Version").InnerText;
        type = int.Parse(node.SelectSingleNode("Type").InnerText);
        changelog = node.SelectSingleNode("ChangeLog").InnerText;
        download = node.SelectSingleNode("Download").InnerText;
        date = node.SelectSingleNode("Date").InnerText;

        string[] vars = version.Split(new[] {'.'});
        string[] vars2 = new string[4];
        vars2[0] = vars[0];
        vars2[1] = int.Parse(vars[1].Substring(0, 2)).ToString();
        vars2[2] = int.Parse(vars[1].Substring(2, 2)).ToString();
        vars2[3] = int.Parse(vars[1].Substring(4, 4)).ToString();
        version = string.Join(".", vars2);
    }

    public void Draw()
    {
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label("Version: " + version + " (" + typeStr + "). " + date, titleStyle);

        GUILayout.Label(changelog, changelogStyle);

        if (GUILayout.Button("Download"))
        {
            Process.Start("http://infinity-code.com/products_update/download.php?k=" + download);
        }

        GUILayout.EndVertical();
    }

    public string typeStr
    {
        get { return Enum.GetName(typeof (OnlineMapsUpdateChannel), type); }
    }
}

public enum OnlineMapsUpdateChannel
{
    stable = 10,
    releaseCandidate = 20,
    beta = 30,
    alpha = 40,
    working = 50
}
