
using System;
using ImageSorter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilterableCollectionTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestFilterFunction()
        {
            FilterableCollection<int> test = new FilterableCollection<int>();
            for (int i = 0; i < 100; i++)
            {
                test.Add(i);
            }
            test.Filter(x => x < 50);
            foreach (int i in test)
            {
                if (i >= 50) Assert.Fail();
            }
            Assert.AreEqual(test.Count,50);
            test.Restore();
            Assert.AreEqual(test.Count,100);
            test.Restore();
            Assert.AreEqual(test.Count,100);
        }
    }
}
