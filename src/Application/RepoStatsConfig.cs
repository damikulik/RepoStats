using System.Globalization;

namespace RepoStats.Application;

internal record RepoStatsConfig(int Frequency, string Encoding, CultureInfo CultureInfo);
