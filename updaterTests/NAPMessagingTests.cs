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
    public class NAPMessagingTests
    {
        [TestMethod()]
        public void GetAllUpdaterStateTest()
        {
            //arrange
            var messager = new NAPMessaging();
            //action
            messager.GetAllUpdaterState();


            //assert
            Assert.Fail();
        }

        [TestMethod()]
        public async Task PostUpdaterStateTest()
        {
            //arrange
            var param = new NAPUpdateModel();
            param.MarketId = "2";
           
            param.McpId = 6;

            var messager = new NAPMessaging();
            //action
            //var result = await messager.PostUpdaterState(param);


            //assert
            //Assert.IsTrue(result);
        }
    }
}