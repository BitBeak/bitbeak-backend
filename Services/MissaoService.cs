using BitBeakAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BitBeakAPI.Services
{
    public class MissaoService
    {
        private readonly BitBeakContext _context;

        public MissaoService(BitBeakContext context)
        {
            _context = context;
        }

        public async Task AtualizarProgressoMissao(int intIdUsuario, int intIdMissao, TipoMissao tipoMissao, int intIncremento)
        {
            try
            {
                var objUsuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.IdUsuario == intIdUsuario);

                var objMissaoAtiva = await _context.ProgressoMissoes
                    .Include(m => m.Missao)
                    .FirstOrDefaultAsync(m => m.IdUsuario == intIdUsuario && m.IdMissao == intIdMissao && m.Missao.TipoMissao == tipoMissao && !m.Completa);

                if (objMissaoAtiva != null)
                {
                    objMissaoAtiva.ProgressoAtual += intIncremento;
                    _context.ProgressoMissoes.Update(objMissaoAtiva);
                    await _context.SaveChangesAsync(); 

                    if (objMissaoAtiva.ProgressoAtual >= objMissaoAtiva.Missao.Objetivo)
                    {
                        objMissaoAtiva.Completa = true;

                        if (objUsuario != null)
                        {
                            objUsuario.ExperienciaUsuario += objMissaoAtiva.Missao.RecompensaExperiencia;
                            objUsuario.ExperienciaQuinzenalUsuario += objMissaoAtiva.Missao.RecompensaExperiencia;
                            objUsuario.Penas += objMissaoAtiva.Missao.RecompensaPenas;
                            _context.Usuarios.Update(objUsuario);
                        }

                        await _context.SaveChangesAsync(); 
                        await AtivarProximaMissao(intIdUsuario, tipoMissao);  
                    }
                }
                else
                {
                    throw new InvalidOperationException("Nenhuma missão ativa correspondente encontrada.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro na aplicação: {ex.Message}");
                throw;
            }
        }

        private async Task AtivarProximaMissao(int intIdUsuario, TipoMissao tipoMissao)
        {
            var ultimaMissaoConcluida = await _context.ProgressoMissoes
                .Include(pm => pm.Missao)
                .Where(pm => pm.IdUsuario == intIdUsuario && pm.Missao.TipoMissao == tipoMissao && pm.Completa)
                .OrderByDescending(pm => pm.Missao.IdMissao)
                .FirstOrDefaultAsync();

            if (ultimaMissaoConcluida != null)
            {
                Console.WriteLine($"Última missão concluída ID: {ultimaMissaoConcluida.Missao.IdMissao}");
                var proximaMissaoId = ultimaMissaoConcluida.Missao.IdMissao + 1;

                var proximaMissao = await _context.Missoes
                    .FirstOrDefaultAsync(m => m.IdMissao == proximaMissaoId && m.TipoMissao == tipoMissao);

                if (proximaMissao != null)
                {
                    var novaMissaoAtiva = new ModelMissaoProgresso
                    {
                        IdUsuario = intIdUsuario,
                        IdMissao = proximaMissao.IdMissao,
                        ProgressoAtual = 0,
                        Completa = false
                    };
                    _context.ProgressoMissoes.Add(novaMissaoAtiva);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new InvalidOperationException("Não há uma próxima missão disponível para ativação.");
                }
            }
            else
            {
                Console.WriteLine("Nenhuma missão foi concluída deste tipo para ativar a próxima.");
                throw new InvalidOperationException("Nenhuma missão foi concluída deste tipo para ativar a próxima.");
            }
        }

        public async Task<int> BuscarMissaoAtiva(int intIdUsuario, TipoMissao objTipoMissao)
        {
            var objMissaoAtiva = await _context.ProgressoMissoes
                .Include(m => m.Missao)
                .FirstOrDefaultAsync(m => m.IdUsuario == intIdUsuario && m.Missao.TipoMissao == objTipoMissao && !m.Completa);

            if (objMissaoAtiva != null)
            {
                return objMissaoAtiva.IdMissao;
            }
            else
            {
                throw new InvalidOperationException("Não há missão ativa deste tipo para o usuário.");
            }
        }

    }
}
