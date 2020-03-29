using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static TrivialTestRunner.TestEntry;

namespace TrivialTestRunner
{
    public class Case : Attribute { }
    public class fCase : Case { }
    public class TestFailure : Exception {
        public TestFailure(string message) : base(message) { }
        
    }
    
    public static class Assert
    {
        public static void IsTrue(bool v)
        {
            if (!v) throw new TestFailure("Assert not true");
        }
        public static void AreEqual<T>(T expected, T actual) 
        {

            if (!object.Equals(expected, actual)) throw new TestFailure($"Assert.AreEqual - expected [{expected}] got [{actual}]");
        }
    }

    [DebuggerDisplay("{Mi} {Kind}")]
    public class TestEntry
    {
        public enum TestKind
        {
            None,
            Focused,
            Normal
        }

        public TestKind Kind { get; set; }
        public MethodInfo Mi { get; set; }
        // the single instance of the class. If not static
        public object Instance { get; set; }
        public string Name => $"{Mi?.ReflectedType?.Name}.{Mi?.Name}";
    }

    public class TestResult
    {
        public TestEntry Entry;
        public bool Failed;
        public string Message;
        public string Summary => (Failed ? "FAIL " : "OK   ") + Entry.Name;
    }


    public static class TRunner
    {
        public static List<TestEntry> Entries = new List<TestEntry>();
        public static List<TestResult> Results = new List<TestResult>();
        public static bool CrashHard = false;
        
        public static Dictionary<Type, object> TestFixtures { get; }= new Dictionary<Type, object>();

        public static T GetTestFixture<T>() where T: class
        {
            return TestFixtures[typeof(T)] as T;
        } 
        public static void Clear()
        {
            Entries.Clear();
            Results.Clear();
        }
        private static TestEntry[] DiscoverTests<T>() where T: new()
        {
            TestKind resolveTestKind(IReadOnlyList<Case> attrs)
            {
                if (!attrs.Any())
                {
                    return TestKind.None;
                }
                if (attrs.Any(a => a is fCase))
                {
                    return TestKind.Focused;
                }
                return TestKind.Normal;
            }


            var klass = typeof(T);
            var instance = new T();
            TestFixtures.Add(typeof(T), instance);
            
            var testEntries = klass.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Select(mi => (mi: mi, kind: resolveTestKind(mi.GetCustomAttributes<Case>().ToList())))
                .Where(pair => pair.kind != TestKind.None)
                .Select(pair => new TestEntry
                {
                    Kind = pair.kind,
                    Mi = pair.mi,
                    Instance = instance

                }).ToArray();
            return testEntries;

        }
        public static void AddTests<T>() where T: new()
        {
            var tests = DiscoverTests<T>();
            Entries.AddRange(tests);

        }

        private static async Task<bool> RunOneTest(TestEntry te)

        {
            async Task InvokeIt()
            {
                var instance = te.Mi.IsStatic ? null : te.Instance;
                var ret = te.Mi.Invoke(instance, null);
                if (ret == null)
                {
                    return;
                }

                var asTask = ret as Task;
                if (asTask != null)
                {
                    await asTask;
                }

            }
            if (CrashHard)
            {
                await InvokeIt();
                return true;
            }

            try
            {
                await InvokeIt();
                Results.Add(new TestResult
                {
                    Entry = te,
                    Failed = false,
                    Message = null

                });

                return true;

            } catch (Exception e)
            {
                Results.Add(new TestResult
                {
                    Entry = te,
                    Failed = true,
                    Message = e.InnerException?.Message + " " + e.InnerException?.StackTrace.ToString()
                });
                return false;
            }
        }


        private static async Task<bool> RunTestList(IEnumerable<TestEntry> testList)
        {
            var listOk = true;
            foreach (var test in testList)
            {
                var ok = await RunOneTest(test);
                if (!ok)
                {
                    Console.WriteLine($"Fail {test.Name}");
                    listOk = false;

                }
            }
            return listOk;
        }
        public static async Task RunTestsAsync()
        {
            var focused = Entries.Where(te => te.Kind == TestKind.Focused).ToArray();
            if (focused.Length > 0)
            {
                await RunTestList(focused);
            }
            else
            {
                await RunTestList(Entries);
            }


            foreach (var tr in Results.Where(r => r.Failed))
            {
                Console.WriteLine($"Failed {tr.Entry.Name}\n{tr.Message}");
            }
        }

        public static void ReportAll()
        {
            Report(Results);
        }


        public static void Report(IEnumerable<TestResult> results) 
        {
            var strings = results.Select(r => r.Summary);
            Console.WriteLine(String.Join("\n", strings));

        }
        // count of errors. Usable is exit code when returning from main
        public static int ExitStatus => Results.Count(r => r.Failed);
    }
}
