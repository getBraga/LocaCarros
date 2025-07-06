using LocaCarros.Application.DTOs.MarcasDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.Interfaces
{
    public interface IMarcaService
    {
        Task<IEnumerable<MarcaDTO>> GetMarcasAsync();
        Task<MarcaDTO?> GetByIdAsync(int id);
        Task<MarcaDTO> AddAsync(MarcaDTOAdd marcaDto);
        Task<MarcaDTO> UpdateAsync(MarcaDTO marcaDto);
        Task<bool> RemoveAsync(int id);
        Task<bool> PodeDeletarMarcaAsync(int id);
     
    }
}
