using LocaCarros.Application.DTOs.CarrosDtos;
using LocaCarros.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [Authorize]
        public async Task<ActionResult<IEnumerable<CarroDTO>>> Get()
        {
           
            var carros = await _carroService.GetCarrosAsync();
            return Ok(new {success=true, data = carros });
        }
       
        [HttpGet("{id}")]
        public async Task<ActionResult<CarroDTO>> Get(int id)
        {
            var carro = await _carroService.GetCarroByIdAsync(id);
            if (carro == null) return NotFound(new {success=false, message= "O carro não foi encontrado!" });
            return Ok(new {success = true, data = carro });
        }

        [HttpPost]
        public async Task<ActionResult<CarroDTO>> Post(CarroDTOAdd carroDtoAdd)
        {
            var carro = await _carroService.CreateAsync(carroDtoAdd);
            return Ok(new {success = true, data = carro });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var result = await _carroService.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new {success=false, message= "Carro não encontrado!" });
            }
            return Ok(new {success=true, message= "Carro removido com sucesso!" });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CarroDTO>> Put(int id, CarroDTOUpdate carroDto)
        {
            if (id != carroDto.Id)
            {
                return BadRequest(new {success = false, message = "Id do carro não confere!" });
            }

            var updatedCarro = await _carroService.UpdateAsync(carroDto);
            if(updatedCarro == null)
            {
                return NotFound(new { success = false, message = "Carro não encontrado!" });
            }   
            return Ok(new {success = true, data = updatedCarro});

        }
    }
}
