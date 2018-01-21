# TrivialTestRunner
Dotnet Test Runner For The Rest Of Us (tm)

## Use case

You have a small library with a modest set of tests. You don't care about "test explorer" or whatnot, but rather want to have a simple console application to run your tests.  You don't want to add fat loaders scanning your assemblies because they slow down the debugger launch.

## Usage

In your test console application (where classes TestClass1 and TestClass2 contain the test methods)

```csharp
static void Main(string[] args)
{
    TRunner.AddTests<TestClass1>();
    TRunner.AddTests<TestClass2>();
    TRunner.RunTests();
    TRunner.ReportAll();
}
```

In your test classes:

```csharp
// normal case
[Case]
public static void Test1()
{
    Assert.IsTrue(true);
}

[Case]
public static void Test2()
{
    Assert.IsTrue(true);
}

// focused case
[fCase]
public static void Test3()
{
    Assert.IsTrue(true);
}

```

Note that one of the tests is "focused", i.e. fCase. If there are any focused tests in your set, only the focused tests will be run (yes, this means you don't need that "test explorer" to launch tests one by one)

If you want not to catch exceptions while running the tests (i.e. you want to trigger the debugger when it happens), do:

```csharp

// CrashHard will raise exception all the way
TRunner.CrashHard = true;
```