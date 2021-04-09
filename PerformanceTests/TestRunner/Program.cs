using PerformanceTests.Calculations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PerformanceTests.TestRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var testsToRun = SelectTests();

            var aw = new AutoStopwatch("Main");
            using (aw)
            {
                foreach (var t in testsToRun)
                {
                    t.Run(aw);
                }
            }

            var text = new StringBuilder();
            aw.Print(text);
            Console.WriteLine(text.ToString());
        }

        private static List<ITest> SelectTests()
        {
            var toLoad = new FileInfo(typeof(Program).Assembly.Location).Directory.GetFiles("*.dll");
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var fi in toLoad)
            {
                if (!loadedAssemblies.Any(a => a.GetName().Name + fi.Extension == fi.Name))
                {
                    Assembly.LoadFile(fi.FullName);
                }
            }
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var tests = new List<ITest>();
            foreach (var a in assemblies)
            {
                foreach (var t in a.GetTypes())
                {
                    if (t.GetInterfaces().Contains(typeof(ITest)))
                    {
                        tests.Add((ITest)Activator.CreateInstance(t));
                    }
                }
            }

            for (int i = 0; i < tests.Count; i++)
            {
                Console.WriteLine("{0,4} {1}", i, tests[i].Name);
            }

            Console.WriteLine("Please select the tests to run. Select multiple tests with space or type 'all' to run all tests.");
            Console.Write("Tests to run: ");
            var input = Console.ReadLine().Trim().ToLower();
            if (input == "all")
                return tests;

            var testsToRun = new List<ITest>();
            return input.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(s =>
                {
                    if(int.TryParse(s, out var i))
                        return i;
                    return -1;
                })
                .Where(i => i >= 0 && i < tests.Count)
                .Select(i => tests[i])
                .ToList();
        }
    }
}
