using System;
using System.Collections.Generic;
using System.IO;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public abstract class Stage
    {
        public string Label { get; protected set; }
        public abstract Stream Input { get; }
        public abstract Stream Output { get; }
        public abstract bool EndOfOutput { get; }
        public abstract void Terminate();
        public abstract void WaitForDone();
        public abstract bool Done { get; }
        public abstract bool Failed { get; }
        public abstract void Start();
        public abstract IEnumerable<string> Messages { get; }
    }
}
