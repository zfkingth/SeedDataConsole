using System;
using System.Collections.Generic;

namespace SeedDataConsole.Models
{
    public partial class ProjectInfo
    {
        public ProjectInfo()
        {
            SensorInfo = new HashSet<SensorInfo>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<SensorInfo> SensorInfo { get; set; }
    }
}
