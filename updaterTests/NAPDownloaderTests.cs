using Microsoft.VisualStudio.TestTools.UnitTesting;
using updater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace updater.Tests
{
    [TestClass()]
    public class NAPDownloaderTests
    {
        [TestMethod()]
        public void CheckerTest()
        {
            //arrange
            var download = new NAPDownloader();
            //act
            var downloader = download.Checker("David");
            //assert
            Assert.IsNotNull(downloader);
        }
    }
}