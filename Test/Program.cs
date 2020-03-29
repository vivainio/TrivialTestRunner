using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TrivialTestRunner.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            TRunner.AddTests<TestClass>();
            await TRunner.RunTestsAsync();
            // to see object was really created and modified
            Assert.AreEqual(TRunner.GetTestFixture<TestClass>().InstanceString, "wasrun");
            Assert.IsTrue(TestClass.WasRun.SequenceEqual(new[] {1, 2, 3, 4, 5, 6}));
            TRunner.ReportAll();
            Assert.AreEqual(0, TRunner.ExitStatus);
            TRunner.Clear();
            
            TRunner.AddTests<TestFocusedCases>();
            await TRunner.RunTestsAsync();
            TRunner.ReportAll();
            TRunner.Clear();            
            TRunner.AddTests<SomeFailures>();
            await TRunner.RunTestsAsync();
            TRunner.ReportAll();
            Assert.IsTrue(SomeFailures.WasRun.SequenceEqual(new[] {1, 2, 3, 4, 5}));
            
            // CrashHard will raise exception all the way
            TRunner.CrashHard = true;

            bool ok = true;
            try
            {
                await TRunner.RunTestsAsync();
                ok = false;
            }
            catch (TargetInvocationException e)
            {
                Assert.IsTrue(e.InnerException?.GetType() == typeof(TestFailure));
                ok = true;
            }

            Assert.IsTrue(ok);

            Assert.AreEqual(4, TRunner.ExitStatus);
            
            
        }
    }
}