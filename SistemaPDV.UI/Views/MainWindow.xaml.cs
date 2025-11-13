using System.Windows;
using SistemaPDV.Models.Entities;
using SistemaPDV.UI.Views;

namespace SistemaPDV.UI
{
    public partial class MainWindow : Window
    {
        private readonly Usuario _usuarioLogado;

        public MainWindow(Usuario usuario)
        {
            InitializeComponent();
            _usuarioLogado = usuario;
            txtUsuarioLogado.Text = $"Usuário: {usuario.Nome} ({usuario.Tipo})";
        }

        private void BtnPDV_Click(object sender, RoutedEventArgs e)
        {
            var vendasWindow = new VendasWindow(_usuarioLogado);
            vendasWindow.Show();
        }

        private void BtnProdutos_Click(object sender, RoutedEventArgs e)
        {
            var produtosWindow = new ProdutosWindow();
            produtosWindow.Show();
        }

        private void BtnClientes_Click(object sender, RoutedEventArgs e)
        {
            var clientesWindow = new ClientesWindow();
            clientesWindow.Show();
        }


    
        private void BtnRelatorios_Click(object sender, RoutedEventArgs e)
        {
            var relatoriosWindow = new RelatoriosWindow();
            relatoriosWindow.Show();
        }

       
        private void BtnUsuarios_Click(object sender, RoutedEventArgs e)
        {
            if (_usuarioLogado.Tipo != "Administrador")
            {
                MessageBox.Show("Apenas administradores podem acessar esta area.",
                    "Acesso Negado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var usuariosWindow = new UsuariosWindow();
            usuariosWindow.Show();
        }

      
        private void BtnConfiguracoes_Click(object sender, RoutedEventArgs e)
        {
            var configWindow = new ConfiguracoesWindow();
            configWindow.Show();
        }

        private void BtnSair_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Deseja realmente sair?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}