using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using System.ComponentModel.DataAnnotations;

namespace LocaCarros.Application.DTOs.ModelosDtos
{
    public class ModeloDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório")]
        [MinLength(2, ErrorMessage = "O Nome deve ter no mínimo 2 caracteres.")]
        [MaxLength(10, ErrorMessage = "O Nome deve ter no máximo 10 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Versão é obrigatória")]
        [MinLength(2, ErrorMessage = "A Versão deve ter no mínimo 2 caracteres.")]
        [MaxLength(5, ErrorMessage = "A Versão deve ter no máximo 10 caracteres.")]
        public string Versao { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Motorização é obrigatória")]
        [Range(1.0, 10.0, ErrorMessage = "A Motorização deve ser um valor entre 1.0 e 10.0.")]
        public decimal Motorizacao { get; set; }


       public int MarcaId { get; set; }
      
       public string MarcaNome { get; set; } = null!;


        [Required(ErrorMessage = "O Tipo de Carroceria é obrigatório")]
        public EnumTipoCarroceria TipoCarroceria { get; set; } = EnumTipoCarroceria.Hatch;


    }
}
