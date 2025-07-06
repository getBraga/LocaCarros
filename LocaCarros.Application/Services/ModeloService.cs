using AutoMapper;
using LocaCarros.Application.DTOs.ModelosDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LocaCarros.Application.Services
{
    public class ModeloService : IModeloService
    {
        private readonly IModeloRepository _modeloRepository;
        private readonly IMapper _mapper;
        public ModeloService(IModeloRepository modeloRepository, IMapper mapper)
        {
            _modeloRepository = modeloRepository ?? throw new ArgumentNullException(nameof(modeloRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<ModeloDTO> AddAsync(ModeloDTOAdd modeloDto)
        {
         
            var modelo = _mapper.Map<Modelo>(modeloDto);


            var result = await _modeloRepository.CreateAsync(modelo);

            return _mapper.Map<ModeloDTO>(result);

         }

        public async Task<ModeloDTO?> GetByIdAsync(int id)
        {
            var modelo = await _modeloRepository.GetModeloByIdAsync(id);
            if (modelo == null)
            {
                return null;
            }
            return _mapper.Map<ModeloDTO>(modelo);
        }

        public async Task<IEnumerable<ModeloDTO>> GetModelosAsync()
        {
            var modelos = await _modeloRepository.GetModelosAsync();
            return _mapper.Map<IEnumerable<ModeloDTO>>(modelos);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var modelo = await _modeloRepository.GetModeloByIdAsync(id);
            if(modelo == null)
            {
                return false;
            }
            return await _modeloRepository.DeleteAsync(modelo);
        }

        public async Task<ModeloDTO> UpdateAsync(ModeloDTOUpdate modeloDto)
        {
            var modeloUpdate = _mapper.Map<Modelo>(modeloDto);
            var result = await _modeloRepository.UpdateAsync(modeloUpdate);
            return _mapper.Map<ModeloDTO>(result);
        }
    }
}
