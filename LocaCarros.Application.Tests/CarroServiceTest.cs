using AutoMapper;
using LocaCarros.Application.DTOs.CarrosDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Application.Services;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using LocaCarros.Domain.Exceptions;
using LocaCarros.Domain.Interfaces;
using Moq;


namespace LocaCarros.Application.Tests
{
    public class CarroServiceTest
    {
        private readonly Mock<IMapper> _mapper;
        private readonly CarroService _carroService;
        private readonly Modelo _modelo;
        private readonly Carro _carro;
        private readonly DateTime _dateTimeNow = DateTime.Now;
        private readonly CarroDTO _carroDto;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        public CarroServiceTest()
        {
            _mapper = new Mock<IMapper>();
            _unitOfWork = new Mock<IUnitOfWork>(); 
            _carroService = new CarroService(
               _unitOfWork.Object,
                _mapper.Object
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
            _unitOfWork.Setup(u => u.Carros).Returns(_unitOfWork.Object.Carros);
        }


        [Fact]
        public async Task GetCarrosAsync_DeveRetornarListaDeCarros()
        {
            _unitOfWork
                .Setup(c => c.Carros.GetCarrosAsync())
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
            _unitOfWork.Setup(c => c.Carros.GetCarrosByModeloIdAsync(1)).ReturnsAsync(new List<Carro> { _carro });

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
            _unitOfWork
                .Setup(c => c.Carros.GetCarroByIdAsync(1))
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
            _unitOfWork
                .Setup(c => c.Carros.GetCarroByIdAsync(1))
                .ReturnsAsync((Carro)null!);
            var carro = await _carroService.GetCarroByIdAsync(1);
            Assert.Null(carro);
        }
        [Fact]
        public async Task DeleteAsync_DeveDeletarCarro()
        {
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWork.Setup(c => c.Carros.DeleteAsync(_carro)).ReturnsAsync(true);
            var carroDeletado = await _carroService.DeleteAsync(1);
            Assert.True(carroDeletado);

        }
        [Fact]
        public async Task DeleteAsync_DeveRetornarFalseQuandoCarroNaoExistir()
        {
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync((Carro?)null);
            var carroDeletado = await _carroService.DeleteAsync(1);
            Assert.False(carroDeletado);
        }

        [Fact]
        public async Task DeleteAsync_QuandoDeleteRetornaFalse_DeveRetornarFalse()
        {

            _unitOfWork
                .Setup(c => c.Carros.GetCarroByIdAsync(1))
                .ReturnsAsync(_carro);

            _unitOfWork
                .Setup(c => c.Carros.DeleteAsync(_carro))
                .ReturnsAsync(false);

            var result = await _carroService.DeleteAsync(1);
            Assert.False(result);
        }
        [Fact]
        public async Task DeleteAsync_DeveLancarDomainExceptionExcecaoQuandoNaoDeletarCarro()
        {
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWork.Setup(c => c.Carros.DeleteAsync(_carro)).ThrowsAsync(new DomainException("Erro ao deletar carro."));
            var ex = await Assert.ThrowsAsync<DomainException>(() => _carroService.DeleteAsync(1));
            Assert.Equal("Erro ao deletar carro.", ex.Message);
        }
        [Fact]
        public async Task DeleteAsync_DeveLancarExceptionExcecaoQuandoNaoDeletarCarro()
        {
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWork.Setup(c => c.Carros.DeleteAsync(_carro)).ThrowsAsync(new Exception("Erro ao deletar carro."));
            var ex = await Assert.ThrowsAsync<Exception>(() => _carroService.DeleteAsync(1));
            Assert.Equal("Erro ao deletar carro.", ex.Message);
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
            _unitOfWork.Setup(m => m.Modelos.GetModeloByIdAsync(1)).ReturnsAsync(_modelo);
            _unitOfWork.Setup(c => c.Carros.CreateAsync(_carro)).ReturnsAsync(_carro);
            _mapper.Setup(m => m.Map<CarroDTO>(_carro))
             .Returns(_carroDto);
            var result = await _carroService.CreateAsync(carroDtoAdd);
            Assert.NotNull(result);
            Assert.Equal("ABC-1234", result.Placa);
            Assert.Equal("Vermelho", result.Cor);

        }
        [Fact]
        public async Task CreateAsync_DeveLancarExcecaoQuandoTentarCriar()
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
            _unitOfWork.Setup(m => m.Modelos.GetModeloByIdAsync(1)).ThrowsAsync(new Exception("Erro ao criar carro."));
          
            var ex = await Assert.ThrowsAsync<Exception>(() => _carroService.CreateAsync(carroDtoAdd));
            Assert.Equal("Erro ao criar carro.", ex.Message);
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
            _unitOfWork.Setup(m => m.Modelos.GetModeloByIdAsync(1)).ReturnsAsync((Modelo)null!);
            var ex = await Assert.ThrowsAsync<DomainException>(() => _carroService.CreateAsync(carroDtoAdd));
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
            _unitOfWork.Setup(m => m.Modelos.GetModeloByIdAsync(1)).ReturnsAsync(_modelo);
            _unitOfWork.Setup(c => c.Carros.CreateAsync(_carro)).ThrowsAsync(new DomainException("Erro ao criar carro."));
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
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync((Carro)null!);
            _unitOfWork.Setup(m => m.Modelos.GetModeloByIdAsync(1)).ReturnsAsync(_modelo);
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
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWork.Setup(m => m.Modelos.GetModeloByIdAsync(1)).ReturnsAsync((Modelo)null!);
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
            _unitOfWork.Setup(m => m.Modelos.GetModeloByIdAsync(1)).ReturnsAsync(_modelo);
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWork.Setup(c => c.Carros.UpdateAsync(_carro)).ReturnsAsync(_carro);
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
            _unitOfWork.Setup(m => m.Modelos.GetModeloByIdAsync(1)).ReturnsAsync(_modelo);
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWork.Setup(c => c.Carros.UpdateAsync(_carro)).ThrowsAsync(new Exception("Erro ao atualizar carro"));
            _mapper.Setup(m => m.Map<CarroDTO>(_carro)).Returns(_carroDto);
            var ex = await Assert.ThrowsAsync<Exception>(() => _carroService.UpdateAsync(carroDtoUpdate));
            Assert.Equal("Erro ao atualizar carro.", ex.Message);
        }
    
        [Fact]
        public async Task GetEntidadeCarroByIdAsync_DeveRetornarCarroPorId()
        {
            _unitOfWork
                .Setup(c => c.Carros.GetCarroByIdAsync(1))
                .ReturnsAsync(_carro);
            var carro = await _unitOfWork.Object.Carros.GetCarroByIdAsync(1);
            Assert.NotNull(carro);
            Assert.Equal("AAAAAAA", carro.Placa);
            Assert.Equal("Vermelho", carro.Cor);
        }
        [Fact]
        public async Task GetEntidadeCarroByIdAsync_DeveRetornarNullQuandoCarroNaoExistir()
        {
            _unitOfWork
                .Setup(c => c.Carros.GetCarroByIdAsync(1))
                .ReturnsAsync((Carro)null!);
            var carro = await _carroService.GetCarroByIdAsync(1);
            Assert.Null(carro);
        }



    }

}
