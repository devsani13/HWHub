using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HWHub
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AdicionarPage : ContentPage
	{
		public AdicionarPage ()
		{
			InitializeComponent ();

            MaxValuePicker.SelectedIndex = 4; // Seleciona o valor máximo de 5 inicialmente
            OnMaxValueChanged(this, EventArgs.Empty);
        }

        private async void FecharPagina(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void OnChoosePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                // Apresentar as opções ao usuário (Câmera ou Galeria)
                var action = await DisplayActionSheet("Escolha uma opção", "Cancelar", null, "Tirar foto", "Escolher da galeria");

                if (action == "Tirar foto")
                {
                    // Método para capturar foto com a câmera
                    var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                    {
                        Title = "Tire uma foto"
                    });

                    if (photo != null)
                    {
                        var stream = await photo.OpenReadAsync();
                        ImagePreview.Source = ImageSource.FromStream(() => stream);
                        ImagePreview.Padding = 0;
                    }
                }
                else if (action == "Escolher da galeria")
                {
                    // Método para selecionar foto da galeria
                    var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                    {
                        Title = "Escolha uma foto"
                    });

                    if (photo != null)
                    {
                        var stream = await photo.OpenReadAsync();
                        ImagePreview.Source = ImageSource.FromStream(() => stream);
                        ImagePreview.Padding = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private void OnMaxValueChanged(object sender, EventArgs e)
        {
            if (MaxValuePicker.SelectedIndex != -1)
            {
                int maxValue = (int)MaxValuePicker.SelectedItem;

                // Atualiza o label com a nova informação
                NumberLabel.Text = $"1/{maxValue}";

                // Reseta o valor inicial para 1
                currentValue = 1;
            }
        }

        private void OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            int currentValue = (int)e.NewValue;
            int maxValue = (int)MaxValuePicker.SelectedItem;
            NumberLabel.Text = $"{currentValue}/{maxValue}";
        }

        // Incremento manual ao clicar no botão "+"
        private int currentValue = 1;  // Variável para armazenar o valor atual

        private void OnIncreaseClicked(object sender, EventArgs e)
        {
            int maxValue = (int)MaxValuePicker.SelectedItem;

            if (currentValue < maxValue)
            {
                currentValue++;
                NumberLabel.Text = $"{currentValue}/{maxValue}";
            }
        }

        private void OnDecreaseClicked(object sender, EventArgs e)
        {
            if (currentValue > 1)
            {
                currentValue--;
                int maxValue = (int)MaxValuePicker.SelectedItem;
                NumberLabel.Text = $"{currentValue}/{maxValue}";
            }
        }
    }
}