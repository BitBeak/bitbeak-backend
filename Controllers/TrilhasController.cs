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
    public class TrilhasController : ControllerBase
    {
        private readonly BitBeakContext _context;

        public TrilhasController(BitBeakContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Função para listar todas a trilhas e seu dados
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModelTrilha>>> GetTrilhas()
        {
            return await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .ThenInclude(q => q.Opcoes)
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .ThenInclude(q => q.Lacunas)
                .ToListAsync();
        }

        /// <summary>
        /// Função para listar os dados de uma trilha específica
        /// </summary>
        /// <param name="intId">Obrigatório enviar o id da Trilha</param>
        /// <returns></returns>
        [HttpGet("{intId}")]
        public async Task<ActionResult<ModelTrilha>> GetTrilha(int intId)
        {
            ModelTrilha? objModelTrilha = new();

            objModelTrilha = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .ThenInclude(q => q.Opcoes)
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .ThenInclude(q => q.Lacunas)
                .FirstOrDefaultAsync(t => t.IdTrilha == intId);

            if (objModelTrilha == null)
            {
                return NotFound("Trilha não encontrada!");
            }

            return objModelTrilha;
        }

        /// <summary>
        /// Função para criar uma Trilha
        /// </summary>
        /// <param name="objModelTrilha">Necessário passar dados da Trilha - Nome e Descrição são obrigatórios.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ModelTrilha>> PostTrilha(ModelTrilha objModelTrilha)
        {
            _context.Trilhas.Add(objModelTrilha);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrilha), new { id = objModelTrilha.IdTrilha }, objModelTrilha);
        }

        /// <summary>
        /// Função para editar uma Trilha
        /// </summary>
        /// <param name="intId">Obrigatório passar o id da Trilha</param>
        /// <param name="objModelTrilha">Passar os campos que serão editados e ou adicionados</param>
        /// <returns></returns>
        [HttpPut("{intId}")]
        public async Task<IActionResult> PutTrilha(int intId, ModelTrilha objModelTrilha)
        {
            ModelTrilha? objTrilha = new();

            // Se o idTrilha (cabeçalho) for diferente do IdTrilha (json) = Erro de trilhas diferentes
            if (intId != objModelTrilha.IdTrilha)
            {
                return BadRequest("Erro - Id da trilha não coincidem!");
            }

            objTrilha = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .FirstOrDefaultAsync(t => t.IdTrilha == intId);

            // Se os dados estiverem vazios = Erro de dados da Trilha não preenchidos
            if (objTrilha == null)
            {
                return NotFound("Erro - Dados da Troilha não preenchidos");
            }

            // Atualizar nome e descrição apenas se os campos não estiverem vazios ou nulos
            if (!string.IsNullOrWhiteSpace(objModelTrilha.Nome))
            {
                objTrilha.Nome = objModelTrilha.Nome;
            }

            if (!string.IsNullOrWhiteSpace(objModelTrilha.Descricao))
            {
                objTrilha.Descricao = objModelTrilha.Descricao;
            }

            // Atualizar ou adicionar os níveis da trilha
            foreach (ModelNivelTrilha objNivel in objModelTrilha.Niveis)
            {
                ModelNivelTrilha? objNivelExistente = objTrilha.Niveis.FirstOrDefault(n => n.IdNivel == objNivel.IdNivel);

                if (objNivelExistente != null)
                {
                    objNivelExistente.Nivel = objNivel.Nivel;

                    // Atualizar as questões do nível
                    foreach (ModelQuestao objQuestao in objNivel.Questoes)
                    {
                        ModelQuestao? objQuestaoExistente = new();

                        objQuestaoExistente = objNivelExistente.Questoes.FirstOrDefault(q => q.IdQuestao == objQuestao.IdQuestao);

                        if (objQuestaoExistente != null)
                        {
                            objQuestaoExistente.Enunciado = objQuestao.Enunciado;
                            objQuestaoExistente.Tipo = objQuestao.Tipo;
                            objQuestaoExistente.SolucaoEsperada = objQuestao.SolucaoEsperada;

                            objQuestaoExistente.Opcoes = objQuestao.Opcoes;
                            objQuestaoExistente.Lacunas = objQuestao.Lacunas;
                        }
                        else
                        {
                            objNivelExistente.Questoes.Add(objQuestao);
                        }
                    }
                }
                else
                {
                    objTrilha.Niveis.Add(objNivel);
                }
            }

            // Não remover níveis que não foram atualizados

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

        [HttpPost("{trilhaId}/Niveis/{nivelId}/AdicionarQuestao")]
        public async Task<IActionResult> AdicionarQuestaoExistente(int trilhaId, int nivelId, [FromBody] int questaoId)
        {
            var trilha = await _context.Trilhas
                .Include(t => t.Niveis)
                .ThenInclude(n => n.Questoes)
                .FirstOrDefaultAsync(t => t.IdTrilha == trilhaId);

            if (trilha == null)
            {
                return NotFound("Trilha não encontrada");
            }

            var nivel = trilha.Niveis.FirstOrDefault(n => n.IdNivel == nivelId);
            if (nivel == null)
            {
                return NotFound("Nível não encontrado");
            }

            var questao = await _context.Questoes.FindAsync(questaoId);
            if (questao == null)
            {
                return NotFound("Questão não encontrada");
            }

            nivel.Questoes.Add(questao);
            await _context.SaveChangesAsync();

            return Ok("Questão adicionada ao nível com sucesso");
        }

        // DELETE: api/Trilhas/5
        [HttpDelete("{intId}")]
        public async Task<IActionResult> DeleteTrilha(int intId)
        {
            ModelTrilha? objModelTrilha = await _context.Trilhas.FindAsync(intId);

            if (objModelTrilha == null)
            {
                return NotFound("Erro - Trilha não encontrada!");
            }

            _context.Trilhas.Remove(objModelTrilha);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TrilhaExists(int intId)
        {
            return _context.Trilhas.Any(e => e.IdTrilha == intId);
        }
    }
}
