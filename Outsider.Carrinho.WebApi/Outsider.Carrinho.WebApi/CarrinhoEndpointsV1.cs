using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Outsider.Carrinho.WebAPI.Data;

public static class CarrinhoEndpointsV1
{
    private static readonly CarrinhoContext _context;

    public static RouteGroupBuilder MapCarrinhoApiV1(this RouteGroupBuilder group)
    {
        group.MapGet("/", ObterCarrinho);
        group.MapPost("/", AdicionarItemCarrinho);
        group.MapPut("/{id}", AtualizarItemCarrinho);
        group.MapDelete("/{id}", RemoverItemCarrinho);

        return group;
    }

    public static async Task<Ok<CarrinhoCliente>> ObterCarrinho()
        => TypedResults.Ok(await ObterCarrinhoCliente() ?? new CarrinhoCliente());

    public static async Task<Ok> AdicionarItemCarrinho(CarrinhoItem item)
    {
        var carrinho = await ObterCarrinhoCliente();

        if (carrinho == null)
            ManipularNovoCarrinho(item);
        else
            ManipularCarrinhoExistente(carrinho, item);

        await PersistirDados();
        return TypedResults.Ok();
    }

    public static async Task<Results<Ok, NotFound>> AtualizarItemCarrinho(Guid produtoId, CarrinhoItem item)
    {
        var carrinho = await ObterCarrinhoCliente();
        var itemCarrinho = await ObterItemCarrinhoValidado(produtoId, carrinho, item);

        if (itemCarrinho == null) return TypedResults.Ok();

        carrinho.AtualizarUnidades(itemCarrinho, item.Quantidade);

        _context.CarrinhoItens.Update(itemCarrinho);
        _context.CarrinhoCliente.Update(carrinho);

        await PersistirDados();
        return TypedResults.Ok();
    }

    public static async Task<Results<Ok, NoContent>> RemoverItemCarrinho(Guid produtoId)
    {
        var carrinho = await ObterCarrinhoCliente();

        var itemCarrinho = await ObterItemCarrinhoValidado(produtoId, carrinho);
        if (itemCarrinho == null) return TypedResults.NoContent();

        carrinho.RemoverItem(itemCarrinho);

        _context.CarrinhoItens.Remove(itemCarrinho);
        _context.CarrinhoCliente.Update(carrinho);

        await PersistirDados();
        return TypedResults.Ok();
    }

    private static async Task<CarrinhoCliente> ObterCarrinhoCliente()
    {
        return await _context.CarrinhoCliente
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.ClienteId == null);
    }

    private static void ManipularNovoCarrinho(CarrinhoItem item)
    {
        var carrinho = new CarrinhoCliente();
        carrinho.AdicionarItem(item);

        _context.CarrinhoCliente.Add(carrinho);
    }

    private static void ManipularCarrinhoExistente(CarrinhoCliente carrinho, CarrinhoItem item)
    {
        var produtoItemExistente = carrinho.CarrinhoItemExistente(item);

        carrinho.AdicionarItem(item);

        if (produtoItemExistente)
            _context.CarrinhoItens.Update(carrinho.ObterPorProdutoId(item.ProdutoId));
        else
            _context.CarrinhoItens.Add(item);

        _context.CarrinhoCliente.Update(carrinho);
    }

    private static async Task<CarrinhoItem> ObterItemCarrinhoValidado(Guid produtoId, CarrinhoCliente carrinho, CarrinhoItem item = null)
    {
        if (item != null && produtoId != item.ProdutoId)
            return null;

        if (carrinho == null)
            return null;

        var itemCarrinho = await _context.CarrinhoItens
            .FirstOrDefaultAsync(i => i.CarrinhoId == carrinho.Id && i.ProdutoId == produtoId);

        if (itemCarrinho == null || !carrinho.CarrinhoItemExistente(itemCarrinho))
            return null;

        return itemCarrinho;
    }

    private static async Task PersistirDados() => await _context.SaveChangesAsync();
}