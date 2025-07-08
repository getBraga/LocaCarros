using LocaCarros.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.DTOs.AlugueisDtos
{
    public class AluguelDTO
    {
        public int Id { get; private set; }

        public decimal ValorAluguel { get;  set; }
        public string DataInicio { get;  set; } = string.Empty;
        public string  DataDevolucao { get;  set; }  = string.Empty;
        public int CarroId { get;  set; }
        public string CarroPlaca { get; set; } = null!;
        public int CarroAno { get; set; }
        public string CarroCor { get; set; } = null!;
        public string CarroDataFabricacao { get; set; } = string.Empty;
        public string CarroStatus { get; set; } = string.Empty;
        public string ModeloNome { get; set; } = null!;

    }
}
