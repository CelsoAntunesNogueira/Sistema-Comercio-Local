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
    public partial class ClientesWindow : Window
    {
        private readonly BaseRepository<Cliente> _repository;
        private List<Cliente> _clientesCache;
        private Cliente _clienteSelecionado;

        public ClientesWindow()
        {
            InitializeComponent();
            _repository = new BaseRepository<Cliente>(new AppDbContext());
            CarregarClientes();
        }

        private async void CarregarClientes()
        {
            try
            {
                _clientesCache = await _repository.GetAllAsync();
                dgClientes.ItemsSource = _clientesCache;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar clientes: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtPesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filtro = txtPesquisa.Text.ToLower();

            if (string.IsNullOrWhiteSpace(filtro))
            {
                dgClientes.ItemsSource = _clientesCache;
            }
            else
            {
                var clientesFiltrados = _clientesCache.Where(c =>
                    c.Nome.ToLower().Contains(filtro) ||
                    (c.CPF != null && c.CPF.Contains(filtro)) ||
                    (c.Telefone != null && c.Telefone.Contains(filtro)) ||
                    c.Id.ToString().Contains(filtro)
                ).ToList();

                dgClientes.ItemsSource = clientesFiltrados;
            }
        }

        private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                var cliente = new Cliente
                {
                    Nome = txtNome.Text.Trim(),
                    CPF = string.IsNullOrWhiteSpace(txtCPF.Text) ? null : txtCPF.Text.Trim(),
                    Telefone = string.IsNullOrWhiteSpace(txtTelefone.Text) ? null : txtTelefone.Text.Trim(),
                    Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                    Ativo = chkAtivo.IsChecked ?? true
                };

                if (_clienteSelecionado != null)
                {
                    cliente.Id = _clienteSelecionado.Id;
                    cliente.DataCriacao = _clienteSelecionado.DataCriacao;
                    await _repository.UpdateAsync(cliente);
                    MessageBox.Show("Cliente atualizado com sucesso!",
                        "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    await _repository.AddAsync(cliente);
                    MessageBox.Show("Cliente cadastrado com sucesso!",
                        "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LimparCampos();
                CarregarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar cliente: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var cliente = button?.DataContext as Cliente;

            if (cliente != null)
            {
                CarregarClienteParaEdicao(cliente);
            }
        }

        private async void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var cliente = button?.DataContext as Cliente;

            if (cliente == null) return;

            var result = MessageBox.Show(
                $"Deseja realmente excluir o cliente '{cliente.Nome}'?",
                "Confirmar Exclusao",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _repository.DeleteAsync(cliente.Id);
                    MessageBox.Show("Cliente excluido com sucesso!",
                        "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    CarregarClientes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir cliente: {ex.Message}",
                        "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cliente = dgClientes.SelectedItem as Cliente;
            if (cliente != null)
            {
                CarregarClienteParaEdicao(cliente);
            }
        }

        private void CarregarClienteParaEdicao(Cliente cliente)
        {
            _clienteSelecionado = cliente;
            txtId.Text = cliente.Id.ToString();
            txtNome.Text = cliente.Nome;
            txtCPF.Text = cliente.CPF;
            txtTelefone.Text = cliente.Telefone;
            txtEmail.Text = cliente.Email;
            chkAtivo.IsChecked = cliente.Ativo;
            btnSalvar.Content = "Atualizar";
        }

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void LimparCampos()
        {
            _clienteSelecionado = null;
            txtId.Clear();
            txtNome.Clear();
            txtCPF.Clear();
            txtTelefone.Clear();
            txtEmail.Clear();
            chkAtivo.IsChecked = true;
            btnSalvar.Content = "Salvar";
            txtNome.Focus();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Informe o nome do cliente!",
                    "Atencao", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNome.Focus();
                return false;
            }

            return true;
        }

        private void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            CarregarClientes();
            LimparCampos();
        }

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}