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

            // Aqui a miniatura já tem a posição carregada, não precisamos de uma variável separada
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

                // Atualiza a exibição da posição com a nova coleção
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
                await Navigation.PopModalAsync(); // Volta para a página anterior
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

                    // Atualizando a imagem na instância do _miniatura
                    _miniatura.ImagemPath = localPath;  // Isso vai garantir que a imagem seja salva corretamente

                    ImagePreview.Source = ImageSource.FromFile(localPath);
                    ImagePreview.Padding = 0;

                    _imagemPath = localPath;
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
                int result = await _databaseHelper.DeletarMiniaturaAsync(_miniatura);

                if (result > 0)
                {
                    await DisplayAlert("Sucesso", "Miniatura excluída com sucesso!", "OK");
                    await Navigation.PopModalAsync(); // Volta para a página anterior
                }
                else
                {
                    await DisplayAlert("Erro", "Erro ao excluir a miniatura.", "OK");
                }
            }
        }
    }
}