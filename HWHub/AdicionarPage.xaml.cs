﻿using System;
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
	public partial class AdicionarPage : ContentPage
	{

        private readonly DatabaseHelper _databaseHelper;
        private string _imagemPath;

        public AdicionarPage ()
		{
			InitializeComponent ();

            MaxValuePicker.SelectedIndex = 4;
            OnMaxValueChanged(this, EventArgs.Empty);

            _databaseHelper = new DatabaseHelper();
        }

        private async void FecharPagina(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
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

        private void OnMaxValueChanged(object sender, EventArgs e)
        {
            if (MaxValuePicker.SelectedIndex != -1)
            {
                int maxValue = (int)MaxValuePicker.SelectedItem;

                NumberLabel.Text = $"1/{maxValue}";

                currentValue = 1;
            }
        }

        private void OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            int currentValue = (int)e.NewValue;
            int maxValue = (int)MaxValuePicker.SelectedItem;
            NumberLabel.Text = $"{currentValue}/{maxValue}";
        }

        private int currentValue = 1;

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

        private async void OnSalvarClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Nome.Text) ||
                string.IsNullOrWhiteSpace(Colecao.Text) ||
                string.IsNullOrWhiteSpace(Marca.Text) ||
                string.IsNullOrWhiteSpace(Cor.Text))
            {
                await DisplayAlert("Erro", "Preencha todos os campos", "OK");
                return;
            }

            int limiteColecao = Convert.ToInt32(MaxValuePicker.SelectedItem);
            int posicaoColecao = currentValue;

            Miniatura miniatura = new Miniatura
            {
                Nome = Nome.Text,
                Colecao = Colecao.Text,
                Marca = Marca.Text,
                Quantidade = Convert.ToInt32(MaxValuePicker.SelectedItem ?? "0"),
                Cor = Cor.Text,
                THunt = option1.IsChecked,
                ImagemPath = _imagemPath,
                Posição = posicaoColecao,
                LimiteColecao = limiteColecao
            };

            await _databaseHelper.SalvarMiniaturaAsync(miniatura);
            await DisplayAlert("Sucesso", "Miniatura salva com sucesso!", "OK");

            Nome.Text = "";
            Colecao.Text = "";
            Marca.Text = "";
            Cor.Text = "";
            option1.IsChecked = false;
            option2.IsChecked = false;
            ImagePreview.Source = "camera.png";
            ImagePreview.Padding = 10;
            _imagemPath = null;

            await Navigation.PopModalAsync();
        }
    }
}