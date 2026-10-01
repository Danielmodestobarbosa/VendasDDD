using FluentAssertions;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Pedidos.Entities;
using Vendas.Domain.Pedidos.Enums;
using Vendas.Domain.Pedidos.Events;

namespace Vendas.Domain.Tests.Pedidos.Entities;

public class PagamentoTests
{
    [Fact(DisplayName = "Deve criar um pagamento válido com status pendente")]
    public void Deve_Criar_Pagamento_Valido_Com_Status_Pendente()
    {
        var pedidoId = Guid.NewGuid();
        var metodo = MetodoPagamento.CartaoCredito;
        var valor = 100m;

        var pagamento = new Pagamento(pedidoId, metodo, valor);

        pagamento.PedidoId.Should().Be(pedidoId);
        pagamento.MetodoPagamento.Should().Be(metodo);
        pagamento.Valor.Should().Be(valor);
        pagamento.StatusPagamento.Should().Be(StatusPagamento.Pendente);
        pagamento.DataPagamento.Should().BeNull();
        pagamento.CodigoTransacao.Should().BeNull();
    }

    [Fact(DisplayName = "Não deve criar pagamento com valor menor ou igual a zero")]
    public void Nao_Deve_Criar_Pagamento_Com_Valor_Invalido()
    {
        var pedidoId = Guid.NewGuid();

        Action act = () => new Pagamento(pedidoId, MetodoPagamento.Pix, 0);

        act.Should()
           .Throw<DomainException>()
           .WithMessage("O valor do pagamento deve ser maior que zero.");
    }

    [Fact(DisplayName = "Não deve definir código de transação nulo ou vazio")]
    public void Nao_Deve_Definir_Codigo_Transacao_Nulo_Ou_Vazio()
    {
        var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.Pix, 100m);

        Action act = () => pagamento.DefinirCodigoTransacao("");

        act.Should()
           .Throw<DomainException>()
           .WithMessage("Código da transação inválido.");
    }

    [Fact(DisplayName = "Deve definir código de transação válido e atualizar DataAtualizacao")]
    public void Deve_Definir_Codigo_Transacao_Valido()
    {
        var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.CartaoCredito, 100m);
        var codigo = "TXN-12345";

        pagamento.DefinirCodigoTransacao(codigo);

        pagamento.CodigoTransacao.Should().Be(codigo);
        pagamento.DataAtualizacao.Should().NotBeNull();
    }

    [Fact(DisplayName = "Não deve redefinir código de transação já definido")]
    public void Nao_Deve_Redefinir_Codigo_Transacao()
    {
        var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.CartaoCredito, 100m);
        pagamento.DefinirCodigoTransacao("TXN-001");

        Action act = () => pagamento.DefinirCodigoTransacao("TXN-002");

        act.Should()
           .Throw<DomainException>()
           .WithMessage("O código de transação já foi definido.");
    }

    [Fact(DisplayName = "Deve gerar código de transação local automaticamente")]
    public void Deve_Gerar_Codigo_Transacao_Local()
    {
        var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.Pix, 200m);

        pagamento.GerarCodigoTransacaoLocal();

        pagamento.CodigoTransacao.Should().StartWith("LOCAL-");
        pagamento.CodigoTransacao.Should().HaveLength(14); // LOCAL- + 8 chars
        pagamento.DataAtualizacao.Should().NotBeNull();
    }

    [Fact(DisplayName = "Deve confirmar pagamento pendente com código válido e gerar evento completo")]
    public void Deve_Confirmar_Pagamento_Com_Codigo_Valido_E_Evento_Completo()
    {
        var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.CartaoCredito, 300m);
        pagamento.GerarCodigoTransacaoLocal(); // Simula gateway

        pagamento.ConfirmarPagamento();

        pagamento.StatusPagamento.Should().Be(StatusPagamento.Aprovado);
        pagamento.DataPagamento.Should().NotBeNull();
        pagamento.DataAtualizacao.Should().NotBeNull();

        var evento = pagamento.DomainEvents.OfType<PagamentoAprovadoEvent>().FirstOrDefault();
        evento.Should().NotBeNull();
        evento!.PagamentoId.Should().Be(pagamento.Id);
        evento.PedidoId.Should().Be(pagamento.PedidoId);
        evento.Valor.Should().Be(pagamento.Valor);
        evento.CodigoTransacao.Should().Be(pagamento.CodigoTransacao);
        evento.DataPagamento.Should().Be(pagamento.DataPagamento);
    }

    [Fact(DisplayName = "Não deve confirmar pagamento sem código de transação")]
    public void Nao_Deve_Confirmar_Sem_Codigo_Transacao()
    {
        var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.Pix, 100m);

        Action act = () => pagamento.ConfirmarPagamento();

        act.Should()
           .Throw<DomainException>()
           .WithMessage("O pagamento não pode ser confirmado sem o código de transação.");
    }
    [Fact(DisplayName = "Não deve confirmar pagamento que não está pendente")]
    public void Nao_Deve_Confirmar_Pagamento_Que_Nao_Esta_Pendente()
    {
        var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.Pix, 100m);
        pagamento.GerarCodigoTransacaoLocal();
        pagamento.ConfirmarPagamento();

        Action act = () => pagamento.ConfirmarPagamento();

        act.Should()
           .Throw<DomainException>()
           .WithMessage("Apenas pagamentos pendentes podem ser confirmados.");
    }

    [Fact(DisplayName = "Deve recusar pagamento pendente e gerar evento de rejeição com dados corretos")]
    public void Deve_Recusar_Pagamento_Pendente_E_Gerar_Evento_Com_Dados()
    {
        var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.Pix, 120m);

        pagamento.RecusarPagamento();

        pagamento.StatusPagamento.Should().Be(StatusPagamento.Recusado);
        pagamento.DataPagamento.Should().NotBeNull();
        pagamento.DataAtualizacao.Should().NotBeNull();

        var evento = pagamento.DomainEvents.OfType<PagamentoRejeitadoEvent>().FirstOrDefault();
        evento.Should().NotBeNull();
        evento!.PagamentoId.Should().Be(pagamento.Id);
        evento.PedidoId.Should().Be(pagamento.PedidoId);
        evento.Valor.Should().Be(pagamento.Valor);
        evento.CodigoTransacao.Should().Be(pagamento.CodigoTransacao);
        evento.DataPagamento.Should().Be(pagamento.DataPagamento);
    }

    [Fact(DisplayName = "Não deve recusar pagamento que não está pendente")]
    public void Nao_Deve_Recusar_Pagamento_Que_Nao_Esta_Pendente()
    {
        var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.Pix, 120m);
        pagamento.GerarCodigoTransacaoLocal();
        pagamento.ConfirmarPagamento();

        Action act = () => pagamento.RecusarPagamento();

        act.Should()
           .Throw<DomainException>()
           .WithMessage("Apenas pagamentos pendentes podem ser recusados.");
    }
}
