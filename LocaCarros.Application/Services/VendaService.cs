using AutoMapper;
using LocaCarros.Application.DTOs.VendasDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using LocaCarros.Domain.Exceptions;
using LocaCarros.Domain.Interfaces;
using System.Globalization;


namespace LocaCarros.Application.Services
{
    public class VendaService : IVendaService
    {
        private readonly IMapper _mapper;
     
        private readonly IUnitOfWork _unitOfWork;
        
        public VendaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
          
            _mapper = mapper;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<VendaDTO> CreateAsync(VendaDTOAdd vendaDtoAdd)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var dataVenda = ObterDataVendaValida(vendaDtoAdd.DataVenda);

                var carro = await VerificarCarroPorId(vendaDtoAdd.CarroId);
                var venda = _mapper.Map<Venda>(vendaDtoAdd);
                venda.SetDataVenda(dataVenda);
                venda.SetCarro(carro);

                var vendaResult = await _unitOfWork.Vendas.CreateAsync(venda);
                await MarcarComoVendido(carro);
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

        private async Task<Carro> VerificarCarroPorId(int carroId)
        {
            var carro = await _unitOfWork.Carros.GetCarroByIdAsync(carroId) 
                ?? throw new DomainException("Carro não encontrado");
            return carro;
        }
       private async Task MarcarComoVendido(Carro carro)
        {
            carro.ValidarDisponibilidadeParaVenda();
            carro.SetStatus(EnumCarroStatus.Vendido);
            await _unitOfWork.Carros.UpdateAsync(carro);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var venda = await ObterVendaOuLancarAsync(id);
                await AtualizarCarroParaDisponivelAsync(venda.Carro.Id);
                var result = await _unitOfWork.Vendas.DeleteAsync(venda);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch(DomainException ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new DomainException(ex.Message);
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
                var venda = await _unitOfWork.Vendas.GetVendaByIdAsync(id);
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
            var vendas = await _unitOfWork.Vendas.GetVendasAsync();
            return _mapper.Map<IEnumerable<VendaDTO>>(vendas);
        }



        public async Task<VendaDTO> UpdateAsync(VendaDTOUpdate vendaDtoUpdate)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var venda = await ObterVendaOuLancarAsync(vendaDtoUpdate.Id);

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
            var novoCarro = await VerificarCarroPorId(dto.CarroId);
            var dataVenda = ObterDataVendaValida(dto.DataVenda);

            venda.SetDataVenda(dataVenda);
            venda.SetValorVenda(dto.ValorVenda);
            venda.SetCarro(novoCarro);

            await AtualizarStatusDosCarrosVendaAsync(carroAnterior, novoCarro);
        }

        private void AtualizarVendaExistente(Venda venda, VendaDTOUpdate dto)
        {
            _mapper.Map(dto, venda);
        }

        private async Task<Venda> ObterVendaOuLancarAsync(int vendaId)
        {
            var venda = await _unitOfWork.Vendas.GetVendaByIdAsync(vendaId);
            return venda ?? throw new DomainException("Venda não encontrada");
        }

        private DateTime ObterDataVendaValida(string dataVendaStr)
        {
            if (!DateTime.TryParseExact(dataVendaStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataVenda))
                throw new DomainException("Data de venda inválida");
            return dataVenda;
        }


        private async Task AtualizarCarroParaDisponivelAsync(int carroId)
        {
            var carro = await _unitOfWork.Carros.GetCarroByIdAsync(carroId);
            if (carro != null)
            {
                carro.SetStatus(EnumCarroStatus.Disponivel);
                await AtualizarDadosCarrosRemoverAsync(carro);
            }
        }
        private async Task<Carro> AtualizarDadosCarrosRemoverAsync(Carro carroUpdate)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var carro = await _unitOfWork.Carros.GetCarroByIdAsync(carroUpdate.Id);
                if (carro == null)
                    throw new DomainException("Carro não encontrado.");
                var modelo = await _unitOfWork.Modelos.GetModeloByIdAsync(carroUpdate.Modelo.Id);
                if (modelo == null)
                    throw new DomainException("Modelo não encontrado.");
                carro.Update(carroUpdate.Placa, carroUpdate.Ano, carroUpdate.Cor, carroUpdate.DataFabricacao, carroUpdate.Status, modelo);
                var result = await _unitOfWork.Carros.UpdateAsync(carro);
                await _unitOfWork.CommitAsync();
                return result;
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

        private async Task AtualizarStatusDosCarrosVendaAsync(Carro? carroAnterior, Carro novoCarro)
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
    }
}
