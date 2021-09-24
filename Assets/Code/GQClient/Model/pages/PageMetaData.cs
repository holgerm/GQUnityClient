using System.Xml;
using System.Xml.Serialization;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;

namespace Code.GQClient.Model.pages
{
    public class PageMetaData : Page
	{
        /// <summary>
        /// Needed by reflection e.g. in some apps.
        /// </summary>
        /// <param name="reader"></param>
		public PageMetaData(XmlReader reader) : base(reader) { }
        
        #region Runtime API
        public override bool CanStart ()
		{
			return false;
		}
		#endregion


		#region XML Serialization
		protected override void ReadContent (XmlReader reader, XmlRootAttribute xmlRootAttr)
		{
			switch (reader.LocalName) {
			case GQML.PAGE_METADATA_STRINGMETA:
				StringMetaData smde = new StringMetaData(reader);
				if (smde.Key == null)
					break;
				if (QuestManager.CurrentlyParsingQuest.metadata.ContainsKey (smde.Key)) {
					QuestManager.CurrentlyParsingQuest.metadata [smde.Key] = smde.Value;
				} else {
					QuestManager.CurrentlyParsingQuest.metadata.Add (smde.Key, smde.Value);
				}
				break;
			default:
				base.ReadContent (reader);
				break;
			}
		}
		#endregion

	}

	public class StringMetaData
	{
		#region Runtime API

		public int Id {
			get;
			protected set;
		}

		public string Key {
			get;
			protected set;
		}

		public string Value {
			get;
			protected set;
		}
		#endregion


		#region XML Serialization
		public StringMetaData (XmlReader reader)
		{
			GQML.AssertReaderAtStart (reader, GQML.PAGE_METADATA_STRINGMETA);

			// Read Attributes:
			Id = GQML.GetIntAttribute (GQML.ID, reader);
			Key = GQML.GetStringAttribute (GQML.PAGE_METADATA_STRINGMETA_KEY, reader);
			Value = GQML.GetStringAttribute (GQML.PAGE_METADATA_STRINGMETA_VALUE, reader);

			// consume the whole stringmeta tag:
			reader.ReadInnerXml ();
		}

        protected StringMetaData() { }
		#endregion

		#region Null
		public static NullStringMetaData Null = new NullStringMetaData ();

		public class NullStringMetaData : StringMetaData
		{

			internal NullStringMetaData ()
			{

			}
		}
		#endregion
	}
}
