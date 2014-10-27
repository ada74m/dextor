using System.Collections.Generic;
using System.Threading.Tasks;
using Dextor.Extensions;
using FakeItEasy;
using NUnit.Framework;

namespace Dextor.Tests
{
    [TestFixture]
    public class RemoteProcessTests
    {
        [Test]
        public void ShouldSetStdInToResultOfAnotherTask()
        {
            var taskCompleter = new TaskCompletionSource<byte[]>();
            var otherTask = taskCompleter.Task;
            var resultOfOtherTask = "result of other task".ToUtf8EncodedBytes();

            var executionBroker = A.Fake<IExecutionBroker>();

            var sut = new RemoteProcess(executionBroker, A.Fake<IParameterFormatter>())
            {
                StdInTask = otherTask
            };

            taskCompleter.SetResult(resultOfOtherTask);

            var task = sut.GetStdOut();

            task.Wait();

            A.CallTo(() => executionBroker.Run(
                A<ProcessSpec>.That.Matches(ps => ps.StdIn.Equals(resultOfOtherTask)))
            ).MustHaveHappened();
        }

        [Test]
        public void ShouldSetupFilesWithResultsOfOtherTasks()
        {
            var firstFileCompleter = new TaskCompletionSource<byte[]>();
            var firstFileTask = firstFileCompleter.Task;
            var contentsOfFirstFile = "contents of first file".ToUtf8EncodedBytes();

            var secondFileCompleter = new TaskCompletionSource<byte[]>();
            var secondFileTask = secondFileCompleter.Task;
            var contentsOfSecondFile = "contents of second file".ToUtf8EncodedBytes();

            var executionBroker = A.Fake<IExecutionBroker>();

            var sut = new RemoteProcess(executionBroker, A.Fake<IParameterFormatter>());

            sut.SetupFile("file1.txt", firstFileTask);
            sut.SetupFile("file2.txt", secondFileTask);

            firstFileCompleter.SetResult(contentsOfFirstFile);
            secondFileCompleter.SetResult(contentsOfSecondFile);

            var task = sut.GetStdOut();

            task.Wait();

            A.CallTo(() => executionBroker.Run(
                A<ProcessSpec>.That.Matches(ps =>
                    ps.Files["file1.txt"].Equals(contentsOfFirstFile)
                    && ps.Files["file2.txt"].Equals(contentsOfSecondFile)
                    ))
                ).MustHaveHappened();
        }

        [Test]
        public void ShouldSetupEnvironmentVariablesWithResultsOfOtherTasks()
        {
            var firstEnvVarCompleter = new TaskCompletionSource<byte[]>();
            var firstEnvVarTask = firstEnvVarCompleter.Task;
            var firstEnvVarValue = "env var value 1".ToUtf8EncodedBytes();

            var secondEnvVarCompleter = new TaskCompletionSource<byte[]>();
            var secondEnvVarTask = secondEnvVarCompleter.Task;
            var secondEnvVarValue = "env var value 2".ToUtf8EncodedBytes();

            var executionBroker = A.Fake<IExecutionBroker>();

            var sut = new RemoteProcess(executionBroker, A.Fake<IParameterFormatter>());

            sut.SetupEnvironmentVariable("ENV1", firstEnvVarTask);
            sut.SetupEnvironmentVariable("ENV2", secondEnvVarTask);

            firstEnvVarCompleter.SetResult(firstEnvVarValue);
            secondEnvVarCompleter.SetResult(secondEnvVarValue);

            var task = sut.GetStdOut();

            task.Wait();

            A.CallTo(() => executionBroker.Run(
                A<ProcessSpec>.That.Matches(ps =>
                    ps.EnvironmentVariables["ENV1"].Equals(firstEnvVarValue)
                    && ps.EnvironmentVariables["ENV2"].Equals(secondEnvVarValue)
                ))
            ).MustHaveHappened();
        }

        [Test]
        public void ShouldSetupParametersWithResultsOfOtherTasks()
        {
            var firstParameterCompleter = new TaskCompletionSource<byte[]>();
            var firstParameterTask = firstParameterCompleter.Task;
            var firstParameterValue = "param1value".ToUtf8EncodedBytes();

            var secondParameterCompleter = new TaskCompletionSource<byte[]>();
            var secondParameterTask = secondParameterCompleter.Task;
            var secondParameterValue = "param2value".ToUtf8EncodedBytes();

            var executionBroker = A.Fake<IExecutionBroker>();

            var patternFormatter = A.Fake<IParameterFormatter>();
            A.CallTo(() => 
                patternFormatter.Format(
                    "some parameter pattern",
                    A<Dictionary<string, byte[]>>.That.Matches(d => 
                        d["PARAM1"].Equals(firstParameterValue) &&
                        d["PARAM2"].Equals(secondParameterValue)
                    ))).Returns("formatted parameters");

            var sut = new RemoteProcess(executionBroker, patternFormatter)
            {
                Parameters = "some parameter pattern"
            };

            sut.SetupParamater("PARAM1", firstParameterTask);
            sut.SetupParamater("PARAM2", secondParameterTask);

            firstParameterCompleter.SetResult(firstParameterValue);
            secondParameterCompleter.SetResult(secondParameterValue);

            var task = sut.GetStdOut();

            task.Wait();

            A.CallTo(() => executionBroker.Run(
                A<ProcessSpec>.That.Matches(ps =>
                    ps.Parameters.Equals("formatted parameters")
                ))
            ).MustHaveHappened();
        }
    }
}