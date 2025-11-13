using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SistemaPDV.Data.Context;
using SistemaPDV.Data.Repositories;
using SistemaPDV.Models.Entities;

namespace SistemaPDV.UI.Views
{
    public partial class UsuariosWindow : Window
    {
        private readonly BaseRepository<Usuario> _repository;
        private List<Usuario> _usuariosCache;
        private Usuario _usuarioSelecionado;

        public UsuariosWindow()
        {
            InitializeComponent();
            _repository = new BaseRepository<Usuario>(new AppDbContext());
            CarregarUsuarios();
        }

        private async void CarregarUsuarios()
        {
            try
            {
                _usuariosCache = await _repository.GetAllAsync();
                dgUsuarios.ItemsSource = _usuariosCache;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar usuarios: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtPesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filtro = txtPesquisa.Text.ToLower();

            if (string.IsNullOrWhiteSpace(filtro))
            {
                dgUsuarios.ItemsSource = _usuariosCache;
            }
            else
            {
                var usuariosFiltrados = _usuariosCache.Where(u =>
                    u.Nome.ToLower().Contains(filtro) ||
                    u.Login.ToLower().Contains(filtro) ||
                    u.Tipo.ToLower().Contains(filtro)
                ).ToList();

                dgUsuarios.ItemsSource = usuariosFiltrados;
            }
        }

        private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                var tipoSelecionado = (cmbTipo.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (_usuarioSelecionado != null)
                {
                    // Atualizacao
                    _usuarioSelecionado.Nome = txtNome.Text.Trim();
                    _usuarioSelecionado.Login = txtLogin.Text.Trim();
                    _usuarioSelecionado.Tipo = tipoSelecionado;
                    _usuarioSelecionado.Ativo = chkAtivo.IsChecked ?? true;

                    // So atualiza senha se foi digitada
                    if (!string.IsNullOrWhiteSpace(txtSenha.Password))
                    {
                        _usuarioSelecionado.SenhaHash = HashPassword(txtSenha.Password);
                    }

                    await _repository.UpdateAsync(_usuarioSelecionado);
                    MessageBox.Show("Usuario atualizado com sucesso!",
                        "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Inclusao
                    var usuario = new Usuario
                    {
                        Nome = txtNome.Text.Trim(),
                        Login = txtLogin.Text.Trim(),
                        SenhaHash = HashPassword(txtSenha.Password),
                        Tipo = tipoSelecionado,
                        Ativo = chkAtivo.IsChecked ?? true,
                        DataCriacao = DateTime.Now
                    };

                    await _repository.AddAsync(usuario);
                    MessageBox.Show("Usuario cadastrado com sucesso!",
                        "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LimparCampos();
                CarregarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar usuario: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var usuario = button?.DataContext as Usuario;

            if (usuario != null)
            {
                CarregarUsuarioParaEdicao(usuario);
            }
        }

        private void BtnAlterarSenha_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var usuario = button?.DataContext as Usuario;

            if (usuario != null)
            {
                var novaSenha = Microsoft.VisualBasic.Interaction.InputBox(
                    "Digite a nova senha:",
                    "Alterar Senha",
                    "",
                    -1, -1);

                if (!string.IsNullOrWhiteSpace(novaSenha))
                {
                    var confirmacao = Microsoft.VisualBasic.Interaction.InputBox(
                        "Confirme a nova senha:",
                        "Confirmar Senha",
                        "",
                        -1, -1);

                    if (novaSenha == confirmacao)
                    {
                        try
                        {
                            usuario.SenhaHash = HashPassword(novaSenha);
                            _repository.UpdateAsync(usuario).Wait();
                            MessageBox.Show("Senha alterada com sucesso!",
                                "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erro ao alterar senha: {ex.Message}",
                                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("As senhas nao coincidem!",
                            "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private async void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var usuario = button?.DataContext as Usuario;

            if (usuario == null) return;

            if (usuario.Login == "admin")
            {
                MessageBox.Show("O usuario administrador padrao nao pode ser excluido!",
                    "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Deseja realmente excluir o usuario '{usuario.Nome}'?",
                "Confirmar Exclusao",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _repository.DeleteAsync(usuario.Id);
                    MessageBox.Show("Usuario excluido com sucesso!",
                        "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    CarregarUsuarios();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir usuario: {ex.Message}",
                        "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DgUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var usuario = dgUsuarios.SelectedItem as Usuario;
            if (usuario != null)
            {
                CarregarUsuarioParaEdicao(usuario);
            }
        }

        private void CarregarUsuarioParaEdicao(Usuario usuario)
        {
            _usuarioSelecionado = usuario;
            txtId.Text = usuario.Id.ToString();
            txtNome.Text = usuario.Nome;
            txtLogin.Text = usuario.Login;
            txtSenha.Clear();
            txtConfirmarSenha.Clear();
            cmbTipo.Text = usuario.Tipo;
            chkAtivo.IsChecked = usuario.Ativo;
            btnSalvar.Content = "Atualizar";
            txtDicaSenha.Visibility = Visibility.Visible;
        }

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void LimparCampos()
        {
            _usuarioSelecionado = null;
            txtId.Clear();
            txtNome.Clear();
            txtLogin.Clear();
            txtSenha.Clear();
            txtConfirmarSenha.Clear();
            cmbTipo.SelectedIndex = 0;
            chkAtivo.IsChecked = true;
            btnSalvar.Content = "Salvar";
            txtDicaSenha.Visibility = Visibility.Collapsed;
            txtNome.Focus();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Informe o nome do usuario!",
                    "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNome.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Informe o login!",
                    "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLogin.Focus();
                return false;
            }

            // Validar senha apenas se for novo usuario ou se digitou algo
            if (_usuarioSelecionado == null || !string.IsNullOrWhiteSpace(txtSenha.Password))
            {
                if (string.IsNullOrWhiteSpace(txtSenha.Password))
                {
                    MessageBox.Show("Informe a senha!",
                        "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtSenha.Focus();
                    return false;
                }

                if (txtSenha.Password != txtConfirmarSenha.Password)
                {
                    MessageBox.Show("As senhas nao coincidem!",
                        "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtConfirmarSenha.Focus();
                    return false;
                }

                if (txtSenha.Password.Length < 6)
                {
                    MessageBox.Show("A senha deve ter no minimo 6 caracteres!",
                        "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtSenha.Focus();
                    return false;
                }
            }

            return true;
        }

        private string HashPassword(string password)
        {
            // Use BCrypt em producao
            return Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(password)
            );
        }

        private void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            CarregarUsuarios();
            LimparCampos();
        }

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}