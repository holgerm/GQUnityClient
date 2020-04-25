using System.Collections.Generic;
using Code.GQClient.Conf;
using Code.GQClient.Util;

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
            
            // read new server info metadata:
            if (qi.NewVersionOnServer?.Metadata != null)
            {
                foreach (var md in qi.NewVersionOnServer.Metadata)
                {
                    ReadMd(md, topics);
                }
            }

            return topics;
        }

        private static void ReadMd(MetaDataInfo md, ICollection<string> topics)
        {
            switch (md.Key)
            {
                case "topic":
                    var netVal = md.Value.StripQuotes();
                    if (netVal != "" && !topics.Contains(netVal))
                        topics.Add(netVal);
                    break;
            }
        }
    }

}