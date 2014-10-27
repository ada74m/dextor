using System;
using System.Linq;
using System.Threading.Tasks;
using Dextor.Extensions;

namespace Dextor
{
    public class LineSplitter
    {
        public Task<byte[]> InputTask { private get; set; }

        public async Task<byte[][]> GetItems()
        {
            await InputTask;

            var text = System.Text.Encoding.UTF8.GetString(InputTask.Result);

            var parts = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            return parts.Select(p => p.ToUtf8EncodedBytes()).ToArray();
        }
    }
}