using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

public class Telas
{
    public static void TelaMenu()
    {
        Console.WriteLine("Opção Inválida, tente novamente.");
        bool SistemaEstaLigado = true;

        do
        {
            Console.Clear();
            Console.WriteLine("Seja Bem-vindo ao Banco EnergyMobi\n");
            Console.WriteLine("Escolha a opção desejada:");
            Console.WriteLine("(1) - Entrar na conta\n(2) - Abrir conta\n(0) - Sair do Sistema");

            string opcao = Console.ReadLine();
            Console.Clear();

            if (opcao == "1")
            {
                TelaLogin();
            }
            else if (opcao == "2")
            {
                TelaCriarConta();
            }
            else if (opcao == "0")
            {
                Console.WriteLine("Finalizando o sistema...");
                SistemaEstaLigado = false;
            }
            else
            {   
                Console.Clear();
                Console.WriteLine("Opção inválida, tente novamente.");
            }

        } while (SistemaEstaLigado);
    }

    private  static void TelaCriarConta()
    {
        bool criarContaInputIsValid = true;
        List<Pessoa> listaDePessoas = FuncoesDoSistema.LerArquivoPessoas();

        if (listaDePessoas != null)
        {
            do
            {
                int opcao = -1;

                Console.WriteLine("Selecione a Pessoa de quem deseja criar a conta: ");

                for (int i = 0; i < listaDePessoas.Count; i++) // Lista as pessoas cadastras no arquivo pessoas.json
                {
                    Pessoa pessoa = listaDePessoas[i];
                    Console.WriteLine($"({i}) - Nome: {pessoa.Nome}, Idade: {pessoa.Idade}, CPF: {pessoa.CPF}");
                }

                if (int.TryParse(Console.ReadLine(), out opcao) && opcao >= 0 && opcao < listaDePessoas.Count) // Valida a entrada do usuário
                {   
                    Console.Clear();
                    Cliente clienteAlvo = FuncoesDoSistema.BuscarClientePorNome(listaDePessoas[opcao].Nome); // Procura pelo nome da pessoa escolhida

                    if (FuncoesDoSistema.ValidadorDeIdade(listaDePessoas[opcao]))
                    {
                        Console.WriteLine("Pessoa menor de idade, não é possível criar conta.\n");
                    }
                    else
                    {                    
                        if (clienteAlvo == null) // Null = Pessoa não foi encontrada na lista de Clientes, logo não tem conta no Banco ainda
                        {
                            bool inputSenhaIsValid = true;

                            string senhaDoCliente = "";

                            do
                            {
                                
                                Console.WriteLine("Digite uma senha numérica de 6 digitos para sua conta");

                                string inputSenha = Console.ReadLine();
                                Console.Clear();

                                Regex regex = new Regex(@"^\d{6}$"); // validador 6 digitos numéricos

                                if (regex.IsMatch(inputSenha))
                                {
                                    senhaDoCliente = inputSenha;
                                    inputSenhaIsValid = false;
                                }
                                else
                                {
                                    Console.WriteLine("Senha inválida, tente novamente.");
                                }

                            }while(inputSenhaIsValid);

                            FuncoesDoSistema.CriarCliente(listaDePessoas[opcao], senhaDoCliente); // Cria o cliente selecionado
                            clienteAlvo = FuncoesDoSistema.BuscarClientePorNome(listaDePessoas[opcao].Nome); // Atualiza o clienteAlvo
                        }

                        // clienteAlvo = FuncoesDoSistema.BuscarClientePorNome(listaDePessoas[opcao].Nome); // Atualiza o clienteAlvo

                        if (clienteAlvo.Contas.Count == 0)
                        {
                            Console.WriteLine("Deseja abrir qual tipo de conta?");

                            bool alternativaAbrirContaIsValida = true;

                            do
                            {
                                Console.WriteLine("(1) - Conta Corrente\n(2) - Conta Poupança\n(3) - As duas\n(0) - Voltar");
                                if (int.TryParse(Console.ReadLine(), out int alternativa))
                                {
                                    Console.Clear();
                                    switch (alternativa)
                                    {
                                        case 1:
                                            Console.WriteLine("Conta Corrente selecionada.");
                                            FuncoesDoSistema.CriarContaDoCliente(listaDePessoas[opcao].Nome, "corrente");
                                            break;
                                        case 2:
                                            Console.WriteLine("Conta Poupança selecionada.");
                                            FuncoesDoSistema.CriarContaDoCliente(listaDePessoas[opcao].Nome, "poupanca");
                                            break;
                                        case 3:
                                            Console.WriteLine("As duas foram selecionadas.");
                                            FuncoesDoSistema.CriarContaDoCliente(listaDePessoas[opcao].Nome, "corrente&poupanca");
                                            break;
                                        case 0:
                                            Console.WriteLine("Voltar selecionado.");
                                            break;
                                        default:
                                            Console.WriteLine("Opção inválida. Por favor, escolha uma opção válida.");
                                            break;
                                    }
                                    alternativaAbrirContaIsValida = false;
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("Entrada inválida. Por favor, insira uma alternativa válida.");
                                }
                            } while (alternativaAbrirContaIsValida);
                            
                            criarContaInputIsValid = false;
                        }
                        else if (clienteAlvo.Contas.Count == 2) // Se possuir 2 contas não abre a opção de criação
                        {
                            Console.WriteLine("Cliente já possui uma Conta Corrente e uma Conta Poupança.\n");
                            criarContaInputIsValid = false; // Encerra a tela de Criação de Contas
                        }
                        else if (clienteAlvo.Contas.Count < 2)
                        {
                            Regex regexCorrente = new Regex(@"^\d{4}-0$"); // Regex Conta Corrente
                            Regex regexPoupanca = new Regex(@"^\d{4}-1$"); // Regex Conta Poupanca

                            if (regexCorrente.IsMatch(clienteAlvo.Contas[0])) // Verifica no array de contas do Cliente pelo digito verificador se é Corrente ou poupança
                            {
                                Console.WriteLine("Deseja abrir qual tipo de conta?");

                                bool alternativaAbrirContaIsValida = true;

                                do
                                {
                                    Console.WriteLine("(1) - Conta Poupança\n(0) - Voltar");
                                    if (int.TryParse(Console.ReadLine(), out int alternativa))
                                    {
                                        switch (alternativa)
                                        {
                                            case 1:
                                                Console.WriteLine("Conta Poupança selecionada.");
                                                FuncoesDoSistema.CriarContaDoCliente(listaDePessoas[opcao].Nome, "poupanca");
                                                break;
                                            case 0:
                                                Console.WriteLine("Voltar selecionado.");
                                                break;
                                            default:
                                                Console.WriteLine("Opção inválida. Por favor, escolha uma opção válida.");
                                                break;
                                        }
                                        alternativaAbrirContaIsValida = false;
                                        criarContaInputIsValid = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Entrada inválida. Por favor, insira uma alternativa válida.");
                                    }
                                } while (alternativaAbrirContaIsValida);
                            }
                            else if (regexPoupanca.IsMatch(clienteAlvo.Contas[0]))
                            {
                                Console.WriteLine("Deseja abrir qual tipo de conta?");

                                bool alternativaAbrirContaIsValida = true;

                                do
                                {
                                    Console.WriteLine("(1) - Conta Corrente\n(0) - Voltar");
                                    if (int.TryParse(Console.ReadLine(), out int alternativa))
                                    {
                                        switch (alternativa)
                                        {
                                            case 1:
                                            Console.WriteLine("Conta Corrente selecionada.");
                                            FuncoesDoSistema.CriarContaDoCliente(listaDePessoas[opcao].Nome, "corrente");
                                            break;
                                            case 0:
                                                Console.WriteLine("Voltar selecionado.");
                                                break;
                                            default:
                                                Console.WriteLine("Opção inválida. Por favor, escolha uma opção válida.");
                                                break;
                                        }
                                        alternativaAbrirContaIsValida = false;
                                        criarContaInputIsValid = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Entrada inválida. Por favor, insira uma alternativa válida.");
                                    }
                                } while (alternativaAbrirContaIsValida);
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Opção Inválida, tente novamente.");
                        }
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Opção Inválida, tente novamente.\n");
                }
            } while (criarContaInputIsValid);
        }
        else
        {
            Console.WriteLine("Não há pessoas válidas para criação de contas. Voltando para o Menu Inicial...");
            Thread.Sleep(2000); // Para simular o carregamento do sistema
        }
    }

    private static void TelaLogin()
    {
        bool cpfIsValid = true;
        bool senhaIsValid = true;

        do
        {
            Console.WriteLine("Digite o CPF do Cliente com no formato ***.***.***-** ou apenas 0 para voltar:");
            string cpf = Console.ReadLine();
            Console.Clear();
            
            if (FuncoesDoSistema.ValidarExistenciaDeContaPorCPF(cpf))
            {
                Cliente usuario = FuncoesDoSistema.ProcurarClientePorCPF(cpf);
                do
                {
                    Console.WriteLine("Digite a senha númerica de 6 digitos:");
                    string senha = Console.ReadLine();
                    Console.Clear();

                    if (senha.Equals(usuario.Senha))
                    {
                        TelaAcessoCliente(usuario);
                        senhaIsValid = false;
                    }
                    else if (senha.Equals("0"))
                    {
                        senhaIsValid = false;
                    }
                    else
                    {
                        Console.WriteLine("Senha inválida, tente Novamente ou Digite 0 para voltar\n");
                    }
                }while(senhaIsValid);
                cpfIsValid = false;
            }
            else if(cpf.Equals("0"))
            {
                cpfIsValid = false;
            }
            else
            {
                Console.WriteLine("CPF Inválido, tente novamente. Caso deseje voltar, digite apenas 0\n");
            }

        }while(cpfIsValid);

    }

    private static void TelaAcessoCliente(Cliente cliente)
    {
        Console.WriteLine($"Seja Bem-Vindo sr(a) {cliente.Nome}\n");

        if (cliente.Contas.Count == 2)
        {
            bool opcaoIsValid = true;

            Console.WriteLine("Deseja acessar qual conta?:");

            do
            {
                Console.WriteLine("(0) - Corrente\n(1) - Poupanca");

                string opcao = Console.ReadLine();
                Console.Clear();
                if (opcao.Equals("0"))
                {
                    TelaContaCorrente(FuncoesDoSistema.ProcurarContaCorrentePorCPF(cliente.CPF), cliente.Nome);
                    opcaoIsValid = false;
                }
                else if(opcao.Equals("1"))
                {
                    TelaContaPoupanca(FuncoesDoSistema.ProcurarContaPoupancaPorCPF(cliente.CPF), cliente.Nome);
                    opcaoIsValid = false;
                }
                else
                {
                    Console.WriteLine("Opção inválida! Tente novamente.");
                }
            }while(opcaoIsValid);
        }
        else
        {
            Regex regexCorrente = new Regex(@"^\d{4}-0$"); // Regex Conta Corrente

            if (regexCorrente.IsMatch(cliente.Contas[0]))
            {
                TelaContaCorrente(FuncoesDoSistema.ProcurarContaCorrentePorCPF(cliente.CPF), cliente.Nome);
            }
            else
            {
                TelaContaPoupanca(FuncoesDoSistema.ProcurarContaPoupancaPorCPF(cliente.CPF), cliente.Nome);
            }
        }
    }

    private static void TelaContaCorrente(ContaCorrente contaCorrente, string titularNome)
    {
        ContaCorrente contaAtualizada = contaCorrente;
        bool opcaoIsValid = true;

        do
        {
            Console.WriteLine($"Conta Corrente de {titularNome}:");
            Console.WriteLine("(1) - Saldo\n(2) - Sacar\n(3) - Depositar\n(4) - Transferir\n(0) - Sair");
            if (int.TryParse(Console.ReadLine(), out int opcao) && opcao >= 0 && opcao <= 4)
            {
                Console.Clear();
                switch (opcao)
                {
                    case 1:
                        contaAtualizada = FuncoesDoSistema.ProcurarContaCorrentePorCPF(contaAtualizada.Titular);
                        contaAtualizada.ImprimirSaldo();
                        break;
                    case 2:
                        contaAtualizada = FuncoesDoSistema.ProcurarContaCorrentePorCPF(contaAtualizada.Titular);
                        bool valorIsValid = true;
                        do
                        {
                            Console.WriteLine("Qual valor deseja sacar?");
                            if (double.TryParse(Console.ReadLine(), out double valorSaque) && valorSaque > 0)
                            {
                                Console.Clear();
                                contaAtualizada.Sacar(valorSaque);
                                FuncoesDoSistema.AtualizarConta(contaAtualizada);
                                valorIsValid = false;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Valor inválido, tente novamente.");
                            }
                        }while(valorIsValid);
                        break;
                    case 3:
                        Console.WriteLine("Qual valor deseja depositar?");
                        if (double.TryParse(Console.ReadLine(), out double valorDeposito) && valorDeposito > 0)
                        {
                            Console.Clear();
                            contaAtualizada.Depositar(valorDeposito);
                            FuncoesDoSistema.AtualizarConta(contaAtualizada);
                            break;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Valor inválido, tente novamente.");
                        }
                        break;
                    case 4:
                        Console.Clear();
                        Transferencia(contaAtualizada);
                        break;
                    case 0:
                        Console.Clear();
                        opcaoIsValid = false;
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Opção inválida. Por favor, escolha uma opção válida.");
                        break;
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Opção inválida, tente novamente.");
            }
        }while(opcaoIsValid);
    }

    private static void TelaContaPoupanca(ContaPoupanca contaPoupanca, string titularNome)
    {
        ContaPoupanca contaAtualizada = contaPoupanca;
        bool opcaoIsValid = true;

        do
        {
            Console.WriteLine($"Conta Poupanca de {titularNome}:");
            Console.WriteLine(
            "(1) - Saldo\n(2) - Sacar\n(3) - Depositar\n(4) - Transferir\n(0) - Sair");
            if (int.TryParse(Console.ReadLine(), out int opcao) && opcao >= 0 && opcao <= 4)
            {
                switch (opcao)
                {
                    case 1:
                        contaAtualizada = FuncoesDoSistema.ProcurarContaPoupancaPorCPF(contaAtualizada.Titular);
                        contaAtualizada.ImprimirSaldo();
                        break;
                    case 2:
                        contaAtualizada = FuncoesDoSistema.ProcurarContaPoupancaPorCPF(contaAtualizada.Titular);
                        bool valorIsValid = true;
                        do
                        {
                            Console.Clear();
                            Console.WriteLine("Qual valor deseja sacar?");
                            if (double.TryParse(Console.ReadLine(), out double valorSaque) && valorSaque > 0)
                            {
                                
                                Console.Clear();
                                contaAtualizada.Sacar(valorSaque);
                                FuncoesDoSistema.AtualizarConta(contaAtualizada);
                                valorIsValid = false;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Valor inválido, tente novamente.\n");
                            }
                        }while(valorIsValid);
                        break;
                    case 3:
                        Console.Clear();
                        Console.WriteLine("Qual valor deseja depositar?");
                        if (double.TryParse(Console.ReadLine(), out double valorDeposito) && valorDeposito > 0)
                        {
                            contaAtualizada.Depositar(valorDeposito);
                            FuncoesDoSistema.AtualizarConta(contaAtualizada);
                            break;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Valor inválido, tente novamente.\n");
                        }
                        break;
                    case 4:
                        Console.Clear();
                        Transferencia(contaAtualizada);
                        break;
                    case 0:
                        opcaoIsValid = false;
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Opção inválida. Por favor, escolha uma opção válida.");
                        break;
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Opção inválida, tente novamente.");
            }
        }while(opcaoIsValid);
    }

    private static void Transferencia(ContaCorrente contaCorrente)
    {
        Cliente cliente = FuncoesDoSistema.ProcurarClientePorCPF(contaCorrente.Titular);

        if(cliente.Contas.Count == 2)
        {
            bool entryIsValid = true;
            do
            {
                Console.WriteLine("Deseja Transferir para sua poupança ou para outra conta?");
                Console.WriteLine("(1) - Poupança\n(2) - Outra conta\n(0) - Voltar");
                if (int.TryParse(Console.ReadLine(), out int opcao) && opcao >= 0 && opcao < 3)
                {
                    switch (opcao)
                    {
                        case 0:
                            Console.Clear();
                            entryIsValid = false;
                            break;
                        case 1:
                            bool valorIsValid = true;
                            do
                            {
                                Console.Clear();
                                Console.WriteLine("Digite o valor que deseja transferir:");
                                if(double.TryParse(Console.ReadLine(), out double valor) && valor > 0)
                                {
                                    contaCorrente.TransferirParaPoupança(valor);
                                    FuncoesDoSistema.AtualizarConta(contaCorrente);
                                    valorIsValid = false;
                                    entryIsValid = false;
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("Valor inválido, tente novamente.\n");
                                }
                            } while (valorIsValid);
                            break;
                        case 2:
                            valorIsValid = true;
                            do
                            {
                                Regex regexCorrente = new Regex(@"^\d{4}-0$"); // Regex Conta Corrente
                                Regex regexPoupanca = new Regex(@"^\d{4}-1$"); // Regex Conta Poupanca
                                
                                Console.Clear();
                                Console.WriteLine("Digite o número da conta com o dígito verificador no formato ****-* ou 0 para voltar:");
                                string contaBeneficiaria = Console.ReadLine();

                                if (contaBeneficiaria.Equals("0"))
                                {
                                    valorIsValid = false;
                                    entryIsValid = false;
                                }
                                else if (contaBeneficiaria.Equals(contaCorrente.NumeroConta))
                                {   
                                    Console.Clear();
                                    Console.WriteLine("Você não pode transferir para a mesma conta.\n");
                                }
                                else if(regexCorrente.IsMatch(contaBeneficiaria)) // Lógica para Conta Corrente
                                {
                                    ContaCorrente contaAlvo = FuncoesDoSistema.BuscarContaCorrentePorNumConta(contaBeneficiaria);
                                    if (contaAlvo != null)
                                    {
                                        do
                                        {
                                            Console.WriteLine($"Digite o valor que deseja transferir para {FuncoesDoSistema.ProcurarClientePorCPF(contaAlvo.Titular).Nome}:");
                                            if(double.TryParse(Console.ReadLine(), out double valor) && valor > 0 && Math.Abs(valor * 100 % 1) == 0)
                                            {
                                                contaCorrente.TransferenciaBancaria(contaAlvo, valor);
                                                FuncoesDoSistema.AtualizarConta(contaCorrente);
                                                valorIsValid = false;
                                                entryIsValid = false;
                                            }
                                            else
                                            {   
                                                Console.Clear();
                                                Console.WriteLine("Valor inválido, tente novamente.\n");
                                            }
                                        } while (valorIsValid);
                                    }
                                    else
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Conta não encontrada, tente novamente.\n");
                                    }
                                }
                                else if(regexPoupanca.IsMatch(contaBeneficiaria)) // Lógica para Conta Poupança
                                {
                                    ContaPoupanca contaAlvo = FuncoesDoSistema.BuscarContaPoupancaPorNumConta(contaBeneficiaria);
                                    if (contaAlvo != null)
                                    {
                                        do
                                        {
                                            Console.Clear();
                                            Console.WriteLine($"Digite o valor que deseja transferir para {FuncoesDoSistema.ProcurarClientePorCPF(contaAlvo.Titular).Nome}:");
                                            if(double.TryParse(Console.ReadLine(), out double valor) && valor > 0)
                                            {
                                                contaCorrente.TransferenciaBancaria(contaAlvo, valor);
                                                FuncoesDoSistema.AtualizarConta(contaCorrente);
                                                valorIsValid = false;
                                            }
                                            else
                                            {
                                                Console.Clear();
                                                Console.WriteLine("Valor inválido, tente novamente.\n");
                                            }
                                        } while (valorIsValid);
                                    }
                                    else
                                    {
                                        
                                        Console.Clear();
                                        Console.WriteLine("Conta não encontrada, tente novamente.\n");
                                    }
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("Entrada inválida ou conta não encontrada, tente novamente.\n");
                                }
                            } while (valorIsValid);
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine("Opção inválida. Tente novamente.\n");
                            break;
                    }
                }
            }while(entryIsValid);
        }
        else
        {
            bool valorIsValid = true;
            do
            {
                Regex regexCorrente = new Regex(@"^\d{4}-0$"); // Regex Conta Corrente
                Regex regexPoupanca = new Regex(@"^\d{4}-1$"); // Regex Conta Poupanca

                Console.WriteLine("Digite o número da conta com o dígito verificador no formato ****-* ou 0 para voltar:");
                string contaBeneficiaria = Console.ReadLine();

                if (contaBeneficiaria.Equals("0"))
                {
                    valorIsValid = false;
                }
                else if (contaBeneficiaria.Equals(contaCorrente.NumeroConta))
                {
                    Console.Clear();
                    Console.WriteLine("Você não pode transferir para a mesma conta.\n");
                }
                else if(regexCorrente.IsMatch(contaBeneficiaria)) // Lógica para Conta Corrente
                {
                    ContaCorrente contaAlvo = FuncoesDoSistema.BuscarContaCorrentePorNumConta(contaBeneficiaria);
                    if (contaAlvo != null)
                    {
                        do
                        {
                            Console.Clear();
                            Console.WriteLine($"Digite o valor que deseja transferir para {FuncoesDoSistema.ProcurarClientePorCPF(contaAlvo.Titular).Nome}:");
                            if(double.TryParse(Console.ReadLine(), out double valor) && valor > 0)
                            {
                                contaCorrente.TransferenciaBancaria(contaAlvo, valor);
                                FuncoesDoSistema.AtualizarConta(contaCorrente);
                                valorIsValid = false;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Valor inválido, tente novamente.\n");
                            }
                        } while (valorIsValid);
                    }
                    else
                    {
                        Console.WriteLine("Conta não encontrada, tente novamente.");
                    }
                }
                else if(regexPoupanca.IsMatch(contaBeneficiaria)) // Lógica para Conta Poupança
                {
                    ContaPoupanca contaAlvo = FuncoesDoSistema.BuscarContaPoupancaPorNumConta(contaBeneficiaria);
                    if (contaAlvo != null)
                    {
                        do
                        {
                            Console.Clear();
                            Console.WriteLine($"Digite o valor que deseja transferir para {FuncoesDoSistema.ProcurarClientePorCPF(contaAlvo.Titular).Nome}:");
                            if(double.TryParse(Console.ReadLine(), out double valor) && valor > 0)
                            {
                                contaCorrente.TransferenciaBancaria(contaAlvo, valor);
                                FuncoesDoSistema.AtualizarConta(contaCorrente);
                                valorIsValid = false;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Valor inválido, tente novamente.\n");
                            }
                        } while (valorIsValid);
                    }
                    else
                    {
                        Console.WriteLine("Conta não encontrada, tente novamente.");
                    }
                }
                else
                {
                    Console.WriteLine("Entrada inválida ou conta não encontrada, tente novamente.");
                }
            } while (valorIsValid);
        }
    }
    private static void Transferencia(ContaPoupanca contaPoupanca)
    {
        Cliente cliente = FuncoesDoSistema.ProcurarClientePorCPF(contaPoupanca.Titular);

        if(cliente.Contas.Count == 2)
        {   
            bool entryIsValid = true;
            do
            {
                Console.WriteLine("Deseja Transferir para sua conta corrente ou para outra conta?");
                Console.WriteLine("(1) - Corrente\n(2) - Outra conta\n(0) - Voltar");
                if (int.TryParse(Console.ReadLine(), out int opcao) && opcao >= 0 && opcao < 3)
                {
                    switch (opcao)
                    {   
                        case 0:
                            Console.Clear();
                            entryIsValid = false;
                            break;
                        case 1:
                            bool valorIsValid = true;
                            do
                            {
                                Console.Clear();
                                Console.WriteLine("Digite o valor que deseja transferir:");
                                if(double.TryParse(Console.ReadLine(), out double valor) && valor > 0)
                                {
                                    contaPoupanca.TransferirParaCorrente(valor);
                                    FuncoesDoSistema.AtualizarConta(contaPoupanca);
                                    valorIsValid = false;
                                    entryIsValid = false;
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("Valor inválido, tente novamente.\n");
                                }
                            } while (valorIsValid);
                            break;
                        case 2:
                            valorIsValid = true;
                            do
                            {
                                Regex regexCorrente = new Regex(@"^\d{4}-0$"); // Regex Conta Corrente
                                Regex regexPoupanca = new Regex(@"^\d{4}-1$"); // Regex Conta Poupanca

                                Console.Clear();
                                Console.WriteLine("Digite o número da conta com o dígito verificador no formato ****-* ou 0 para voltar:");
                                string contaBeneficiaria = Console.ReadLine();

                                if (contaBeneficiaria.Equals("0"))
                                {
                                    valorIsValid = false;
                                }
                                else if (contaBeneficiaria.Equals(contaPoupanca.NumeroConta))
                                {
                                    Console.Clear();
                                    Console.WriteLine("Você não pode transferir para a mesma conta.\n");
                                }
                                else if (regexCorrente.IsMatch(contaBeneficiaria)) // Lógica para Conta Corrente
                                {
                                    ContaCorrente contaAlvo = FuncoesDoSistema.BuscarContaCorrentePorNumConta(contaBeneficiaria);
                                    if (contaAlvo != null)
                                    {
                                        do
                                        {
                                            Console.WriteLine($"Digite o valor que deseja transferir para {FuncoesDoSistema.ProcurarClientePorCPF(contaAlvo.Titular).Nome}:");
                                            if(double.TryParse(Console.ReadLine(), out double valor) && valor > 0)
                                            {
                                                contaPoupanca.TransferenciaBancaria(contaAlvo, valor);
                                                FuncoesDoSistema.AtualizarConta(contaPoupanca);
                                                valorIsValid = false;
                                                entryIsValid = false;
                                            }
                                            else
                                            {
                                                Console.Clear();
                                                Console.WriteLine("Valor inválido, tente novamente.\n");
                                            }
                                        } while (valorIsValid);
                                    }
                                    else
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Conta não encontrada, tente novamente.");
                                    }
                                }
                                else if(regexPoupanca.IsMatch(contaBeneficiaria)) // Lógica para Conta Poupança
                                {
                                    ContaPoupanca contaAlvo = FuncoesDoSistema.BuscarContaPoupancaPorNumConta(contaBeneficiaria);
                                    if (contaAlvo != null)
                                    {
                                        do
                                        {
                                            Console.Clear();
                                            Console.WriteLine($"Digite o valor que deseja transferir para {FuncoesDoSistema.ProcurarClientePorCPF(contaAlvo.Titular).Nome}:");
                                            if(double.TryParse(Console.ReadLine(), out double valor) && valor > 0)
                                            {
                                                contaPoupanca.TransferenciaBancaria(contaAlvo, valor);
                                                FuncoesDoSistema.AtualizarConta(contaPoupanca);
                                                valorIsValid = false;
                                            }
                                            else
                                            {
                                                Console.Clear();
                                                Console.WriteLine("Valor inválido, tente novamente.\n");
                                            }
                                        } while (valorIsValid);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Conta não encontrada, tente novamente.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Entrada inválida ou conta não encontrada, tente novamente.");
                                }
                            } while (valorIsValid);
                            break;
                        default:
                            Console.WriteLine("Opção inválida. Tente novamente.");
                            break;
                    }
                }
            } while (entryIsValid);
        }
        else
        {
            bool valorIsValid = true;
            do
            {
                Regex regexCorrente = new Regex(@"^\d{4}-0$"); // Regex Conta Corrente
                Regex regexPoupanca = new Regex(@"^\d{4}-1$"); // Regex Conta Poupanca

                Console.WriteLine("Digite o número da conta com o dígito verificador no formato ****-* ou 0 para voltar:");
                string contaBeneficiaria = Console.ReadLine();

                if (contaBeneficiaria.Equals("0"))
                {
                    valorIsValid = false;
                }
                else if (contaBeneficiaria.Equals(contaPoupanca.NumeroConta))
                {
                    Console.Clear();
                    Console.WriteLine("Você não pode transferir para a mesma conta.\n");
                }
                else if(regexCorrente.IsMatch(contaBeneficiaria)) // Lógica para Conta Corrente
                {
                    ContaPoupanca contaAlvo = FuncoesDoSistema.BuscarContaPoupancaPorNumConta(contaBeneficiaria);
                    if (contaAlvo != null)
                    {
                        do
                        {
                            Console.Clear();
                            Console.WriteLine($"Digite o valor que deseja transferir para {FuncoesDoSistema.ProcurarClientePorCPF(contaAlvo.Titular).Nome}:");
                            if(double.TryParse(Console.ReadLine(), out double valor) && valor > 0)
                            {
                                contaPoupanca.TransferenciaBancaria(contaAlvo, valor);
                                FuncoesDoSistema.AtualizarConta(contaPoupanca);
                                valorIsValid = false;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Valor inválido, tente novamente.\n");
                            }
                        } while (valorIsValid);
                    }
                    else
                    {
                        Console.WriteLine("Conta não encontrada, tente novamente.");
                    }
                }
                else if(regexPoupanca.IsMatch(contaBeneficiaria)) // Lógica para Conta Poupança
                {
                    ContaPoupanca contaAlvo = FuncoesDoSistema.BuscarContaPoupancaPorNumConta(contaBeneficiaria);
                    if (contaAlvo != null)
                    {
                        do
                        {
                            Console.Clear();
                            Console.WriteLine($"Digite o valor que deseja transferir para {FuncoesDoSistema.ProcurarClientePorCPF(contaAlvo.Titular).Nome}:");
                            if(double.TryParse(Console.ReadLine(), out double valor) && valor > 0)
                            {
                                contaPoupanca.TransferenciaBancaria(contaAlvo, valor);
                                FuncoesDoSistema.AtualizarConta(contaPoupanca);
                                valorIsValid = false;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Valor inválido, tente novamente.\n");
                            }
                        } while (valorIsValid);
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Conta não encontrada, tente novamente.\n");
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Entrada inválida ou conta não encontrada, tente novamente.\n");
                }
            } while (valorIsValid);
        }
    }
}
