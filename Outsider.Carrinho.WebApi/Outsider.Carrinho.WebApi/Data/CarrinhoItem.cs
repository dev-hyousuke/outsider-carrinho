using System.Text.Json.Serialization;

namespace Outsider.Carrinho.WebAPI.Data
{
    public class CarrinhoItem
    {
        public CarrinhoItem() => Id = Guid.NewGuid();

        public Guid Id { get; set; }
        public Guid ProdutoId { get; set; }
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
        public string Imagem { get; set; }

        public Guid CarrinhoId { get; set; }

        [JsonIgnore]
        public CarrinhoCliente CarrinhoCliente { get; set; }

        internal void AssociarCarrinho(Guid carrinhoId) => CarrinhoId = carrinhoId;

        internal decimal CalcularValor() => Quantidade * Valor;

        internal void AdicionarUnidades(int unidades) => Quantidade += unidades;

        internal void AtualizarUnidades(int unidades) => Quantidade = unidades;
    }
}