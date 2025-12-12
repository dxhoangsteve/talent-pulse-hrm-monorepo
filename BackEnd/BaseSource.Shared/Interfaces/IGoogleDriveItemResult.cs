using BaseSource.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.Shared.Interfaces
{
    public interface IGoogleDriveItemResult
    {
        string ID { get; }
        string? ResourceKey { get; }
        GoogleDriveLinkType LinkType { get; }
    }
}
