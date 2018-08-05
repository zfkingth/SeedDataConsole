using System;
using System.Collections.Generic;

namespace SeedDataConsole.Models
{
    public partial class SensorInfo
    {
        public SensorInfo()
        {
            SensorDataOrigin = new HashSet<SensorDataOrigin>();
        }

        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public string SensorCode { get; set; }
        public Guid? SensorTypeId { get; set; }
        public Guid? ProjectSiteId { get; set; }
        public string LayLocation { get; set; }
        public string LocationX { get; set; }
        public string LocationY { get; set; }
        public string LocationZ { get; set; }

        public ProjectInfo Project { get; set; }
        public ICollection<SensorDataOrigin> SensorDataOrigin { get; set; }
    }
}
