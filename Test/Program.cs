using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TrivialTestRunner.Test
{
    class TestClass
    {
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

    }

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

    }

    class Program
    {
        static void Main(string[] args)
        {
            TRunner.AddTests<TestClass>();
            TRunner.RunTests();
            Assert.IsTrue(TestClass.WasRun.SequenceEqual(new[] { 1, 2, 3, 4 }));
            TRunner.ReportAll();
            Assert.AreEqual(0, TRunner.ExitStatus);
            TRunner.Clear();
            TRunner.AddTests<SomeFailures>();
            TRunner.RunTests();
            TRunner.ReportAll();
            Assert.IsTrue(SomeFailures.WasRun.SequenceEqual(new[] { 1, 2, 3 }));

            // CrashHard will raise exception all the way
            TRunner.CrashHard = true;

            bool ok = true;
            try
            {
                TRunner.RunTests();
                ok = false;

            }
            catch (TargetInvocationException e)
            {
                Assert.IsTrue(e.InnerException.GetType() == typeof(TestFailure));
                ok = true;
            }
            Assert.IsTrue(ok);

            Assert.AreEqual(3, TRunner.ExitStatus);

        }
    }
}
