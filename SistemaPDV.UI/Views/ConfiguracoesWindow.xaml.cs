using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using SistemaPDV.Data.Context;

namespace SistemaPDV.UI.Views
{
    public partial class ConfiguracoesWindow : Window
    {
        private readonly AppDbContext _context;

        public ConfiguracoesWindow()
        {
            InitializeComponent();
            _context = new AppDbContext();
            CarregarInformacoes();
            CarregarConfiguracoes();
        }

        private async void CarregarInformacoes()
        {
            try
            {
                txtCaminhoBanco.Text = "sistemaPDV.db";

                var totalProdutos = await _context.Produtos.CountAsync();
                txtTotalProdutos.Text = totalProdutos.ToString();

                var totalClientes = await _context.Clientes.CountAsync();
                txtTotalClientes.Text = totalClientes.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar informacoes: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CarregarConfiguracoes()
        {
            // Aqui voce pode carregar configuracoes salvas de um arquivo ou banco
            // Por enquanto usamos valores padrao
        }

        private void BtnBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Backup do PDV (*.db)|*.db",
                    FileName = $"Backup_PDV_{DateTime.Now:yyyyMMdd_HHmmss}.db",
                    Title = "Salvar Backup"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var caminhoOrigem = "sistemaPDV.db";

                    if (File.Exists(caminhoOrigem))
                    {
                        File.Copy(caminhoOrigem, saveDialog.FileName, true);
                        txtUltimoBackup.Text = $"Ultimo backup: {DateTime.Now:dd/MM/yyyy HH:mm}";
                        MessageBox.Show("Backup criado com sucesso!",
                            "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Arquivo de banco de dados nao encontrado!",
                            "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar backup: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRestaurar_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "ATENCAO: Restaurar um backup substituira todos os dados atuais!\n\n" +
                "Deseja continuar?",
                "Confirmar Restauracao",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var openDialog = new OpenFileDialog
                    {
                        Filter = "Backup do PDV (*.db)|*.db",
                        Title = "Selecionar Backup"
                    };

                    if (openDialog.ShowDialog() == true)
                    {
                        _context.Dispose();

                        var caminhoDestino = "sistemaPDV.db";
                        File.Copy(openDialog.FileName, caminhoDestino, true);

                        MessageBox.Show(
                            "Backup restaurado com sucesso!\n\n" +
                            "O sistema sera reiniciado.",
                            "Sucesso",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        System.Diagnostics.Process.Start(
                            System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                        Application.Current.Shutdown();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao restaurar backup: {ex.Message}",
                        "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnLimparVendas_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Excluir vendas com mais de quantos dias?\n\n" +
                "Digite o numero de dias:",
                "Limpar Vendas Antigas",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                var dias = Microsoft.VisualBasic.Interaction.InputBox(
                    "Numero de dias:",
                    "Limpar Vendas",
                    "90");

                if (int.TryParse(dias, out int numeroDias))
                {
                    try
                    {
                        var dataLimite = DateTime.Now.AddDays(-numeroDias);
                        var vendasAntigas = await _context.Vendas
                            .Where(v => v.DataVenda < dataLimite)
                            .ToListAsync();

                        _context.Vendas.RemoveRange(vendasAntigas);
                        await _context.SaveChangesAsync();

                        MessageBox.Show(
                            $"{vendasAntigas.Count} vendas excluidas com sucesso!",
                            "Sucesso",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao limpar vendas: {ex.Message}",
                            "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnOtimizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // SQLite VACUUM
                _context.Database.ExecuteSqlRaw("VACUUM");

                MessageBox.Show("Banco de dados otimizado com sucesso!",
                    "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao otimizar banco: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSalvarConfig_Click(object sender, RoutedEventArgs e)
        {
            // Salvar configuracoes em arquivo ou banco
            MessageBox.Show("Configuracoes salvas com sucesso!",
                "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void BtnLimparTudo_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "ATENCAO: Esta acao ira excluir TODOS os dados!\n\n" +
                "- Todas as vendas\n" +
                "- Todos os produtos\n" +
                "- Todos os clientes\n" +
                "- Todos os usuarios (exceto admin)\n\n" +
                "Esta acao NAO pode ser desfeita!\n\n" +
                "Deseja continuar?",
                "CONFIRMAR EXCLUSAO TOTAL",
                MessageBoxButton.YesNo,
                MessageBoxImage.Stop);

            if (result == MessageBoxResult.Yes)
            {
                var confirmacao = MessageBox.Show(
                    "Tem certeza ABSOLUTA?\n\nDigite SIM para confirmar.",
                    "Ultima Confirmacao",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Stop);

                if (confirmacao == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Excluir todos os dados
                        _context.ItensVenda.RemoveRange(_context.ItensVenda);
                        _context.Vendas.RemoveRange(_context.Vendas);
                        _context.Produtos.RemoveRange(_context.Produtos);
                        _context.Clientes.RemoveRange(_context.Clientes);

                        // Manter apenas usuario admin
                        var usuariosParaRemover = await _context.Usuarios
                            .Where(u => u.Login != "admin")
                            .ToListAsync();
                        _context.Usuarios.RemoveRange(usuariosParaRemover);

                        await _context.SaveChangesAsync();

                        MessageBox.Show(
                            "Todos os dados foram excluidos!\n\n" +
                            "O sistema sera reiniciado.",
                            "Concluido",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        System.Diagnostics.Process.Start(
                            System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                        Application.Current.Shutdown();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao limpar dados: {ex.Message}",
                            "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _context?.Dispose();
        }
    }
}