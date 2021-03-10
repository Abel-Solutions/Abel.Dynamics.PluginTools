using System;
using System.IO;
using Moq;
using Xunit.Abstractions;

namespace Tests
{
	public class TestBase
	{
		public TestBase(ITestOutputHelper output)
		{
			var textWriter = new Mock<TextWriter>();
			textWriter.Setup(t => t.WriteLine(It.IsAny<string>()))
				.Callback((string s) => output.WriteLine(s));
			Console.SetOut(textWriter.Object);
		}
	}
}