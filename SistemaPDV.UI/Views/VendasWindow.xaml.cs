using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SistemaPDV.Business.Services;
using SistemaPDV.Data.Context;
using SistemaPDV.Data.Repositories;
using SistemaPDV.Models.Entities;

namespace SistemaPDV.UI.Views
{
    public partial class VendasWindow : Window
    {
        private readonly Usuario _usuarioLogado;
        private readonly ProdutoRepository _produtoRepository;
        private readonly VendaService _vendaService;
        private ObservableCollection<ItemVenda> _itensVenda;
        private Cliente _clienteSelecionado;

        public VendasWindow(Usuario usuario)
        {
            InitializeComponent();
            _usuarioLogado = usuario;
            var context = new AppDbContext();
            _produtoRepository = new ProdutoRepository(context);
            _vendaService = new VendaService(context);
            _itensVenda = new ObservableCollection<ItemVenda>();

            dgItensVenda.ItemsSource = _itensVenda;
            txtOperador.Text = $"Operador: {usuario.Nome}";

            // Atalhos de teclado
            this.PreviewKeyDown += Window_PreviewKeyDown;

            txtBuscaProduto.Focus();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F2:
                    BtnBuscarProdutos_Click(null, null);
                    e.Handled = true;
                    break;
                case Key.F4:
                    BtnSelecionarCliente_Click(null, null);
                    e.Handled = true;
                    break;
                case Key.F5:
                    BtnFinalizarVenda_Click(null, null);
                    e.Handled = true;
                    break;
                case Key.F8:
                    BtnRemoverItemSelecionado_Click(null, null);
                    e.Handled = true;
                    break;
                case Key.Escape:
                    BtnCancelarVenda_Click(null, null);
                    e.Handled = true;
                    break;
            }
        }

        private async void TxtBuscaProduto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await AdicionarProduto();
            }
        }

        private async void BtnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            await AdicionarProduto();
        }

        private async Task AdicionarProduto()
        {
            var busca = txtBuscaProduto.Text.Trim();

            if (string.IsNullOrWhiteSpace(busca))
            {
                MessageBox.Show("Informe o código ou nome do produto!",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtQuantidade.Text, out int quantidade) || quantidade <= 0)
            {
                MessageBox.Show("Quantidade inválida!",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtQuantidade.Focus();
                return;
            }

            try
            {
                // Buscar produto por código de barras ou ID
                Produto produto = null;

                if (int.TryParse(busca, out int id))
                {
                    produto = await _produtoRepository.GetByIdAsync(id);
                }

                if (produto == null)
                {
                    produto = await _produtoRepository.GetByCodigoBarrasAsync(busca);
                }

                if (produto == null)
                {
                    MessageBox.Show("Produto não encontrado!",
                        "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtBuscaProduto.SelectAll();
                    txtBuscaProduto.Focus();
                    return;
                }

                if (!produto.Ativo)
                {
                    MessageBox.Show("Produto inativo!",
                        "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (produto.EstoqueAtual < quantidade)
                {
                    MessageBox.Show($"Estoque insuficiente! Disponível: {produto.EstoqueAtual}",
                        "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Verificar se produto já está na lista
                var itemExistente = _itensVenda.FirstOrDefault(i => i.ProdutoId == produto.Id);

                if (itemExistente != null)
                {
                    itemExistente.Quantidade += quantidade;
                }
                else
                {
                    _itensVenda.Add(new ItemVenda
                    {
                        ProdutoId = produto.Id,
                        Produto = produto,
                        Quantidade = quantidade,
                        PrecoUnitario = produto.Preco
                    });
                }

                AtualizarResumo();
                LimparBusca();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar produto: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRemoverItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.DataContext as ItemVenda;

            if (item != null)
            {
                _itensVenda.Remove(item);
                AtualizarResumo();
            }
        }

        private void BtnRemoverItemSelecionado_Click(object sender, RoutedEventArgs e)
        {
            var itemSelecionado = dgItensVenda.SelectedItem as ItemVenda;

            if (itemSelecionado != null)
            {
                _itensVenda.Remove(itemSelecionado);
                AtualizarResumo();
            }
            else
            {
                MessageBox.Show("Selecione um item para remover!",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnSelecionarCliente_Click(object sender, RoutedEventArgs e)
        {
            // Aqui você criaria uma janela de seleção de cliente
            // Por enquanto, vou simular:
            MessageBox.Show("Seleção de cliente - Em desenvolvimento\n\n" +
                          "Pressione OK para continuar sem cliente.",
                          "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnBuscarProdutos_Click(object sender, RoutedEventArgs e)
        {
            // Abrir janela de busca de produtos
            var produtosWindow = new ProdutosWindow();
            produtosWindow.ShowDialog();
        }

        private async void BtnFinalizarVenda_Click(object sender, RoutedEventArgs e)
        {
            if (_itensVenda.Count == 0)
            {
                MessageBox.Show("Adicione itens à venda!",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Confirmar venda no valor de {CalcularTotal():C2}?",
                "Finalizar Venda",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _vendaService.RealizarVendaAsync(
                        _usuarioLogado.Id,
                        _clienteSelecionado?.Id,
                        _itensVenda.ToList()
                    );

                    MessageBox.Show("Venda realizada com sucesso!",
                        "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                    LimparVenda();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao finalizar venda: {ex.Message}",
                        "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnCancelarVenda_Click(object sender, RoutedEventArgs e)
        {
            if (_itensVenda.Count == 0) return;

            var result = MessageBox.Show(
                "Deseja cancelar a venda atual?",
                "Cancelar Venda",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                LimparVenda();
            }
        }

        private void LimparVenda()
        {
            _itensVenda.Clear();
            _clienteSelecionado = null;
            txtClienteSelecionado.Text = "Nenhum cliente selecionado";
            AtualizarResumo();
            LimparBusca();
        }

        private void LimparBusca()
        {
            txtBuscaProduto.Clear();
            txtQuantidade.Text = "1";
            txtBuscaProduto.Focus();
        }

        private void AtualizarResumo()
        {
            txtTotalItens.Text = _itensVenda.Sum(i => i.Quantidade).ToString();
            txtValorTotal.Text = CalcularTotal().ToString("C2");
        }

        private decimal CalcularTotal()
        {
            return _itensVenda.Sum(i => i.Subtotal);
        }

        private void TxtNumerico_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            if (_itensVenda.Count > 0)
            {
                var result = MessageBox.Show(
                    "Há uma venda em andamento. Deseja realmente sair?",
                    "Confirmar",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.No) return;
            }

            this.Close();
        }
    }
}