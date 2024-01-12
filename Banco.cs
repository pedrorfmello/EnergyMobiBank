// using System;
// using System.Collections.Generic;
// using System.IO;
// using Newtonsoft.Json;

// class Program
// {
//     static void Main(string[] args)
//     {
//         Cliente cliente1 = new Cliente("João", 30, "12345678900");
//         Cliente cliente2 = new Cliente("Maria", 25, "98765432100");

//         ContaPoupanca contaPoupanca = new ContaPoupanca(12345, cliente1, 0.05);
//         ContaCorrente contaCorrente = new ContaCorrente(54321, cliente2, 50);

//         contaPoupanca.Depositar(1000);
//         contaPoupanca.CalcularJuros();
//         contaPoupanca.ImprimirSaldo();

//         contaCorrente.Depositar(500);
//         contaCorrente.UsarChequeEspecial(300);
//         contaCorrente.ImprimirSaldo();

//         List<ContaBancaria> contas = new List<ContaBancaria> { contaPoupanca, contaCorrente };

//         // Convertendo contas para JSON e salvando em um arquivo
//         string json = JsonConvert.SerializeObject(contas, Formatting.Indented);
//         File.WriteAllText("contas.json", json);

//         Console.ReadLine();
//     }
// }
