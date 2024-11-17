using System.Globalization;
using System.Text;

using RepoStats.Domain;

namespace RepoStats.Application;

internal class SystemContext(RepoStatsConfig config) : ISystemContext
{
    public Encoding Encoding { get; } = Encoding.GetEncoding(config.Encoding);

    public TimeProvider TimeProvider => TimeProvider.System;

    public CultureInfo Culture => config.CultureInfo;
}
