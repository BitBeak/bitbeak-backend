using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BitBeakAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitBeakAPI.Services;

namespace BitBeakAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrilhasController : ControllerBase
    {
        private readonly BitBeakContext _context;
        private readonly QuestaoService _questaoService;

        // Injetar tanto o contexto do banco de dados quanto o serviço de questões
        public TrilhasController(BitBeakContext context, QuestaoService questaoService)
        {
            _context = context;
            _questaoService = questaoService;
        }

        #region Administradores

        // GET: api/Trilhas
        [HttpGet("ObterListaTrilhas")]
        public async Task<ActionResult<IEnumerable<ModelTrilha>>> ObterListaTrilhas()
        {
            return await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .ToListAsync();
        }

        // GET: api/Trilhas/5
        [HttpGet("ListarDadosTrilha/{intId}")]
        public async Task<ActionResult<ModelTrilha>> ListarDadosTrilha(int intId)
        {
            var objTrilha = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .FirstOrDefaultAsync(t => t.IdTrilha == intId);

            if (objTrilha == null)
            {
                return NotFound();
            }

            return objTrilha;
        }

        // POST: api/Trilhas
        [HttpPost("CriarTrilha")]
        public async Task<ActionResult<ModelTrilha>> CriarTrilha(ModelTrilha trilha)
        {
            _context.Trilhas.Add(trilha);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ListarDadosTrilha), new { intId = trilha.IdTrilha }, trilha);
        }

        // PUT: api/Trilhas/5
        [HttpPut("EditarTrilha/{intId}")]
        public async Task<IActionResult> EditarTrilha(int intId, ModelTrilha objTrilha)
        {
            if (intId != objTrilha.IdTrilha)
            {
                return BadRequest();
            }

            var objTrilhaExistente = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .FirstOrDefaultAsync(t => t.IdTrilha == intId);

            if (objTrilhaExistente == null)
            {
                return NotFound();
            }

            objTrilhaExistente.Nome = objTrilha.Nome ?? objTrilhaExistente.Nome;
            objTrilhaExistente.Descricao = objTrilha.Descricao ?? objTrilhaExistente.Descricao;

            foreach (ModelNivelTrilha objNivelTriha in objTrilha.Niveis)
            {
                var objNivelExistente = objTrilhaExistente.Niveis.FirstOrDefault(n => n.IdNivel == objNivelTriha.IdNivel);
                if (objNivelExistente != null)
                {
                    objNivelExistente.Nivel = objNivelTriha.Nivel;
                    objNivelExistente.Questoes = objNivelTriha.Questoes;
                }
                else
                {
                    objTrilhaExistente.Niveis.Add(objNivelTriha);
                }
            }

            _context.Entry(objTrilhaExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrilhaExists(intId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Trilhas/5
        [HttpDelete("ExcluirTrilha/{intId}")]
        public async Task<IActionResult> ExcluirTrilha(int intId)
        {
            var objTrilha = await _context.Trilhas.FindAsync(intId);
            if (objTrilha == null)
            {
                return NotFound();
            }

            _context.Trilhas.Remove(objTrilha);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        #region Usuários

        // Função para entrar na trilha
        [HttpGet("EntrarNaTrilha/{intId}")]
        public async Task<ActionResult<ModelTrilha>> EntrarNaTrilha(int intId)
        {
            var objTrilha = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .FirstOrDefaultAsync(t => t.IdTrilha == intId);

            if (objTrilha == null)
            {
                return NotFound();
            }

            return objTrilha;
        }

       


        #endregion

        private bool TrilhaExists(int intId)
        {
            return _context.Trilhas.Any(e => e.IdTrilha == intId);
        }
    }
}
