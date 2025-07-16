using LocaCarros.Application.DTOs.AlugueisDtos;
using LocaCarros.Application.Interfaces;
using LocaCarros.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocaCarros.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AluguelController : ControllerBase
    {
        private readonly IAluguelService _aluguelService;
        public AluguelController(IAluguelService aluguelService)
        {
            _aluguelService = aluguelService ?? throw new ArgumentNullException(nameof(aluguelService));
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AluguelDTO>>> Get()
        {
           
            var alugueis = await _aluguelService.GetAlugueisAsync();
            return Ok(new {success = true, data = alugueis });
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AluguelDTO?>> Get(int id)
        {
            var aluguel = await _aluguelService.GetAluguelByIdAsync(id);
            if (aluguel == null) return NotFound(new {success = false, message = "Aluguel não encontrado." });
            return Ok(new { success = true, data = aluguel });
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _aluguelService.DeleteAsync(id);
            if (!result) return NotFound(new { success = false, message = "Aluguel não encontrado ou não pode ser excluído." });
            return Ok(new { success = true, message = "Aluguel removido com sucesso!"});
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<AluguelDTO>> Put(int id, AluguelDTOUpdate aluguelDTOUpdate)
        {
            if (id != aluguelDTOUpdate.Id) return BadRequest(new {success = false , message = "ID do aluguel não corresponde ao ID fornecido na URL." });
            var aluguel = await _aluguelService.UpdateAsync(aluguelDTOUpdate);
            if (aluguel == null) return NotFound(new {success = false, message = "Aluguel não encontrado." });
            return Ok(new {success = true, data  = aluguel });
        }

        [HttpPost]
        public async Task<ActionResult<AluguelDTO>> Post(AluguelDTOAdd aluguelDTOAdd)
        {
            var aluguel = await _aluguelService.CreateAsync(aluguelDTOAdd);
            return Ok(new {success = true, data = aluguel });
        }
    }
}
