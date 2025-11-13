using Microsoft.EntityFrameworkCore;
using SistemaPDV.Data.Context;
using SistemaPDV.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaPDV.Business.Services
{
    public class AutenticacaoService
    {
        private readonly AppDbContext _context;

        public AutenticacaoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> AutenticarAsync(string login, string senha)
        {
            try
            {
                var senhaHash = HashPassword(senha);

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u =>
                        u.Login == login &&
                        u.SenhaHash == senhaHash &&
                        u.Ativo);

                return usuario;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na autenticacao: {ex.Message}");
                return null;
            }
        }

       
        public async Task<bool> CriarUsuarioPadraoAsync()
        {
            try
            {
                // Verifica se já existe admin
                var adminExiste = await _context.Usuarios
                    .AnyAsync(u => u.Login == "admin");

                if (!adminExiste)
                {
                    var usuario = new Usuario
                    {
                        Nome = "Administrador",
                        Login = "admin",
                        SenhaHash = HashPassword("admin123"),
                        Tipo = "Administrador",
                        Ativo = true,
                        DataCriacao = DateTime.Now
                    };

                    _context.Usuarios.Add(usuario);
                    await _context.SaveChangesAsync();
                    return true; // Usuário criado
                }

                return false; // Já existia
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao criar usuario: {ex.Message}");
                return false;
            }
        }

        private string HashPassword(string password)
        {
            // Hash simples para desenvolvimento
            // Em produção use BCrypt.Net-Next
            return Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(password)
            );
        }
    }
}