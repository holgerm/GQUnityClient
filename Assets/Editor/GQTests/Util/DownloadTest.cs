using UnityEngine;
using NUnit.Framework;
using Code.GQClient.Util.http;
using GQ.Editor.Util;
using UnityEngine.Networking;

namespace GQTests.Util
{
    public class DownloadTest
    {
        [Test]
        public void FileAccessViaWWW()
        {
            var filePath = Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "Util/Downloader/hello.txt");

            Assert.IsTrue(Files.ExistsFile(filePath), "File should exist at " + filePath);

            var url = Files.LocalPath4WWW(filePath);
            var www = new WWW(url);

            if (!string.IsNullOrEmpty(www.error))
            {
                var errMsg = www.error;
                www.Dispose();
                Assert.Fail();
            }
            else
            {
                Assert.AreEqual("Hello!", www.text);
            }
        }

        [Test]
        public void FileAccess()
        {
            var started = false;
            var succeeded = false;

            var filePath = Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "Util/Downloader/hello.txt");

            Assert.IsTrue(Files.ExistsFile(filePath), "File should exist at " + filePath);

            var downloader = new Downloader(Files.LocalPath4WWW(filePath), new DownloadHandlerBuffer());
            downloader.OnStart += (d, e) => { started = true; };
            downloader.OnSuccess += (d, e) => { succeeded = true; };
            downloader.OnError += (d, e) => { Assert.Fail("Download Error: " + e.Message); };

            var enumerator = downloader.RunAsCoroutine();
            while (enumerator.MoveNext())
            {
                Debug.Log("in while ... " + (enumerator.Current == null ? "null" : enumerator.Current.ToString()));
            }

            Assert.IsTrue(started, "Should have started the download.");
            Assert.IsTrue(succeeded, "Should have succeeded in downloading.");

            Assert.AreEqual("Hello!", downloader.Result);
        }

        [Test]
        public void DownloaderUsesCoroutine()
        {
            var d = new Downloader("some.url", new DownloadHandlerBuffer(), 60000);

            Assert.IsTrue(d.RunsAsCoroutine);
        }

        [Test]
        public void MultiDownloaderUsesCoroutine()
        {
            var md = new MultiDownloader();

            Assert.IsTrue(md.RunsAsCoroutine);
        }

        [Test]
        public void LocalFileLoader()
        {
            var filePath = Files.CombinePath(GQAssert.TEST_DATA_BASE_DIR, "Util/Downloader/hello.txt");

            var fileLoader = new LocalFileLoader(
                filePath: filePath,
                new DownloadHandlerBuffer()
            );

            fileLoader.Start();

            Assert.AreEqual("Hello!", fileLoader.Result);
        }
    }
}