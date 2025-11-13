using SistemaPDV.Business.Services;
using SistemaPDV.Data.Context;
using SistemaPDV.UI.Views;
using System;
using System.CodeDom.Compiler;
using System.Windows;

namespace SistemaPDV.UI
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                using (var context = new AppDbContext())
                {
                    // Criar banco se não existir
                    context.Database.EnsureCreated();

                    // Criar usuário padrão
                    var authService = new AutenticacaoService(context);
                    var criado = await authService.CriarUsuarioPadraoAsync();

                    if (criado)
                    {
                        System.Diagnostics.Debug.WriteLine("Usuario admin criado!");
                    }
                }

                var loginWindow = new LoginWindow();
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao iniciar: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
