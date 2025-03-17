using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HWHub
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListarMiniaturasPage : ContentPage
    {

        private readonly DatabaseHelper _databaseHelper;

        public ListarMiniaturasPage()
        {
            InitializeComponent();
            _databaseHelper = new DatabaseHelper();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarMiniaturas();
        }

        private async Task CarregarMiniaturas()
        {
            var miniaturas = await _databaseHelper.GetMiniaturasAsync();
            MiniaturasListView.ItemsSource = miniaturas;
        }
    }
}