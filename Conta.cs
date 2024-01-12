using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ContaPoupanca
{
    public string NumeroConta { get; set; }
    public string Titular { get; set; }
    public double Saldo { get; set; }
    public List<Transacao> Transacoes { get; set; }
    public double TaxaDeJuros { get; set; }

    public ContaPoupanca()
    {
        Transacoes = new List<Transacao>();
    }

    public ContaPoupanca(string numeroConta, string titularConta, double saldoInicial, double taxaDeJuros)
    {
        NumeroConta = numeroConta;
        Titular = titularConta;
        Saldo = saldoInicial;
        TaxaDeJuros = taxaDeJuros;
        Transacoes = new List<Transacao>();
    }

    public void ImprimirSaldo()
    {
        Console.Clear();
        string numeroFormatado = string.Format("{0:0.00}", Math.Truncate(Saldo * 100) / 100); // Formata o saldo para 2 casas decimais sem arredondamento
        Console.WriteLine($"O saldo da sua conta é de: R$ {numeroFormatado}\n");
    }

    public void Sacar(double valor)
    {
        if (valor <= Saldo && valor > 0 && Math.Abs(valor * 100 % 1) == 0)
        {
            Console.Clear();
            Saldo -= valor;
            Console.WriteLine($"Você conseguiu sacar com sucesso o valor de: R$ {valor:F2}\n");
            Transacoes.Add(new Transacao(DateTime.Now, "Debito", valor, "Saque"));
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Erro ao sacar. Verifique o valor inserido ou saldo insuficiente.\n");
        }
    }

    public void Depositar(double valor)
    {
        if (valor > 0 && Math.Abs(valor * 100 % 1) == 0)
        {
            Saldo += valor;
            Console.Clear();
            Console.WriteLine($"Você conseguiu depositar com sucesso o valor de: R$ {valor:F2}\n");
            Transacoes.Add(new Transacao(DateTime.Now, "Credito", valor, $"Deposito"));
            
            double rendimentos = valor * TaxaDeJuros/100;
            Saldo += rendimentos;
            Transacoes.Add(new Transacao(DateTime.Now, "Credito", rendimentos, $"Rendimentos"));
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Erro ao depositar, favor verifique o valor inserido.\n");
        }
    }

    public void ReceberTransferencia(double valor, string depositante)
    {
        Saldo += valor;
        Transacoes.Add(new Transacao(DateTime.Now, "Credito", valor, $"Transferencia de {depositante}"));

        double rendimentos = TaxaDeJuros/100;
        Saldo += rendimentos;
        Transacoes.Add(new Transacao(DateTime.Now, "Credito", rendimentos, $"Rendimentos"));
    }

    public void TransferirParaCorrente(double valorDaTransferencia)
    {
        if (valorDaTransferencia <= Saldo && valorDaTransferencia > 0 && Math.Abs(valorDaTransferencia * 100 % 1) == 0)
        {
            Saldo -= valorDaTransferencia;
           
            ContaCorrente contaCorrente = FuncoesDoSistema.ProcurarContaCorrentePorCPF(Titular); // Buscar a conta poupança no sistema
            contaCorrente.ReceberTransferencia(valorDaTransferencia, "sua Conta Poupanca"); // Envia o valo para a Poupança
            FuncoesDoSistema.AtualizarConta(contaCorrente);
            
            Transacoes.Add(new Transacao(DateTime.Now, "Debito", valorDaTransferencia, "Transferencia para Conta Corrente")); // Adiciona a transação na lista de transações
            
            Console.Clear();
            Console.WriteLine("Transferência realizada com sucesso!\n");
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Erro ao transferir. Verifique o valor inserido ou saldo insuficiente.\n");
        }
    }

    public void TransferenciaBancaria(ContaCorrente beneficiario, double valorDaTransferencia)
    {
        if (valorDaTransferencia <= Saldo && valorDaTransferencia > 0 && Math.Abs(valorDaTransferencia * 100 % 1) == 0)
        {
            Saldo -= valorDaTransferencia;
            
            Transacoes.Add(new Transacao(DateTime.Now, "Debito", valorDaTransferencia, $"Transferencia para {FuncoesDoSistema.ProcurarClientePorCPF(beneficiario.Titular).Nome}")); // Adiciona a transação na lista do depositante
            beneficiario.ReceberTransferencia(valorDaTransferencia, FuncoesDoSistema.ProcurarClientePorCPF(beneficiario.Titular).Nome);
            
            FuncoesDoSistema.AtualizarConta(beneficiario);

            Console.Clear();
            Console.WriteLine($"Transferencia para {FuncoesDoSistema.ProcurarClientePorCPF(beneficiario.Titular).Nome} realizada com sucesso!\n");
            
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Valor inválido ou saldo insuficiente.\n");
        }
    }

    public void TransferenciaBancaria(ContaPoupanca beneficiario, double valorDaTransferencia)
    {
        if (valorDaTransferencia <= Saldo && valorDaTransferencia > 0 && Math.Abs(valorDaTransferencia * 100 % 1) == 0)
        {
            Saldo -= valorDaTransferencia;
            
            beneficiario.ReceberTransferencia(valorDaTransferencia, FuncoesDoSistema.ProcurarClientePorCPF(Titular).Nome); // Adiciona o valor ao saldo do beneficiário
            Transacoes.Add(new Transacao(DateTime.Now, "Debito", valorDaTransferencia, $"Transferencia para {FuncoesDoSistema.ProcurarClientePorCPF(beneficiario.Titular).Nome}")); // Adiciona a transação na lista do depositante
            
            FuncoesDoSistema.AtualizarConta(beneficiario);

            Console.Clear();    
            Console.WriteLine($"Transferencia para {FuncoesDoSistema.ProcurarClientePorCPF(beneficiario.Titular).Nome} realizada com sucesso!\n");
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Valor inválido ou saldo insuficiente.\n");
        }
    }
}

