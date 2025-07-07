using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.DTOs.VendasDtos
{
    public class VendaDTOAdd
    {
      
        public decimal ValorVenda { get; set; }
        public string DataVenda { get; set; } = string.Empty;
        public int CarroId { get; set; }
  
    }
}
