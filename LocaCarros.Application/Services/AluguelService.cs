using AutoMapper;
using LocaCarros.Application.DTOs.AlugueisDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Entities;
using LocaCarros.Domain.Enuns;
using LocaCarros.Domain.Exceptions;
using LocaCarros.Domain.Interfaces;
using System.Globalization;
namespace LocaCarros.Application.Services
{
    public class AluguelService : IAluguelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AluguelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;   
            _mapper = mapper;
        }
        public async Task<AluguelDTO> CreateAsync(AluguelDTOAdd aluguelDtoAdd)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var carro = await VerificaCarroId(aluguelDtoAdd.CarroId);
              
                var aluguel = _mapper.Map<Aluguel>(aluguelDtoAdd);
                aluguel.SetCarro(carro);
                await AtualizarDadosCarro(carro);
                var result = await _unitOfWork.Alugueis.CreateAsync(aluguel);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<AluguelDTO>(result);
            }
            catch (DomainException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception("Erro ao criar aluguel.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
               
                var aluguel = await _unitOfWork.Alugueis.GetAluguelByIdAsync(id);
                if (aluguel == null) throw new DomainException("Aluguel não encontrado!");
                var result = await _unitOfWork.Alugueis.DeleteAsync(aluguel);
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
                throw new Exception("Erro ao excluir aluguel.", ex);
            }
        }

        public async Task<IEnumerable<AluguelDTO>> GetAlugueisAsync()
        {
            var alugueis = await _unitOfWork.Alugueis.GetAlugueisAsync();
            return _mapper.Map<IEnumerable<AluguelDTO>>(alugueis);
        }

        public async Task<AluguelDTO?> GetAluguelByIdAsync(int id)
        {
            var aluguel = await _unitOfWork.Alugueis.GetAluguelByIdAsync(id);
            if (aluguel == null) return null;
            return _mapper.Map<AluguelDTO>(aluguel);
        }

        public async Task<AluguelDTO> UpdateAsync(AluguelDTOUpdate aluguelDtoUpdate)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var aluguel = await VerificarAluguelId(aluguelDtoUpdate.Id);
                if (aluguelDtoUpdate.CarroId != aluguel.CarroId)
                {
                    await AtualizarAluguelComNovoCarro(aluguel, aluguelDtoUpdate);
                }
                else
                {
                    AtualizarAluguelExistente(aluguel, aluguelDtoUpdate);
                }
                var result = await _unitOfWork.Alugueis.UpdateAsync(aluguel);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<AluguelDTO>(result);
            }
            catch (DomainException)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new Exception("Erro ao atualizar aluguel.", ex);
            }
        }


        private async Task AtualizarDadosCarro(Carro carro)
        {

            carro.ValidarDisponibilidadeParaAluguel();
            carro.SetStatus(EnumCarroStatus.Alugado);
            var modelo = await _unitOfWork.Modelos.GetModeloByIdAsync(carro.Modelo.Id);
            carro.ValidarHasModelo(modelo);
            carro.Update(carro.Placa, carro.Ano, carro.Cor, carro.DataFabricacao, carro.Status, modelo!);
            await _unitOfWork.Carros.UpdateAsync(carro);
        }
        private async Task<Carro> VerificaCarroId(int carroId)
        {
            var carro = await _unitOfWork.Carros.GetCarroByIdAsync(carroId);
           if(carro == null) throw new DomainException("Carro não encontrado.");
            return carro;
        }
        private async Task<Aluguel> VerificarAluguelId(int aluguelId)
        {
            return await _unitOfWork.Alugueis.GetAluguelByIdAsync(aluguelId) ??
              throw new DomainException("Aluguel não encontrado.");

        }
    
        private async Task AtualizarAluguelComNovoCarro(Aluguel aluguel, AluguelDTOUpdate aluguelDtoUpdate)
        {
            var carroAnterior = aluguel.Carro;
            var novoCarro = await VerificaCarroId(aluguelDtoUpdate.CarroId);
             var dataInicio = ObterDataAluguel(aluguelDtoUpdate.DataInicio);
            var dataDevolucao = ObterDataAluguel(aluguelDtoUpdate.DataDevolucao);
            aluguel.TrocarCarro(novoCarro, dataInicio, dataDevolucao, aluguelDtoUpdate.ValorAluguel);
            await AtualizarStatusDosCarrosAsync(carroAnterior, novoCarro);
        }


        private async Task AtualizarStatusDosCarrosAsync(Carro? carroAnterior, Carro novoCarro)
        {
            var carrosParaAtualizar = new List<Carro>();

            if (carroAnterior != null)
            {
                carroAnterior.SetStatus(EnumCarroStatus.Disponivel);
                carrosParaAtualizar.Add(carroAnterior);
            }

            novoCarro.SetStatus(EnumCarroStatus.Alugado);
            carrosParaAtualizar.Add(novoCarro);

            await _unitOfWork.Carros.UpdatesListAsync(carrosParaAtualizar);
        }

        private static DateTime ObterDataAluguel(string dataStr)
        {
            if (!DateTime.TryParseExact(dataStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataVenda))
                throw new DomainException("Data de aluguel inválida");
            return dataVenda;
        }
        private static void AtualizarAluguelExistente(Aluguel aluguel, AluguelDTOUpdate aluguelDtoUpdate)
        {

            aluguel.SetValorAluguel(aluguelDtoUpdate.ValorAluguel);
            aluguel.SetDataInicio(ObterDataAluguel(aluguelDtoUpdate.DataInicio));
            aluguel.SetDataDevolucao(ObterDataAluguel(aluguelDtoUpdate.DataDevolucao));
        }
    }
}
