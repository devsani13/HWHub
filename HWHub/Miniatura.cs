using System;
using System.Collections.Generic;
using System.Text;
using HWHub;
using SQLite;

namespace HWHub
{
    public class Miniatura
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string ImagemPath { get; set; }
        public string Nome { get; set; }
        public string Colecao { get; set; }
        public string Marca { get; set; }
        public int Quantidade { get; set; }
        public string Cor { get; set; }
        public bool THunt { get; set; }
        public int Posição { get; set; }
        public int LimiteColecao { get; set; }
        public string PosicaoLimite => $"{Posição}/{LimiteColecao}";
    }
}
