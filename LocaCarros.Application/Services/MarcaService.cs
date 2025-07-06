using AutoMapper;
using LocaCarros.Application.DTOs.MarcasDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Interfaces;

namespace LocaCarros.Application.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly IMarcaRepository _marcaRepository;
        private readonly IModeloRepository _modeloRepository;
        private readonly IMapper _mapper;
        public MarcaService(IMarcaRepository marcaRepository, IMapper mapper, IModeloRepository modeloRepository)
        {
            _marcaRepository = marcaRepository ?? throw new ArgumentNullException(nameof(marcaRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _modeloRepository = modeloRepository;
        }
        public async Task<MarcaDTO> AddAsync(MarcaDTOAdd marcaDto)
        {
            var marca = _mapper.Map<Marca>(marcaDto);
            Marca result = await _marcaRepository.CreateAsync(marca);
            return _mapper.Map<Marca, MarcaDTO>(result);
        }

        public async Task<MarcaDTO?> GetByIdAsync(int id)
        {
            var marca = await _marcaRepository.GetMarcaByIdAsync(id);
            if (marca == null)
            {
                return null;
            }
            MarcaDTO reult = _mapper.Map<Marca, MarcaDTO>(marca);
            return reult;

        }

        public async Task<IEnumerable<MarcaDTO>> GetMarcasAsync()
        {
            IEnumerable<Marca> marcas = await _marcaRepository.GetMarcasAsync();
            return _mapper.Map<IEnumerable<Marca>, IEnumerable<MarcaDTO>>(marcas);
        }

        public async Task<bool> PodeDeletarMarcaAsync(int id)
        {
           var marcaModelos = await _modeloRepository.GetModelosByMarcaIdAsync(id);
          return marcaModelos == null || !marcaModelos.Any();
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var marca = await _marcaRepository.GetMarcaByIdAsync(id);
            if (marca == null)
            {
                return false;
            }
            if (!await PodeDeletarMarcaAsync(id))
            {
                throw new InvalidOperationException("Não é possível excluir marca com modelos.");
            }
         
            bool resultMarca = await _marcaRepository.DeleteAsync(marca);
            return resultMarca;
        }

        public async Task<MarcaDTO> UpdateAsync(MarcaDTO marcaDto)
        {
            var marca = _mapper.Map<MarcaDTO, Marca>(marcaDto);
            marca.Update(marca.Nome);
            var result = await _marcaRepository.UpdateAsync(marca);
            
            return _mapper.Map<Marca, MarcaDTO>(result);
        }
    }
}
