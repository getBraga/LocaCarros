using LocaCarros.Application.DTOs.CarrosDtos;
using LocaCarros.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocaCarros.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarroController : ControllerBase
    {
        private readonly ICarroService _carroService;
        public CarroController(ICarroService carroService)
        {
            _carroService = carroService ?? throw new ArgumentNullException(nameof(carroService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarroDTO>>> Get()
        {
            var carros = await _carroService.GetCarrosAsync();
            return Ok(carros);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarroDTO>> Get(int id)
        {
            var carro = await _carroService.GetCarroByIdAsync(id);
            if (carro == null) return NotFound("O carro não foi encontrado!");
            return Ok(carro);
        }

        [HttpPost]
        public async Task<ActionResult<CarroDTO>> Post(CarroDTOAdd carroDtoAdd)
        {
            var carro = await _carroService.CreateAsync(carroDtoAdd);
            return Ok(carro);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _carroService.DeleteAsync(id);
            if (!result)
            {
                return NotFound("Carro não encontrado!");
            }
            return Ok("Carro removido com sucesso!");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CarroDTO>> Update(int id, CarroDTOUpdate carroDto)
        {
            if (id != carroDto.Id)
            {
                return BadRequest("Id do carro não confere!");
            }

            var updatedCarro = await _carroService.UpdateAsync(carroDto);
            if(updatedCarro == null)
            {
                return NotFound("Carro não encontrado!");
            }   
            return Ok(updatedCarro);

        }
    }
}
