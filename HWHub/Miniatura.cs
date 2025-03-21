﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using HWHub;
using SQLite;

namespace HWHub
{
    public class Miniatura
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        private string _imagemPath;
        public string ImagemPath
        {
            get => _imagemPath;
            set
            {
                if (_imagemPath != value)
                {
                    _imagemPath = value;
                    OnPropertyChanged(nameof(ImagemPath)); // Notifica a UI sobre a mudança
                }
            }
        }
        public string Nome { get; set; }
        public string Colecao { get; set; }
        public string Marca { get; set; }
        public int Quantidade { get; set; }
        public string Cor { get; set; }
        public bool THunt { get; set; }
        public int Posição { get; set; }
        public int LimiteColecao { get; set; }
        public string PosicaoLimite => $"{Posição}/{LimiteColecao}";

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
