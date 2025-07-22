using AutoMapper;
using LocaCarros.Application.DTOs.CarrosDtos;
using LocaCarros.Application.DTOs.VendasDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using LocaCarros.Domain.Exceptions;
using LocaCarros.Domain.Interfaces;


namespace LocaCarros.Application.Services
{
    public class VendaService : IVendaService
    {
        private readonly IVendaRepository _vendaRepository;
        private readonly IMapper _mapper;
        private readonly ICarroRepository _carroRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICarroService _carroService;
        public VendaService(IVendaRepository vendaRepository, IMapper mapper, ICarroRepository carroRepository, IUnitOfWork unitOfWork,  ICarroService carroService)
        {
            _vendaRepository = vendaRepository;
            _mapper = mapper;
            _carroRepository = carroRepository;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _carroService = carroService;
        }
        public async Task<VendaDTO> CreateAsync(VendaDTOAdd vendaDtoAdd)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var dataVenda = ObterDataVendaValida(vendaDtoAdd.DataVenda);


                var carro = await _carroService.GetEntidadeCarroByIdAsync(vendaDtoAdd.CarroId);
                if (carro == null)
                    throw new DomainException("Carro não encontrado");

                var venda = _mapper.Map<Venda>(vendaDtoAdd);
                venda.SetDataVenda(dataVenda);
                venda.SetCarro(carro);

                var vendaResult = await _unitOfWork.Vendas.CreateAsync(venda);
                await _carroService.MarcarComoVendidoAsync(carro);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<VendaDTO>(vendaResult);
            }
            catch (DomainException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception("Erro inesperado ao criar venda.", ex);
            }
        }


        private async Task<Venda?> VendaExistsAsync(int id)
        {
            var venda = await _vendaRepository.GetVendaByIdAsync(id);
            return venda;
        }
        private async Task AtualizarCarroParaDisponivelAsync(int carroId)
        {
            var carro = await _carroRepository.GetCarroByIdAsync(carroId);
            if (carro != null)
            {
                carro.SetStatus(EnumCarroStatus.Disponivel);
                await _carroRepository.UpdateAsync(carro);
            }
        }
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var venda = await VendaExistsAsync(id);
                if (venda == null) return false;
                await _unitOfWork.BeginTransactionAsync();
                await AtualizarCarroParaDisponivelAsync(venda.Carro.Id);
                var result = await _vendaRepository.DeleteAsync(venda);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception("Aconteceu um erro inesperado ao excluir", ex);
            }

        }

        public async Task<VendaDTO?> GetVendaByIdAsync(int id)
        {
            try
            {
                var venda = await _vendaRepository.GetVendaByIdAsync(id);
                if (venda == null) return null;
                return _mapper.Map<VendaDTO>(venda);
            }
            catch (Exception)
            {
                throw new Exception("Aconteceu um erro inesperado ao buscar");
            }
        }

        public async Task<IEnumerable<VendaDTO>> GetVendasAsync()
        {
            var vendas = await _vendaRepository.GetVendasAsync();
            return _mapper.Map<IEnumerable<VendaDTO>>(vendas);
        }



        public async Task<VendaDTO> UpdateAsync(VendaDTOUpdate vendaDtoUpdate)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var venda = await ObterVendaExistenteAsync(vendaDtoUpdate.Id);

                if (vendaDtoUpdate.CarroId != venda.CarroId)
                {
                    await AtualizarVendaComNovoCarroAsync(venda, vendaDtoUpdate);
                }
                else
                {
                    AtualizarVendaExistente(venda, vendaDtoUpdate);
                }

                var vendaAtualizada = await _unitOfWork.Vendas.UpdateAsync(venda);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<VendaDTO>(vendaAtualizada);
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task AtualizarVendaComNovoCarroAsync(Venda venda, VendaDTOUpdate dto)
        {
            var carroAnterior = venda.Carro;
            var novoCarro = await _carroService.GetEntidadeCarroByIdAsync(dto.CarroId);
            if (novoCarro == null)
                throw new DomainException("Carro não encontrado");
            var dataVenda = ObterDataVendaValida(dto.DataVenda);

            venda.SetDataVenda(dataVenda);
            venda.SetValorVenda(dto.ValorVenda);
            venda.SetCarro(novoCarro);

            await _carroService.AtualizarStatusDosCarrosAsync(carroAnterior, novoCarro);
        }

        private void AtualizarVendaExistente(Venda venda, VendaDTOUpdate dto)
        {
            _mapper.Map(dto, venda);
        }

        private async Task<Venda> ObterVendaExistenteAsync(int vendaId)
        {
            var venda = await _unitOfWork.Vendas.GetVendaByIdAsync(vendaId);
            return venda ?? throw new DomainException("Venda não encontrada");
        }

        private DateTime ObterDataVendaValida(string dataVendaStr)
        {
            if (!DateTime.TryParse(dataVendaStr, out var dataVenda))
                throw new DomainException("Data de venda inválida");
            return dataVenda;
        }
    }
}
