using System.Windows;
using System.Windows.Input;
using SistemaPDV.Business.Services;
using SistemaPDV.Data.Context;

namespace SistemaPDV.UI.Views
{
    public partial class LoginWindow : Window
    {
        private readonly AutenticacaoService _autenticacaoService;

        public LoginWindow()
        {
            InitializeComponent();
            _autenticacaoService = new AutenticacaoService(new AppDbContext());

            // Focar no campo de login ao abrir
            txtLogin.Focus();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = txtLogin.Text;
            var senha = txtSenha.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(senha))
            {
                MessageBox.Show("Preencha todos os campos!", "Atencao",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var usuario = await _autenticacaoService.AutenticarAsync(login, senha);

            if (usuario != null)
            {
                var mainWindow = new MainWindow(usuario);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Login ou senha invalidos!", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtSenha.Clear();
                txtLogin.Focus();
            }
        }

        //  Método para pressionar Enter no campo Login
        private void TxtLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtSenha.Focus(); // Move para o campo senha
            }
        }

        //  Método para pressionar Enter no campo Senha
        private void TxtSenha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnLogin_Click(sender, e); // Executa o login
            }
        }
    }
}