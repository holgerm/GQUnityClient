using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Code.GQClient.Conf;
using Newtonsoft.Json;
using GQ.Editor.UI;

namespace GQTests.Editor.JSON
{
    public class JSONConfigTest
    {
        [Test]
        public void JSONColorConverterTest()
        {
            Config configOrigin = new Config();

            Color32 c1 = new Color32(2, 3, 4, 5);
            configOrigin.headerBgColor = c1;

            string json = JsonConvert.SerializeObject(configOrigin, Formatting.Indented);

            Config configRead = Config._doDeserializeConfig(json);

            Color32 r1 = configRead.headerBgColor;

            Assert.AreEqual(c1.r, r1.r);
            Assert.AreEqual(c1.g, r1.g);
            Assert.AreEqual(c1.b, r1.b);
            Assert.AreEqual(c1.a, r1.a);
        }


        [Test]
        public void JSON_AndroidSdkVersions_Converter_Serialize()
        {
            Config configOrigin = new Config();

            configOrigin.androidMinSDKVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            string json = JsonConvert.SerializeObject(configOrigin, Formatting.Indented);
            Assert.That(json.Contains(@"""androidMinSDKVersion"": ""AndroidApiLevelAuto"","));

            configOrigin.androidMinSDKVersion = AndroidSdkVersions.AndroidApiLevel19;
            json = JsonConvert.SerializeObject(configOrigin, Formatting.Indented);
            Assert.That(json.Contains(@"""androidMinSDKVersion"": ""AndroidApiLevel19"","));

            configOrigin.androidMinSDKVersion = AndroidSdkVersions.AndroidApiLevel26;
            json = JsonConvert.SerializeObject(configOrigin, Formatting.Indented);
            Assert.That(json.Contains(@"""androidMinSDKVersion"": ""AndroidApiLevel26"","));
        }

        [Test]
        public void JSON_AndroidSdkVersions_Converter_Deserialize()
        {
            Config configOrigin = new Config();
            configOrigin.androidMinSDKVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            string json = JsonConvert.SerializeObject(configOrigin, Formatting.Indented);

            Config configRead = Config._doDeserializeConfig(json);
            Assert.AreEqual(
                AndroidSdkVersions.AndroidApiLevelAuto,
                configRead.androidMinSDKVersion
            );

            configOrigin.androidMinSDKVersion = AndroidSdkVersions.AndroidApiLevel19;
            json = JsonConvert.SerializeObject(configOrigin, Formatting.Indented);

            configRead = Config._doDeserializeConfig(json);
            Assert.AreEqual(
                AndroidSdkVersions.AndroidApiLevel19,
                configRead.androidMinSDKVersion
            );
            configOrigin.androidMinSDKVersion = AndroidSdkVersions.AndroidApiLevel26;
            json = JsonConvert.SerializeObject(configOrigin, Formatting.Indented);

            configRead = Config._doDeserializeConfig(json);
            Assert.AreEqual(
                AndroidSdkVersions.AndroidApiLevel26,
                configRead.androidMinSDKVersion
            );
        }

        [Test]
        public void NextPaletteColor()
        {
            Color32 c1 = new Color(1f, 1f, 1f, 1f);
            Color32 c2 = new Color(2f, 2f, 2f, 2f);
            Color32 c3 = new Color(3f, 3f, 3f, 3f);
            Color32 c4 = new Color(4f, 4f, 4f, 4f);
            var colors =
                new Color32[]
                {
                    c1, c2, c3, c4,
                    Color.black, Color.black, Color.black, Color.black, Color.black, Color.black
                };
            var config = new Config
            {
                colorPaletteSize = 4,
                colorPalette = colors
            };
            Assert.AreEqual(colors, config.colorPalette);
            
            Assert.AreEqual(0, Config._paletteColorCounter);

            Assert.AreEqual(c1, config.NextPaletteColor);
            Assert.AreEqual(1, Config._paletteColorCounter);

            Assert.AreEqual(c2, config.NextPaletteColor);
            Assert.AreEqual(2, Config._paletteColorCounter);
            Assert.AreEqual(c3, config.NextPaletteColor);
            Assert.AreEqual(3, Config._paletteColorCounter);
            Assert.AreEqual(c4, config.NextPaletteColor);
           
            Assert.AreEqual(0, Config._paletteColorCounter);
            Assert.AreEqual(c1, config.NextPaletteColor);
            Assert.AreEqual(1, Config._paletteColorCounter);
            Assert.AreEqual(c2, config.NextPaletteColor);
            Assert.AreEqual(2, Config._paletteColorCounter);
            Assert.AreEqual(c3, config.NextPaletteColor);
            Assert.AreEqual(3, Config._paletteColorCounter);
            Assert.AreEqual(c4, config.NextPaletteColor);

            Assert.AreEqual(0, Config._paletteColorCounter);
            Assert.AreEqual(c1, config.NextPaletteColor);
            Assert.AreEqual(1, Config._paletteColorCounter);
        }
    }
}