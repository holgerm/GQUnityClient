using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TellImageColor : MonoBehaviour
{

    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("COLOR: " + image.color.ToString());
    }
}
