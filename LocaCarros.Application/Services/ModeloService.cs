using AutoMapper;
using LocaCarros.Application.DTOs.ModelosDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Exceptions;
using LocaCarros.Domain.Interfaces;


namespace LocaCarros.Application.Services
{
    public class ModeloService : IModeloService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ModeloService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        private async Task<bool> IsModeloExistsAsync(string nome)
        {
            var modelo = await _unitOfWork.Modelos.GetModeloByNomeAsync(nome);
            return modelo != null && modelo.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase);
        }
        public async Task<ModeloDTO> AddAsync(ModeloDTOAdd modeloDto)
        {
           await _unitOfWork.BeginTransactionAsync();
           try { 
            if (string.IsNullOrWhiteSpace(modeloDto.Nome) || modeloDto.Nome.Length < 3)
            {
                throw new DomainException("O nome do modelo deve ter pelo menos 3 caracteres.");
            }
            var modelo = _mapper.Map<Modelo>(modeloDto);
            if (await IsModeloExistsAsync(modelo.Nome))
            {
                throw new DomainException("Já existe um modelo com esse nome.");
            }

            var result = await _unitOfWork.Modelos.CreateAsync(modelo);
            if(result == null)
            {
                throw new DomainException("Erro ao criar o modelo.");
            }
            await _unitOfWork.CommitAsync();
                return _mapper.Map<ModeloDTO>(result);
            }catch(DomainException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
         }
      

        public async Task<ModeloDTO?> GetByIdAsync(int id)
        {
            var modelo = await _unitOfWork.Modelos.GetModeloByIdAsync(id);
            if (modelo == null)
            {
                return null;
            }
          
            return _mapper.Map<ModeloDTO>(modelo);
        }
     
       
        public async Task<IEnumerable<ModeloDTO>> GetModelosAsync()
        {
            var modelos = await _unitOfWork.Modelos.GetModelosAsync();
            return _mapper.Map<IEnumerable<ModeloDTO>>(modelos);
        }
        private async Task<int> ModeloHasCarrosAsync(int modeloId)
        {
            var carros = await _unitOfWork.Carros.GetCarrosByModeloIdAsync(modeloId);
            return carros.Count();
        }
        public async Task<bool> RemoveAsync(int id)
        {
            var modelo = await _unitOfWork.Modelos.GetModeloByIdAsync(id);
            if(modelo == null)
            {
                return false;
            }
            var quantidadeCarros = await ModeloHasCarrosAsync(id);
            modelo.ValidarRemover(quantidadeCarros);
          
            return await _unitOfWork.Modelos.DeleteAsync(modelo);
        }

        public async Task<ModeloDTO> UpdateAsync(ModeloDTOUpdate modeloDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var modeloExistente = await _unitOfWork.Modelos.GetModeloByIdAsync(modeloDto.Id);
                if (modeloExistente == null)
                {
                    throw new DomainException("Modelo não encontrado.");
                }
                var verificaNomeModelo = await _unitOfWork.Modelos.GetModeloByNomeAsync(modeloDto.Nome);
                verificaNomeModelo?.ValidarModeloComMesmoNomePorId(verificaNomeModelo.Id, modeloDto.Id);


                var modeloUpdate = _mapper.Map<Modelo>(modeloDto);

                var result = await _unitOfWork.Modelos.UpdateAsync(modeloUpdate);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<ModeloDTO>(result);
            }
            catch (DomainException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<ModeloDTO>> GetByMarcaIdAsync(int marcaId)
        {
            var modelos = await _unitOfWork.Modelos.GetModelosByMarcaIdAsync(marcaId);
            return _mapper.Map<IEnumerable<ModeloDTO>>(modelos);
        }
    }
}
