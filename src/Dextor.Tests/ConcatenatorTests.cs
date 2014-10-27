using System;
using System.Threading.Tasks;
using Dextor.Extensions;
using NUnit.Framework;

namespace Dextor.Tests
{
    [TestFixture]
    public class ConcatenatorTests
    {
        [Test]
        public void ShouldConcatenateItems()
        {
            var inputCompleter = new TaskCompletionSource<byte[][]>();
            var inputTask = inputCompleter.Task;
            var inputContents = new[]
            {
                "uno".ToUtf8EncodedBytes(),
                "dos".ToUtf8EncodedBytes(),
                "tres".ToUtf8EncodedBytes()
            };

            var sut = new Concatenator(Environment.NewLine.ToUtf8EncodedBytes());
            sut.InputTask = inputTask;

            inputCompleter.SetResult(inputContents);

            var task = sut.Concatenate();

            task.Wait();

            var expectedResult = (
                "uno" + Environment.NewLine + 
                "dos" + Environment.NewLine + 
                "tres"
            ).ToUtf8EncodedBytes();
            
            Assert.AreEqual(expectedResult, task.Result);
        }
    }
}