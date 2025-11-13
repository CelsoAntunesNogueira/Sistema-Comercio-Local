using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SistemaPDV.Data.Context;
using SistemaPDV.Data.Repositories;
using SistemaPDV.Models.Entities;

namespace SistemaPDV.UI.Views
{
    public partial class ProdutosWindow : Window
    {
        private readonly ProdutoRepository _repository;
        private List<Produto> _produtosCache;
        private Produto _produtoSelecionado;

        public ProdutosWindow()
        {
            InitializeComponent();
            _repository = new ProdutoRepository(new AppDbContext());
            CarregarProdutos();
        }

        private async void CarregarProdutos()
        {
            try
            {
                _produtosCache = await _repository.GetAllAsync();
                dgProdutos.ItemsSource = _produtosCache;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar produtos: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtPesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filtro = txtPesquisa.Text.ToLower();

            if (string.IsNullOrWhiteSpace(filtro))
            {
                dgProdutos.ItemsSource = _produtosCache;
            }
            else
            {
                var produtosFiltrados = _produtosCache.Where(p =>
                    p.Nome.ToLower().Contains(filtro) ||
                    (p.CodigoBarras != null && p.CodigoBarras.Contains(filtro)) ||
                    p.Id.ToString().Contains(filtro)
                ).ToList();

                dgProdutos.ItemsSource = produtosFiltrados;
            }
        }

        private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                var produto = new Produto
                {
                    Nome = txtNome.Text.Trim(),
                    CodigoBarras = string.IsNullOrWhiteSpace(txtCodigoBarras.Text)
                        ? null : txtCodigoBarras.Text.Trim(),
                    Preco = decimal.Parse(txtPreco.Text),
                    EstoqueAtual = int.Parse(txtEstoqueAtual.Text),
                    EstoqueMinimo = int.Parse(txtEstoqueMinimo.Text),
                    Ativo = chkAtivo.IsChecked ?? true
                };

                if (_produtoSelecionado != null)
                {
                    // Atualização
                    produto.Id = _produtoSelecionado.Id;
                    produto.DataCriacao = _produtoSelecionado.DataCriacao;
                    await _repository.UpdateAsync(produto);
                    MessageBox.Show("Produto atualizado com sucesso!",
                        "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Inclusão
                    await _repository.AddAsync(produto);
                    MessageBox.Show("Produto cadastrado com sucesso!",
                        "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LimparCampos();
                CarregarProdutos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar produto: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var produto = button?.DataContext as Produto;

            if (produto != null)
            {
                CarregarProdutoParaEdicao(produto);
            }
        }

        private async void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var produto = button?.DataContext as Produto;

            if (produto == null) return;

            var result = MessageBox.Show(
                $"Deseja realmente excluir o produto '{produto.Nome}'?",
                "Confirmar Exclusão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _repository.DeleteAsync(produto.Id);
                    MessageBox.Show("Produto excluído com sucesso!",
                        "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    CarregarProdutos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir produto: {ex.Message}",
                        "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DgProdutos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var produto = dgProdutos.SelectedItem as Produto;
            if (produto != null)
            {
                CarregarProdutoParaEdicao(produto);
            }
        }

        private void CarregarProdutoParaEdicao(Produto produto)
        {
            _produtoSelecionado = produto;
            txtId.Text = produto.Id.ToString();
            txtNome.Text = produto.Nome;
            txtCodigoBarras.Text = produto.CodigoBarras;
            txtPreco.Text = produto.Preco.ToString("F2");
            txtEstoqueAtual.Text = produto.EstoqueAtual.ToString();
            txtEstoqueMinimo.Text = produto.EstoqueMinimo.ToString();
            chkAtivo.IsChecked = produto.Ativo;
            btnSalvar.Content = "💾 Atualizar";
        }

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void LimparCampos()
        {
            _produtoSelecionado = null;
            txtId.Clear();
            txtNome.Clear();
            txtCodigoBarras.Clear();
            txtPreco.Clear();
            txtEstoqueAtual.Clear();
            txtEstoqueMinimo.Text = "0";
            chkAtivo.IsChecked = true;
            btnSalvar.Content = "💾 Salvar";
            txtNome.Focus();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Informe o nome do produto!",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNome.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPreco.Text) ||
                !decimal.TryParse(txtPreco.Text, out decimal preco) || preco <= 0)
            {
                MessageBox.Show("Informe um preço válido!",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPreco.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEstoqueAtual.Text) ||
                !int.TryParse(txtEstoqueAtual.Text, out int estoque) || estoque < 0)
            {
                MessageBox.Show("Informe um estoque válido!",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEstoqueAtual.Focus();
                return false;
            }

            return true;
        }

        private void TxtNumerico_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permite apenas números e vírgula/ponto
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            CarregarProdutos();
            LimparCampos();
        }

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}