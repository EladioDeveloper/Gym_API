using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymAPI.Models
{
    public class EjercicioRutina
    {
        public int ID { get; set; }
        public int IDCategoria { get; set; }
        public string Nombre { get; set; }
    }
}
