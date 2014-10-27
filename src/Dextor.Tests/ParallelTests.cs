using System;
using System.Text;
using System.Threading.Tasks;
using Dextor.Extensions;
using FakeItEasy;
using NUnit.Framework;

namespace Dextor.Tests
{
    [TestFixture]
    public class ParallelTests
    {
        [Test]
        public void ShouldDoSomething()
        {
            var inputCompleter = new TaskCompletionSource<byte[][]>();
            var inputTask = inputCompleter.Task;
            var inputContents = new[]
            {
                "ein".ToUtf8EncodedBytes(),
                "zwei".ToUtf8EncodedBytes(),
                "drei".ToUtf8EncodedBytes()
            };

            var executionBroker = A.Fake<IExecutionBroker>();

            Func<Task<byte[]>, Task<byte[]>> buildIterationTask = iterationInput => 
                new RemoteProcess(executionBroker, A.Fake<IParameterFormatter>()) {
                    StdInTask = iterationInput
                }.GetStdOut();

            var sut = new Parallel(buildIterationTask) {InputTask = inputTask};

            var task = sut.GetItems();

            inputCompleter.SetResult(inputContents);

            task.Wait();

            A.CallTo(() => executionBroker.Run(A<ProcessSpec>.That.Matches(ps => Encoding.UTF8.GetString(ps.StdIn) == "ein"))).MustHaveHappened();
            A.CallTo(() => executionBroker.Run(A<ProcessSpec>.That.Matches(ps => Encoding.UTF8.GetString(ps.StdIn) == "zwei"))).MustHaveHappened();
            A.CallTo(() => executionBroker.Run(A<ProcessSpec>.That.Matches(ps => Encoding.UTF8.GetString(ps.StdIn) == "drei"))).MustHaveHappened();

            var results = task.Result;
            foreach (var result in results)
            {
                Console.WriteLine(Encoding.UTF8.GetString(result));
            }

        } 
    }
}