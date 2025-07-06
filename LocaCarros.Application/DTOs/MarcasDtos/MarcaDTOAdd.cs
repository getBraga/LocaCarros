using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.DTOs.MarcasDtos
{
    public class MarcaDTOAdd
    {
        [MinLength(2)]
        [MaxLength(10)]
        [DisplayName("Nome")]
        public string Nome { get; set; } = string.Empty;
    }
}
