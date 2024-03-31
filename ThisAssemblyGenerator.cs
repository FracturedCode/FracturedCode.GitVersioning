using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace FracturedCode.GitVersioning;

[Generator(LanguageNames.CSharp)]
public class ThisAssemblyGenerator : ISourceGenerator
{
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
				RedirectStandardError = true,
				CreateNoWindow = true,
			}
		};
		proc.Start();
		proc.WaitForExit();
		string procOut = proc.StandardOutput.ReadToEnd().TrimEnd();
		if (proc.ExitCode != 0)
		{
			throw new Exception($"\"git {args}\" failed with exit code {proc.ExitCode}." +
				$"\nstdout: {procOut}\nstderr: {proc.StandardError.ReadToEnd()}");
		}
		return procOut;
	}

	public void Initialize(GeneratorInitializationContext context)
	{
		string commitSha = Git("rev-parse HEAD");
		bool isClean = Git("status --porcelain") == string.Empty;
		string source = $$"""
			[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
			internal static partial class ThisAssembly {
				internal const bool IsClean = {{isClean.ToString().ToLower()}};
				internal const string CommitSha = "{{commitSha}}";
				internal const string Version = "{{commitSha}}{{(isClean ? "" : "-dirty")}}";
			}
			""";
		context.RegisterForPostInitialization(c => c.AddSource("ThisAssembly.g.cs", source));
	}

	public void Execute(GeneratorExecutionContext context) { }
}