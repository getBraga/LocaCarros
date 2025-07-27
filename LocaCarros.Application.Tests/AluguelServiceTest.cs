using AutoMapper;
using LocaCarros.Application.DTOs.AlugueisDtos;
using LocaCarros.Application.DTOs.CarrosDtos;
using LocaCarros.Application.Services;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using LocaCarros.Domain.Exceptions;
using LocaCarros.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.Tests
{
    public class AluguelServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;
        private readonly Modelo _modelo;
        private readonly Carro _carro;
        private readonly Aluguel _aluguel;
        private readonly AluguelDTO _aluguelDto;
        private readonly AluguelDTOAdd _aluguelDTOAdd;
        private readonly AluguelService _aluguelService;
        private readonly DateTime _dateTimeNow = DateTime.Now;
        private readonly AluguelDTOUpdate _aluguelUpdateDto;
        public AluguelServiceTest()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _mapper = new Mock<IMapper>();
            _aluguelService = new AluguelService(
               _unitOfWork.Object,
               _mapper.Object
           );

            _modelo = new("Modelo A", "1.0", 1.0m, EnumTipoCarroceria.Coupe, new Marca("Marca A"));
            _carro = new("ALJ1155", 2025, "Verde", _dateTimeNow.AddYears(-10), EnumCarroStatus.Disponivel, _modelo);
            _aluguel = new(123m, _dateTimeNow.AddMonths(-2), _dateTimeNow.AddMonths(-1), _carro);
            _aluguelDto = new AluguelDTO
            {
                Id = 1,
                ValorAluguel = 123m,
                DataInicio = _dateTimeNow.AddMonths(-2).ToString(),
                DataDevolucao = _dateTimeNow.AddMonths(-1).ToString(),
                ModeloNome = _modelo.Nome,
                CarroAno = _carro.Ano,
                CarroCor = _carro.Cor,
                CarroDataFabricacao = _carro.DataFabricacao.ToString("yyyy-MM-dd"),
                CarroId = _carro.Id,
                CarroStatus = _carro.Status.ToString(),
                CarroPlaca = _carro.Placa,


            };
            _aluguelDTOAdd = new AluguelDTOAdd
            {
                CarroId = 1,
                DataInicio = _dateTimeNow.AddMonths(-2).ToString(),
                DataDevolucao = _dateTimeNow.AddMonths(-1).ToString(),
                ValorAluguel = 123m
            };
            _aluguelUpdateDto = new AluguelDTOUpdate
            {
                Id = 1,
                CarroId = 1,
                DataInicio = _dateTimeNow.AddMonths(-2).ToString("dd/MM/yyyy"),
                DataDevolucao = _dateTimeNow.AddMonths(-1).ToString("dd/MM/yyyy"),
                ValorAluguel = 123m
            };
        }

        [Fact]
        public async Task DeveBuscarAlugueis()
        {
            _unitOfWork.Setup(a => a.Alugueis.GetAlugueisAsync()).ReturnsAsync(new List<Aluguel> { _aluguel });
            _mapper.Setup(m => m.Map<IEnumerable<AluguelDTO>>(It.IsAny<object>()))
                   .Returns([_aluguelDto]);

            var alugueis = await _aluguelService.GetAlugueisAsync();
            Assert.NotNull(alugueis);
            Assert.IsAssignableFrom<IEnumerable<AluguelDTO>>(alugueis);
            Assert.Single(alugueis);
            Assert.Equal(123m, alugueis.First().ValorAluguel);

        }

        [Fact]
        public async Task DeveBuscarAluguel_PorId()
        {
            _unitOfWork.Setup(a => a.Alugueis.GetAluguelByIdAsync(1)).ReturnsAsync(_aluguel);
            _mapper.Setup(m => m.Map<AluguelDTO>(It.IsAny<object>()))
                   .Returns(_aluguelDto);
            var aluguel = await _aluguelService.GetAluguelByIdAsync(1);
            Assert.NotNull(aluguel);
            Assert.IsType<AluguelDTO>(aluguel);
            Assert.Equal(1, aluguel.Id);

        }
        [Fact]
        public async Task DeveBuscarAluguel_DeveLancarExcessaoAoBuscarPorId()
        {
            _unitOfWork.Setup(a => a.Alugueis.GetAluguelByIdAsync(1)).ReturnsAsync((Aluguel)null!);
            var aluguelNull = await _aluguelService.GetAluguelByIdAsync(1);
            Assert.Null(aluguelNull);
        }
        [Fact]
        public async Task DeveCriarAluguel_Sucesso()
        {
            _unitOfWork.Setup(a => a.Alugueis.CreateAsync(_aluguel)).ReturnsAsync(_aluguel);
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWork.Setup(c => c.Modelos.GetModeloByIdAsync(1)).ReturnsAsync(_modelo);
            _mapper.Setup(m => m.Map<Aluguel>(It.IsAny<object>())).Returns(_aluguel);
            _mapper.Setup(m => m.Map<AluguelDTO>(It.IsAny<object>()))
                             .Returns(_aluguelDto);
            _modelo.GetType().GetProperty("Id")!.SetValue(_modelo, 1);
            _carro.GetType().GetProperty("ModeloId")!.SetValue(_carro, 1);
            var aluguelCreate = await _aluguelService.CreateAsync(_aluguelDTOAdd);
            Assert.NotNull(aluguelCreate);
            Assert.IsType<AluguelDTO>(aluguelCreate);
            Assert.Equal(1, aluguelCreate.Id);
        }
        [Fact]
        public async Task DeveCriarAluguel_DeveLancarExcessaoAoCriarAluguel()
        {
            _unitOfWork.Setup(a => a.Alugueis.CreateAsync(_aluguel)).Throws(new Exception("Erro ao criar aluguel."));
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWork.Setup(c => c.Modelos.GetModeloByIdAsync(1)).ReturnsAsync(_modelo);
            _mapper.Setup(m => m.Map<Aluguel>(It.IsAny<object>())).Returns(_aluguel);
            _mapper.Setup(m => m.Map<AluguelDTO>(It.IsAny<object>()))
                             .Returns(_aluguelDto);
            _modelo.GetType().GetProperty("Id")!.SetValue(_modelo, 1);
            _carro.GetType().GetProperty("ModeloId")!.SetValue(_carro, 1);
           var ex =  await Assert.ThrowsAsync<Exception>(() => _aluguelService.CreateAsync(_aluguelDTOAdd));
            Assert.Contains("Erro ao criar aluguel.", ex.Message);
        }
        [Fact]
        public async Task DeveCriarAluguel_DeveLancarExcessaoAoCriarAluguelComCarroInvalido()
        {
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync((Carro)null!);
            var ex = await Assert.ThrowsAsync<DomainException>(() => _aluguelService.CreateAsync(_aluguelDTOAdd));
            Assert.Equal("Carro não encontrado.", ex.Message);
        }

        [Fact]
        public async Task DeveAtualizarAluguel_Sucesso()
        {
            _unitOfWork.Setup(a => a.Alugueis.GetAluguelByIdAsync(1)).ReturnsAsync(_aluguel);
            _unitOfWork.Setup(a => a.Alugueis.UpdateAsync(_aluguel)).ReturnsAsync(_aluguel);
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWork.Setup(c => c.Modelos.GetModeloByIdAsync(1)).ReturnsAsync(_modelo);
            _mapper.Setup(m => m.Map<Aluguel>(It.IsAny<object>())).Returns(_aluguel);
            _mapper.Setup(m => m.Map<AluguelDTO>(It.IsAny<object>()))
                             .Returns(_aluguelDto);
            _modelo.GetType().GetProperty("Id")!.SetValue(_modelo, 1);
            _carro.GetType().GetProperty("ModeloId")!.SetValue(_carro, 1);
            var aluguelUpdate = await _aluguelService.UpdateAsync(new AluguelDTOUpdate
            {
                Id = 1,
                CarroId = 1,
                DataInicio = _dateTimeNow.AddMonths(-2).ToString("dd/MM/yyyy"),
                DataDevolucao = _dateTimeNow.AddMonths(-1).ToString("dd/MM/yyyy"),
                ValorAluguel = 123m
            });
            Assert.NotNull(aluguelUpdate);
            Assert.IsType<AluguelDTO>(aluguelUpdate);
            Assert.Equal(1, aluguelUpdate.Id);
        }
        [Fact]
        public async Task DeveAtualizarAluguel_DeveLancarExcessaoAoAtualizarAluguelComIdInvalido()
        {
            _unitOfWork.Setup(a => a.Alugueis.GetAluguelByIdAsync(1)).ReturnsAsync((Aluguel)null!);
            var ex = await Assert.ThrowsAsync<DomainException>(() => _aluguelService.UpdateAsync(new AluguelDTOUpdate
            {
                Id = 1,
                CarroId = 1,
                DataInicio = _dateTimeNow.AddMonths(-2).ToString("dd/MM/yyyy"),
                DataDevolucao = _dateTimeNow.AddMonths(-1).ToString("dd/MM/yyyy"),
                ValorAluguel = 123m
            }));
            Assert.Equal("Aluguel não encontrado.", ex.Message);
        }
        [Fact]
        public async Task DeveAtualizarAluguel_ComMesmoIdCarro()
        {
            _aluguel.GetType().GetProperty("Id")!.SetValue(_aluguel, 1);
            _aluguel.GetType().GetProperty("CarroId")!.SetValue(_aluguel, 1);
            _modelo.GetType().GetProperty("Id")!.SetValue(_modelo, 1);
            _aluguelDto.GetType().GetProperty("CarroId")!.SetValue(_aluguelDto, 1);
            _carro.GetType().GetProperty("ModeloId")!.SetValue(_carro, 1);
        
            _unitOfWork.Setup(a => a.Alugueis.GetAluguelByIdAsync(1)).ReturnsAsync(_aluguel);
            _unitOfWork.Setup(a => a.Alugueis.UpdateAsync(_aluguel)).ReturnsAsync(_aluguel);
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWork.Setup(c => c.Modelos.GetModeloByIdAsync(1)).ReturnsAsync(_modelo);
            _mapper.Setup(m => m.Map<Aluguel>(It.IsAny<object>())).Returns(_aluguel);
            _mapper.Setup(m => m.Map<AluguelDTO>(It.IsAny<object>()))
                             .Returns(_aluguelDto);
           
            var aluguelUpdate = await _aluguelService.UpdateAsync(_aluguelUpdateDto);
            Assert.NotNull(aluguelUpdate);
            Assert.IsType<AluguelDTO>(aluguelUpdate);
            Assert.Equal(1, aluguelUpdate.Id);
            Assert.Equal(1, aluguelUpdate.CarroId);
        }


        [Fact]
        public async Task DeveAtualizarAluguel_DeveLancarExcessaoAluguelDataInvalida()
        {
            _unitOfWork.Setup(a => a.Alugueis.GetAluguelByIdAsync(1)).ReturnsAsync(_aluguel);
            _unitOfWork.Setup(a => a.Alugueis.UpdateAsync(_aluguel)).ReturnsAsync(_aluguel);
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWork.Setup(c => c.Modelos.GetModeloByIdAsync(1)).ReturnsAsync(_modelo);
            _mapper.Setup(m => m.Map<Aluguel>(It.IsAny<object>())).Returns(_aluguel);
            _mapper.Setup(m => m.Map<AluguelDTO>(It.IsAny<object>()))
                             .Returns(_aluguelDto);
            _aluguel.GetType().GetProperty("Id")!.SetValue(_aluguel, 1);
            _aluguel.GetType().GetProperty("CarroId")!.SetValue(_aluguel, 1);
            _modelo.GetType().GetProperty("Id")!.SetValue(_modelo, 1);
            _carro.GetType().GetProperty("ModeloId")!.SetValue(_carro, 1);
            _aluguelUpdateDto.DataInicio = _dateTimeNow.AddMonths(-1).ToString();
            _aluguelUpdateDto.DataDevolucao = _dateTimeNow.AddMonths(-2).ToString();
            var domainEx = await Assert.ThrowsAnyAsync<DomainException>(() =>  _aluguelService.UpdateAsync(_aluguelUpdateDto));
            Assert.Equal("Data de aluguel inválida", domainEx.Message);
        }
        [Fact]
        public async Task DeveAtualizarAluguel_DeveLancarExcessaoAoAtualizarAluguel()
        {
            _unitOfWork.Setup(a => a.Alugueis.GetAluguelByIdAsync(1)).ReturnsAsync(_aluguel);
            _unitOfWork.Setup(a => a.Alugueis.UpdateAsync(_aluguel)).Throws(new Exception("Erro ao atualizar aluguel."));
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync(_carro);
            _unitOfWork.Setup(c => c.Modelos.GetModeloByIdAsync(1)).ReturnsAsync(_modelo);
            _mapper.Setup(m => m.Map<Aluguel>(It.IsAny<object>())).Returns(_aluguel);
            _mapper.Setup(m => m.Map<AluguelDTO>(It.IsAny<object>()))
                             .Returns(_aluguelDto);
            _modelo.GetType().GetProperty("Id")!.SetValue(_modelo, 1);
            _carro.GetType().GetProperty("ModeloId")!.SetValue(_carro, 1);
            var ex = await Assert.ThrowsAsync<Exception>(() => _aluguelService.UpdateAsync(_aluguelUpdateDto));
            Assert.Contains("Erro ao atualizar aluguel.", ex.Message);
        }
        [Fact]
        public async Task DeveAtualizarAluguel_DeveLancarExcessaoAoAtualizarAluguelComCarroInvalido()
        {
            _unitOfWork.Setup(a => a.Alugueis.GetAluguelByIdAsync(1)).ReturnsAsync(_aluguel);
            _unitOfWork.Setup(c => c.Carros.GetCarroByIdAsync(1)).ReturnsAsync((Carro)null!);
            var ex = await Assert.ThrowsAsync<DomainException>(() => _aluguelService.UpdateAsync(_aluguelUpdateDto));
            Assert.Equal("Carro não encontrado.", ex.Message);
        }

        [Fact]
        public async Task DeveDeletarAluguel_Sucesso() { 
        
          _unitOfWork.Setup(a => a.Alugueis.GetAluguelByIdAsync(1)).ReturnsAsync(_aluguel);
          _unitOfWork.Setup(a => a.Alugueis.DeleteAsync(_aluguel)).ReturnsAsync(true);
           var deleteAluguel =  await _aluguelService.DeleteAsync(1);
            Assert.True(deleteAluguel); 
        }
        [Fact]
        public async Task DeveDeletarAluguel_DeveLancarDomainExceptionAoNaoEncontrarAluguel() {
            _unitOfWork.Setup(a => a.Alugueis.GetAluguelByIdAsync(1)).ReturnsAsync((Aluguel)null!);
            var domainEx = await Assert.ThrowsAsync<DomainException>(() => _aluguelService.DeleteAsync(1));
            Assert.Equal("Aluguel não encontrado!", domainEx.Message);
        }
        [Fact]
        public async Task DeveDeletarAluguel_DeveLancarException() {
            _unitOfWork.Setup(a => a.Alugueis.GetAluguelByIdAsync(1)).ReturnsAsync(_aluguel);
            _unitOfWork.Setup(a => a.Alugueis.DeleteAsync(_aluguel)).Throws(new Exception("Erro ao atualizar aluguel."));
            var ex = await Assert.ThrowsAsync<Exception>(()=> _aluguelService.DeleteAsync(1));
            Assert.Contains("Erro ao excluir aluguel.", ex.Message);


        }
    }
}
