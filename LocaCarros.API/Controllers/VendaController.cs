using LocaCarros.Application.DTOs.VendasDtos;
using LocaCarros.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocaCarros.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendaController : ControllerBase
    {
        private IVendaService _vendaService;
        public VendaController(IVendaService vendaService)
        {
            _vendaService = vendaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendaDTO>>> Get()
        {
            var vendas = await _vendaService.GetVendasAsync();
            return Ok(vendas);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<VendaDTO?>> Get(int id)
        {
            var venda = await _vendaService.GetVendaByIdAsync(id);
            if (venda == null) return NotFound("Venda não encontrada.");
            return Ok(venda);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _vendaService.DeleteAsync(id);
            if (!result) return NotFound("Venda não encontrada ou não pode ser excluída.");
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<VendaDTO>> Put(int id, VendaDTOUpdate vendaDTOUpdate)
        {
        
            if (id != vendaDTOUpdate.Id) return BadRequest("ID da venda não corresponde ao ID fornecido na URL.");
            var venda = await _vendaService.UpdateAsync(vendaDTOUpdate);
            if (venda == null) return NotFound("Venda não encontrada.");
            return Ok(venda);
        }
        [HttpPost]
        public async Task<ActionResult<VendaDTO>> Post(VendaDTOAdd vendaDTOAdd)
        {
            if (vendaDTOAdd == null) return BadRequest("Venda não pode ser nula.");
            var venda = await _vendaService.CreateAsync(vendaDTOAdd);
            return  Ok(venda);
        }
    }
}
