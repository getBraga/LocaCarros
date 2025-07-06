using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace LocaCarros.Application.DTOs.MarcasDtos
{
    public class MarcaDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="O Nome é obrigatório")]
        [MinLength(2)]
        [MaxLength(10)]
        [DisplayName("Nome")]
        public string Nome { get; set; } = string.Empty;


    }
}
