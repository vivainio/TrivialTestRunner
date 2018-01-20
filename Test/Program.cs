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
            WasRun.Add(1);
        }

        [Case]
        public static void OkTest2()
        {
            WasRun.Add(2);
        }
    }

    class TwoFails
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

    }

    class Program
    {
        static void Main(string[] args)
        {
            TRunner.AddTests<TestClass>();
            TRunner.RunTests();
            Assert.IsTrue(TestClass.WasRun.SequenceEqual(new[] { 1, 2 }));
            TRunner.ReportAll();

            TRunner.Clear();
            TRunner.AddTests<TwoFails>();
            TRunner.RunTests();
            TRunner.ReportAll();
            Assert.IsTrue(TwoFails.WasRun.SequenceEqual(new[] { 1, 2 }));

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
        }
    }
}
