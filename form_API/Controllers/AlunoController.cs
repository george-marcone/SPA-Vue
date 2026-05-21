using System.Threading.Tasks;
using form_API.Services;
using form_API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace form_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlunoController : ControllerBase
    {
        private readonly ILogger<AlunoController> _logger;
        private readonly IAlunoService _alunoService;

        public AlunoController(IAlunoService alunoService, ILogger<AlunoController> logger)
        {
            _alunoService = alunoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _alunoService.GetAllAsync(true);
                return Ok(result);
            }
            catch
            {
                _logger.LogError("Erro ao obter alunos");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        [HttpGet("{AlunoId}")]
        public async Task<IActionResult> GetByAlunoId(int AlunoId)
        {
            try
            {
                var result = await _alunoService.GetByIdAsync(AlunoId, true);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch
            {
                _logger.LogError("Erro ao obter aluno por id");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        [HttpGet("ByProfessor/{ProfessorId}")]
        public async Task<IActionResult> GetByProfessorId(int ProfessorId)
        {
            try
            {
                var result = await _alunoService.GetByProfessorIdAsync(ProfessorId, true);
                return Ok(result);
            }
            catch
            {
                _logger.LogError("Erro ao obter alunos por professor");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(AlunoCreateEditViewModel model)
        {
            try
            {
                var created = await _alunoService.AddAsync(model);
                return CreatedAtAction(nameof(GetByAlunoId), new { AlunoId = created.Id }, created);
            }
            catch
            {
                _logger.LogError("Erro ao criar aluno");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        [HttpPut("{AlunoId}")]
        public async Task<IActionResult> Put(int AlunoId, AlunoCreateEditViewModel model)
        {
            try
            {
                var updated = await _alunoService.UpdateAsync(AlunoId, model);
                if (updated == null) return NotFound();
                return CreatedAtAction(nameof(GetByAlunoId), new { AlunoId = updated.Id }, updated);
            }
            catch
            {
                _logger.LogError("Erro ao atualizar aluno");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }

        [HttpDelete("{AlunoId}")]
        public async Task<IActionResult> Delete(int AlunoId)
        {
            try
            {
                var deleted = await _alunoService.DeleteAsync(AlunoId);
                if (!deleted) return NotFound();
                return Ok();
            }
            catch
            {
                _logger.LogError("Erro ao excluir aluno");
                return StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados Falhou");
            }
        }
    }
}