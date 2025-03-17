using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HWHub
{
    public partial class MainPage : ContentPage
    {
        private List<Miniatura> todasMiniaturas;
        private List<Miniatura> miniaturasFiltradas;
        private readonly DatabaseHelper _databaseHelper;

        public MainPage()
        {
            InitializeComponent();
            _databaseHelper = new DatabaseHelper();
            CarregarMiniaturas();
        }

        private async void Adicionar(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new AdicionarPage());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarMiniaturas();
            var miniaturas = await _databaseHelper.GetMiniaturasAsync();
        }

        private async Task CarregarMiniaturas()
        {
            todasMiniaturas = await _databaseHelper.GetMiniaturasAsync();
            miniaturasFiltradas = new List<Miniatura>(todasMiniaturas);
            MiniaturasListView.ItemsSource = miniaturasFiltradas;
        }

        private async void MiniaturasListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Miniatura miniaturaSelecionada)
            {
                await Navigation.PushModalAsync(new EditarMiniaturaPage(miniaturaSelecionada));
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string termoPesquisa = e.NewTextValue?.ToLower().Trim() ?? "";

            if (string.IsNullOrWhiteSpace(termoPesquisa))
            {
                miniaturasFiltradas = new List<Miniatura>(todasMiniaturas);
            }
            else
            {
                miniaturasFiltradas = todasMiniaturas
                    .Where(m =>
                        (!string.IsNullOrEmpty(m.Nome) && m.Nome.ToLower().Contains(termoPesquisa)) ||
                        (!string.IsNullOrEmpty(m.Colecao) && m.Colecao.ToLower().Contains(termoPesquisa)) ||
                        (!string.IsNullOrEmpty(m.Marca) && m.Marca.ToLower().Contains(termoPesquisa)) ||
                        (!string.IsNullOrEmpty(m.Cor) && m.Cor.ToLower().Contains(termoPesquisa)) ||
                        (m.THunt && "thunt".Contains(termoPesquisa)) ||
                        (!m.THunt && "comum".Contains(termoPesquisa))
                    )
                    .ToList();
            }

            MiniaturasListView.ItemsSource = miniaturasFiltradas;
        }
    }
}
