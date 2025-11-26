namespace Dunder_Store.DTO
{
    public class ClienteDTOInput
    {
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string NumEndereco { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Localidade { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public bool? IsAdmin { get; set; } = null;
    }

    public class ClienteDTOOutput
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string NumEndereco { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Localidade { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
        public DateTime DataCadastro { get; set; }

        public ClienteDTOOutput(Guid id, string nome, string cpf, string email, string senha, string cep, string numEndereco,
                                 string logradouro, string bairro, string localidade, string uf, bool isAdmin, DateTime dataCadastro)
        {
            Id = id;
            Nome = nome;
            Cpf = cpf;
            Email = email;
            Senha = senha;
            Cep = cep;
            NumEndereco = numEndereco;
            Logradouro = logradouro;
            Bairro = bairro;
            Localidade = localidade;
            Uf = uf;
            IsAdmin = isAdmin;
            DataCadastro = dataCadastro;
        }
    }
}
