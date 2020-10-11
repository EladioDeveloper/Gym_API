using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymAPI.Models
{
    public class DiasRutina
    {
        public int ID { get; set; }
        public int IDRutina { get; set; }
        public int IDDia { get; set; }
        public int IDEjercicio { get; set; }
        public int Repeticiones { get; set; }
        public int Series { get; set; }
    }
}
