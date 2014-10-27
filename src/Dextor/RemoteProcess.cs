using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dextor
{
    public class RemoteProcess
    {
        private readonly IExecutionBroker _executionBroker;
        private readonly IParameterFormatter _patternFormatter;

        public Task<byte[]> StdInTask { private get; set; }

        private readonly IDictionary<string, Task<byte[]>> _fileContentsTasks = new Dictionary<string, Task<byte[]>>();
        private readonly IDictionary<string, Task<byte[]>> _environmentVariableTasks = new Dictionary<string, Task<byte[]>>();
        private readonly IDictionary<string, Task<byte[]>> _parameterValueTasks = new Dictionary<string, Task<byte[]>>();
        public string Parameters { get; set; }

        public RemoteProcess(IExecutionBroker executionBroker, IParameterFormatter patternFormatter)
        {
            _executionBroker = executionBroker;
            _patternFormatter = patternFormatter;
        }

        public async Task<byte[]> GetStdOut()
        {
            var processSpec = new ProcessSpec();

            if (StdInTask != null)
            {
                await StdInTask;
                processSpec.StdIn = StdInTask.Result;
            }

            foreach (var kvp in _fileContentsTasks)
            {
                var path = kvp.Key;
                var task = kvp.Value;
                await task;
                processSpec.Files.Add(path, task.Result);
            }

            foreach (var kvp in _environmentVariableTasks)
            {
                var path = kvp.Key;
                var task = kvp.Value;
                await task;
                processSpec.EnvironmentVariables.Add(path, task.Result);
            }

            var parameterValues = new Dictionary<string, byte[]>();
            foreach (var kvp in _parameterValueTasks)
            {
                var name = kvp.Key;
                var task = kvp.Value;
                await task;
                parameterValues.Add(name, task.Result);
            }
            processSpec.Parameters = _patternFormatter.Format(Parameters, parameterValues);

            await _executionBroker.Run(processSpec);
            
            var dummyResult = "line1"
                 + Environment.NewLine + "line2"
                 + Environment.NewLine + "line3";

            return System.Text.Encoding.UTF8.GetBytes(dummyResult);

        }

        public void SetupFile(string path, Task<byte[]> fileContentsTask)
        {
            _fileContentsTasks.Add(path, fileContentsTask);
        }

        public void SetupEnvironmentVariable(string name, Task<byte[]> valueTask)
        {
            _environmentVariableTasks.Add(name, valueTask);
        }

        public void SetupParamater(string name, Task<byte[]> parameterValueTask)
        {
            _parameterValueTasks.Add(name, parameterValueTask);
        }
    }
}