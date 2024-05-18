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
        [HttpGet("{id}")]
        public async Task<ActionResult<ModelQuestao>> ObterListaQuestao(int id)
        {
            var questao = await _context.Questoes.Include(q => q.Opcoes).Include(q => q.Lacunas).FirstOrDefaultAsync(q => q.IdQuestao == id);

            if (questao == null)
            {
                return NotFound();
            }

            return questao;
        }

        [HttpPost]
        public async Task<ActionResult<ModelQuestao>> AdicionarQuestao(ModelQuestao objModelQuestao)
        {
            // Verifica se existe um nível e define o objeto, se não houver, deixar como null
            if (objModelQuestao.IdNivel.HasValue)
            {
                var objNivel = await _context.NiveisTrilha.FindAsync(objModelQuestao.IdNivel.Value);

                if (objNivel == null)
                {
                    return NotFound("Nível não encontrado.");
                }

                objModelQuestao.Nivel = objNivel;
            }

            // Adicionar a questão ao contexto para obter um ID
            var opcoes = objModelQuestao.Opcoes.ToList(); // Clonar as opções para adicionar depois
            objModelQuestao.Opcoes.Clear(); // Limpar as opções antes de adicionar a questão

            _context.Questoes.Add(objModelQuestao);
            await _context.SaveChangesAsync();

            // Atribuir a questão às opções após ter um ID
            foreach (var opcao in opcoes)
            {
                opcao.IdOpcao = 0;
                opcao.IdQuestao = objModelQuestao.IdQuestao;
                opcao.Questao = objModelQuestao;
                _context.OpcoesResposta.Add(opcao);
            }

            await _context.SaveChangesAsync();

            // Adicionar as opções de volta à questão para retorno
            objModelQuestao.Opcoes = opcoes;

            return CreatedAtAction(nameof(ObterListaQuestao), new { id = objModelQuestao.IdQuestao }, objModelQuestao);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestao(int id, ModelQuestao objModelQuestao)
        {
            if (id != objModelQuestao.IdQuestao)
            {
                return BadRequest();
            }

            var questaoExistente = await _context.Questoes
                .Include(q => q.Opcoes)
                .Include(q => q.Lacunas)
                .FirstOrDefaultAsync(q => q.IdQuestao == id);

            if (questaoExistente == null)
            {
                return NotFound();
            }

            questaoExistente.Enunciado = objModelQuestao.Enunciado ?? questaoExistente.Enunciado;
            questaoExistente.Tipo = objModelQuestao.Tipo;
            questaoExistente.SolucaoEsperada = objModelQuestao.SolucaoEsperada ?? questaoExistente.SolucaoEsperada;

            // Atualizar as opções
            questaoExistente.Opcoes.Clear();
            foreach (var opcao in objModelQuestao.Opcoes)
            {
                questaoExistente.Opcoes.Add(opcao);
            }

            // Atualizar as lacunas
            questaoExistente.Lacunas.Clear();
            foreach (var lacuna in objModelQuestao.Lacunas)
            {
                lacuna.Questao = questaoExistente;
                questaoExistente.Lacunas.Add(lacuna);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestaoExists(id))
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

        // PUT: api/Questoes/{questaoId}/Nivel/{nivelId}
        [HttpPut("{questaoId}/Nivel/{nivelId}")]
        public async Task<IActionResult> AssociarQuestaoANivel(int questaoId, int nivelId)
        {
            var questao = await _context.Questoes.FindAsync(questaoId);
            if (questao == null)
            {
                return NotFound("Questão não encontrada");
            }

            var nivel = await _context.NiveisTrilha.FindAsync(nivelId);
            if (nivel == null)
            {
                return NotFound("Nível não encontrado");
            }

            questao.IdNivel = nivelId;
            questao.Nivel = nivel;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestaoExists(questaoId))
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestao(int id)
        {
            var questao = await _context.Questoes.FindAsync(id);
            if (questao == null)
            {
                return NotFound();
            }

            _context.Questoes.Remove(questao);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool QuestaoExists(int id)
        {
            return _context.Questoes.Any(e => e.IdQuestao == id);
        }
    }
}