public class ContaCorrente
{
    public string NumeroConta { get; set; }
    public string Titular { get; set; }
    public double Saldo { get; set; }
    public List<Transacao> Transacoes { get; set; }
    public double TaxaDeChequeEspecial { get; set; }
    public double LimiteChequeEspecial { get; set; }
    public double LimiteChequeEspecialRef { get; set; }

    public ContaCorrente()
    {
        Transacoes = new List<Transacao>();
    
    }

    public ContaCorrente(string numeroConta, string titularConta, double saldoInicial, double taxaDeChequeEspecial)
    {
        NumeroConta = numeroConta;
        Titular = titularConta;
        Saldo = saldoInicial;
        TaxaDeChequeEspecial = taxaDeChequeEspecial;
        LimiteChequeEspecialRef = 1000.00;
        LimiteChequeEspecial = LimiteChequeEspecialRef;
        Transacoes = new List<Transacao>();
    }

    public void ImprimirSaldo()
    {
        if(LimiteChequeEspecial < LimiteChequeEspecialRef && Saldo == 0)
        {   
            double saldoCalculado = LimiteChequeEspecialRef - LimiteChequeEspecial; // Calcula o saldo negativo baseado pelo uso do cheque especial
                        
            string numeroFormatado = string.Format("{0:0.00}", Math.Truncate(saldoCalculado * 100) / 100); // Formata o saldo para 2 casas decimais sem arredondamento
            Console.WriteLine($"Limites Disponíveis:\nSaldo: R$ -{numeroFormatado}\nCheque Especial: R$ {LimiteChequeEspecial:F2}\n");
        }
        else if (Saldo >= 0)
        {
            string numeroFormatado = string.Format("{0:0.00}", Math.Truncate(Saldo * 100) / 100); // Formata o saldo para 2 casas decimais sem arredondamento
            Console.WriteLine($"O seu saldo da conta corrente é de: R$ {numeroFormatado:F2}\n");
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Erro ao imprimir Saldo!\n");
        }
    }

