using ExercicioAspNET.Models;

namespace ExercicioAspNET.Repositories
{
    // Define o contrato (interface) público para o repositório de usuários, definindo quais métodos devem ser

    public interface IUsuarioRepositorio
    {
        // Declara a assinatura de um método assíncrono para cadastrar um novo usuário.
        // Recebe o modelo do usuário e a senha pura (sem hash), retornando um booleano indicando o sucesso da operação.
        Task<bool> Cadastrar(UsuarioModel usuario, string senhaPura);
        // Declara a assinatura de um método assíncrono para buscar um usuário pelo seu endereço de e-mail.
        // Retorna o objeto UsuarioModel se encontrado, ou null caso contrário (permitindo valores nulos com o '?').
        Task<UsuarioModel?> BuscarEmail(string email);
    }
}