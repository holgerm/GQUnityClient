/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

/// <summary>
/// This class is used to request to Open Street Map Overpass API.\n
/// You can create a new instance using OnlineMapsOSMAPIQuery.Find.\n
/// Open Street Map Overpass API documentation: http://wiki.openstreetmap.org/wiki/Overpass_API/Language_Guide \n
/// You can test your queries using: http://overpass-turbo.eu/ \n
/// </summary>
[System.Serializable]
public class OnlineMapsOSMAPIQuery: OnlineMapsGoogleAPIQuery
{
    /// <summary>
    /// Gets the type of query to API.
    /// </summary>
    public override OnlineMapsQueryType type
    {
        get { return OnlineMapsQueryType.osm; }
    }

    /// <summary>
    /// Constructor.
    /// Use OnlineMapsOSMAPIQuery.Find for create request.
    /// </summary>
    /// <param name="data">Overpass QL request</param>
    public OnlineMapsOSMAPIQuery(string data)
    {
        _status = OnlineMapsQueryStatus.downloading;
        string url = "http://overpass.osm.rambler.ru/cgi/interpreter?data=" + data;
#if UNITY_WEBPLAYER
        www = new WWW("http://service.infinity-code.com/redirect.php?" + url);
#else
        www = new WWW(url);
#endif
    }

    /// <summary>
    /// Query the Open Street Map Overpass API.
    /// </summary>
    /// <param name="data">Overpass QL request</param>
    /// <returns>Instance of the query</returns>
    public static OnlineMapsOSMAPIQuery Find(string data)
    {
        OnlineMapsOSMAPIQuery query = new OnlineMapsOSMAPIQuery(data);
        OnlineMaps.instance.AddGoogleAPIQuery(query);
        return query;
    }

    /// <summary>
    /// Get data from the response Open Street Map Overpass API.
    /// </summary>
    /// <param name="response">Response from Overpass API</param>
    /// <param name="nodes">List of nodes</param>
    /// <param name="ways">List of ways</param>
    /// <param name="relations">List of relations</param>
    public static void ParseOSMResponse(string response, out List<OnlineMapsOSMNode> nodes, out List<OnlineMapsOSMWay> ways, out List<OnlineMapsOSMRelation> relations)
    {
        XmlDocument document = new XmlDocument();
        document.LoadXml(response);

        nodes = new List<OnlineMapsOSMNode>();
        ways = new List<OnlineMapsOSMWay>();
        relations = new List<OnlineMapsOSMRelation>();

        if (document.DocumentElement == null) return;
        foreach (XmlNode node in document.DocumentElement.ChildNodes)
        {
            if (node.Name == "node") nodes.Add(new OnlineMapsOSMNode(node));
            else if (node.Name == "way") ways.Add(new OnlineMapsOSMWay(node));
            else if (node.Name == "relation") relations.Add(new OnlineMapsOSMRelation(node));
        }
    }
}

/// <summary>
/// The base class of Open Streen Map element.
/// </summary>
public class OnlineMapsOSMBase
{
    /// <summary>
    /// Element ID
    /// </summary>
    public string id;

    /// <summary>
    /// Element tags
    /// </summary>
    public List<OnlineMapsOSMTag> tags;

