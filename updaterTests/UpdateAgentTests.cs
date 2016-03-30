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
        public void UnzipPackageTest()
        {
            //arrange
            var unzip = new UpdateAgent();

            //act            
            unzip.UnzipPackage(@"C:\Users\Pavilion\Documents\test\test.zip", @"C:\Users\Pavilion\Documents\unzip");
           
            //assert
            Assert.IsNotNull(true);
        }
    }
}