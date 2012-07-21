using PatternRecognition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace UnitTests
{
    [TestClass()]
    public class PreProcessTest
    {
        string savePath = AppDomain.CurrentDomain.BaseDirectory + @"\Images\";

        [TestMethod()]
        public void ExtractTest()
        {
            Bitmap input = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\Test.bmp");
            PreProcess target = new PreProcess(input);
            List<Bitmap> actual = target.Extract();
            for (int i = 0; i < actual.Count; i++)
            {
                actual[i].Save(savePath + (i + 1).ToString() + ".bmp");
            }
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
