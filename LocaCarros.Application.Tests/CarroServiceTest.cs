using AutoMapper;
using LocaCarros.Application.DTOs.CarrosDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Application.Services;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using LocaCarros.Domain.Exceptions;
using LocaCarros.Domain.Interfaces;
using LocaCarros.Infra.Data.Transaction;
using Moq;


namespace LocaCarros.Application.Tests
{
    public class CarroServiceTest
    {
        private readonly Mock<ICarroRepository> _carroRepository;
        private readonly Mock<IModeloService> _modeloService;
        private readonly Mock<IMapper> _mapper;
        private readonly CarroService _carroService;
        private readonly Modelo _modelo;
        private readonly Carro _carro;
        private readonly DateTime _dateTimeNow = DateTime.Now;
        private readonly CarroDTO _carroDto;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        public CarroServiceTest()
        {
            _carroRepository = new Mock<ICarroRepository>();
            _modeloService = new Mock<IModeloService>();
            _mapper = new Mock<IMapper>();
            _unitOfWork = new Mock<IUnitOfWork>(); 
            _carroService = new CarroService(
                _carroRepository.Object,
                _mapper.Object,
                _modeloService.Object,
                _unitOfWork.Object
            );

            _modelo = new Modelo("Modelo A", "Versão A", 1.0m,
              EnumTipoCarroceria.Sedan, new Marca("Marca A"));

            _carro = new Carro("AAAAAAA", _dateTimeNow.Year, "Vermelho",
                _dateTimeNow.AddYears(-1), EnumCarroStatus.Disponivel, _modelo);

            _carroDto = new CarroDTO
            {
                Id = 1,
                Placa = "ABC-1234",
                Ano = _dateTimeNow.Year,
                Cor = "Vermelho",
                DataFabricacao = _dateTimeNow.AddYears(-1).ToString(),
                MarcaNome = _modelo.Marca.Nome,
                ModeloMotorizacao = _modelo.Motorizacao,
                ModeloNome = _modelo.Nome,
                ModeloTipoCarroceria = _modelo.TipoCarroceria.ToString(),
                ModeloVersao = _modelo.Versao,
                Status = EnumCarroStatus.Disponivel.ToString(),
                ModeloId = 1
            };
            _unitOfWork.Setup(u => u.Carros).Returns(_carroRepository.Object);
        }


