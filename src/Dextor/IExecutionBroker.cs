using System.Threading.Tasks;

namespace Dextor
{
    public interface IExecutionBroker
    {
        Task Run(ProcessSpec spec);
    }
}