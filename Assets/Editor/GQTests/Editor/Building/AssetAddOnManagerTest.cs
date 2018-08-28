using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using GQ.Client.Conf;
using GQ.Editor.Building;
using System.Collections.Generic;

namespace GQTests.Editor.Building
{

    public class AssetAddOnManagerTest
    {

        [Test]
        public void CalculateAddOnsToLoad()
        {
            Config oldConfig = new Config();
            oldConfig.assetAddOns = new string[] {
                "onlyOldA", "onlyOldB", "bothA", "bothB"
            };

            Config newConfig = new Config();
            newConfig.assetAddOns = new string[] {
                "onlyNewA", "onlyNewB", "bothA", "bothB"
            };

            List<string> aaosToLoad = AssetAddOnManager.calculateAAOsToLoad(oldConfig, newConfig);
            Assert.Contains("onlyNewA", aaosToLoad);
            Assert.Contains("onlyNewB", aaosToLoad);
            Assert.False(aaosToLoad.Contains("bothA"));
            Assert.False(aaosToLoad.Contains("bothB"));
        }

        [Test]
        public void CalculateAddOnsToUnload()
        {
            Config oldConfig = new Config();
            oldConfig.assetAddOns = new string[] {
                "onlyOldA", "onlyOldB", "bothA", "bothB"
            };

            Config newConfig = new Config();
            newConfig.assetAddOns = new string[] {
                "onlyNewA", "onlyNewB", "bothA", "bothB"
            };

            List<string> aaosToUnload = AssetAddOnManager.calculateAAOsToUnload(oldConfig, newConfig);
            Assert.Contains("onlyOldA", aaosToUnload);
            Assert.Contains("onlyOldB", aaosToUnload);
            Assert.False(aaosToUnload.Contains("bothA"));
            Assert.False(aaosToUnload.Contains("bothB"));
        }

    }
}