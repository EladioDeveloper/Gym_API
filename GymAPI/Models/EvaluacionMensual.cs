using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymAPI.Models
{
    public class EvaluacionMensual
    {
        public int ID { get; set; }
        public int IDCliente { get; set; }
        public int IDMes { get; set; }
        public int Calorias { get; set; }
        public float Altura { get; set; }
        public float Peso { get; set; }
        public float Grasa { get; set; }
        public string Comentarios { get; set; }
    }
}
