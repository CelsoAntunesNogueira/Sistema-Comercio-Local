using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using SistemaPDV.Data.Context;
using SistemaPDV.Models.Entities;

namespace SistemaPDV.UI.Views
{
    public partial class RelatoriosWindow : Window
    {
        private readonly AppDbContext _context;

        public RelatoriosWindow()
        {
            InitializeComponent();
            _context = new AppDbContext();

            // Definir periodo padrao (ultimo mes)
            dtInicio.SelectedDate = DateTime.Now.AddMonths(-1);
            dtFim.SelectedDate = DateTime.Now;

            // Carregar dados iniciais
            CarregarRelatorios();
        }

        private async void BtnGerar_Click(object sender, RoutedEventArgs e)
        {
            CarregarRelatorios();
        }

        private async void CarregarRelatorios()
        {
            try
            {
                var dataInicio = dtInicio.SelectedDate ?? DateTime.Now.AddMonths(-1);
                var dataFim = dtFim.SelectedDate ?? DateTime.Now;

                // Adicionar 1 dia ao fim para incluir o dia inteiro
                dataFim = dataFim.AddDays(1).AddSeconds(-1);

                // 1. Vendas no periodo
                var vendas = await _context.Vendas
                    .Include(v => v.Usuario)
                    .Include(v => v.Cliente)
                    .Where(v => v.DataVenda >= dataInicio && v.DataVenda <= dataFim)
                    .OrderByDescending(v => v.DataVenda)
                    .ToListAsync();

                dgVendas.ItemsSource = vendas;

                // 2. Cards de resumo
                txtTotalVendas.Text = vendas.Count.ToString();
                txtFaturamento.Text = vendas.Sum(v => v.ValorTotal).ToString("C2");

                // 3. Produtos mais vendidos - CORRIGIDO
                var todosItens = await _context.ItensVenda
                    .Include(i => i.Produto)
                    .Include(i => i.Venda)
                    .Where(i => i.Venda.DataVenda >= dataInicio && i.Venda.DataVenda <= dataFim)
                    .ToListAsync();

                // Fazer o GroupBy em memória (não no banco)
                var produtosMaisVendidos = todosItens
                    .GroupBy(i => new { i.ProdutoId, NomeProduto = i.Produto.Nome })
                    .Select(g => new
                    {
                        ProdutoId = g.Key.ProdutoId,
                        NomeProduto = g.Key.NomeProduto,
                        Quantidade = g.Sum(i => i.Quantidade),
                        ValorTotal = g.Sum(i => i.Subtotal)
                    })
                    .OrderByDescending(x => x.Quantidade)
                    .Take(10)
                    .ToList();

                // Adicionar posicao
                var listaProdutos = produtosMaisVendidos
                    .Select((item, index) => new
                    {
                        Posicao = index + 1,
                        item.NomeProduto,
                        item.Quantidade,
                        item.ValorTotal
                    })
                    .ToList();

                dgProdutosMaisVendidos.ItemsSource = listaProdutos;

                // 4. Produtos com estoque baixo
                var estoqueBaixo = await _context.Produtos
                    .Where(p => p.EstoqueAtual <= p.EstoqueMinimo && p.Ativo)
                    .OrderBy(p => p.EstoqueAtual)
                    .ToListAsync();

                dgEstoqueBaixo.ItemsSource = estoqueBaixo;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar relatorios: {ex.Message}\n\n{ex.InnerException?.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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