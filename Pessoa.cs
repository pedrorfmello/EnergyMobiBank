public class Pessoa
{
    public string Nome { get; set; }
    public int Idade { get; set; }
    public string CPF { get; set; }

    public Pessoa(string nome, int idade, string cpf)
    {
        Nome = nome;
        Idade = idade;
        CPF = cpf;
    }

    public override string ToString()
    {
        return $"Nome: {Nome}, Idade: {Idade}, CPF: {CPF}";
    }
}