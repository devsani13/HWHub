using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using HWHub;
using System.IO;
using Xamarin.Essentials;

namespace HWHub
{
    public class DatabaseHelper
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseHelper()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "miniaturas.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Miniatura>().Wait();
        }

        public Task<int> SalvarMiniaturaAsync(Miniatura miniatura)
        {
            return _database.InsertAsync(miniatura);
        }

        public async Task<List<Miniatura>> GetMiniaturasAsync()
        {
            return await _database.Table<Miniatura>().ToListAsync();
        }

        public async Task<int> AtualizarMiniaturaAsync(Miniatura miniatura)
        {
            return await _database.UpdateAsync(miniatura);
        }

        public async Task<int> DeletarMiniaturaAsync(Miniatura miniatura)
        {
            return await _database.DeleteAsync(miniatura);
        }
    }
}
