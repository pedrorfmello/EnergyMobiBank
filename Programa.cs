using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Iniciando o Sistema...");

        string pessoasPath = Path.Combine("Data", "Pessoas", "pessoas.json");
        string clientesPath = Path.Combine("Data", "Clientes", "clientes.json");
        string contasCPath = Path.Combine("Data", "Contas", "contasCorrentes.json");
        string contasPPath = Path.Combine("Data", "Contas", "contasPoupancas.json");

        if (!File.Exists(pessoasPath))
        {
            Console.WriteLine("Dependência não encontrada!");
            CriadorDeArquivo.CriarArquivosDependencias(pessoasPath);
        }
        if (!File.Exists(clientesPath))
        {
            Console.WriteLine("Dependência não encontrada!");
            CriadorDeArquivo.CriarArquivosDependencias(clientesPath);
        }
        if (!File.Exists(contasCPath))
        {
            Console.WriteLine("Dependência não encontrada!");
            CriadorDeArquivo.CriarArquivosDependencias(contasCPath);
        }
        if (!File.Exists(contasPPath))
        {
            Console.WriteLine("Dependência não encontrada!");
            CriadorDeArquivo.CriarArquivosDependencias(contasPPath);
        }
        
        Telas.TelaMenu(); // Chama a tela de Menu Inicial
    }
}
