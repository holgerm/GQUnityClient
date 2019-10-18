using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System;
using System.Xml;
using GQ.Client.Util;

namespace GQ.Client.Model
{

    /// <summary>
    /// SLS Quests have a main page that relies on the MetaData page. Hence, its keys and values are stored in the globally accessible dictionary 
    /// QuestManager.CurrentlyParsingQuest.metadata<string, string>.
    /// 
    /// Thus, the model here is pretty simple.
    /// 
    /// The UI Controller does the whiole work using the global metadata dictionary.
    /// </summary>
    public class PageWCCPOI : PageMetaData
    {
        #region State

        public string Text { get; set; }
        public string KeyWords { get; set; }
        public string Necessities { get; set; }
        public string ImageUrl { get; set; }
        public string ImageRights { get; set; }

        #endregion


        #region XML Serialization
        public PageWCCPOI(XmlReader reader) : base(reader) { }

        protected override void ReadContent(XmlReader reader)
        {
            switch (reader.LocalName)
            {
                case GQML.PAGE_METADATA_STRINGMETA:
                    StringMetaData smde = new StringMetaData(reader);
                    if (smde.Key == null)
                        break;
                    if (!QuestManager.CurrentlyParsingQuest.metadata.ContainsKey(smde.Key))
                    {
                        switch (smde.Key)
                        {
                            case "content":
                                Text = smde.Value.StripQuotes();
                                break;
                            case "bild":
                                ImageUrl = smde.Value;
                                QuestManager.CurrentlyParsingQuest.AddMedia(ImageUrl);
                                break;
                            case "bildrechte":
                                ImageRights = smde.Value.StripQuotes();
                                break;
                            case "Material":
                                Necessities = smde.Value.StripQuotes();
                                break;
                            default:
                                if (QuestManager.CurrentlyParsingQuest.metadata.ContainsKey(smde.Key))
                                {
                                    QuestManager.CurrentlyParsingQuest.metadata[smde.Key] = smde.Value;
                                }
                                else
                                {
                                    QuestManager.CurrentlyParsingQuest.metadata.Add(smde.Key, smde.Value);
                                }
                                break;
                        }
                    }
                    break;
                default:
                    base.ReadContent(reader);
                    break;
            }
        }

        #endregion


        #region Runtime API

        public override bool CanStart()
        {
            return true;
        }

        #endregion


    }

}