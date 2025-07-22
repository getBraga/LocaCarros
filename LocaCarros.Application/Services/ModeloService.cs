using AutoMapper;
using LocaCarros.Application.DTOs.ModelosDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Exceptions;
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
        private readonly ICarroRepository _carroRepository;
        public ModeloService(IModeloRepository modeloRepository, IMapper mapper, ICarroRepository carroRepository)
        {
            _modeloRepository = modeloRepository ;
            _mapper = mapper;
            _carroRepository = carroRepository;
        }
        private async Task<bool> IsModeloExistsAsync(string nome)
        {
            var modelo = await _modeloRepository.GetModeloByNomeAsync(nome);
            return modelo != null && modelo.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase);
        }
        public async Task<ModeloDTO> AddAsync(ModeloDTOAdd modeloDto)
        {
         
            var modelo = _mapper.Map<Modelo>(modeloDto);
            if (await IsModeloExistsAsync(modelo.Nome))
            {
                throw new DomainException("Já existe um modelo com esse nome.");
            }

            var result = await _modeloRepository.CreateAsync(modelo);
            if(result == null)
            {
                throw new DomainException("Erro ao criar o modelo.");
            }
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
     
        public async Task<Modelo?> GetByIdEntidadeAsync(int id)
        {
            var modelo = await _modeloRepository.GetModeloByIdAsync(id);
            if (modelo == null)
            {
                return null;
            }

            return modelo;
        }
        public async Task<IEnumerable<ModeloDTO>> GetModelosAsync()
        {
            var modelos = await _modeloRepository.GetModelosAsync();
            return _mapper.Map<IEnumerable<ModeloDTO>>(modelos);
        }
        private async Task<bool> ModeloHasCarrosAsync(int modeloId)
        {
            var carros = await _carroRepository.GetCarrosByModeloIdAsync(modeloId);
            return carros.Any();
        }
        public async Task<bool> RemoveAsync(int id)
        {
            var modelo = await _modeloRepository.GetModeloByIdAsync(id);
            if(modelo == null)
            {
                return false;
            }
            if( await ModeloHasCarrosAsync(id) == true)
            {
                throw new DomainException("Não é possível excluir um modelo que possui carros associados.");
            }
            return await _modeloRepository.DeleteAsync(modelo);
        }

        public async Task<ModeloDTO> UpdateAsync(ModeloDTOUpdate modeloDto)
        {
            var modeloExistente = await _modeloRepository.GetModeloByIdAsync(modeloDto.Id);
            if (modeloExistente == null)
            {
                throw new DomainException("Modelo não encontrado.");
            }
            var existingModeloComMesmoNome = await _modeloRepository.GetModeloByNomeAsync(modeloDto.Nome);

            if (existingModeloComMesmoNome != null && existingModeloComMesmoNome.Id != modeloDto.Id )
            {
                throw new DomainException("Já existe um modelo com o nome informado.");
            }
           
            var modeloUpdate = _mapper.Map<Modelo>(modeloDto);
           
            var result = await _modeloRepository.UpdateAsync(modeloUpdate);
            return _mapper.Map<ModeloDTO>(result);
        }

     
    }
}
