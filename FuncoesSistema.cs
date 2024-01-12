using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class FuncoesDoSistema
{
    private static void AddRmvContasNoCliente(string nomeCliente, string operacao, string numConta)
    {
        try
        {
            Cliente clienteAtualizado = BuscarClientePorNome(nomeCliente); // Busca o cliente no arquivo de clientes
            List<Cliente> listaDeClientes = LerArquivoClientes(); // Carrega a lista de clientes do arquivo

            if (operacao.ToLower() == "adicionar" && listaDeClientes != null)
            {
                for (int i = 0; i < listaDeClientes.Count; i++) // Percorre a lista de clientes
                {
                    if (listaDeClientes[i].Nome == nomeCliente) // Acha o cliente pelo nome
                    {
                        listaDeClientes[i].Contas.Add(numConta); // Adiciona o número da conta ao Array de contas
                    }
                }
            }
            if (operacao.ToLower() == "remover" && listaDeClientes != null)
            {
                for (int i = 0; i < listaDeClientes.Count; i++) // Percorre a lista de clientes
                {
                    if (listaDeClientes[i].Nome == nomeCliente) // Acha o cliente pelo nome
                    {
                        listaDeClientes[i].Contas.Remove(numConta); // Remove a conta escolhida pelo número
                    }
                }
            }

            AtualizarArquivoClientes(listaDeClientes);
        }
        catch (JsonException e)
        {
            Console.WriteLine($"Ocorreu um erro na desserialização do JSON: {e.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro: {ex.Message}");
        }
    }

    private static void AtualizarArquivoClientes(List<Cliente> listaDeClientesAtualizada)
    {
        string clientesPath = Path.Combine("Data", "Clientes", "clientes.json");

        try
        {
            string jsonClientes = JsonSerializer.Serialize(listaDeClientesAtualizada, new JsonSerializerOptions { WriteIndented = true }); // Serializa a lista de clientes para JSON

            File.WriteAllText(clientesPath, jsonClientes); // Escreve o JSON no arquivo
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro ao escrever no arquivo JSON: {ex.Message}");
        }
    }

    public static void AtualizarConta(ContaCorrente contaAtualizada)
    {
        string contasJSONPath = Path.Combine("Data", "Contas", "contasCorrentes.json");
        List<ContaCorrente> listaBase = LerArquivoContaCorrente();
        
        for (int i = 0; i < listaBase.Count; i++)
        {
            if (listaBase[i].Titular == contaAtualizada.Titular)
            {
                listaBase[i] = contaAtualizada;
            }
        }

        try
        {
            string jsonString = JsonSerializer.Serialize(listaBase, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(contasJSONPath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro ao salvar as contas correntes: {ex.Message}");
        }
    }

    public static void AtualizarConta(ContaPoupanca contaAtualizada)
    {
        string contasJSONPath = Path.Combine("Data", "Contas", "contasPoupancas.json");
        List<ContaPoupanca> listaBase = LerArquivoContaPoupanca();
        
        for (int i = 0; i < listaBase.Count; i++)
        {
            if (listaBase[i].Titular == contaAtualizada.Titular)
            {
                listaBase[i] = contaAtualizada;
            }
        }

        try
        {
            string jsonString = JsonSerializer.Serialize(listaBase, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(contasJSONPath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro ao salvar as contas correntes: {ex.Message}");
        }
    }

    public static Cliente BuscarClientePorNome(string nomeDoCliente)
    {
        List<Cliente> clientes = LerArquivoClientes();

        if (clientes.Count > 0)
        {
            for (int i = 0; i < clientes.Count(); i++)
            {
                if (clientes[i].Nome == nomeDoCliente)
                {
                    return clientes[i];
                }
            }
        }

        return null;
    }

    public static ContaCorrente BuscarContaCorrentePorNumConta(string numConta)
    {
        List<ContaCorrente> listaContaCorrente = LerArquivoContaCorrente();

        foreach (var conta in listaContaCorrente)
        {
            if (conta.NumeroConta == numConta)
            {
                return conta;
            }
        }
        
        return null;
    }
    
    public static ContaPoupanca BuscarContaPoupancaPorNumConta(string numConta)
    {
        List<ContaPoupanca> listaContaPoupanca = LerArquivoContaPoupanca();

        foreach (var conta in listaContaPoupanca)
        {
            if (conta.NumeroConta == numConta)
            {
                return conta;
            }
        }
        
        return null;
    }

    public static string BuscarNumeroContaPoupancaPorCPF(string titular)
    {
        Regex regexPoupanca = new Regex(@"^\d{4}-1$"); // Regex Conta Poupanca
        Cliente cliente = ProcurarClientePorCPF(titular);

        foreach (var conta in cliente.Contas)
        {
            if (regexPoupanca.IsMatch(conta))
            {
                return conta;
            }
        }

        return "";
    }

    public static void CriarCliente(Pessoa pessoaACadastrar, string senhaDoCliente)
    {
        string clientesJSONPath = Path.Combine("Data", "Clientes", "clientes.json");

        Cliente novoCliente = new Cliente(pessoaACadastrar.Nome, pessoaACadastrar.Idade, pessoaACadastrar.CPF, senhaDoCliente, new List<string>()); //Cria o objeto Cliente com os dados da Pessoa selecionada

        if (File.Exists(clientesJSONPath) && new FileInfo(clientesJSONPath).Length > 0) // Verifica se o arquivo existe
        {
            try
            {
                string conteudoAtual = File.ReadAllText(clientesJSONPath); // Lê o conteúdo atual do arquivo

                List<Cliente> listaDeClientes = JsonSerializer.Deserialize<List<Cliente>>(conteudoAtual); // Salva o conteúdo em uma lista deserializando a string JSON

                listaDeClientes.Add(novoCliente); // Adiciona novoCliente ao final do conteúdo existente

                string jsonString = JsonSerializer.Serialize(listaDeClientes, new JsonSerializerOptions { WriteIndented = true }); // Transforma a lista atualizada em uma string JSON

                File.WriteAllText(clientesJSONPath, jsonString); // Grava o conteúdo atualizado de volta no arquivo
            }
            catch (JsonException e)
            {
                Console.WriteLine($"Ocorreu um erro na desserialização do JSON: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }
        else
        {
            List<Cliente> listaDeClientes = new()
            {
                novoCliente
            };

            string jsonString = JsonSerializer.Serialize(listaDeClientes, new JsonSerializerOptions { WriteIndented = true }); // Transforma a lista em uma string
            File.WriteAllText(clientesJSONPath, jsonString); // Se o arquivo não existe, cria um novo arquivo e escreve novoCliente nele
        }
    }

    public static void CriarContaDoCliente(string nomeDoCliente, string tipoDeConta)
    {
        try
        {
            if (tipoDeConta.ToLower().Equals("corrente"))
            {
                string numConta = GerarNumeroConta("corrente"); // Gera o número da conta
                string CPF = BuscarClientePorNome(nomeDoCliente).CPF; // Busca o Cliente e pega o valor de CPF

                ContaCorrente novaContaCorrente = new(numConta, CPF, 0.0, 10.0);
                AddRmvContasNoCliente(nomeDoCliente, "adicionar", numConta); // Adiciona o número da conta ao Array Contas no obj Cliente

                CriarContaCorrente(novaContaCorrente);
            }
            else if (tipoDeConta.ToLower().Equals("poupanca"))
            {
                string numConta = GerarNumeroConta("poupanca");

                string CPF = BuscarClientePorNome(nomeDoCliente).CPF;

                ContaPoupanca novaContaPoupanca = new(numConta, CPF, 0.0, 1);
                AddRmvContasNoCliente(nomeDoCliente, "adicionar", numConta); // Adiciona o número da conta ao Array Contas no obj Cliente

                CriarContaPoupanca(novaContaPoupanca);
            }
            else if (tipoDeConta.ToLower().Equals("corrente&poupanca"))
            {
                string numConta = GerarNumeroConta("corrente"); // Gera o número da conta
                string CPF = BuscarClientePorNome(nomeDoCliente).CPF; // Busca o Cliente e pega o valor de CPF

                ContaCorrente novaContaCorrente = new(numConta, CPF, 0.0, 10.0);
                CriarContaCorrente(novaContaCorrente);
                AddRmvContasNoCliente(nomeDoCliente, "adicionar", numConta); // Adiciona o número da conta ao Array Contas no obj Cliente

                string numConta2 = GerarNumeroConta("poupanca");

                ContaPoupanca novaContaPoupanca = new(numConta2, CPF, 0.0, 1);
                CriarContaPoupanca(novaContaPoupanca);
                AddRmvContasNoCliente(nomeDoCliente, "adicionar", numConta2); // Adiciona o número da conta ao Array Contas no obj Cliente
            }
            else
            {
                Console.WriteLine("Tipo de conta inválida.");
            }
        }
        catch (JsonException e)
        {
            Console.WriteLine($"Ocorreu um erro na desserialização do JSON: {e.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro: {ex.Message}");
        }
    }

    private static void CriarContaCorrente(ContaCorrente novaContaCorrente)
    {
        string contasJSONPath = Path.Combine("Data", "Contas", "contasCorrentes.json");
        List<ContaCorrente> listaDeContas = LerArquivoContaCorrente();
        listaDeContas.Add(novaContaCorrente);

        try
        {
            string jsonString = JsonSerializer.Serialize(listaDeContas, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(contasJSONPath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro ao salvar as contas correntes: {ex.Message}");
        }
    }

    private static void CriarContaPoupanca(ContaPoupanca novaContaPoupanca)
    {
        string contasJSONPath = Path.Combine("Data", "Contas", "contasPoupancas.json");
        List<ContaPoupanca> listaDeContas = LerArquivoContaPoupanca();
        listaDeContas.Add(novaContaPoupanca);

        try
        {
            string jsonString = JsonSerializer.Serialize(listaDeContas, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(contasJSONPath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro ao salvar as contas poupancas: {ex.Message}");
        }
    }

    private static string GerarNumeroConta(string tipoDeConta)
    {   
        bool contaIsValid = true;
    
        do
        {
            if (tipoDeConta.ToLower().Equals("corrente"))
            {
                Random random = new Random();
                int aleatNum = random.Next(1000, 9999);
                
                string conta = aleatNum+"-0";

                if (!ProcurarDuplicidadeDeConta("corrente", conta))
                {
                    return conta;
                }
            }

            if (tipoDeConta.ToLower().Equals("poupanca"))
            {
                Random random = new Random();
                int aleatNum = random.Next(1000, 9999);
                
                string conta = aleatNum+"-1";

                if (!ProcurarDuplicidadeDeConta("corrente", conta))
                {
                    return conta;
                }
            }

        }while(contaIsValid);

        return "";
    }

    public static List<Pessoa> LerArquivoPessoas()
    {
        string pessoasPath = Path.Combine("Data", "Pessoas", "pessoas.json");

        if (File.Exists(pessoasPath))
        {
            try
            {
                string jsonString = File.ReadAllText(pessoasPath);
                List<Pessoa> listaDePessoas = JsonSerializer.Deserialize<List<Pessoa>>(jsonString);

                if (listaDePessoas != null)
                {
                    return listaDePessoas;
                }
            }
            catch (JsonException e)
            {
                Console.WriteLine($"Ocorreu um erro na desserialização do JSON: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("O arquivo não foi encontrado.");
        }

        return new List<Pessoa>();
    }

    private static List<Cliente> LerArquivoClientes()
    {
        string clientesPath = Path.Combine("Data", "Clientes", "clientes.json");

        if (File.Exists(clientesPath))
        {
            try
            {
                string jsonString = File.ReadAllText(clientesPath);

                if (string.IsNullOrEmpty(jsonString))
                {
                    return new List<Cliente>();
                }
                else
                {
                    List<Cliente> listaDeClientes = JsonSerializer.Deserialize<List<Cliente>>(jsonString);

                    return listaDeClientes;
                }
            }
            catch (JsonException e)
            {
                Console.WriteLine($"Ocorreu um erro na desserialização do JSON: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("O arquivo não foi encontrado.");
        }

        return new List<Cliente>();
    }

    private static List<ContaCorrente> LerArquivoContaCorrente()
    {
        string contasCPath = Path.Combine("Data", "Contas", "contasCorrentes.json");

        try
        {
            string jsonString = File.ReadAllText(contasCPath);

            if (jsonString != "")
            {
                List<ContaCorrente> contas = JsonSerializer.Deserialize<List<ContaCorrente>>(jsonString);
                return contas;
            }
            else
            {
                return new List<ContaCorrente>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro ao ler as contas correntes: {ex.Message}");
            return new List<ContaCorrente>();
        }
    }

    private static List<ContaPoupanca> LerArquivoContaPoupanca()
    {
        string contasCPath = Path.Combine("Data", "Contas", "contasPoupancas.json");

        try
        {
            string jsonString = File.ReadAllText(contasCPath);
    
            if (jsonString != "")
            {
                List<ContaPoupanca> contas = JsonSerializer.Deserialize<List<ContaPoupanca>>(jsonString);
                return contas;
            }
            else
            {
                return new List<ContaPoupanca>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro ao ler as contas correntes: {ex.Message}");
            return new List<ContaPoupanca>();
        }
    }

    public static ContaCorrente ProcurarContaCorrentePorCPF(string CPF)
    {
        List<ContaCorrente> contasCorrentes = LerArquivoContaCorrente();

        if (contasCorrentes.Count != 0)
        {
            foreach (ContaCorrente conta in contasCorrentes)
            {
                if (conta.Titular == CPF)
                {
                    return conta;
                }
            }
        }

        return null;
    }

    public static ContaPoupanca ProcurarContaPoupancaPorCPF(string CPF)
    {
        List<ContaPoupanca> contasPoupancas = LerArquivoContaPoupanca();

        if (contasPoupancas.Count != 0)
        {
            foreach (ContaPoupanca conta in contasPoupancas)
            {
                if (conta.Titular == CPF)
                {
                    return conta;
                }
            }
        }

        return null;
    }

    private static bool ProcurarDuplicidadeDeConta(string tipoDeConta, string numConta)
    {
        if (tipoDeConta.ToLower().Equals("corrente"))
        {
            List<ContaCorrente> listaDeContasCorrentes = LerArquivoContaCorrente();

            if (listaDeContasCorrentes.Count != 0)
            {
                foreach (ContaCorrente conta in listaDeContasCorrentes)
                {
                    if(conta.NumeroConta.Equals(numConta))
                    {
                        return true;
                    }
                }
            }
        }

        if (tipoDeConta.ToLower().Equals("poupanca"))
        {
            List<ContaPoupanca> listaDeContasPoupancas = LerArquivoContaPoupanca();

            if (listaDeContasPoupancas.Count != 0)
            {
                foreach (ContaPoupanca conta in listaDeContasPoupancas)
                {
                    if(conta.NumeroConta.Equals(numConta))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static Cliente ProcurarClientePorCPF(string CPF)
    {
        List<Cliente> clientes = LerArquivoClientes();

        foreach (var cliente in clientes)
        {
            if (cliente.CPF.Equals(CPF))
            {
                return cliente;
            }
        }

        return new Cliente("erro", 404, "erro", "erro", []);
    }

    public static bool ValidarExistenciaDeContaPorCPF(string CPF)
    {
        List<Cliente> clientes = LerArquivoClientes();

        foreach (var cliente in clientes)
        {
            if (cliente.CPF.Equals(CPF))
            {
                return true;
            }
        }

        return false;
    }

    public static bool ValidadorDeIdade(Pessoa pessoa)
    {
        if(pessoa.Idade < 18)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}