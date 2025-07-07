using LocaCarros.Application.DTOs.VendasDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.Interfaces
{
    public interface IVendaService
    {
        Task<IEnumerable<VendaDTO>> GetVendasAsync();
        Task<VendaDTO?> GetVendaByIdAsync(int id);
        Task<VendaDTO> CreateAsync(VendaDTOAdd vendaDtoAdd);
        Task<bool> DeleteAsync(int id);
        Task<VendaDTO> UpdateAsync(VendaDTOUpdate vendaDtoUpdate);
    }
}
