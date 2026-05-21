using System.Threading.Tasks;
using form_API.Services;
using form_API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace form_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessorController : ControllerBase
    {
        private readonly ILogger<ProfessorController> _logger;
        private readonly IProfessorService _professorService;

        public ProfessorController(IProfessorService professorService, ILogger<ProfessorController> logger)
        {
            _professorService = professorService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _professorService.GetAllAsync(true);
                return Ok(result);
            }
            catch
            {
                _logger.LogError("Erro ao obter professores");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        [HttpGet("{ProfessorId}")]
        public async Task<IActionResult> GetByProfessorId(int ProfessorId)
        {
            try
            {
                var result = await _professorService.GetByIdAsync(ProfessorId, true);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch
            {
                _logger.LogError("Erro ao obter professor por id");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProfessorCreateEditViewModel model)
        {
            try
            {
                var created = await _professorService.AddAsync(model);
                return CreatedAtAction(nameof(GetByProfessorId), new { ProfessorId = created.Id }, created);
            }
            catch
            {
                _logger.LogError("Erro ao criar professor");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        [HttpPut("{ProfessorId}")]
        public async Task<IActionResult> Put(int ProfessorId, ProfessorCreateEditViewModel model)
        {
            try
            {
                var updated = await _professorService.UpdateAsync(ProfessorId, model);
                if (updated == null) return NotFound();
                return CreatedAtAction(nameof(GetByProfessorId), new { ProfessorId = updated.Id }, updated);
            }
            catch
            {
                _logger.LogError("Erro ao atualizar professor");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        [HttpDelete("{ProfessorId}")]
        public async Task<IActionResult> Delete(int ProfessorId)
        {
            try
            {
                var deleted = await _professorService.DeleteAsync(ProfessorId);
                if (!deleted) return NotFound();
                return Ok();
            }
            catch
            {
                _logger.LogError("Erro ao excluir professor");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }
    }
}