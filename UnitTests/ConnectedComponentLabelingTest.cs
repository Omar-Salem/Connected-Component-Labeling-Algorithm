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
        #region Member Variables

        private readonly string _baseDirectory, _inputDirectory;
        private string _outputDirectory;
        private readonly CCL _connectedComponentLabelling;

        #endregion

        #region Constructor

        public ConnectedComponentLabelingTest()
        {
            _baseDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
            _inputDirectory = Path.Combine(_baseDirectory, "Input");
            _connectedComponentLabelling = new CCL();
        }

        #endregion

        #region Test Initialize

        [TestInitialize()]
        public void Init()
        {
            _outputDirectory = Path.Combine(_baseDirectory, "Output");
        }

        #endregion

        #region Test Methods

        [TestMethod()]
        public void Process_OneEntityTest()
        {
            //Arrange
            Bitmap input = new Bitmap(Path.Combine(_inputDirectory, "One.bmp"));
            _outputDirectory = Path.Combine(_outputDirectory, "One");
            Directory.CreateDirectory(_outputDirectory);

            //Act
            var images = _connectedComponentLabelling.Process(input);

            foreach (var image in images)
            {
                image.Value.Save(Path.Combine(_outputDirectory, image.Key + ".bmp"));
            }

            //Assert
            Assert.AreEqual(1, images.Count);
        }

        [TestMethod()]
        public void Process_TwoEntitiesTest()
        {
            //Arrange
            Bitmap input = new Bitmap(Path.Combine(_inputDirectory, "Two.bmp"));
            _outputDirectory = Path.Combine(_outputDirectory, "Two");
            Directory.CreateDirectory(_outputDirectory);

            //Act
            var images = _connectedComponentLabelling.Process(input);

            foreach (var image in images)
            {
                image.Value.Save(Path.Combine(_outputDirectory, image.Key + ".bmp"));
            }

            //Assert
            Assert.AreEqual(2, images.Count);
        }

        [TestMethod()]
        public void Process_SixEntitiesTest()
        {
            //Arrange
            Bitmap input = new Bitmap(Path.Combine(_inputDirectory, "Six.bmp"));
            _outputDirectory = Path.Combine(_outputDirectory, "Six");
            Directory.CreateDirectory(_outputDirectory);

            //Act
            var images = _connectedComponentLabelling.Process(input);

            foreach (var image in images)
            {
                image.Value.Save(Path.Combine(_outputDirectory, image.Key + ".bmp"));
            }

            //Assert
            Assert.AreEqual(6, images.Count);
        }

        #endregion
    }
}