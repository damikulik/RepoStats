using System.Globalization;
using System.Text;

using RepoStats.Domain;

namespace RepoStats.Application;

internal class SystemContext(RepoStatsConfig config) : ISystemContext
{
    public Decoder Decoder { get; } = Encoding.GetEncoding(config.Encoding).GetDecoder();

    public TimeProvider TimeProvider => TimeProvider.System;

    public CultureInfo Culture => config.CultureInfo;
}
