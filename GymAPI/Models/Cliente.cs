using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymAPI.Models
{
    public class Cliente
    {
        public int ID { get; set; }
        public int IDRutina { get; set; }
        public int IDDireccion { get; set; }
        public int IDInscripcion { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public DateTime FNacimiento { get; set; }
        public DateTime FRegistro { get; set; }
    }
}
