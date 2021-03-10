using System;
using System.IO;
using NSubstitute;
using Xunit.Abstractions;

namespace Dynamocs.DevTools.Tests
{
	public class TestBase
	{
		public TestBase(ITestOutputHelper output)
		{
			var textWriter = Substitute.For<TextWriter>();
			textWriter.WriteLine(Arg.Do<string>(output.WriteLine));
			Console.SetOut(textWriter);
		}
	}
}