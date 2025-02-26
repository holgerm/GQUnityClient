﻿// #define DEBUG_LOG

using System.Collections.Generic;
using System.Text;
using Code.GQClient.Conf;
using Code.GQClient.Err;
using GQClient.Model;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.UI.media;
using Code.GQClient.Util;
using Code.GQClient.Util.http;
using Code.GQClient.Util.tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.GQClient.UI.map
{
    public class QuestMarker : Marker
    {
        public QuestMarker(QuestInfo info)
        {
            Data = info;
        }

        public QuestInfo Data { get; set; }


        public override void OnTouchOMM()
        {
#if DEBUG_LOG
            Debug.Break();
#endif

            Data.Play();
        }

        public override void OnTouchOMM(OnlineMapsMarkerBase marker)
        {
            OnTouchOMM();
        }

        protected void Play()
        {
            Log.SignalErrorToDeveloper(
                "This Function should not be called and will be deleted soon. QuestMarker.Play()");

            if (Data == null)
            {
                Log.SignalErrorToDeveloper("Tried to play quest for QuestMarker without QuestInfo data.");
                return;
            }

            // Load quest data: game.xml
            var loadGameXml =
                new LocalFileLoader(
                    filePath: QuestManager.GetLocalPath4Quest(Data.Id) + QuestManager.QUEST_FILE_NAME, 
                    new DownloadHandlerBuffer()
                );
            _ = Base.Instance.GetDownloadBehaviour(
                loadGameXml,
                $"Loading {Config.Current.nameForQuestSg}"
            );

            var questStarter = new QuestStarter();

            var t =
                new TaskSequence(loadGameXml, questStarter);

            t.Start();
        }

        public override Texture2D Texture
        {
            get
            {
                var categoryId = Data.CurrentCategoryId;
                var textureId = "marker." + categoryId;
                var t = TextureManager.Instance.GetTexture(textureId);

                bool usedDefaultTexture = false;

                if (t == null)
                {
                    // load basic marker texture and white alpha background template:
                    var markerOutline = Config.Current.marker.GetTexture2D();
                    t = new Texture2D(markerOutline.width, markerOutline.height);

                    Texture2D symbol = null;
                    try
                    {
                        var cat = Config.Current.GetCategory(categoryId);
                        // ReSharper disable once MergeSequentialChecks
                        if (null == cat || null == cat.symbol)
                        {
                            Log.SignalErrorToDeveloper(
                                $"Category symbol texture not found for quest {Data.Id} named {Data.Name}. Using default symbol.");
                        }
                        else
                        {
                            symbol = cat.symbol.GetTexture2D();
                            if (symbol == null)
                            {
                                Log.SignalErrorToDeveloper(
                                    $"Symbol Texture not found for category {cat.symbol}. Using default symbol.");
                            }
                            else if (symbol.width > t.width)
                            {
                                Log.SignalErrorToDeveloper(
                                    "Symbol Texture too wide. Must not be wider than marker outline. Using default symbol.");
                                symbol = null;
                            }
                            else if (symbol.height > t.width)
                            {
                                Log.SignalErrorToDeveloper(
                                    "Symbol Texture too high. Must not be higher than marker outline width. Using default symbol.");
                                symbol = null;
                            }
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        usedDefaultTexture = true;
                        StringBuilder msg = new StringBuilder();
                        foreach (var key in RTConfig.Current.categoryDict.Keys)
                        {
                            msg.Append(key + "; ");
                        }


                        Log.SignalErrorToAuthor(
                            $"Quest {Data.Id}: Category " +
                            $"{categoryId} not found. Using default symbol. " +
                            $"Texture: t.height: {t.height}; cats# {RTConfig.Current.categoryDict.Keys.Count} : {msg}");
                    }

                    var outlineColors = markerOutline.GetPixels32();

                    var alphaBg = Resources.Load<Texture2D>(MARKER_ALPHA_BG_PATH);
                    var alphaColors = alphaBg.GetPixels32();

                    if (symbol == null)
                    {
                        // use default symbol:
                        symbol = Resources.Load<Texture2D>(RTImagePath.DEFAULT_CAT_IMAGE_PATH);
                    }

                    var symbolColors = symbol.GetPixels32();
                    var symbolXMin = (t.width - symbol.width) / 2;
                    var symbolXMax = (t.width + symbol.width) / 2 - 1;
                    var symbolYMin = t.height - (t.width + symbol.height) / 2;
                    var symbolYMax = t.height - (t.width - symbol.height) / 2 - 1;

                    var i = 0; // counter for fast access in flat color arrays for marker outline and alpha circle
                    var j = 0; // counter for symbol colors array (which is often smaller
                    for (var y = 0; y < markerOutline.height; y++)
                    {
                        for (var x = 0; x < markerOutline.width; x++)
                        {
                            if (symbolYMin <= y && y <= symbolYMax && symbolXMin <= x && x <= symbolXMax)
                            {
                                if (alphaColors[i].a == 255)
                                {
                                    if (symbolColors[j].a > 0)
                                    {
                                        // replace white base color with fg color:
                                        symbolColors[j].r = Config.Current.markerSymbolFGColor.r;
                                        symbolColors[j].g = Config.Current.markerSymbolFGColor.g;
                                        symbolColors[j].b = Config.Current.markerSymbolFGColor.b;

                                        // we take symbol pixels if we find them above the opaque white circle:
                                        alphaColors[i] = symbolColors[j];
                                    }
                                    else
                                    {
                                        // we take the marker background alpha as specified for this product:
                                        alphaColors[i].a = Config.Current.markerBGAlpha;
                                    }
                                }

                                j++;
                            }
                            else if (alphaColors[i].a == 255)
                            {
                                // outside of the symbol but inside the white circle, we also use only the specified transparency:
                                alphaColors[i].a = Config.Current.markerBGAlpha;
                            }

                            // colorize marker outline (keeping alpha channel):
                            outlineColors[i].r = Config.Current.markerColor.r;
                            outlineColors[i].g = Config.Current.markerColor.g;
                            outlineColors[i].b = Config.Current.markerColor.b;

                            // blend outline above alpha circle (already eventually including symbol):
                            outlineColors[i] = TextureManager.Blend(outlineColors[i], alphaColors[i]);

                            i++;
                        }
                    }

                    t.SetPixels32(outlineColors);
                    t.Apply();
                    // cache this marker texture, if not default used:
                    if (!usedDefaultTexture)
                        TextureManager.Instance.Add(textureId, t);
                }

                return t;
            }
        }
    }
}