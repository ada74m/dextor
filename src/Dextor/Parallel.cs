using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dextor
{
    public class Parallel
    {
        private readonly Func<Task<byte[]>, Task<byte[]>> _buildIterationTask;

        public Parallel(Func<Task<byte[]>, Task<byte[]>> buildIterationTask)
        {
            _buildIterationTask = buildIterationTask;
        }

        public Task<byte[][]> InputTask { private get; set; }
        
        public async Task<byte[][]> GetItems()
        {
            await InputTask;
            
            var inputs = InputTask.Result;

            var tasks = inputs.Select(input =>
            {
                var completer = new TaskCompletionSource<byte[]>();
                completer.SetResult(input);
                return _buildIterationTask(completer.Task);
            }).ToArray();

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result).ToArray();

        }
    }
}