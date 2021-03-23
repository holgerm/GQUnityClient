using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Code.GQClient.Err;
using Code.GQClient.Model.gqml;
using Code.GQClient.Model.mgmt.quests;
using Code.GQClient.Util;
using UnityEngine;

namespace Code.GQClient.Model.actions
{
    public class ActionUploadFile : Action
    {
        private string Fileref { get; set; }
        
        private string NamePrefix { get; set; }
        private string Method { get; set; }
        private string Url { get; set; }
        private string Auth { get; set; }

        public ActionUploadFile(XmlReader reader) : base(reader)
        {
        }

        protected override void ReadAttributes(XmlReader reader)
        {
            Fileref = GQML.GetStringAttribute(GQML.ACTION_UPLOADFILE_FILEREF, reader);
            NamePrefix = GQML.GetStringAttribute(GQML.ACTION_UPLOADFILE_NAMEPREFIX, reader);
            Method = GQML.GetStringAttribute(GQML.ACTION_UPLOADFILE_METHOD, reader);
            Url = GQML.GetStringAttribute(GQML.ACTION_UPLOADFILE_URL, reader);
            Auth = GQML.GetStringAttribute(GQML.ACTION_UPLOADFILE_AUTH, reader);
        }

        public override void Execute()
        {
            if (Fileref == null || Url == null || Auth == null)
                return;

            
            UploadFile(Fileref, NamePrefix.MakeReplacements(), Url, Auth);
        }

        private static async Task UploadFile(
            string filename, string filenamePrefix, string urlBase, string auth)
        {
            if (QuestManager.Instance.CurrentPage.Parent.MediaStore.TryGetValue(
                GQML.PREFIX_RUNTIME_MEDIA + filename,
                out var rtMediaInfo))
            {
                string filePath = rtMediaInfo.LocalPath;

                if (File.Exists(filePath))
                {
                    byte[] fileData = File.ReadAllBytes(filePath);
                    HttpContent data = new ByteArrayContent(fileData);
                    var url = urlBase + filenamePrefix + rtMediaInfo.LocalFileName;
                    using (var client = new HttpClient())
                    {
                        string token = $"Basic {auth}";
                        Debug.Log($"Going to upload file with url: {url}");
                        client.DefaultRequestHeaders.Add("Authorization", token);
                        var response = await client.PutAsync(url, data);

                        string result = response.Content.ReadAsStringAsync().Result;
                        Debug.Log($"Result: {result}");
                    }
                }
            }
            else
            {
                Log.SignalErrorToDeveloper($"FileUpload did not work, local file not found: {filename}");
            }
        }
    }
}