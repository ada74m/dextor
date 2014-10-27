using System;
using System.Threading.Tasks;
using Dextor.Extensions;
using NUnit.Framework;

namespace Dextor.Tests
{
    [TestFixture]
    public class LineSplitterTests
    {
        [Test]
        public void ShouldSplitInputOnLines()
        {
            // should it be encoding aware?.

            var completer = new TaskCompletionSource<byte[]>();
            var inputTask = completer.Task;


            var sut = new LineSplitter();
            sut.InputTask = inputTask;

            var input = ("one" + Environment.NewLine + "two" + Environment.NewLine + "three").ToUtf8EncodedBytes();
            completer.SetResult(input);

            var task = sut.GetItems();

            task.Wait();

            var expectedResult = new[] { "one".ToUtf8EncodedBytes(), "two".ToUtf8EncodedBytes(), "three".ToUtf8EncodedBytes() };
            CollectionAssert.AreEqual(expectedResult, task.Result);

        }
    }
}