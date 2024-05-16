using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BitBeakAPI.Models;
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
        public async Task<ActionResult<IEnumerable<ModelQuestao>>> GetQuestoes()
        {
            return await _context.Questoes.Include(q => q.Opcoes).Include(q => q.Lacunas).ToListAsync();
        }

        // GET: api/Questoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModelQuestao>> GetQuestao(int id)
        {
            var questao = await _context.Questoes.Include(q => q.Opcoes).Include(q => q.Lacunas).FirstOrDefaultAsync(q => q.IdQuestao == id);

            if (questao == null)
            {
                return NotFound();
            }

            return questao;
        }

        // POST: api/Questoes
        [HttpPost]
        public async Task<ActionResult<ModelQuestao>> PostQuestao(ModelQuestao questao)
        {
            if (questao.Tipo == TipoQuestao.Pergunta && questao.Opcoes != null)
            {
                foreach (var opcao in questao.Opcoes)
                {
                    opcao.Questao = questao;
                }
            }

            if (questao.Tipo == TipoQuestao.Lacuna && questao.Lacunas != null)
            {
                foreach (var lacuna in questao.Lacunas)
                {
                    lacuna.Questao = questao;
                }
            }

            _context.Questoes.Add(questao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuestao), new { id = questao.IdQuestao }, questao);
        }

        // PUT: api/Questoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestao(int id, ModelQuestao questao)
        {
            if (id != questao.IdQuestao)
            {
                return BadRequest();
            }

            _context.Entry(questao).State = EntityState.Modified;

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
