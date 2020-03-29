using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrivialTestRunner.Test
{
    class TestClass
    {
        public string InstanceString { get; set; } = "notset";
        public static List<int> WasRun = new List<int>();

        [Case]
        public static void OkTest1()
        {
            Assert.IsTrue(true);
            WasRun.Add(1);
        }

        [Case]
        public static void OkTest2()
        {
            WasRun.Add(2);
        }

        [Case]
        public static void OkTest3()
        {
            Assert.AreEqual(10, 10);
            Assert.AreEqual("hello", "hello");

            WasRun.Add(3);
        }

        [Case]
        public static void OkTest4()
        {
            WasRun.Add(4);
        }

        [Case]
        public void OkInstanceTest()
        {
            // woohoo, this is asserted to have been set
            InstanceString = "wasrun";
            Assert.IsTrue(true);
        }

        [Case]
        public async Task OkInstanceTestWithTask()
        {
            Assert.IsTrue(true);
            await Task.CompletedTask;
        }
    }
}