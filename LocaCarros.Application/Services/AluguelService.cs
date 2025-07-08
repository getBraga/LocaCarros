using AutoMapper;
using LocaCarros.Application.DTOs.AlugueisDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.Services
{
    public class AluguelService : IAluguelService
    {
        private readonly IAluguelRepository _aluguelRepository;
        private readonly IMapper _mapper;
        public AluguelService(IAluguelRepository aluguelRepository, IMapper mapper)
        {
            _aluguelRepository = aluguelRepository;
            _mapper = mapper;
        }
        public async Task<AluguelDTO> CreateAsync(AluguelDTOAdd aluguelDtoAdd)
        {
           var aluguel = _mapper.Map<Aluguel>(aluguelDtoAdd);
            var result = await _aluguelRepository.CreateAsync(aluguel);
            return _mapper.Map<AluguelDTO>(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
        var aluguel = await _aluguelRepository.GetAluguelByIdAsync(id);
            if (aluguel == null) return false;
            return await _aluguelRepository.DeleteAsync(aluguel);
        }

        public async Task<IEnumerable<AluguelDTO>> GetAlugueisAsync()
        {
            var alugueis = await _aluguelRepository.GetAlugueisAsync();
            return _mapper.Map<IEnumerable<AluguelDTO>>(alugueis);
        }

        public async Task<AluguelDTO?> GetAluguelByIdAsync(int id)
        {
            var aluguel = await _aluguelRepository.GetAluguelByIdAsync(id);
            if (aluguel == null) return null;
            return _mapper.Map<AluguelDTO>(aluguel);
        }

        public async Task<AluguelDTO> UpdateAsync(AluguelDTOUpdate aluguelDtoUpdate)
        {
            var aluguel = _mapper.Map<Aluguel>(aluguelDtoUpdate);
            var updatedAluguel = await _aluguelRepository.UpdateAsync(aluguel);
            return _mapper.Map<AluguelDTO>(updatedAluguel);
        }
    }
}
