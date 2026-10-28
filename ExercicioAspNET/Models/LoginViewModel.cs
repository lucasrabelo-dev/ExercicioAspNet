// Importa o namespace que contém os atributos de validação de dados (Data Annotations)
using System.ComponentModel.DataAnnotations;

// Define o namespace onde o modelo de login está localizado dentro do projeto
namespace ExercicioAspNET.Models
{
    //Declaração da classe pública que representa os dados enviados no formulário de login
    public class LoginViewModel
    {
        // Torna o campo 'Email' obrigatório; se estiver vazio, exibe a mensagem personalizada
        [Required(ErrorMessage = "Informe o e-mail.")]
        // Valida se o texto digitado está no formato padrão de um e-mail válido
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        // Propriedade pública que armazena o e-mail, inicializada por segurança com string vazia
        public string Email { get; set; } = string.Empty;

        // Torna o campo 'Senha' obrigatório; se estiver vazio, exibe a mensagem personalizada
        [Required(ErrorMessage = "Informe a senha.")]
        // Informa ao ASP.NET Core que este campo é do tipo senha (mascara os caracteres na interface)
        [DataType(DataType.Password)]
        // Propriedade pública que armazena a senha, inicializada com string vazia
        public string Senha { get; set; } = string.Empty;
    }
}