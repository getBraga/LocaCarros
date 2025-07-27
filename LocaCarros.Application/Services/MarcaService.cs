using AutoMapper;
using LocaCarros.Application.DTOs.MarcasDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Exceptions;
using LocaCarros.Domain.Interfaces;

namespace LocaCarros.Application.Services
{
    public class MarcaService : IMarcaService
    {
  
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MarcaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<MarcaDTO> AddAsync(MarcaDTOAdd marcaDto)
        {
            var marca = _mapper.Map<Marca>(marcaDto);
            Marca result = await _unitOfWork.Marcas.CreateAsync(marca);
            return _mapper.Map<Marca, MarcaDTO>(result);
        }

        public async Task<MarcaDTO?> GetByIdAsync(int id)
        {
            var marca = await _unitOfWork.Marcas.GetMarcaByIdAsync(id);
            if (marca == null)
            {
                return null;
            }
            var marcaDto = _mapper.Map<Marca, MarcaDTO>(marca);
            return marcaDto;

        }

        public async Task<IEnumerable<MarcaDTO>> GetMarcasAsync()
        {
            IEnumerable<Marca> marcas = await _unitOfWork.Marcas.GetMarcasAsync();
            return _mapper.Map<IEnumerable<Marca>, IEnumerable<MarcaDTO>>(marcas);
        }

        private async Task<bool> PodeDeletarMarcaAsync(int id)
        {
            var marcaModelos = await _unitOfWork.Modelos.GetModelosByMarcaIdAsync(id);
    
            return marcaModelos!= null && marcaModelos.Any();
        }

        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var marca = await _unitOfWork.Marcas.GetMarcaByIdAsync(id);
                if (marca == null)
                {
                    return false;
                }
                var validaPodeRemover = await PodeDeletarMarcaAsync(id);
                marca.ValidarRemover(validaPodeRemover);

                bool resultMarca = await _unitOfWork.Marcas.DeleteAsync(marca);
                await _unitOfWork.CommitAsync();
                return resultMarca;
            }
            catch (DomainException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<MarcaDTO> UpdateAsync(MarcaDTO marcaDto)
        {
            var marca = _mapper.Map<MarcaDTO, Marca>(marcaDto);
            marca.Update(marca.Nome);
            var result = await _unitOfWork.Marcas.UpdateAsync(marca);

            return _mapper.Map<Marca, MarcaDTO>(result);
        }
    }
}
