using System.Collections.Generic;

namespace Dextor
{
    public class ProcessSpec
    {
        public byte[] StdIn { get; set; }

        private readonly IDictionary<string, byte[]> _files = new Dictionary<string, byte[]>();
        public IDictionary<string, byte[]> Files
        {
            get { return _files; }
        }

        private readonly IDictionary<string, byte[]> _environmentVariables = new Dictionary<string, byte[]>();
        public IDictionary<string, byte[]> EnvironmentVariables
        {
            get { return _environmentVariables;  }
        }

        public string Parameters { get; set; }

        public override string ToString()
        {
            return string.Format("Process spec: STDIN='{0}'", System.Text.Encoding.UTF8.GetString(StdIn));
        }
    }
}