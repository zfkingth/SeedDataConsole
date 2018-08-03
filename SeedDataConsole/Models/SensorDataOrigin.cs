using System;
using System.Collections.Generic;

namespace SeedDataConsole.Models
{
    public partial class SensorDataOrigin
    {
        public Guid Id { get; set; }
        public Guid? SensorId { get; set; }
        public DateTime? MeaTime { get; set; }
        public double? MeaValue1 { get; set; }
        public double? MeaValue2 { get; set; }
        public double? MeaValue3 { get; set; }
        public double? ResValue1 { get; set; }
        public double? ResValue2 { get; set; }
        public double? ResValue3 { get; set; }
        public byte? Status { get; set; }

        public SensorInfo Sensor { get; set; }
    }
}
