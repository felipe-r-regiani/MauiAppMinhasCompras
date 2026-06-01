using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProdutos : ContentPage
{
    ObservableCollection<Produto> lista = new();

	public ListaProdutos()
	{
		InitializeComponent();

        lst_produtos.ItemsSource = lista;

        /*lst_produtos.ItemsSource = new List<Models.Produto>()
        {
            new Models.Produto() { Id=1, Descricao="Maçã", Preco=3.99, Quantidade=2 },
            new Models.Produto() { Id=2, Descricao="Pera", Preco=1.99, Quantidade=5 },
            new Models.Produto() { Id=3, Descricao="Uva", Preco=5.78, Quantidade=6 },
            new Models.Produto() { Id=4, Descricao="Mamão", Preco=2.18, Quantidade=1 },
            new Models.Produto() { Id=5, Descricao="Melão", Preco=3.75, Quantidade=12 }
        };*/
	}

    protected async override void OnAppearing()
    {
        try
        {
            lista.Clear();
            List<Produto> tmp = await App.Db.GetAll();
            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ops", ex.Message, "OK");
        }
    }

    private async void ToolbarItem_Clicked_Somar(object sender, EventArgs e)
    {
        double soma = lista.Sum(i => i.Total);

        string msg = $"O total é {soma:C}";

        await DisplayAlertAsync("Total", msg, "OK");
    }

    private void ToolbarItem_Clicked_Adicionar(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Views.CadastroProduto());
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string q = e.NewTextValue;
            lst_produtos.IsRefreshing = true;
            lista.Clear();
            List<Produto> tmp = await App.Db.Search(q);
            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ops", ex.Message, "OK");
        } finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {            
            lst_produtos.IsRefreshing = true;
            lista.Clear();
            List<Produto> tmp = await App.Db.GetAll();
            tmp.ForEach(i => lista.Add(i));
            txt_search.Text = "";
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private async void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            Produto? p = e.SelectedItem as Produto;

            await Navigation.PushAsync(new Views.CadastroProduto
            {
                BindingContext = p
            });

        } catch(Exception ex)
        {
            await DisplayAlertAsync("Ops", ex.Message, "OK");
        }
    }

    private async void MenuItem_Clicked_Remover(object sender, EventArgs e)
    {
        try
        {
            MenuItem? selecionado = sender as MenuItem;
            Produto? p = selecionado?.BindingContext as Produto;

            lst_produtos.IsRefreshing = true;

            bool confirmacao = await DisplayAlertAsync(
                "Tem certeza?",
                $"Remover {p.Descricao}?",
                "OK", "Cancelar"
                );

            if(confirmacao)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }
}