using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.IO;
using ConnectedComponentLabeling;

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
            var images = target.Process(input);

            foreach (var image in images)
            {
                image.Value.Save(savePath + image.Key + ".bmp");
            }

            //Assert
            Assert.AreEqual(2, images.Count);
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