    public void Sacar(double valor)
    {
        if (valor <= Saldo && Math.Abs(valor * 100 % 1) == 0)
        {
            Saldo -= valor;
            Console.Clear();
            Console.WriteLine($"Você conseguiu sacar com sucesso o valor de: R$ {valor:F2}\n");
            Transacoes.Add(new Transacao(DateTime.Now, "Debito", valor, "Saque"));
        }
        else if (valor <= 0 || LimiteChequeEspecial + Saldo - valor - TaxaDeChequeEspecial  < 0)
        {
            Console.Clear();
            Console.WriteLine("Erro ao sacar. Verifique o valor inserido ou saldo insuficiente.");
            Console.WriteLine($"Limites Disponíveis:\nSaldo: R${Saldo:F2}\nCheque Especial: R${LimiteChequeEspecial:F2}\n");
        }
        else if (valor > Saldo && Math.Abs(valor * 100 % 1) == 0)
        {
            if (valor - Saldo + TaxaDeChequeEspecial <= LimiteChequeEspecial)
            {
                if (Saldo > 0)
                {
                    double usoCheque = (Saldo - valor - TaxaDeChequeEspecial) * -1;
                    LimiteChequeEspecial -= usoCheque;
                }
                else
                {
                    LimiteChequeEspecial -= valor + TaxaDeChequeEspecial;
                }
                
                Saldo -= valor;

                if (Saldo < 0)
                {
                    Saldo = 0;
                }

                Console.Clear();
                Console.WriteLine($"Você conseguiu sacar com sucesso o valor de: R$ {valor:F2}");
                Console.WriteLine($"Foram utilizados R$ {LimiteChequeEspecialRef - LimiteChequeEspecial:F2} do seu Cheque Especial com a taxa de R$ {TaxaDeChequeEspecial:F2}.");
                Console.WriteLine($"Você ainda tem R$ {LimiteChequeEspecial:F2} de limite no Cheque Especial.\n");

                Transacoes.Add(new Transacao(DateTime.Now, "Debito", valor, "Saque"));
                Transacoes.Add(new Transacao(DateTime.Now, "Debito", TaxaDeChequeEspecial, "Taxa cheque especial"));
            }

        }
        else
        {
            Console.Clear();
            Console.WriteLine("Erro ao sacar. Verifique o valor inserido ou saldo insuficiente.\n");
        }
    }

    public void Depositar(double valor)
    {   
        if (valor > 0 && Math.Abs(valor * 100 % 1) == 0)
        {
            if(LimiteChequeEspecial < LimiteChequeEspecialRef && Saldo == 0)
            {
                LimiteChequeEspecial += valor;
                
                if (LimiteChequeEspecial > LimiteChequeEspecialRef)
                {
                    Saldo = LimiteChequeEspecial - LimiteChequeEspecialRef;
                    LimiteChequeEspecial = LimiteChequeEspecialRef;
                }
            }
            else
            {
                Saldo += valor;
            }

            Console.Clear();
            Console.WriteLine($"Depósito no valor de {valor:F2} realizado com sucesso!\n");
            Transacoes.Add(new Transacao(DateTime.Now, "Credito", valor, $"Deposito"));
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Erro ao depositar, favor verifique o valor inserido.\n");
        }
    }

