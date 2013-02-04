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

        #region Test Methods

        [TestMethod()]
        public void Process_OneEntityTest()
        {
            //Arrange
            IConnectedComponentLabeling target = new CCL();
            Bitmap input = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\One.bmp");

            //Act
            var images = target.Process(input);

            foreach (var image in images)
            {
                image.Value.Save(savePath + image.Key + ".bmp");
            }

            //Assert
            Assert.AreEqual(1, images.Count);
        }

        [TestMethod()]
        public void Process_TwoEntitiesTest()
        {
            //Arrange
            IConnectedComponentLabeling target = new CCL();
            Bitmap input = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\Two.bmp");

            //Act
            var images = target.Process(input);

            foreach (var image in images)
            {
                image.Value.Save(savePath + image.Key + ".bmp");
            }

            //Assert
            Assert.AreEqual(2, images.Count);
        }

        [TestMethod()]
        public void Process_SixEntitiesTest()
        {
            //Arrange
            IConnectedComponentLabeling target = new CCL();
            Bitmap input = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\Six.bmp");

            //Act
            var images = target.Process(input);

            foreach (var image in images)
            {
                image.Value.Save(savePath + image.Key + ".bmp");
            }

            //Assert
            Assert.AreEqual(6, images.Count);
        } 

        #endregion

        #region Cleanup

        [TestCleanup()]
        public void Clean()
        {
            string[] files = Directory.GetFiles(savePath);
            foreach (var img in files)
            {
                File.Delete(img);
            }
        } 

        #endregion
    }
}