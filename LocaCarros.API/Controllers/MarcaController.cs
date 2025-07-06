using LocaCarros.Application.DTOs.MarcasDtos;
using LocaCarros.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LocaCarros.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MarcaController : ControllerBase
    {
        private readonly IMarcaService _marcaService;
        public MarcaController(IMarcaService marcaService)
        {
            _marcaService = marcaService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarcaDTO>>> Get()
        {
            var marcas = await _marcaService.GetMarcasAsync();
            return Ok(marcas);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<MarcaDTO>> Get(int id)
        {
            var marca = await _marcaService.GetByIdAsync(id);
            if (marca == null)
            {
                return NotFound(new { message = "Nenhuma marca encontrada!" });
            }
            return Ok(marca);
        }

        [HttpPost]
        public async Task<ActionResult<MarcaDTO>> Post(MarcaDTOAdd marcaDto)
        {
            var marca = await _marcaService.AddAsync(marcaDto);
            return Ok(marca);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<MarcaDTO>> Update(int id, MarcaDTO marcaDto)
        {
            if (id != marcaDto.Id)
            {
                return BadRequest(new { message = "Id da marca não confere!" });
            }
            var marca = await _marcaService.UpdateAsync(marcaDto);
            return Ok(marca);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<MarcaDTO>> Delete(int id)
        {
            try
            {
                var marcaDelete = await _marcaService.RemoveAsync(id);
                if (!marcaDelete)
                {
                    return NotFound(new { message = "Marca não encontrada!" });
                }
                return Ok(new { message = "Marca removida com sucesso!" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
