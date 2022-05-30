using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Code.GQClient.Util;
using UnityEngine;

namespace GQClient.Model
{

    public class TopicTreeReader
    {

        public static List<string> ReadTopicsFromMetadata(QuestInfo qi)
        {
            var topics = new List<string>();
            
            // read current metadata:
            if (qi.Metadata != null)
            {
                foreach (var md in qi.Metadata)
                {
                    ReadMd(md, topics);
                }
            }
            
            // BUGFIX: DO not take topics from new server metadata, before update, since it does not reflect current quest version
            // // read new server info metadata:
            // if (qi.NewVersionOnServer?.Metadata != null)
            // {
            //     foreach (var md in qi.NewVersionOnServer.Metadata)
            //     {
            //         ReadMd(md, topics);
            //     }
            // }

            return topics;
        }

        private static void ReadMd(MetaDataInfo md, ICollection<string> topics)
        {
            switch (md.Key)
            {
                case "topic":
                    var netVal = Regex.Unescape(md.Value.StripQuotes());
                    if (netVal != "" && !topics.Contains(netVal))
                    {
                        topics.Add(netVal);
                    }

                    break;
            }
        }
    }

}