        [Fact]
        public async Task GetCarrosAsync_DeveRetornarListaDeCarros()
        {
            _carroRepository
                .Setup(c => c.GetCarrosAsync())
                .ReturnsAsync(new List<Carro> { _carro });

            _mapper
                .Setup(m => m.Map<IEnumerable<CarroDTO>>(It.IsAny<object>()))
                .Returns([_carroDto]);

            var carros = await _carroService.GetCarrosAsync();

            Assert.NotNull(carros);
            Assert.IsAssignableFrom<IEnumerable<CarroDTO>>(carros);
            Assert.Single(carros);
            Assert.Equal("ABC-1234", carros.First().Placa);
            Assert.Equal("Vermelho", carros.First().Cor);
        }
        [Fact]
        public async Task GetCarrosByModeloIdAsync_DeveRetornarCarrosModelos()
        {
            _carroRepository.Setup(c => c.GetCarrosByModeloIdAsync(1)).ReturnsAsync(new List<Carro> { _carro });

            _mapper
                .Setup(m => m.Map<IEnumerable<CarroDTO>>(It.IsAny<object>()))
                .Returns([_carroDto]);

            var carros = await _carroService.GetCarrosByModeloIdAsync(1);
            Assert.IsAssignableFrom<IEnumerable<CarroDTO>>(carros);
            Assert.NotNull(carros);
            Assert.Equal(1, carros.First().Id);
        }
        [Fact]
        public async Task GetCarroByIdAsync_DeveRetornarCarroPorId()
        { 
            _carroRepository
                .Setup(c => c.GetCarroByIdAsync(1))
                .ReturnsAsync(_carro);
            _mapper.Setup(m => m.Map<CarroDTO>(_carro))
                .Returns(_carroDto);
            var carro = await _carroService.GetCarroByIdAsync(1);
            Assert.NotNull(carro);
            Assert.Equal("ABC-1234", carro.Placa);
            Assert.Equal("Vermelho", carro.Cor);
        }
        [Fact]
        public async Task GetCarroByIdAsync_DeveRetornarNullQuandoCarroNaoExistir()
        {
            _carroRepository
                .Setup(c => c.GetCarroByIdAsync(1))
                .ReturnsAsync((Carro)null!);
            var carro = await _carroService.GetCarroByIdAsync(1);
            Assert.Null(carro);
        }
        [Fact]
        public async Task DeleteAsync_DeveDeletarCarro() {
            _carroRepository.Setup(c => c.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _carroRepository.Setup(c => c.DeleteAsync(_carro)).ReturnsAsync(true);
            var carroDeletado = await _carroService.DeleteAsync(1);
            Assert.True(carroDeletado);

        }
        [Fact]
        public async Task DeleteAsync_DeveRetornarFalseQuandoCarroNaoExistir() {
            _carroRepository.Setup(c => c.GetCarroByIdAsync(1)).ReturnsAsync((Carro?)null);
            var carroDeletado = await _carroService.DeleteAsync(1);
            Assert.False(carroDeletado);
        }

        [Fact]
        public async Task DeleteAsync_QuandoDeleteRetornaFalse_DeveRetornarFalse()
        {

            _carroRepository
                .Setup(c => c.GetCarroByIdAsync(1))
                .ReturnsAsync(_carro); 

            _carroRepository
                .Setup(c => c.DeleteAsync(_carro))
                .ReturnsAsync(false); 

            var result = await _carroService.DeleteAsync(1);
            Assert.False(result); 
        }

        [Fact]
        public async Task CreateAsync_DeveCriarCarroComSucesso()
        {
            var carroDtoAdd = new CarroDTOAdd
            {
                Placa = "ABC-1234",
                Ano = _dateTimeNow.Year,
                Cor = "Vermelho",
                DataFabricacao = _dateTimeNow.AddYears(-1),
                ModeloId = 1
            };
            _mapper.Setup(m => m.Map<Carro>(carroDtoAdd)).Returns(_carro);
            _modeloService.Setup(m => m.GetByIdEntidadeAsync(1)).ReturnsAsync(_modelo);
            _carroRepository.Setup(c => c.CreateAsync(_carro)).ReturnsAsync(_carro);
            _mapper.Setup(m => m.Map<CarroDTO>(_carro))
             .Returns(_carroDto);
            var result = await _carroService.CreateAsync(carroDtoAdd);
            Assert.NotNull(result);
            Assert.Equal("ABC-1234", result.Placa);
            Assert.Equal("Vermelho", result.Cor);
          
        }
        [Fact]
        public async Task CreateAsync_DeveLancarExcecaoQuandoModeloNaoExistir()
        {
            var carroDtoAdd = new CarroDTOAdd
            {
                Placa = "ABC-1234",
                Ano = _dateTimeNow.Year,
                Cor = "Vermelho",
                DataFabricacao = _dateTimeNow.AddYears(-1),
                ModeloId = 1
            };
            _mapper.Setup(m => m.Map<Carro>(carroDtoAdd)).Returns(_carro);
            _modeloService.Setup(m => m.GetByIdEntidadeAsync(1)).ReturnsAsync((Modelo)null!);
           var ex =  await Assert.ThrowsAsync<DomainException>(() => _carroService.CreateAsync(carroDtoAdd));
            Assert.Equal("Modelo não encontrado.", ex.Message);

        }


        [Fact]
        public async Task CreateAsync_DeveLancarExcecaoQuandoCarroInvalido()
        {
            var carroDtoAdd = new CarroDTOAdd
            {
                Placa = "ABC-1234",
                Ano = _dateTimeNow.Year,
                Cor = "Vermelho",
                DataFabricacao = _dateTimeNow.AddYears(-1),
                ModeloId = 1
            };
            _mapper.Setup(m => m.Map<Carro>(carroDtoAdd)).Returns(_carro);
            _modeloService.Setup(m => m.GetByIdEntidadeAsync(1)).ReturnsAsync(_modelo);
            _carroRepository.Setup(c => c.CreateAsync(_carro)).ThrowsAsync(new DomainException("Erro ao criar carro."));
            await Assert.ThrowsAsync<DomainException>(() => _carroService.CreateAsync(carroDtoAdd));
        }


        [Fact]
        public async Task UpdateAsync_DeveLancarExcecaoQuandoCarroInvalido()
        {
            var carroDtoUpdate = new CarroDTOUpdate
            {
                Id = 1,
                Placa = "XYZ-5678",
                Ano = _dateTimeNow.Year,
                Cor = "Azul",
                DataFabricacao = _dateTimeNow.AddYears(-2),
                ModeloId = 1
            };
            _mapper.Setup(m => m.Map<Carro>(carroDtoUpdate)).Returns(_carro);
            _modeloService.Setup(m => m.GetByIdEntidadeAsync(1)).ReturnsAsync(_modelo);
            var ex = await Assert.ThrowsAsync<DomainException>(() => _carroService.UpdateAsync(carroDtoUpdate));
            Assert.Equal("Carro não encontrado.", ex.Message);
        }
        [Fact]
        public async Task UpdateAsync_DeveLancarExcecaoQuandoModeloNaoExistir()
        {
            var carroDtoUpdate = new CarroDTOUpdate
            {
                Id = 1,
                Placa = "XYZ-5678",
                Ano = _dateTimeNow.Year,
                Cor = "Azul",
                DataFabricacao = _dateTimeNow.AddYears(-2),
                ModeloId = 1
            };
            _mapper.Setup(m => m.Map<Carro>(carroDtoUpdate)).Returns(_carro);
            _carroRepository.Setup(c => c.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _modeloService.Setup(m => m.GetByIdEntidadeAsync(1)).ReturnsAsync((Modelo)null!);
            var ex = await Assert.ThrowsAsync<DomainException>(() => _carroService.UpdateAsync(carroDtoUpdate));
            Assert.Equal("Modelo não encontrado.", ex.Message);
        }
        [Fact]
        public async Task UpdateAsync_DeveAtualizarCarroComSucesso()
        {
            var carroDtoUpdate = new CarroDTOUpdate
            {
                Id = 1,
                Placa = "ABC-1234",
                Ano = _dateTimeNow.Year,
                Cor = "Vermelho",
                DataFabricacao = _dateTimeNow.AddYears(-2),
                ModeloId = 1
            };
            _carro.Update(carroDtoUpdate.Placa, carroDtoUpdate.Ano, carroDtoUpdate.Cor,
    carroDtoUpdate.DataFabricacao, EnumCarroStatus.Disponivel, _modelo);

            _mapper.Setup(m => m.Map<Carro>(carroDtoUpdate)).Returns(_carro);
            _modeloService.Setup(m => m.GetByIdEntidadeAsync(1)).ReturnsAsync(_modelo);
            _carroRepository.Setup(c => c.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _carroRepository.Setup(c => c.UpdateAsync(_carro)).ReturnsAsync(_carro);
            _mapper.Setup(m => m.Map<CarroDTO>(_carro)).Returns(_carroDto);
            var result = await _carroService.UpdateAsync(carroDtoUpdate);
            Assert.NotNull(result);
            Assert.Equal("ABC-1234", result.Placa);
            Assert.Equal("Vermelho", result.Cor);
        }
        [Fact]
        public async Task UpdateAsync_DeveLancarExcecaoQuandoNaoSalvar()
        {
            var carroDtoUpdate = new CarroDTOUpdate
            {
                Id = 1,
                Placa = "XYZ-5678",
                Ano = _dateTimeNow.Year,
                Cor = "Azul",
                DataFabricacao = _dateTimeNow.AddYears(-2),
                ModeloId = 1
            };
            _mapper.Setup(m => m.Map<Carro>(carroDtoUpdate)).Returns(_carro);
            _modeloService.Setup(m => m.GetByIdEntidadeAsync(1)).ReturnsAsync(_modelo);
            _carroRepository.Setup(c => c.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _carroRepository.Setup(c => c.UpdateAsync(_carro)).ThrowsAsync(new Exception("Erro ao atualizar carro"));
            _mapper.Setup(m => m.Map<CarroDTO>(_carro)).Returns(_carroDto);
            var ex = await Assert.ThrowsAsync<Exception>(() => _carroService.UpdateAsync(carroDtoUpdate));
            Assert.Equal("Erro ao atualizar carro.", ex.Message);
        }
        [Fact]
        public async Task AtualizarStatusDosCarrosAsync_DeveAtualizarStatusDosCarrosCorretamente()
        {
            // Arrange
            var carroAnterior = new Carro("AAAAAAA", _dateTimeNow.Year, "Vermelho",
                _dateTimeNow.AddYears(-1), EnumCarroStatus.Vendido, _modelo);
            var novoCarro = new Carro("BBBBBBB", _dateTimeNow.Year, "Azul",
                _dateTimeNow.AddYears(-1), EnumCarroStatus.Disponivel, _modelo);

            // Act
            await _carroService.AtualizarStatusDosCarrosAsync(carroAnterior, novoCarro);

            // Assert
            Assert.Equal(EnumCarroStatus.Disponivel, carroAnterior.Status);
            Assert.Equal(EnumCarroStatus.Vendido, novoCarro.Status);

            _carroRepository.Verify(c => c.UpdatesListAsync(It.Is<IEnumerable<Carro>>(lista =>
                lista.Contains(carroAnterior) && lista.Contains(novoCarro)
            )), Times.Once);
        }

        [Fact]
        public async Task MarcarComoVendidoAsync_DeveMarcarCarroComoVendido()
        {
            _carro.ValidarDisponibilidadeParaVenda();
            await _carroService.MarcarComoVendidoAsync(_carro);
            Assert.Equal(EnumCarroStatus.Vendido, _carro.Status);
            _carroRepository.Verify(c => c.UpdateAsync(_carro), Times.Once);
        }
        [Fact]
        public async Task MarcarComoVendidoAsync_DeveLancarExcecaoQuandoCarroNaoDisponivel()
        {
            _carro.SetStatus(EnumCarroStatus.Vendido); 
            var ex = await Assert.ThrowsAsync<DomainException>(() => _carroService.MarcarComoVendidoAsync(_carro));
            Assert.Contains("O carro com a placa", ex.Message);
        }

        [Fact]
        public async Task ValidarDisponibilidadeDoCarroAsync_DeveValidarDisponibilidadeDoCarro()
        {
            _carro.ValidarDisponibilidadeParaVenda();
            await _carroService.ValidarDisponibilidadeDoCarroAsync(_carro);
            Assert.Equal(EnumCarroStatus.Disponivel, _carro.Status);
        }
        [Fact]
        public async Task ValidarDisponibilidadeDoCarroAsync_DeveLancarExcecaoQuandoCarroNaoDisponivel()
        {
            _carro.SetStatus(EnumCarroStatus.Vendido);
            var ex = await Assert.ThrowsAsync<DomainException>(() => _carroService.ValidarDisponibilidadeDoCarroAsync(_carro));
            Assert.Contains("O carro com a placa", ex.Message);
        }
        [Fact]
        public async Task GetEntidadeCarroByIdAsync_DeveRetornarCarroPorId()
        {
            _carroRepository
                .Setup(c => c.GetCarroByIdAsync(1))
                .ReturnsAsync(_carro);
            var carro = await _carroService.GetEntidadeCarroByIdAsync(1);
            Assert.NotNull(carro);
            Assert.Equal("AAAAAAA", carro.Placa);
            Assert.Equal("Vermelho", carro.Cor);
        }
        [Fact]
        public async Task GetEntidadeCarroByIdAsync_DeveRetornarDomainExceptionQuandoCarroNaoExistir()
        {
            _carroRepository
                .Setup(c => c.GetCarroByIdAsync(1))
                .ReturnsAsync((Carro)null!);
            var ex = await Assert.ThrowsAsync<DomainException>(() => _carroService.GetEntidadeCarroByIdAsync(1));
            Assert.Equal("Carro não encontrado", ex.Message);
        }
      

   
    }

}
