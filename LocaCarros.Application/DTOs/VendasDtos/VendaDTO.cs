using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.DTOs.VendasDtos
{
    public class VendaDTO
    {
        public int Id { get; private set; }
        public decimal ValorVenda { get;  set; }
        public string DataVenda { get;  set; } = string.Empty;
        public string CarroPlaca { get;  set; } = null!;
        public int CarroAno { get;  set; }
        public string CarroCor { get;  set; } = null!;
        public string CarroDataFabricacao { get;  set; } = string.Empty;
        public string CarroStatus { get;  set; } = string.Empty;
        public string ModeloNome { get;  set; } = null!;
     
    }
}
