using ConnectedComponentLabeling.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.IO;
using ConnectedComponentLabeling.Contracts;

namespace UnitTests
{
    [TestClass()]
    public class ConnectedComponentLabelingTest
    {
        string savePath = AppDomain.CurrentDomain.BaseDirectory + @"\Images\";

        [TestMethod()]
        public void ProcessTest()
        {
            //Arrange
            IConnectedComponentLabeling target = new CCL();
            Bitmap input = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\Test.bmp");

            //Act
            var actual = target.Process(input);
            for (int i = 0; i < actual.Count; i++)
            {
                actual[i].Save(savePath + (i + 1).ToString() + ".bmp");
            }

            //Assert
            Assert.IsTrue(actual.Count == 2);
        }

        [TestCleanup()]
        public void Clean()
        {
            string[] files = Directory.GetFiles(savePath);
            foreach (var img in files)
            {
                File.Delete(img);
            }
        }
    }
}