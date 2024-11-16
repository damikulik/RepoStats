using System.Globalization;
using System.Text;

namespace RepoStats.Application;

internal record RepoStatsConfig(int Frequency, string Encoding, CultureInfo CultureInfo);
