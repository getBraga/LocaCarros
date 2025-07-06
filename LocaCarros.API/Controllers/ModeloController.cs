using LocaCarros.Application.DTOs.ModelosDtos;
using LocaCarros.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocaCarros.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModeloController : ControllerBase
    {
        private readonly IModeloService _modeloService;
        public ModeloController(IModeloService modeloService)
        {
            _modeloService = modeloService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModeloDTO>>> Get()
        {
            var modelos = await _modeloService.GetModelosAsync();
            return Ok(modelos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ModeloDTO>> Get(int id)
        {
            var modelo = await _modeloService.GetByIdAsync(id);
            if (modelo == null)
            {
                return NotFound(new { message = "Nenhum modelo encontrado!" });
            }
            return Ok(modelo);
        }

        [HttpPost]
        public async Task<ActionResult<ModeloDTO>> Post(ModeloDTOAdd modeloDtoAdd)
        {
            var modelo = await _modeloService.AddAsync(modeloDtoAdd);
            return Ok(modelo);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ModeloDTO>> Update(int id, ModeloDTOUpdate modeloDto)
        {
            if (id != modeloDto.Id)
            {
                return BadRequest(new { message = "Id do modelo não confere!" });
            }
            var modelo = await _modeloService.UpdateAsync(modeloDto);
            return Ok(modelo);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<ModeloDTO>> Delete(int id)
        {
            var modeloDelete = await _modeloService.RemoveAsync(id);
            if (!modeloDelete)
            {
                return NotFound(new { message = "Modelo não encontrado!" });
            }
            return Ok(new { message = "Modelo removido com sucesso!" });
        }
    }
}
