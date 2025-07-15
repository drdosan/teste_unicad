using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raizen.UniCad.Model.View
{
    public class CsonlineVehicleRequestView
    {
        public string Id { get; set; }
       public string LicencePlate { get; set; }
       public int? Axles { get; set; }
       public int? Tare { get; set; }
       public float? MaxWeight { get; set; }
       public bool? AxleSpacing { get; set; }
       public bool? WarnCustomers { get; set; }
       public bool? IsDeleted { get; set; }
       public decimal? MaxVolume { get; set; }
       public DateTime? DtChecklistExpire { get; set; }
       public DateTime? DtCheckingExpire { get; set; }
       public DateTime? DtQualificationExpire { get; set; }
       public DateTime? DtEnvironmentalPermitExpire { get; set; }
    }
}
