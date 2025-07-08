using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.DTOs.CarrosDtos
{
    public class CarroDTO
    {
        public int Id { get; set; }

       
        public string Placa { get; set; } = null!;
      
        public int Ano { get; set; }

        public string Cor { get; set; } = null!;
        public string DataFabricacao { get; set; } = string.Empty;
        public string Status { get; set; }  = string.Empty;
        public int ModeloId { get; set; }

        public string ModeloVersao { get;  set; } = string.Empty;
        public decimal ModeloMotorizacao { get;  set; }
       
        public string ModeloNome { get; set; } = null!;
        public string MarcaNome { get; set; } = null!;
        public string ModeloTipoCarroceria { get; set; } = null!;
    }
}
