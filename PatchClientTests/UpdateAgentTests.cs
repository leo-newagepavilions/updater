using Microsoft.VisualStudio.TestTools.UnitTesting;
using Updater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Updater.Tests
{
    [TestClass()]
    public class UpdateAgentTests
    {
        [TestMethod()]
        public void CheckupZipCreatedDateTimeTest()
        {
            //arrange
            var updater = new UpdateAgent();

            //act
            var datetime = updater.CheckupZipCreatedDateTime(@"C:\Users\Pavilion\Documents\test\test1.1.01.zip");

            //assert
            Assert.IsNotNull(datetime);


        }

        [TestMethod()]
        public void RunScriptTest()
        {
            //arrange
            var updater = new UpdateAgent();

            //act
            string[] arguments = new string[] { @"C:\NAP", @"C:\MarketName_Patches\PatchTester\Patches" };
            
            var runner = updater.RunScript(@"C:\MarketName_Patches\PatchTester\testshell.ps1", arguments);


            Assert.Fail();
        }
    }
}