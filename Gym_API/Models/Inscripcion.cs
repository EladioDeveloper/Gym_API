using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gym_API.Models
{
    public class Inscripcion
    {
        public int ID { get; set; }
        public int IDPlan { get; set; }
        public DateTime FPago { get; set; }
        public DateTime FExpiracion { get; set; }
        public bool AutoRenovacion { get; set; }
    }
}
