using LocaCarros.Application.DTOs.AlugueisDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.Interfaces
{
    public interface IAluguelService
    {
        Task<IEnumerable<AluguelDTO>> GetAlugueisAsync();
        Task<AluguelDTO?> GetAluguelByIdAsync(int id);
        Task<AluguelDTO> CreateAsync(AluguelDTOAdd aluguelDtoAdd);
        Task<bool> DeleteAsync(int id);
        Task<AluguelDTO> UpdateAsync(AluguelDTOUpdate aluguelDtoUpdate);
    }
}
