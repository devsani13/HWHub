﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HWHub
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AdicionarPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
