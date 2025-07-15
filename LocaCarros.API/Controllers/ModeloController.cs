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
            return Ok(new {success= true, data = modelos });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ModeloDTO>> Get(int id)
        {
            var modelo = await _modeloService.GetByIdAsync(id);
            if (modelo == null)
            {
                return NotFound(new { sucess = false, message = "Nenhum modelo encontrado!" });
            }
            return Ok(new { success = true, data = modelo }); ;
        }

        [HttpPost]
        public async Task<ActionResult<ModeloDTO>> Post(ModeloDTOAdd modeloDtoAdd)
        {
            var modelo = await _modeloService.AddAsync(modeloDtoAdd);
            if (modelo == null)
            {
                return BadRequest(new { message = "Falha ao criar modelo!" });
            }
            return Ok(modelo);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ModeloDTO>> Put(int id, ModeloDTOUpdate modeloDto)
        {
            if (id != modeloDto.Id)
            {
                return BadRequest(new {sucess= false, message = "Id do modelo não confere!" });
            }
            var modelo = await _modeloService.UpdateAsync(modeloDto);

            return Ok(new { success = true, data = modelo });
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<ModeloDTO>> Delete(int id)
        {
            var modeloDelete = await _modeloService.RemoveAsync(id);
            if (!modeloDelete)
            {
                return NotFound(new { sucess = false, message = "Modelo não encontrado!" });
            }
            return Ok(new { sucess = true, message = "Modelo removido com sucesso!" });
        }
    }
}
