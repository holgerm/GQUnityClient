using System.Collections.Generic;
using Code.GQClient.Conf;
using Code.GQClient.Util;

namespace GQClient.Model
{

    public class TopicTreeReader
    {

        public static List<string> ReadTopicsFromMetadata(MetaDataInfo[] metadata)
        {

            var topics = new List<string>();
            string netVal;
            foreach (var md in metadata)
            {
                switch (md.Key)
                {
                    case "topic":
                        netVal = md.Value.StripQuotes();
                        if (netVal != "")
                            topics.Add(netVal);
                        break;
                 }
            }
           return topics;
        }
    }

}