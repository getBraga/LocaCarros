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
            _unitOfWork = unitOfWork;
        }
        public async Task<VendaDTO> CreateAsync(VendaDTOAdd vendaDtoAdd)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
           
                var venda = _mapper.Map<Venda>(vendaDtoAdd);

                var dataVenda = ObterDataVendaValida(vendaDtoAdd.DataVenda);
                venda.SetDataVenda(dataVenda);
                var carro = await VerificarCarroPorId(vendaDtoAdd.CarroId, venda);
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

        public async Task<bool> DeleteAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var venda = await ObterVendaOuLancarAsync(id);
                await AtualizarCarroParaDisponivelAsync(venda);
                var result = await _unitOfWork.Vendas.DeleteAsync(venda);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch (DomainException ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new DomainException(ex.Message);
            }
         
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception("Aconteceu um erro inesperado ao excluir.", ex);
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
            catch (DomainException ex)
            {
                throw new DomainException(ex.Message);
            }
            catch (Exception)
            {
                throw new Exception("Aconteceu um erro inesperado.");
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
                    await AtualizarVendaExistente(venda, vendaDtoUpdate);
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


        private async Task<Carro> VerificarCarroPorId(int carroIdDto , Venda venda )
        {
          
            var carro = await _unitOfWork.Carros.GetCarroByIdAsync(carroIdDto);
            venda.SetCarro(carro);
            return carro!;


        }
        private async Task MarcarComoVendido(Carro carro)
        {
            carro.ValidarDisponibilidadeParaVenda();
            carro.SetStatus(EnumCarroStatus.Vendido);
            await _unitOfWork.Carros.UpdateAsync(carro);
        }

        private async Task AtualizarVendaComNovoCarroAsync(Venda venda, VendaDTOUpdate dto)
        {
            var carroAnterior = venda.Carro;
            var novoCarro = await VerificarCarroPorId(dto.CarroId, venda);
            venda.SetValorVenda(dto.ValorVenda);
            var dataVenda = ObterDataVendaValida(dto.DataVenda);
            venda.SetDataVenda(dataVenda);
            await AtualizarStatusDosCarrosVendaAsync(carroAnterior, novoCarro);
        }

        private async Task AtualizarVendaExistente(Venda venda, VendaDTOUpdate dto)
        {
             await VerificarCarroPorId(dto.CarroId, venda);

            _mapper.Map(dto, venda);
            var dataVenda = ObterDataVendaValida(dto.DataVenda);
            venda.SetDataVenda(dataVenda);

        }

        private async Task<Venda> ObterVendaOuLancarAsync(int vendaId)
        {
            var venda = await _unitOfWork.Vendas.GetVendaByIdAsync(vendaId);
            return venda ?? throw new DomainException("Venda não encontrada!");
        }

        private DateTime ObterDataVendaValida(string dataVendaStr)
        {
            
            string[] formatosAceitos = {
            "dd/MM/yyyy",
            "dd/MM/yyyy HH:mm:ss"
        };
            if (!DateTime.TryParseExact(dataVendaStr, formatosAceitos, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataVenda))
                throw new DomainException("Data da venda inválida.");
        
            return dataVenda;
        }


        private async Task AtualizarCarroParaDisponivelAsync(Venda venda)
        {
            var carro =  await VerificarCarroPorId(venda.CarroId, venda);
            carro.SetStatus(EnumCarroStatus.Disponivel);
            await AtualizarDadosDoCarroAsync(carro);

        }
        private async Task<Carro> AtualizarDadosDoCarroAsync(Carro carroUpdate)
        {

            var modelo = await _unitOfWork.Modelos.GetModeloByIdAsync(carroUpdate.Modelo.Id);
            carroUpdate.ValidarHasModelo(modelo);
            carroUpdate.Update(carroUpdate.Placa, carroUpdate.Ano, carroUpdate.Cor, carroUpdate.DataFabricacao, carroUpdate.Status, modelo!);
            var result = await _unitOfWork.Carros.UpdateAsync(carroUpdate);
            return result;
        }

        private async Task AtualizarStatusDosCarrosVendaAsync(Carro? carroAnterior, Carro novoCarro)
        {
            novoCarro.ValidarDisponibilidadeParaVenda();
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
