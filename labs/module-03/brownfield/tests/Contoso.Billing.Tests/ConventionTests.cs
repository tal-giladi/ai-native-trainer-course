using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Xunit;

namespace Contoso.Billing.Tests;

// Conventions that code review kept missing, turned into tests (see docs/adr/0007).
// They guard the code, and they also guard the AI layer: an agent that follows a stale
// rule ("use SqlHelper", "use DateTime.Now") now fails the build instead of slipping past review.
public class ConventionTests
{
    private static readonly char Sep = Path.DirectorySeparatorChar;

    private static string SrcRoot([CallerFilePath] string here = "") =>
        Path.GetFullPath(Path.Combine(Path.GetDirectoryName(here)!, "..", "..", "src"));

    private static List<string> SourceFiles() =>
        Directory.EnumerateFiles(SrcRoot(), "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains($"{Sep}obj{Sep}") && !f.Contains($"{Sep}bin{Sep}"))
            .ToList();

    [Fact]
    public void Only_the_Legacy_folder_may_call_SqlHelper()
    {
        var offenders = SourceFiles()
            .Where(f => !f.Contains($"{Sep}Legacy{Sep}"))
            .Where(f => File.ReadAllText(f).Contains("SqlHelper."))
            .ToList();
        Assert.True(offenders.Count == 0, "SqlHelper used outside Legacy/: " + string.Join(", ", offenders));
    }

    [Fact]
    public void No_code_reads_the_local_wall_clock()
    {
        var pattern = new Regex(@"DateTime(Offset)?\.Now\b");
        var offenders = SourceFiles().Where(f => pattern.IsMatch(File.ReadAllText(f))).ToList();
        Assert.True(offenders.Count == 0, "Use IClock.UtcNow instead: " + string.Join(", ", offenders));
    }
}