    public void TransferirParaPoupança(double valorDaTransferencia)
    {
        if (valorDaTransferencia <= Saldo && valorDaTransferencia > 0 && Math.Abs(valorDaTransferencia * 100 % 1) == 0)
        {
            Saldo -= valorDaTransferencia;
           
            ContaPoupanca poupanca = FuncoesDoSistema.ProcurarContaPoupancaPorCPF(Titular); // Buscar a conta poupança no sistema
            poupanca.ReceberTransferencia(valorDaTransferencia, "sua Conta Corrente"); // Envia o valo para a Poupança
            FuncoesDoSistema.AtualizarConta(poupanca);
            
            Transacoes.Add(new Transacao(DateTime.Now, "Debito", valorDaTransferencia, "Transferencia para Poupanca")); // Adiciona a transação na lista de transações
            
            Console.Clear();
            Console.WriteLine("Transferência realizada com sucesso!\n");
        }
        else if (LimiteChequeEspecial + Saldo - valorDaTransferencia - TaxaDeChequeEspecial  < 0)
        {
            Console.Clear();
            Console.WriteLine("Erro ao transferir. Verifique o valor inserido ou saldo insuficiente.\n");
        }
        else if (valorDaTransferencia > Saldo && Math.Abs(valorDaTransferencia * 100 % 1) == 0)
        {
            if(valorDaTransferencia + TaxaDeChequeEspecial - Saldo < LimiteChequeEspecial)
            {
                double usoCheque = valorDaTransferencia - Saldo + TaxaDeChequeEspecial; // Calcula quando do cheque está sendo utilizando acrescido da taxa

                Saldo -= valorDaTransferencia + TaxaDeChequeEspecial; // Atualiza o valor do Saldo
                if (Saldo < 0)
                {
                    Saldo = 0;
                }

                LimiteChequeEspecial -= usoCheque;

                ContaPoupanca poupanca = FuncoesDoSistema.ProcurarContaPoupancaPorCPF(Titular); // Buscar a conta poupança no sistema
                poupanca.ReceberTransferencia(valorDaTransferencia, "sua Conta Corrente"); // Envia o valo para a Poupança
                FuncoesDoSistema.AtualizarConta(poupanca);
                
                Transacoes.Add(new Transacao(DateTime.Now, "Debito", valorDaTransferencia, "Transferencia para Poupanca")); // Adiciona a transação na lista de transações
                Transacoes.Add(new Transacao(DateTime.Now, "Debito", TaxaDeChequeEspecial, "Taxa cheque especial"));

                Console.Clear();
                Console.WriteLine("Transferência realizada com sucesso!\n");
            }
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Erro ao transferir. Verifique o valor inserido ou saldo insuficiente.\n");
        }
        
    }

    public void TransferenciaBancaria(ContaCorrente beneficiario, double valorDaTransferencia)
    {
        if (valorDaTransferencia <= Saldo && valorDaTransferencia > 0 && Math.Abs(valorDaTransferencia * 100 % 1) == 0)
        {
            Saldo -= valorDaTransferencia;
            
            Transacoes.Add(new Transacao(DateTime.Now, "Debito", valorDaTransferencia, $"Transferencia para {FuncoesDoSistema.ProcurarClientePorCPF(beneficiario.Titular).Nome}")); // Adiciona a transação na lista do depositante
            beneficiario.ReceberTransferencia(valorDaTransferencia, FuncoesDoSistema.ProcurarClientePorCPF(beneficiario.Titular).Nome);
            
            FuncoesDoSistema.AtualizarConta(beneficiario);

            Console.Clear();
            Console.WriteLine($"Transferencia para {FuncoesDoSistema.ProcurarClientePorCPF(beneficiario.Titular).Nome} realizada com sucesso!\n");
            
        }
        else if (valorDaTransferencia > Saldo && valorDaTransferencia > 0 && Math.Abs(valorDaTransferencia * 100 % 1) == 0)
        {
            if (valorDaTransferencia - Saldo + TaxaDeChequeEspecial < LimiteChequeEspecial) //
            {

                LimiteChequeEspecial -= valorDaTransferencia - Saldo + TaxaDeChequeEspecial; // Desconta o montante faltante do limite do cheque especial acrescido da taxa
                
                Saldo -= valorDaTransferencia + TaxaDeChequeEspecial; // Desconta o valor do saldo da conta do depositante
                if (Saldo < 0)
                {
                    Saldo = 0;
                }
                
                beneficiario.ReceberTransferencia(valorDaTransferencia, FuncoesDoSistema.ProcurarClientePorCPF(Titular).Nome); // Adiciona o valor ao saldo do beneficiário
                FuncoesDoSistema.AtualizarConta(beneficiario);
                
                Transacoes.Add(new Transacao(DateTime.Now, "Debito", valorDaTransferencia, $"Transferencia para {FuncoesDoSistema.ProcurarClientePorCPF(beneficiario.Titular).Nome}")); // Adiciona a transação na lista do depositante
                
                Console.Clear();
                Console.WriteLine($"Transferencia para {FuncoesDoSistema.ProcurarClientePorCPF(beneficiario.Titular).Nome} realizada com sucesso!\n");
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Saldo Insuficiente.\n");
            }
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Erro ao transferir. Verifique o valor inserido ou saldo insuficiente.\n");
        }
    }

