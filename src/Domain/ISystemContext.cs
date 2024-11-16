using System.Globalization;
using System.Text;

namespace RepoStats.Domain;

public interface ISystemContext
{
    Decoder Decoder { get; }

    CultureInfo Culture { get; }

    TimeProvider TimeProvider { get; }
}
