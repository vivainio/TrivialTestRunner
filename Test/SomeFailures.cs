using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrivialTestRunner.Test
{
    class SomeFailures
    {
        public static List<int> WasRun = new List<int>();

        [Case]
        public static void Fail1()
        {
            WasRun.Add(1);
            Assert.IsTrue(false);
        }
        [Case]
        public static void Fail2()
        {
            WasRun.Add(2);
            Assert.IsTrue(false);
        }

        [Case]
        public static void Fail3()
        {
            WasRun.Add(3);

            Assert.AreEqual(10, 20);
           
        }

        [Case]
        public static async Task FailingTestWithTask()
        {
            Assert.IsTrue(false);
            await Task.CompletedTask;

        }
        [Case]
        public static async Task FailingInstanceTestWithTask()
        {
            Assert.IsTrue(true);
            await Task.CompletedTask;
        }
    }
}