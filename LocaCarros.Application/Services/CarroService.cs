using AutoMapper;
using LocaCarros.Application.DTOs.CarrosDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Interfaces;


namespace LocaCarros.Application.Services
{


    public class CarroService : ICarroService
    {
        private readonly ICarroRepository _carroRepository;
        private readonly IModeloRepository _modeloRepository;
        private readonly IMapper _mapper;
        public CarroService(ICarroRepository carroRepository, IMapper mapper, IModeloRepository modeloRepository)
        {
            _carroRepository = carroRepository ?? throw new ArgumentNullException(nameof(carroRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _modeloRepository = modeloRepository;
        }
        public async Task<CarroDTO> CreateAsync(CarroDTOAdd dto)
        {
            var carros = _mapper.Map<Carro>(dto);
            //var modelo = await _modeloRepository.GetModeloByIdAsync(dto.ModeloId);
            //if (modelo == null) throw new Exception("Modelo não encontrado.");

            //var carro = new Carro(
            //    dto.Placa,
            //    dto.Ano,
            //    dto.Cor,
            //    dto.DataFabricacao,
            //    dto.Status,
            //   modelo

            //);
            var result = await _carroRepository.CreateAsync(carros);
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

        public async Task<IEnumerable<CarroDTO>> GetCarrosAsync()
        {
            var carros = await _carroRepository.GetCarrosAsync();
            
            return _mapper.Map<IEnumerable<CarroDTO>>(carros);
        }

        public Task<IEnumerable<CarroDTO>> GetCarrosByModeloIdAsync(int modeloId)
        {
            throw new NotImplementedException();
        }

        public async Task<CarroDTO> UpdateAsync(CarroDTOUpdate carroUpdate)
        {
            var carro = _mapper.Map<Carro>(carroUpdate);
            var result =  await _carroRepository.UpdateAsync(carro);
            if (result == null) throw new Exception("Erro ao atualizar o carro.");
            return _mapper.Map<CarroDTO>(result);
        }
    }
}
