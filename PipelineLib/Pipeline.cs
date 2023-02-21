using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ThrowException.CSharpLibs.PipelineLib
{
    public class Pipeline
    {
        private readonly List<Stage> _stages;
        private readonly List<StageConnection> _connections;
        public Output Output { get; private set; }
        public long TotalBytes { get; private set; }
        public EventHandler TotalBytesUpdated { get; private set; }

        public Pipeline()
        {
            _stages = new List<Stage>();
            _connections = new List<StageConnection>();
        }

        private void UpdateTotalBytes(long totalBytes)
        {
            TotalBytes = totalBytes;
            TotalBytesUpdated?.Invoke(this, new EventArgs());
        }

        public TStage Add<TStage>(TStage stage, bool inputIsTotalBytes = false)
            where TStage : Stage
        {
            if (stage == null) throw new ArgumentNullException(nameof(stage));

            if (_stages.Any())
            {
                var connection = new StageConnection(_stages.Last(), stage)
                {
                    UpdateTotalBytes = inputIsTotalBytes ? UpdateTotalBytes : (Action<long>)null
                };
                _connections.Add(connection);
            }
            else if (inputIsTotalBytes)
            {
                throw new ArgumentException("inputIsTotalBytes cannot be used on the first stage");
            }
            _stages.Add(stage);
            return stage;
        }

        public void Add(Func<Stage, Output> create)
        {
            if (create == null) throw new ArgumentNullException(nameof(create));
            if (Output != null) throw new InvalidOperationException("output already added");

            Output = create(_stages.Last());
        }

        public void Start()
        {
            if (!_stages.Any()) throw new InvalidOperationException("no stages added");
            if (Output == null) throw new InvalidOperationException("output not added");

            foreach (var stage in _stages)
            {
                stage.Start();
            }

            foreach (var connection in _connections)
            {
                connection.Start();
            }

            Output.Start();
        }

        public bool Done
        {
            get
            {
                return _stages.All(s => s.Done) && _connections.All(c => c.Done) && Output.Done;
            }
        }

        public bool Failed
        {
            get
            {
                return _stages.Any(s => s.Failed) || _connections.Any(c => c.Failed) || Output.Failed;
            }
        }

        public void Terminate()
        {
            foreach (var stage in _stages)
            {
                stage.Terminate();
            }

            foreach (var connection in _connections)
            {
                connection.WaitForDone();
            }

            Output.WaitForDone();
        }

        public void WaitForDone()
        {
            while (!Done)
            { 
                if (Failed)
                { 
                    foreach (var stage in _stages)
                    {
                        stage.Terminate();
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }

            foreach (var stage in _stages)
            {
                stage.WaitForDone();
            }

            foreach (var connection in _connections)
            {
                connection.WaitForDone();
            }

            Output.WaitForDone();
        }

        public IEnumerable<string> Messages
        {
            get
            {
                foreach (var stage in _stages)
                {
                    foreach (var message in stage.Messages)
                    {
                        yield return string.Format("{0}: {1}", stage.Label, message);
                    }
                }
            }
        }
    }
}
