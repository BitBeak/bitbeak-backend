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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModelQuestao>>> ObterListaQuestoes()
        {
            return await _context.Questoes.Include(q => q.Opcoes).Include(q => q.Lacunas).ToListAsync();
        }

        // GET: api/Questoes/5
        [HttpGet("{intId}")]
        public async Task<ActionResult<ModelQuestao>> ObterListaQuestao(int intId)
        {
            ModelQuestao? objQuestao = new();

            try
            {
                objQuestao = await _context.Questoes.Include(q => q.Opcoes).Include(q => q.Lacunas).FirstOrDefaultAsync(q => q.IdQuestao == intId);

                if (objQuestao == null)
                {
                    return NotFound();
                }

                return objQuestao;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost]
        public async Task<ActionResult<ModelQuestao>> AdicionarQuestao(ModelQuestao objModelQuestao)
        {
            List<OpcaoResposta> objOpcoes = new();
            ModelNivelTrilha? objNivel = new();

            try
            {
                // Verifica se existe um nível e define o objeto, se não houver, deixar como null
                if (objModelQuestao.IdNivel.HasValue)
                {
                    objNivel = await _context.NiveisTrilha.FindAsync(objModelQuestao.IdNivel.Value);

                    if (objNivel == null)
                    {
                        return NotFound("Nível não encontrado.");
                    }

                    objModelQuestao.Nivel = objNivel;
                }

                // Adicionar a questão ao contexto para obter um ID
                objOpcoes = objModelQuestao.Opcoes.ToList(); // Clonar as opções para adicionar depois
                objModelQuestao.Opcoes.Clear(); // Limpar as opções antes de adicionar a questão

                _context.Questoes.Add(objModelQuestao);
                await _context.SaveChangesAsync();

                // Atribuir a questão às opções após ter um ID
                foreach (OpcaoResposta objOpcao in objOpcoes)
                {
                    objOpcao.IdOpcao = 0;
                    objOpcao.IdQuestao = objModelQuestao.IdQuestao;
                    objOpcao.Questao = objModelQuestao;
                    _context.OpcoesResposta.Add(objOpcao);
                }

                await _context.SaveChangesAsync();

                // Adicionar as opções de volta à questão para retorno
                objModelQuestao.Opcoes = objOpcoes;

                return CreatedAtAction(nameof(ObterListaQuestao), new { id = objModelQuestao.IdQuestao }, objModelQuestao);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{intId}")]
        public async Task<IActionResult> PutQuestao(int intId, ModelQuestao objModelQuestao)
        {
            try
            {
                if (intId != objModelQuestao.IdQuestao)
                {
                    return BadRequest();
                }

                ModelQuestao? objQuestaoExistente = await _context.Questoes
                    .Include(q => q.Opcoes)
                    .Include(q => q.Lacunas)
                    .FirstOrDefaultAsync(q => q.IdQuestao == intId);

                if (objQuestaoExistente != null)
                {
                    objQuestaoExistente.Enunciado = objModelQuestao.Enunciado ?? objQuestaoExistente.Enunciado;
                    objQuestaoExistente.Tipo = objModelQuestao.Tipo;
                    objQuestaoExistente.SolucaoEsperada = objModelQuestao.SolucaoEsperada ?? objQuestaoExistente.SolucaoEsperada;

                    // Atualizar as opções
                    objQuestaoExistente.Opcoes.Clear();
                    foreach (OpcaoResposta objOpcao in objModelQuestao.Opcoes)
                    {
                        objQuestaoExistente.Opcoes.Add(objOpcao);
                    }

                    // Atualizar as lacunas
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
                        if (!QuestaoExists(intId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH: api/Questoes/{questaoId}/Nivel/{nivelId}
        [HttpPatch("{intQuestaoId}/Nivel/{nivelId}")]
        public async Task<IActionResult> AdicionarQuestaoEmNivel(int intQuestaoId, int intNivelId)
        {
            ModelQuestao? objModelQuestao = new();
            ModelNivelTrilha? objModelNivel = new();

            try
            {
                objModelQuestao = await _context.Questoes.FindAsync(intQuestaoId);
                if (objModelQuestao == null)
                {
                    return NotFound("Questão não encontrada");
                }

                objModelNivel = await _context.NiveisTrilha.FindAsync(intNivelId);
                if (objModelNivel == null)
                {
                    return NotFound("Nível não encontrado");
                }

                objModelQuestao.IdNivel = intNivelId;
                objModelQuestao.Nivel = objModelNivel;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestaoExists(intQuestaoId))
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        // DELETE: api/Questoes/5
        [HttpDelete("{intId}")]
        public async Task<IActionResult> DeleteQuestao(int intId)
        {
            ModelQuestao? objQuestao = new();

            try
            {
                objQuestao = await _context.Questoes.FindAsync(intId);
                if (objQuestao == null)
                {
                    return NotFound();
                }

                _context.Questoes.Remove(objQuestao);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool QuestaoExists(int intId)
        {
            return _context.Questoes.Any(e => e.IdQuestao == intId);
        }
    }
}
