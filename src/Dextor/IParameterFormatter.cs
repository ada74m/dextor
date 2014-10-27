using System.Collections.Generic;

namespace Dextor
{
    public interface IParameterFormatter
    {
        string Format(string formatString, IDictionary<string, byte[]> values);
    }
}