using AutoMapper;
using LocaCarros.Application.DTOs.CarrosDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using LocaCarros.Domain.Exceptions;
using LocaCarros.Domain.Interfaces;
using LocaCarros.Infra.Data.Transaction;


namespace LocaCarros.Application.Services
{


    public class CarroService : ICarroService
    {
        private readonly ICarroRepository _carroRepository;
        private readonly IModeloService _modeloService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CarroService(ICarroRepository carroRepository, IMapper mapper, IModeloService modeloService, IUnitOfWork unitOfWork)
        {
            _carroRepository = carroRepository ;
            _mapper = mapper;
            _modeloService = modeloService;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task AtualizarStatusDosCarrosAsync(Carro? carroAnterior, Carro novoCarro)
        {
            var carrosParaAtualizar = new List<Carro>();

            if (carroAnterior != null)
            {
                carroAnterior.SetStatus(EnumCarroStatus.Disponivel);
                carrosParaAtualizar.Add(carroAnterior);
            }

            novoCarro.SetStatus(EnumCarroStatus.Vendido);
            carrosParaAtualizar.Add(novoCarro);

            await _unitOfWork.Carros.UpdatesListAsync(carrosParaAtualizar);
        }

        public async Task MarcarComoVendidoAsync(Carro carro)
        {
            carro.ValidarDisponibilidadeParaVenda();
            carro.SetStatus(EnumCarroStatus.Vendido);
            await _carroRepository.UpdateAsync(carro);
        }
        public async Task ValidarDisponibilidadeDoCarroAsync(Carro carro)
        {
            carro.ValidarDisponibilidadeParaVenda();
            await Task.CompletedTask;
        }
        public async Task<CarroDTO> CreateAsync(CarroDTOAdd dto)
        {
             var carro = _mapper.Map<Carro>(dto);
                var modelo = await _modeloService.GetByIdEntidadeAsync(dto.ModeloId);
                if (modelo == null) throw new DomainException("Modelo não encontrado.");


                var result = await _carroRepository.CreateAsync(carro);
                return _mapper.Map<CarroDTO>(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
          var carro = await _carroRepository.GetCarroByIdAsync(id);
            if (carro == null) return false;
            return await _carroRepository.DeleteAsync(carro);
        }

        public async Task<CarroDTO?> GetCarroByIdAsync(int id)
        {
            var carro = await _carroRepository.GetCarroByIdAsync(id);
            if (carro == null) return null;
            return _mapper.Map<CarroDTO>(carro);
        }
        public async Task<Carro> GetEntidadeCarroByIdAsync(int id)
        {
            var carro = await _carroRepository.GetCarroByIdAsync(id);
            if (carro == null)
                throw new DomainException("Carro não encontrado");
            return carro;
        }
        public async Task<IEnumerable<CarroDTO>> GetCarrosAsync()
        {
            var carros = await _carroRepository.GetCarrosAsync();
            
            return _mapper.Map<IEnumerable<CarroDTO>>(carros);
        }

        public async Task<IEnumerable<CarroDTO>> GetCarrosByModeloIdAsync(int modeloId)
        {
            var carros = await _carroRepository.GetCarrosByModeloIdAsync(modeloId);
            return _mapper.Map<IEnumerable<CarroDTO>>(carros);
        }

        public async Task<CarroDTO> UpdateAsync(CarroDTOUpdate carroUpdate)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var carro = await _carroRepository.GetCarroByIdAsync(carroUpdate.Id);
                if (carro == null)
                    throw new DomainException("Carro não encontrado.");

                var modelo = await _modeloService.GetByIdEntidadeAsync(carroUpdate.ModeloId);
                if (modelo == null)
                    throw new DomainException("Modelo não encontrado.");

              
                carro.Update(carroUpdate.Placa, carroUpdate.Ano, carroUpdate.Cor, carroUpdate.DataFabricacao, carroUpdate.Status, modelo);

                var result = await _carroRepository.UpdateAsync(carro);

                await _unitOfWork.CommitAsync();

                return _mapper.Map<CarroDTO>(result);
            }
            catch (DomainException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception("Erro ao atualizar carro.", ex);
            }
        }



    }
}
