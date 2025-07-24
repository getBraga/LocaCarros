using AutoMapper;
using LocaCarros.Application.DTOs.CarrosDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Exceptions;
using LocaCarros.Domain.Interfaces;



namespace LocaCarros.Application.Services
{


    public class CarroService : ICarroService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CarroService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CarroDTO> CreateAsync(CarroDTOAdd dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {

                var carro = _mapper.Map<Carro>(dto);
                var modelo = await ModeloHasCarro(dto.ModeloId);
                carro.ValidarHasModelo(modelo);
                var result = await _unitOfWork.Carros.CreateAsync(carro);
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
                throw new Exception("Erro ao criar carro.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var carro = await _unitOfWork.Carros.GetCarroByIdAsync(id);
                if (carro == null) return false;
                await _unitOfWork.CommitAsync();
                return await _unitOfWork.Carros.DeleteAsync(carro);
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar carro.", ex);
            }
        }

        public async Task<CarroDTO?> GetCarroByIdAsync(int id)
        {
            var carro = await _unitOfWork.Carros.GetCarroByIdAsync(id);
            if (carro == null) return null;
            return _mapper.Map<CarroDTO>(carro);
        }

        public async Task<IEnumerable<CarroDTO>> GetCarrosAsync()
        {
            var carros = await _unitOfWork.Carros.GetCarrosAsync();

            return _mapper.Map<IEnumerable<CarroDTO>>(carros);
        }

        public async Task<IEnumerable<CarroDTO>> GetCarrosByModeloIdAsync(int modeloId)
        {
            var carros = await _unitOfWork.Carros.GetCarrosByModeloIdAsync(modeloId);
            return _mapper.Map<IEnumerable<CarroDTO>>(carros);
        }

        public async Task<CarroDTO> UpdateAsync(CarroDTOUpdate carroUpdate)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var carro = await BuscarCarroPorId(carroUpdate.Id);

                var modelo = await ModeloHasCarro(carroUpdate.ModeloId);


                carro.Update(carroUpdate.Placa, carroUpdate.Ano, carroUpdate.Cor, carroUpdate.DataFabricacao, carroUpdate.Status, modelo);

                var result = await _unitOfWork.Carros.UpdateAsync(carro);

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

        private async Task<Modelo> ModeloHasCarro(int modeloId)
        {
            var modelo = await _unitOfWork.Modelos.GetModeloByIdAsync(modeloId);
           if(modelo == null)
                throw new DomainException("Modelo não encontrado.");

            return modelo;
        }

        private async Task<Carro> BuscarCarroPorId(int id)
        {
            var carro = await _unitOfWork.Carros.GetCarroByIdAsync(id);
            if (carro == null)
                throw new DomainException("Carro não encontrado.");
            return carro;
        }
    }
}
