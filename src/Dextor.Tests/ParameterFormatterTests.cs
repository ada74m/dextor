using System.Collections.Generic;
using Dextor.Extensions;
using NUnit.Framework;

namespace Dextor.Tests
{
    [TestFixture]
    public class ParameterFormatterTests
    {
        [Test]
        public void ShouldInjectParameterValues()
        {
            var sut = new ParameterFormatter();

            var values = new Dictionary<string, byte[]>
            {
                {"P1", "val1".ToUtf8EncodedBytes()},  
                {"P2", "val2".ToUtf8EncodedBytes()},  
            };

            var result = sut.Format("pre $P1 $P2 $$P1 post", values);

            Assert.That(result, Is.EqualTo("pre val1 val2 $P1 post"));

        }         
    }
}