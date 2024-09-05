using BitBeakAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitBeakAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestoesController : ControllerBase
    {
        private readonly BitBeakContext _context;

        public QuestoesController(BitBeakContext context)
        {
            _context = context;
        }

        // GET: api/Questoes
        [HttpGet("ObterListaQuestoes")]
        public async Task<ActionResult<IEnumerable<ModelQuestao>>> ObterListaQuestoes()
        {
            return await _context.Questoes
                .Include(q => q.Opcoes)
                .Include(q => q.Lacunas)
                .ToListAsync();
        }

        // GET: api/Questoes/5
        [HttpGet("ListarDadosQuestao/{intIdQuestao}")]
        public async Task<ActionResult<ModelQuestao>> ListarDadosQuestao(int intIdQuestao)
        {
            var objQuestao = await _context.Questoes
                .Include(q => q.Opcoes)
                .Include(q => q.Lacunas)
                .FirstOrDefaultAsync(q => q.IdQuestao == intIdQuestao);

            if (objQuestao == null)
            {
                return NotFound();
            }

            return objQuestao;
        }

        [HttpPost("CriarQuestao")]
        public async Task<ActionResult<ModelQuestao>> CriarQuestao(ModelQuestao objModelQuestao)
        {
            List<OpcaoResposta> objOpcoes = new();
            List<Lacuna> objLacunas = new();
            ModelNivelTrilha? objNivel = new();

            try
            {
                if (objModelQuestao.IdNivel.HasValue)
                {
                    objNivel = await _context.NiveisTrilha.FindAsync(objModelQuestao.IdNivel.Value);

                    if (objNivel == null)
                    {
                        return NotFound("Nível não encontrado.");
                    }

                    objModelQuestao.Nivel = objNivel;
                }

                objOpcoes = objModelQuestao.Opcoes.ToList(); 
                objLacunas = objModelQuestao.Lacunas.ToList(); 

                objModelQuestao.Opcoes.Clear();
                objModelQuestao.Lacunas.Clear();

                _context.Questoes.Add(objModelQuestao);
                await _context.SaveChangesAsync();

                foreach (OpcaoResposta objOpcao in objOpcoes)
                {
                    objOpcao.IdOpcao = 0;
                    objOpcao.IdQuestao = objModelQuestao.IdQuestao;
                    objOpcao.Questao = objModelQuestao;
                    _context.OpcoesResposta.Add(objOpcao);
                }

                foreach (Lacuna objLacuna in objLacunas)
                {
                    objLacuna.IdLacuna = 0;
                    objLacuna.IdQuestao = objModelQuestao.IdQuestao;
                    objLacuna.Questao = objModelQuestao;
                    _context.Lacunas.Add(objLacuna);
                }

                await _context.SaveChangesAsync();

                objModelQuestao.Opcoes = objOpcoes;
                objModelQuestao.Lacunas = objLacunas;

                return CreatedAtAction(nameof(ListarDadosQuestao), new { intIdQuestao = objModelQuestao.IdQuestao }, objModelQuestao);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("EditarQuestao/{intIdQuestao}")]
        public async Task<IActionResult> EditarQuestao(int intIdQuestao, ModelQuestao objModelQuestao)
        {
            if (intIdQuestao != objModelQuestao.IdQuestao)
            {
                return BadRequest();
            }

            var objQuestaoExistente = await _context.Questoes
                .Include(q => q.Opcoes)
                .Include(q => q.Lacunas)
                .FirstOrDefaultAsync(q => q.IdQuestao == intIdQuestao);

            if (objQuestaoExistente == null)
            {
                return NotFound();
            }

            objQuestaoExistente.Enunciado = objModelQuestao.Enunciado ?? objQuestaoExistente.Enunciado;
            objQuestaoExistente.Tipo = objModelQuestao.Tipo;
            objQuestaoExistente.SolucaoEsperada = objModelQuestao.SolucaoEsperada ?? objQuestaoExistente.SolucaoEsperada;

            objQuestaoExistente.Opcoes.Clear();
            foreach (OpcaoResposta objOpcao in objModelQuestao.Opcoes)
            {
                objQuestaoExistente.Opcoes.Add(objOpcao);
            }

            objQuestaoExistente.Lacunas.Clear();
            foreach (Lacuna objLacuna in objModelQuestao.Lacunas)
            {
                objLacuna.Questao = objQuestaoExistente;
                objQuestaoExistente.Lacunas.Add(objLacuna);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestaoExists(intIdQuestao))
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

        // PATCH: api/Questoes/{questaoId}/Nivel/{nivelId}
        [HttpPatch("{intIdQuestao}/Nivel/{intIdNivel}")]
        public async Task<IActionResult> AdicionarQuestaoEmNivel(int intIdQuestao, int intIdNivel)
        {
            var objQuestao = await _context.Questoes.FindAsync(intIdQuestao);
            if (objQuestao == null)
            {
                return NotFound("Questão não encontrada");
            }

            var objNivel = await _context.NiveisTrilha.FindAsync(intIdNivel);
            if (objNivel == null)
            {
                return NotFound("Nível não encontrado");
            }

            objQuestao.IdNivel = intIdNivel;
            objQuestao.Nivel = objNivel;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestaoExists(intIdQuestao))
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

        // DELETE: api/Questoes/5
        [HttpDelete("ExcluirQuestao/{intIdQuestao}")]
        public async Task<IActionResult> ExcluirQuestao(int intIdQuestao)
        {
            var objQuestao = await _context.Questoes.FindAsync(intIdQuestao);
            if (objQuestao == null)
            {
                return NotFound("Questão não encontrada!");
            }

            _context.Questoes.Remove(objQuestao);
            await _context.SaveChangesAsync();

            return Ok("Questão excluída com sucesso!");
        }

        private bool QuestaoExists(int intIdQuestao)
        {
            return _context.Questoes.Any(e => e.IdQuestao == intIdQuestao);
        }
    }
}
