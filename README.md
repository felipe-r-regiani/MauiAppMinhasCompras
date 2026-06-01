# AppMinhasCompras

Aplicativo de lista de compras desenvolvido em .NET MAUI com persistência local em SQLite. Permite cadastrar, editar, excluir, pesquisar e somar produtos.

## Funcionalidades

- Cadastro de produtos (descrição, quantidade, preço)
- Listagem de todos os produtos com ID, descrição, preço, quantidade e total
- Edição de produtos ao tocar em um item da lista
- Exclusão de produtos com confirmação via menu de contexto
- Pesquisa de produtos por descrição (filtro em tempo real)
- Soma do valor total de todos os produtos (formatado em R$)
- Pull-to-refresh para recarregar a lista

## Como funciona

O app inicia na tela `ListaProdutos`. As principais ações:

1. **Adicionar** — toolbar "Adicionar" → `CadastroProduto` → preenche formulário → salva → `App.Db.Insert()` → retorna à lista
2. **Editar** — toca em um item → `CadastroProduto` com `BindingContext = produto` → altera → `App.Db.Update()` → retorna
3. **Excluir** — contexto "Remover" → `DisplayAlert` de confirmação → `App.Db.Delete(id)`
4. **Pesquisar** — `SearchBar` com `TextChanged` → `App.Db.Search(q)` atualiza a lista em tempo real
5. **Somar** — toolbar "Somar" → `lista.Sum(p => p.Total)` exibe total em `DisplayAlert` com formatação `:C`
6. **Atualizar** — pull-to-refresh → `App.Db.GetAll()` recarrega a `ListView`

### Estrutura do Projeto

```
Models/Produto.cs                   → Entidade com Id, Descricao, Quantidade, Preco e Total (computado)
Helpers/SQLiteDatabaseHelper.cs     → CRUD assíncrono com SQLiteAsyncConnection
Views/ListaProdutos.xaml(.cs)       → Tela principal com listagem, busca, soma e exclusão
Views/CadastroProduto.xaml(.cs)     → Tela de cadastro/edição
```

## Conceitos novos aprendidos

1. **SQLite completo com async/await** — integração de banco de dados local com `SQLiteAsyncConnection`, `CreateTableAsync`, `InsertAsync`, `UpdateAsync`, `DeleteAsync` e `QueryAsync`.
2. **CRUD completo** — implementação das quatro operações com decisão insert/update baseada na presença de `BindingContext` (null = novo, preenchido = edição).
3. **ObservableCollection\<T\>** — coleção que notifica a UI automaticamente ao adicionar/remover itens da `ListView`.
4. **SearchBar com filtro em tempo real** — evento `TextChanged` dispara busca no banco a cada caractere digitado.
5. **Context Actions (swipe-to-delete)** — `MenuItem` em `ViewCell.ContextActions` para ação de excluir com `DisplayAlert` de confirmação.
6. **Formatação de moeda com CultureInfo** — `Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR")` para exibir valores no formato `R$ 1.234,56`.
7. **Pull-to-refresh** — `IsPullToRefreshEnabled="True"` na `ListView` com evento `Refreshing` vinculado à recarga dos dados.
8. **SQL parameterizado** — uso de `?` placeholders no método `Update` para prevenir SQL injection.
9. **Propriedade computada ignorada pelo SQLite** — `Total { get => Quantidade * Preco; }` não possui `set`, portanto o SQLite não tenta persistí-la; é calculada em memória.
10. **Decisão entre Insert e Update** — padrão `if (BindingContext is Produto p) { App.Db.Update(p); } else { App.Db.Insert(new Produto {...}); }` para reutilizar a mesma tela de cadastro.
