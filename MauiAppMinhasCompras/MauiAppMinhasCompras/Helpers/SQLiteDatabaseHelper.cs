using MauiAppMinhasCompras.Models;
using SQLite;
using System.Runtime.CompilerServices;

namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection conexao;

        public SQLiteDatabaseHelper(string caminho_pro_db3)
        {
            conexao = new SQLiteAsyncConnection(caminho_pro_db3);
            conexao.CreateTableAsync<Produto>().Wait();
        }

        public Task<int> Insert(Produto p)
        {
            return conexao.InsertAsync(p);
        }

        public Task<List<Produto>> Update(Produto p)
        {
            string sql = "Update Produto SET Descricao=?, Quantidade=?, Preco=? " +
                         "WHERE Id=?";

            return conexao.QueryAsync<Produto>(
                sql, p.Descricao, p.Quantidade, p.Preco, p.Id
            );
        }

        public Task<int> Delete(int id)
        {
            return conexao.Table<Produto>().DeleteAsync(i => i.Id == id);
        }

        public Task<List<Produto>> GetAll()
        {
            return conexao.Table<Produto>().ToListAsync();
        }
        public Task<List<Produto>> Search(string q)
        {
            string sql = $"SELECT * FROM Produto WHERE Descricao LIKE '%{q}%' ";
            return conexao.QueryAsync<Produto>(sql);
        }
    } // Fecha classe
} // Fecha namespace
