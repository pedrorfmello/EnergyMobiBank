public class Cliente : Pessoa
{
    public List<string> Contas { get; set; }
    public string Senha { get; set; }

    public Cliente(string nome, int idade, string cpf, string senha, List<string> contas) : base(nome, idade, cpf)
    {
        Senha = senha;
        Contas = contas;
    }

    
    public override string ToString()
    {
        return $"Nome: {Nome}, Idade: {Idade}, CPF: {CPF}, Contas: {Contas}";
    }
}