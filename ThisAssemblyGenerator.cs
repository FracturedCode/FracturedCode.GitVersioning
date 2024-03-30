using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace FracturedCode.GitVersioning;

[Generator(LanguageNames.CSharp)]
public class ThisAssemblyGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		Process proc = new()
		{
			StartInfo = new ProcessStartInfo()
			{
				FileName = "git",
				ArgumentList = { "rev-parse", "HEAD" },
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			}
		};
		proc.Start();
		proc.WaitForExit();
		string commitSha = proc.StandardOutput.ReadToEnd()[..^1];
		string source = $$"""
			[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
			internal static partial class ThisAssembly {
				internal const string CommitSha = "{{commitSha}}";
				internal const string Version = "{{commitSha}}";
			}
			""";
		context.RegisterPostInitializationOutput(c =>
		{
			c.AddSource("ThisAssembly.g.cs", source);
		});
	}
}