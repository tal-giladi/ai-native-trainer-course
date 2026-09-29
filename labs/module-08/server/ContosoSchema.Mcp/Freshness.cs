using System.Globalization;
using System.Text.RegularExpressions;

namespace ContosoSchema.Mcp;

public enum FreshnessStatus { Fresh, Stale, Ahead, Unknown }

/// <summary>
/// Provenance that travels with every answer: which source, which schema version, how old,
/// and whether the repository has migrations the source has not seen.
/// </summary>
public sealed record Freshness(
    FreshnessStatus Status,
    string Source,
    int SourceVersion,
    int? RepoHead,
    IReadOnlyList<string> NotInSource,
    double AgeHours,
    string Reason)
{
    public string Header =>
        $"[schema] source={Source} version=V{SourceVersion:D3} " +
        $"captured-age={FormatAge(AgeHours)} repo-head={(RepoHead is { } h ? $"V{h:D3}" : "unknown")} " +
        $"status={Status.ToString().ToUpperInvariant()}";

    public static string FormatAge(double hours) =>
        hours < 1 ? "live" : hours < 48 ? $"{hours:0.#}h" : $"{hours / 24:0.#}d";

    static readonly Regex MigrationFile = new(@"^V(\d{3,})__\w+\.sql$", RegexOptions.IgnoreCase);

    /// <summary>
    /// The repository's migration folder is the reference: a source that has not applied every V### in it
    /// describes a database that no longer exists on this branch.
    /// </summary>
    public static Freshness Evaluate(SchemaCatalog c, string? migrationsDir, double maxAgeHours, DateTimeOffset now)
    {
        var age = c.Live ? 0 : Math.Max(0, (now - c.CapturedUtc).TotalHours);
        if (migrationsDir is null || !Directory.Exists(migrationsDir))
            return new(FreshnessStatus.Unknown, c.Source, c.SchemaVersion, null, [], age,
                "no migrations folder configured, so the server cannot tell whether this schema is current");

        var versions = Directory.EnumerateFiles(migrationsDir, "V*.sql")
            .Select(Path.GetFileName)
            .Select(f => (File: f!, M: MigrationFile.Match(f!)))
            .Where(x => x.M.Success)
            .Select(x => (x.File, V: int.Parse(x.M.Groups[1].Value, CultureInfo.InvariantCulture)))
            .OrderBy(x => x.V)
            .ToList();
        var head = versions.Count == 0 ? 0 : versions[^1].V;
        var missing = versions.Where(x => x.V > c.SchemaVersion).Select(x => x.File).ToList();

        if (missing.Count > 0)
            return new(FreshnessStatus.Stale, c.Source, c.SchemaVersion, head, missing, age,
                $"the source is at V{c.SchemaVersion:D3} but the repository is at V{head:D3}; not in the source: {string.Join(", ", missing)}");
        if (c.SchemaVersion > head)
            return new(FreshnessStatus.Ahead, c.Source, c.SchemaVersion, head, [], age,
                $"the database is at V{c.SchemaVersion:D3}, ahead of this branch (V{head:D3}); pull before writing SQL against it");
        if (!c.Live && age > maxAgeHours)
            return new(FreshnessStatus.Stale, c.Source, c.SchemaVersion, head, [], age,
                $"the snapshot is {FormatAge(age)} old (limit {maxAgeHours:0}h); an unmigrated hotfix could be missing");
        return new(FreshnessStatus.Fresh, c.Source, c.SchemaVersion, head, [], age, "source matches the repository");
    }
}
