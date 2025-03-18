using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using HWHub;
using SQLite;

namespace HWHub
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditarMiniaturaPage : ContentPage
    {

        private Miniatura _miniatura;
        private DatabaseHelper _databaseHelper = new DatabaseHelper();
        private string _imagemPath;
        private Miniatura miniatura;

        public EditarMiniaturaPage(Miniatura miniatura)
        {
            InitializeComponent();
            this.miniatura = miniatura;
            BindingContext = miniatura;

            MaxValuePicker.SelectedIndex = miniatura.LimiteColecao == 5 ? 0 : 1;
            OnMaxValueChanged(this, EventArgs.Empty);

            NumberLabel.Text = $"{miniatura.Posição}/{miniatura.LimiteColecao}";
        }

        private async void FecharPagina(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private int currentValue = 1;

        private void OnMaxValueChanged(object sender, EventArgs e)
        {
            if (MaxValuePicker.SelectedIndex != -1)
            {
                int maxValue = (int)MaxValuePicker.SelectedItem;
                miniatura.LimiteColecao = maxValue;

                NumberLabel.Text = $"{miniatura.Posição}/{maxValue}";
            }
        }

        private void OnIncreaseClicked(object sender, EventArgs e)
        {
            int maxValue = (int)MaxValuePicker.SelectedItem;

            if (miniatura.Posição < maxValue)
            {
                miniatura.Posição++;
                NumberLabel.Text = $"{miniatura.Posição}/{maxValue}";
            }
        }

        private void OnDecreaseClicked(object sender, EventArgs e)
        {
            if (miniatura.Posição > 1)
            {
                miniatura.Posição--;
                int maxValue = (int)MaxValuePicker.SelectedItem;
                NumberLabel.Text = $"{miniatura.Posição}/{maxValue}";
            }
        }

        private async void OnSalvarClicked(object sender, EventArgs e)
        {
            int result = await _databaseHelper.AtualizarMiniaturaAsync(miniatura);

            if (result > 0)
            {
                await DisplayAlert("Sucesso", "Alterações salvas com sucesso!", "OK");
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Erro", "Erro ao salvar alterações.", "OK");
            }
        }

        private async void OnChoosePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                var action = await DisplayActionSheet("Escolha uma opção", "Cancelar", null, "Tirar foto", "Escolher da galeria");

                FileResult photo = null;
                if (action == "Tirar foto")
                {
                    photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions { Title = "Tire uma foto" });
                }
                else if (action == "Escolher da galeria")
                {
                    photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Escolha uma foto" });
                }

                if (photo != null)
                {
                    string localPath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);

                    using (var stream = await photo.OpenReadAsync())
                    using (var newStream = File.OpenWrite(localPath))
                    {
                        await stream.CopyToAsync(newStream);
                    }

                    // Atualiza o caminho da imagem na miniatura
                    miniatura.ImagemPath = localPath;

                    // Atualiza manualmente o BindingContext (opcional)
                    BindingContext = null;
                    BindingContext = miniatura;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async void OnExcluirClicked(object sender, EventArgs e)
        {
            var confirmacao = await DisplayAlert("Confirmar Exclusão", "Tem certeza que deseja excluir esta miniatura?", "Sim", "Não");

            if (confirmacao)
            {
                int result = await _databaseHelper.DeletarMiniaturaAsync(miniatura);

                if (result > 0)
                {
                    await DisplayAlert("Sucesso", "Miniatura excluída com sucesso!", "OK");
                    await Navigation.PopModalAsync();
                }
                else
                {
                    await DisplayAlert("Erro", "Erro ao excluir a miniatura.", "OK");
                }
            }
        }

        private List<string> nomesCadastrados = new List<string>();
        private List<string> colecoesCadastradas = new List<string>();
        private List<string> marcasCadastradas = new List<string>();
        private List<string> coresCadastradas = new List<string>();

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var miniaturas = await _databaseHelper.GetMiniaturasAsync();

            nomesCadastrados = miniaturas.Select(m => m.Nome).Distinct().ToList();
            colecoesCadastradas = miniaturas.Select(m => m.Colecao).Distinct().ToList();
            marcasCadastradas = miniaturas.Select(m => m.Marca).Distinct().ToList();
            coresCadastradas = miniaturas.Select(m => m.Cor).Distinct().ToList();
        }

        private void OnNomeTextChanged(object sender, TextChangedEventArgs e)
        {
            AtualizarSugestoes(e.NewTextValue, nomesCadastrados, SugestoesNomeListView);
        }

        private void OnColecaoTextChanged(object sender, TextChangedEventArgs e)
        {
            AtualizarSugestoes(e.NewTextValue, colecoesCadastradas, SugestoesColecaoListView);
        }

        private void OnMarcaTextChanged(object sender, TextChangedEventArgs e)
        {
            AtualizarSugestoes(e.NewTextValue, marcasCadastradas, SugestoesMarcaListView);
        }

        private void OnCorTextChanged(object sender, TextChangedEventArgs e)
        {
            AtualizarSugestoes(e.NewTextValue, coresCadastradas, SugestoesCorListView);
        }

        private void AtualizarSugestoes(string texto, List<string> lista, ListView listView)
        {
            if (!string.IsNullOrWhiteSpace(texto))
            {
                var sugestoes = lista.Where(item => item.ToLower().Contains(texto.ToLower())).ToList();
                listView.ItemsSource = sugestoes;
                listView.IsVisible = sugestoes.Any();
            }
            else
            {
                listView.IsVisible = false;
            }
        }

        private void OnSugestaoNomeSelecionada(object sender, ItemTappedEventArgs e)
        {
            Nome.Text = e.Item.ToString();
            SugestoesNomeListView.IsVisible = false;
        }

        private void OnSugestaoColecaoSelecionada(object sender, ItemTappedEventArgs e)
        {
            Colecao.Text = e.Item.ToString();
            SugestoesColecaoListView.IsVisible = false;
        }

        private void OnSugestaoMarcaSelecionada(object sender, ItemTappedEventArgs e)
        {
            Marca.Text = e.Item.ToString();
            SugestoesMarcaListView.IsVisible = false;
        }

        private void OnSugestaoCorSelecionada(object sender, ItemTappedEventArgs e)
        {
            Cor.Text = e.Item.ToString();
            SugestoesCorListView.IsVisible = false;
        }

        // Esconde as sugestões ao perder o foco
        private void OnEntryUnfocused(object sender, FocusEventArgs e)
        {
            SugestoesNomeListView.IsVisible = false;
            SugestoesColecaoListView.IsVisible = false;
            SugestoesMarcaListView.IsVisible = false;
            SugestoesCorListView.IsVisible = false;
        }
    }
}