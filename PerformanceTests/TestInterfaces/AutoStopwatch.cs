using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace PerformanceTests
{
    public class AutoStopwatch : IDisposable
    {
        private bool disposedValue;
        private readonly Stopwatch stopwatch;
        public string StepName { get; }
        public double ElapsedSeconds => this.stopwatch.Elapsed.TotalSeconds;
        private List<AutoStopwatch> children;

        public AutoStopwatch(string stepName)
        {
            this.children = new List<AutoStopwatch>();
            this.StepName = stepName;
            this.stopwatch = Stopwatch.StartNew();
        }

        public AutoStopwatch CreateChild(string prefix = "", string postfix = "", [CallerMemberName] string stepName = "")
        {
            var aw = new AutoStopwatch($"{this.StepName}.{prefix}{stepName}{postfix}");
            this.children.Add(aw);
            return aw;
        }

        public void Dispose()
        {
            if (!this.disposedValue)
            {
                this.stopwatch.Stop();
                disposedValue = true;
            }
            GC.SuppressFinalize(this);
        }

        public void Print(StringBuilder sb, int indent = 0)
        {
            sb.Append('\t', indent);
            sb.Append(this.StepName);
            sb.Append('\t');
            sb.AppendFormat("{0:n4}", this.ElapsedSeconds);
            sb.Append('\n');
            foreach (var c in this.children)
                c.Print(sb, indent + 1);
        }
    }
}
