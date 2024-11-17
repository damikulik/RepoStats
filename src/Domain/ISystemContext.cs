using System.Globalization;
using System.Text;

namespace RepoStats.Domain;

public interface ISystemContext
{
    Encoding Encoding { get; }

    CultureInfo Culture { get; }

    TimeProvider TimeProvider { get; }
}
