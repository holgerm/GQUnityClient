/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/// <summary>
/// Step of the route found by the OnlineMapsFindDirection.
/// </summary>
public class OnlineMapsDirectionStep
{
    /// <summary>
    /// The total distance covered by this step.
    /// </summary>
    public int distance;

    /// <summary>
    /// The total duration of the passage of this step.
    /// </summary>
    public int duration;

    /// <summary>
    /// Location of the endpoint of this step (lat, lng). 
    /// </summary>
    public Vector2 end;

    /// <summary>
    /// Instructions to the current step.
    /// </summary>
    public string instructions;

    /// <summary>
    /// Maneuver the current step.
    /// </summary>
    public string maneuver;

    /// <summary>
    /// A list of locations of points included in the current step.
    /// </summary>
    public List<Vector2> points;

    /// <summary>
    /// Location of the startpoint of this step (lat, lng). 
    /// </summary>
    public Vector2 start;

    /// <summary>
    /// Constructor. \n
    /// <strong>Please do not use. </strong>\n
    /// Use OnlineMapsDirectionStep.TryParse.
    /// </summary>
    /// <param name="node">XMLNode of route</param>
    public OnlineMapsDirectionStep(XmlNode node)
    {
        start = node.GetLatLng("start_location");
        end = node.GetLatLng("end_location");
        duration = node.GetInt("duration/value");
        instructions = node.SelectSingleNode("html_instructions").InnerText;
        distance = node.GetInt("distance/value");

        XmlNode maneuverNode = node.SelectSingleNode("maneuver");
        if (maneuverNode != null) maneuver = maneuverNode.InnerText;
        
        XmlNode encodedPoints = node.SelectSingleNode("polyline/points");
        if (encodedPoints != null) points = OnlineMapsGoogleAPIQuery.DecodePolylinePoints(encodedPoints.InnerText);
    }

    /// <summary>
    /// Converts the route obtained by OnlineMapsFindDirection, a list of the steps of the route.
    /// </summary>
    /// <param name="route">Route obtained by OnlineMapsFindDirection.</param>
    /// <returns>List of OnlineMapsDirectionStep or null.</returns>
    public static List<OnlineMapsDirectionStep> TryParse(string route)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(route);

            XmlNode direction = doc.SelectSingleNode("//DirectionsResponse");
            if (direction == null) return null;

            XmlNode status = direction.SelectSingleNode("status");
            if (status == null || status.InnerText != "OK") return null;

            XmlNode legNode = direction.SelectSingleNode("route/leg");
            if (legNode == null) return null;

            XmlNodeList stepNodes = legNode.SelectNodes("step");
            if (stepNodes == null) return null;

            List<OnlineMapsDirectionStep> steps = new List<OnlineMapsDirectionStep>();

            foreach (XmlNode step in stepNodes)
            {
                OnlineMapsDirectionStep navigationStep = new OnlineMapsDirectionStep(step);
                steps.Add(navigationStep);
            }

            return steps;
        }
        catch { }

        return null;
    }

    /// <summary>
    /// Converts a list of the steps of the route to list of point locations.
    /// </summary>
    /// <param name="steps">List of the steps of the route.</param>
    /// <returns>A list of locations of route.</returns>
    public static List<Vector2> GetPoints(List<OnlineMapsDirectionStep> steps)
    {
        List<Vector2> routePoints = new List<Vector2>();

        foreach (OnlineMapsDirectionStep step in steps)
        {
            if (routePoints.Count > 0) routePoints.RemoveAt(routePoints.Count - 1);
            routePoints.AddRange(step.points);
        }

        return routePoints;
    }
}