using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace GymAPI.Models
{
    public class PlanMembresia
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int TiempoValidez { get; set; }
        public float Monto { get; set; }
        public bool Estado { get; set; }
    }
}
