using System.Security.Claims;
using ExercicioAspNET.Models;
using ExercicioAspNET.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExercicioAspNET.Controllers
{
    //classe ContaController herandado tudo de controller
    public class ContaController : Controller
    {
        // Declaração de um campo somente leitura para injetar o repositório de usuários
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        // Construtor da controladora que recebe a dependência do repositório via Injeção de Dependência (DI)
        public ContaController(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        // Define que este método responde a requisições do tipo HTTP GET para a rota de Registro
        [HttpGet]
        // Ação que retorna a View (página HTML) de cadastro de usuário
        public IActionResult Register() => View();

        // Define que este método responde a requisições do tipo HTTP POST (envio de formulário de registro)
        [HttpPost]
        // Ação assíncrona que recebe os dados básicos do usuário para cadastro
        public async Task<IActionResult> Register(string nome, string email, string senha)
        {
            // Valida se algum dos campos obrigatórios está vazio ou em branco
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                // Armazena uma mensagem de erro temporária para exibir na tela
                ViewBag.Erro = "Preencha todos os campos.";
                // Retorna para a mesma View de registro exibindo o erro
                return View();
            }
            // Consulta no banco de dados se já existe um usuário cadastrado com o e-mail informado
            var usuarioExistente = await _usuarioRepositorio.BuscarEmail(email);
            // Se o e-mail já estiver cadastrado...
            if (usuarioExistente != null)
            {
                // Define a mensagem de erro informando a duplicidade
                ViewBag.Erro = "Este e-mail já está cadastrado.";
                // Retorna para a View de registro
                return View();
            }
            // Instancia um novo objeto de modelo de usuário com os dados recebidos
            var novoUsuario = new UsuarioModel { Nome = nome, Email = email };
            // Chama o repositório para cadastrar o usuário no banco, realizando o hash da senha
            var sucesso = await _usuarioRepositorio.Cadastrar(novoUsuario, senha);

            // Se o cadastro foi bem-sucedido...
            if (sucesso)
            {
                // Define uma mensagem temporária de sucesso (sobrevive a um redirecionamento)
                TempData["Sucesso"] = "Cadastro realizado com sucesso! Faça o login.";
                // Redireciona o usuário para a página de Login
                return RedirectToAction(nameof(Login));
            }
            // Caso ocorra algum erro genérico no cadastro, define a mensagem correspondente
            ViewBag.Erro = "Erro ao cadastrar usuário.";
            // Retorna para a View de registro
            return View();
        }
        // Define que este método responde a requisições HTTP GET para a rota de Login
        [HttpGet]
        // Ação que retorna a View de login
        public IActionResult Login() => View();

        // Define que este método responde a requisições HTTP POST para processar o login
        [HttpPost]
        // Ação assíncrona que recebe o ViewModel contendo e-mail e senha
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Valida se o modelo preenchido atende aos requisitos de validação (Data Annotations)
            if (!ModelState.IsValid) return View(model);

            // Busca o usuário no banco de dados através do e-mail informado
            var usuario = await _usuarioRepositorio.BuscarEmail(model.Email);
            // Se o usuário não for encontrado..
            if (usuario == null)
            {
                // Adiciona um erro genérico ao ModelState para segurança (evita expor se o e-mail existe)
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                // Retorna a View de login com o modelo preenchido
                return View(model);
            }
            // Instancia o utilitário nativo do ASP.NET Core para manipulação/verificação de hashes de senha
            var passwordHasher = new PasswordHasher<UsuarioModel>();
            // Compara a senha informada com o hash armazenado no banco para este usuário
            var resultado = passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, model.Senha);

            // Se a verificação da senha falhar...
            if (resultado == PasswordVerificationResult.Failed)
            {
                // Adiciona um erro genérico ao ModelState
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                // Retorna a View de login
                return View(model);
            }

            // Criando a sessão por Cookies do ASP.NET Core: Define as informações (Claims) do usuário autenticado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nome), // Nome do usuário
                new Claim(ClaimTypes.Email, usuario.Email),// E-mail do usuário
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()) // ID único do usuário
            };
            // Cria a identidade baseada nas claims utilizando o esquema de autenticação por Cookie
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            // Efetua o login do usuário gerando e gravando o cookie de autenticação na resposta HTTP
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            // Redireciona o usuário autenticado para a página inicial protegida (Index)
            return RedirectToAction(nameof(Index));
        }
        // Restringe o acesso a este método apenas para usuários autenticados
        [Authorize]
        // Define que responde a requisições HTTP GET
        [HttpGet]
        // Ação da página inicial protegida
        public IActionResult Index()
        {
            // Retorna a View (Página de Bem-vindo)
            return View();
        }
        // Define que este método responde a requisições HTTP POST para realizar o logout
        [HttpPost]
        // Ação assíncrona para encerrar a sessão do usuário
        public async Task<IActionResult> Logout()
        {
            // Remove o cookie de autenticação do navegador do usuário
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Redireciona para a tela de Login após sair
            return RedirectToAction(nameof(Login));
        }
    }
}