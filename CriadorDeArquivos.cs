using System.Text.Json;

public static class CriadorDeArquivo
{
    public static void CriarArquivosDependencias(string path)
    {
        Console.WriteLine("Criando dependência!");
        try
        {
            if (!string.IsNullOrEmpty(path) && path != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path)); // Cria o diretório onde será salvo o arquivo

                if (Path.GetFileName(path) == "pessoas.json") // Separa o caso específico de pessoas
                {
                    List<Pessoa> clientes = new() // Cria a lista base de possíveis clientes
                    {
                        new Pessoa("Manel", 30, "123.456.789-00"),
                        new Pessoa("Maria", 28, "987.654.321-00"),
                        new Pessoa("Jonas The Kid", 11, "927.851.351-10")
                    };

                    string jsonString = JsonSerializer.Serialize(clientes, new JsonSerializerOptions { WriteIndented = true }); // Transforma em formato JSON
                    File.WriteAllText(path, jsonString); // Cria e preenche o arquivo com o JSON
                }
                else
                {
                    File.Create(path).Close(); // Cria o arquivo vazio
                    Console.WriteLine($"Arquivo {Path.GetFileName(path)} criado com sucesso.");
                }
            }
            else
            {
                Console.WriteLine("Caminho inválido ou nulo.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao criar o arquivo {Path.GetFileName(path)}: {ex.Message}");
        }
    }
}