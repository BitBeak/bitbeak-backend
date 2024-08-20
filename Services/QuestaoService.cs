using BitBeakAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BitBeakAPI.Services
{
    public class QuestaoService
    {
        private readonly BitBeakContext _context;

        public QuestaoService(BitBeakContext context)
        {
            _context = context;
        }

        public async Task<ModelQuestao> ListarDadosQuestao(int idQuestao)
        {
            return await _context.Questoes
                .Include(q => q.Opcoes)  
                .Include(q => q.Lacunas) 
                .FirstOrDefaultAsync(q => q.IdQuestao == idQuestao);
        }
    }

}