    public bool Equals(OnlineMapsOSMBase other)
    {
        if (ReferenceEquals(other, null)) return false;
        if (ReferenceEquals(this, other)) return true;
        return id == other.id;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    /// <summary>
    /// Get tag value for the key.
    /// </summary>
    /// <param name="key">Tag key</param>
    /// <returns>Tag value</returns>
    public string GetTagValue(string key)
    {
        List<OnlineMapsOSMTag> curTags = tags.Where(tag => tag.key == key).ToList();
        if (curTags.Count > 0) return curTags[0].value;
        return string.Empty;
    }

    /// <summary>
    /// Checks for the tag with the specified key and value.
    /// </summary>
    /// <param name="key">Tag key</param>
    /// <param name="value">Tag value</param>
    /// <returns>True - if successful, False - otherwise.</returns>
    public bool HasTag(string key, string value)
    {
        return tags.Any(t => t.key == key && t.value == value);
    }

    /// <summary>
    /// Checks for the tag with the specified keys.
    /// </summary>
    /// <param name="keys">Tag keys.</param>
    /// <returns>True - if successful, False - otherwise.</returns>
    public bool HasTagKey(params string[] keys)
    {
        return keys.Any(key => tags.Any(t => t.key == key));
    }

    /// <summary>
    /// Checks for the tag with the specified values.
    /// </summary>
    /// <param name="values">Tag values</param>
    /// <returns>True - if successful, False - otherwise.</returns>
    public bool HasTagValue(params string[] values)
    {
        return values.Any(val => tags.Any(t => t.value == val));
    }

    /// <summary>
    /// Checks for the tag with the specified key and values.
    /// </summary>
    /// <param name="key">Tag key</param>
    /// <param name="values">Tag values</param>
    /// <returns>True - if successful, False - otherwise.</returns>
    public bool HasTags(string key, params string[] values)
    {
        return tags.Any(tag => tag.key == key && values.Any(v => v == tag.value));
    }
}

/// <summary>
/// Open Street Map node element class
/// </summary>
public class OnlineMapsOSMNode : OnlineMapsOSMBase
{
    /// <summary>
    /// Latitude
    /// </summary>
    public readonly float lat;

    /// <summary>
    /// Longitude
    /// </summary>
    public readonly float lon;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="node">Node</param>
    public OnlineMapsOSMNode(XmlNode node)
    {
        id = node.Attributes["id"].Value;
        lat = float.Parse(node.Attributes["lat"].Value);
        lon = float.Parse(node.Attributes["lon"].Value);

        tags = new List<OnlineMapsOSMTag>();

        foreach (XmlNode subNode in node.ChildNodes) tags.Add(new OnlineMapsOSMTag(subNode));
    }
}

/// <summary>
/// Open Street Map way element class
/// </summary>
public class OnlineMapsOSMWay : OnlineMapsOSMBase
{
    /// <summary>
    /// List of node id;
    /// </summary>
    public readonly List<string> nodeRefs;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="node">Node</param>
    public OnlineMapsOSMWay(XmlNode node)
    {
        id = node.Attributes["id"].Value;
        nodeRefs = new List<string>();
        tags = new List<OnlineMapsOSMTag>();

        foreach (XmlNode subNode in node.ChildNodes)
        {
            if (subNode.Name == "nd") nodeRefs.Add(subNode.Attributes["ref"].Value);
            else if (subNode.Name == "tag") tags.Add(new OnlineMapsOSMTag(subNode));
        }
    }
}

/// <summary>
/// Open Street Map relation element class
/// </summary>
public class OnlineMapsOSMRelation : OnlineMapsOSMBase
{
    /// <summary>
    /// List members of relation
    /// </summary>
    public readonly List<OnlineMapsOSMRelationMember> members;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="node">Node</param>
    public OnlineMapsOSMRelation(XmlNode node)
    {
        id = node.Attributes["id"].Value;
        members = new List<OnlineMapsOSMRelationMember>();
        tags = new List<OnlineMapsOSMTag>();

        foreach (XmlNode subNode in node.ChildNodes)
        {
            if (subNode.Name == "member") members.Add(new OnlineMapsOSMRelationMember(subNode));
            else if (subNode.Name == "tag") tags.Add(new OnlineMapsOSMTag(subNode));
        }
    }
}

/// <summary>
/// Open Street Map relation member class
/// </summary>
public class OnlineMapsOSMRelationMember
{
    /// <summary>
    /// ID of reference element
    /// </summary>
    public readonly string reference;

    /// <summary>
    /// Member role
    /// </summary>
    public readonly string role;

    /// <summary>
    /// Member type
    /// </summary>
    public readonly string type;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="node">Node</param>
    public OnlineMapsOSMRelationMember(XmlNode node)
    {
        type = node.Attributes["type"].Value;
        reference = node.Attributes["ref"].Value;
        role = node.Attributes["role"].Value;
    }
}

/// <summary>
/// Open Street Map element tag class
/// </summary>
public class OnlineMapsOSMTag
{
    /// <summary>
    /// Tag key
    /// </summary>
    public readonly string key;

    /// <summary>
    /// Tag value
    /// </summary>
    public readonly string value;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="node">Node</param>
    public OnlineMapsOSMTag(XmlNode node)
    {
        key = node.Attributes["k"].Value;
        value = node.Attributes["v"].Value;
    }
}