using LocaCarros.Domain.Enuns;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.DTOs.CarrosDtos
{
    public class CarroDTOUpdate
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo Placa é obrigatório.")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "A placa deve ter exatamente 7 caracteres.")]
        public string Placa { get; set; } = null!;
        [Range(1886, int.MaxValue, ErrorMessage = "O ano deve ser maior ou igual a 1886.")]
        public int Ano { get; set; }

        [Required(ErrorMessage = "O campo Cor é obrigatório.")]
        public string Cor { get; set; } = null!;
        [Required]
        [DataType(DataType.Date, ErrorMessage = "A data de fabricação deve ser uma data válida.")]
        public DateTime DataFabricacao { get; set; }
        public EnumCarroStatus Status { get; set; } = EnumCarroStatus.Disponivel;
        [Required]
        public int ModeloId { get; set; }
    }
}