    public void TransferenciaBancaria(ContaPoupanca beneficiario, double valorDaTransferencia)
    {
        if (valorDaTransferencia <= Saldo && Math.Abs(valorDaTransferencia * 100 % 1) == 0)
        {
            Saldo -= valorDaTransferencia;
            
            beneficiario.ReceberTransferencia(valorDaTransferencia, FuncoesDoSistema.ProcurarClientePorCPF(Titular).Nome); // Adiciona o valor ao saldo do beneficiário
            Transacoes.Add(new Transacao(DateTime.Now, "Debito", valorDaTransferencia, $"Transferencia para {FuncoesDoSistema.ProcurarClientePorCPF(beneficiario.Titular).Nome}")); // Adiciona a transação na lista do depositante
            
            FuncoesDoSistema.AtualizarConta(beneficiario);

            Console.Clear();    
            Console.WriteLine($"Transferencia para {FuncoesDoSistema.ProcurarClientePorCPF(beneficiario.Titular).Nome} realizada com sucesso!\n");
        }
        else if (valorDaTransferencia > Saldo && Math.Abs(valorDaTransferencia * 100 % 1) == 0)
        {
            if (valorDaTransferencia - Saldo + TaxaDeChequeEspecial < LimiteChequeEspecial)
            {
                Saldo -= valorDaTransferencia + TaxaDeChequeEspecial; // Desconta o valor do saldo da conta do depositante
                LimiteChequeEspecial -= valorDaTransferencia - Saldo + TaxaDeChequeEspecial; // Desconta o montante faltante do limite do cheque especial acrescido da taxa
                
                beneficiario.ReceberTransferencia(valorDaTransferencia, FuncoesDoSistema.ProcurarClientePorCPF(Titular).Nome); // Adiciona o valor ao saldo do beneficiário
                Transacoes.Add(new Transacao(DateTime.Now, "Debito", valorDaTransferencia, $"Transferencia para {FuncoesDoSistema.ProcurarClientePorCPF(beneficiario.Titular).Nome}")); // Adiciona a transação na lista do depositante
                
                FuncoesDoSistema.AtualizarConta(beneficiario); //Atualiza a conta do beneficiário no arquivo JSON
                
                Console.Clear();
                Console.WriteLine($"Transferencia para {FuncoesDoSistema.ProcurarClientePorCPF(beneficiario.Titular).Nome} realizada com sucesso!\n");
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Saldo Insuficiente.\n");
            }
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Erro ao transferir. Verifique o valor inserido ou saldo insuficiente.\n");
        }
    }

    public void ReceberTransferencia(double valor, string depositante)
    {
        if(LimiteChequeEspecial < LimiteChequeEspecialRef && Saldo == 0)
        {
            LimiteChequeEspecial += valor;
            
            if (LimiteChequeEspecial > LimiteChequeEspecialRef)
            {
                Saldo = LimiteChequeEspecial - LimiteChequeEspecialRef;
                LimiteChequeEspecial = LimiteChequeEspecialRef;
            }
        }
        else
        {
            Saldo += valor;
        }

        Transacoes.Add(new Transacao(DateTime.Now, "Credito", valor, $"Transferencia de {depositante}"));
    }
}

public class Transacao
{
    public DateTime Data { get; set; }
    public string Tipo { get; set; }
    public double Valor { get; set; }
    public string Obs { get; set; }

    public Transacao(DateTime data, string tipo, double valor, string obs)
    {
        Data = data;
        Tipo = tipo;
        Valor = valor;
        Obs = obs;
    }

}
