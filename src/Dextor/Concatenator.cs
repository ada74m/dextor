using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dextor
{
    public class Concatenator
    {
        private readonly byte[] _spacer;

        public Concatenator(byte[] spacer)
        {
            _spacer = spacer;
        }

        public Task<byte[][]> InputTask { private get; set; }

        public async Task<byte[]> Concatenate()
        {
            await InputTask;
        
            var items = InputTask.Result;
            var size = items.Sum(x => x.Length);
            var mem = new MemoryStream(size);
            var writer = new BinaryWriter(mem);
            bool first = true;
            foreach (var item in items)
            {
                if (!first)
                {
                    writer.Write(_spacer);
                }
                first = false;
                writer.Write(item);
            }

            return mem.ToArray();
        }
    }
}