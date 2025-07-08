using AutoMapper;
using LocaCarros.Application.DTOs.VendasDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using LocaCarros.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Application.Services
{
    public class VendaService : IVendaService
    {
        private readonly IVendaRepository _vendaRepository;
        private readonly IMapper _mapper;
        private readonly ICarroRepository _carroRepository;
        private readonly IUnitOfWork _unitOfWork;
        public VendaService(IVendaRepository vendaRepository, IMapper mapper, ICarroRepository carroRepository, IUnitOfWork unitOfWork)
        {
            _vendaRepository = vendaRepository;
            _mapper = mapper;
            _carroRepository = carroRepository;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<VendaDTO> CreateAsync(VendaDTOAdd vendaDtoAdd)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var venda = _mapper.Map<Venda>(vendaDtoAdd);
                var carro = await _carroRepository.GetCarroByIdAsync(vendaDtoAdd.CarroId);
                carro?.ValidarDisponibilidadeParaVenda();


                var vendaResult = await _unitOfWork.Vendas.CreateAsync(venda);
                vendaResult.Carro.SetStatus(EnumCarroStatus.Vendido);
                await _unitOfWork.Carros.UpdateAsync(vendaResult.Carro);

                await _unitOfWork.CommitAsync();


                return _mapper.Map<VendaDTO>(vendaResult);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                if (ex.Message.Contains("O carro com a placa"))
                    throw;

                throw new Exception("Erro inesperado ao criar venda.");
            }
        }


        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var venda = await _vendaRepository.GetVendaByIdAsync(id);
                if (venda == null) return false;
                return await _vendaRepository.DeleteAsync(venda);
            }
            catch (Exception)
            {
                throw new Exception("Aconteceu um erro inesperado ao excluir");
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
                var existingVenda = await _unitOfWork.Vendas.GetVendaByIdAsync(vendaDtoUpdate.Id);
                if (existingVenda == null)
                    throw new Exception("Venda não encontrada");

                var carroAnterior = existingVenda.Carro;

                var novoCarro = await _unitOfWork.Carros.GetCarroByIdAsync(vendaDtoUpdate.CarroId);
                if (novoCarro == null)
                    throw new Exception("Carro novo não encontrado");


                if (!DateTime.TryParse(vendaDtoUpdate.DataVenda, out var dataVenda))
                    throw new Exception("Data de venda inválida");


                existingVenda.SetDataVenda(dataVenda);
                existingVenda.SetValorVenda(vendaDtoUpdate.ValorVenda);
                existingVenda.SetCarro(novoCarro);


                if (carroAnterior?.Id != novoCarro.Id)
                {
                    if (carroAnterior != null)
                    {
                        carroAnterior.SetStatus(EnumCarroStatus.Disponivel);
                        await _unitOfWork.Carros.UpdateAsync(carroAnterior);
                    }

                    novoCarro.SetStatus(EnumCarroStatus.Vendido);
                    await _unitOfWork.Carros.UpdateAsync(novoCarro);
                }


                var resultVenda = await _unitOfWork.Vendas.UpdateAsync(existingVenda);


                await _unitOfWork.CommitAsync();


                return _mapper.Map<VendaDTO>(resultVenda);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
