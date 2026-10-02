using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Villa_API.Data;
using Villa_API.DTO;
using Villa_API.Models;

namespace Villa_API.Controllers
{
    [Route("api/villa")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<VillaDTO>>>> GetVillas()
        {
            var villas = await _db.Villa.ToListAsync();
            var dtoResponseVilla=_mapper.Map<List<VillaDTO>>(villas);
            var response=ApiResponse<IEnumerable<VillaDTO>>.Ok(dtoResponseVilla, "Villas retrieved successfully");
            return Ok(response);
        }

        [HttpGet("{Id:int}")]
        public async Task<ActionResult<ApiResponse<VillaDTO>>> GetVillaById(int Id)
        {
            try
            {
                if (Id <= 0)
                {

                    return NotFound(ApiResponse<object>.NotFound("Villa ID must be greater than 0"));
                }

                var villa = await _db.Villa.FirstOrDefaultAsync(u => u.Id == Id);
                if (villa == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Villa ID {Id} was not found"));
                }

                return Ok(ApiResponse<VillaDTO>.Ok(_mapper.Map<VillaDTO>(villa), "Records retrieved successfully"));
            }

            catch (Exception ex)
            {

                var response = ApiResponse<object>.Error(StatusCodes.Status500InternalServerError, $"An error occurred while retreiving villa with ID {Id}:{ex.Message}");
                return StatusCode(500,response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<VillaCreateDTO>>> CreateVilla(VillaCreateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa  data is required"));
                }

                Villa villa = _mapper.Map<Villa>(villaDTO);

                var duplicateVilla = await _db.Villa.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDTO.Name.ToLower());
                if (duplicateVilla != null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"Villa with name ' {villaDTO.Name} ' already exists"));
                }

                await _db.Villa.AddAsync(villa);
                await _db.SaveChangesAsync();
                var response = ApiResponse<VillaDTO>.CreatedAt(_mapper.Map<VillaDTO>(villa), "Villas created successfully");
                return CreatedAtAction(nameof(CreateVilla), new { id = villa.Id },response);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while creating villa {ex.Message}");
            }
        }

    
        [HttpPut("{Id:int}")]
        public async Task<ActionResult<ApiResponse<VillaDTO>>> UpdatedVilla(int Id, VillaUpdateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa  data is required"));
                }

                if (Id!=villaDTO.Id)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Villa  data is required"));
                }

                var existingVilla = await _db.Villa.FirstOrDefaultAsync(u => u.Id == Id);
                if (existingVilla == null)
                {
                    return NotFound($"Villa with ID {Id} was not found");
                }


                var duplicateVilla=await _db.Villa.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDTO.Name.ToLower());
                if (duplicateVilla != null)
                {
                    return Conflict(ApiResponse<object>.Conflict($"Villa with name ' {villaDTO.Name} ' already exists"));

                }

                _mapper.Map(villaDTO, existingVilla);
                existingVilla.UpdatedDate = DateTime.UtcNow;
               
                await _db.SaveChangesAsync();

                var response = ApiResponse<VillaDTO>.Ok(_mapper.Map<VillaDTO>(villaDTO), "Villas updated successfully");
                return Ok(villaDTO);
            }
            catch (Exception ex)
            {

                var response = ApiResponse<object>.Error(StatusCodes.Status500InternalServerError, $"An error occurred while updateding Villas " ,ex.Message);
                return StatusCode(500, response);

            }
        }

        [HttpDelete("{Id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> DeletedVilla(int Id)
        {
            try
            {
               
                var existingVilla = await _db.Villa.FirstOrDefaultAsync(u => u.Id == Id);
                if (existingVilla == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Villa with ID {Id} not found"));
                }
                _db.Villa.Remove(existingVilla);
                await _db.SaveChangesAsync();

                var response=ApiResponse<object>.NoContent($"Villa ID {Id} deleted successfully");
                return NoContent();

            }
            catch (Exception ex)
            {

                var response = ApiResponse<object>.Error(StatusCodes.Status500InternalServerError, $"An error occurred while deleting villas ", ex.Message);
                return StatusCode(500, response);
            }
        }
    }
}
