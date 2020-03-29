using System.Threading.Tasks;

namespace TrivialTestRunner.Test
{
    public class TestFocusedCases
    {
        [Case]
        public void NotRun1()
        {
            // crash because this should never be run
            Assert.IsTrue(false);
        }

        [Case]
        public void NotRun2()
        {
            // crash because this should never be run
            Assert.IsTrue(false);

        }

        [fCase]
        public static void Focused1()
        {
        }

        [fCase]
        public static Task Focused2()
        {
            return Task.CompletedTask;

        }

        [fCase]
        public Task Focused3()
        {
            return Task.CompletedTask;
        }
    }
}