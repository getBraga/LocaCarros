using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.DTOs.AlugueisDtos
{
    public class AluguelDTOUpdate
    {
        public int Id { get; set; }
        public decimal ValorAluguel { get; set; }
        public string DataInicio { get; set; } = string.Empty;
        public string DataDevolucao { get; set; } = string.Empty;
        public int CarroId { get; set; }
    }
}
