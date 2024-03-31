using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace FracturedCode.GitVersioning;

[Generator(LanguageNames.CSharp)]
public class ThisAssemblyGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		string commitSha = Git("rev-parse HEAD");
		bool isClean = Git("status --porcelain") == string.Empty;
		string source = $$"""
			// This type may not update on hot reloads idk
			[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
			internal static partial class ThisAssembly {
				internal const bool IsClean = {{isClean.ToString().ToLower()}};
				internal const string CommitSha = "{{commitSha}}";
				internal const string Version = "{{commitSha}}{{(isClean ? "" : "-dirty")}}";
			}
			""";
		context.RegisterPostInitializationOutput(c =>
		{
			c.AddSource("ThisAssembly.g.cs", source);
		});
	}

	private static string Git(string args)
	{
		Process proc = new()
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = "git",
				Arguments = args,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			}
		};
		proc.Start();
		proc.WaitForExit();
		string procOut = proc.StandardOutput.ReadToEnd()[..^1];
		if (proc.ExitCode != 0)
		{
			throw new Exception($"\"git {args}\" failed with exit code {proc.ExitCode}." +
				$"\nstdout: {procOut}\nstderr: {proc.StandardError.ReadToEnd()}");
		}
		return procOut;
	}
}