using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace CasaToro.Novasoft.Fotos.Controllers
{
    public class LoginController : Controller
    {
        private const string LDAP_SERVER = "192.168.80.21";
        private const string DOMAIN = "CASATORO.LOC";

        // Acción para mostrar la vista de inicio de sesión
        public IActionResult Index()
        {
            return View();
        }

        // Acción para manejar el envío del formulario de inicio de sesión
        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            if (authentication(username, password))
            {
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                return View();
            }
        }

        // Acción para cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        // Método para autenticar al usuario
        public bool authentication(string username, string password)
        {
            try
            {
                string usrDn = DOMAIN + @"\" + username;
                DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://" + LDAP_SERVER, usrDn, password);
                DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry);

                // Configurar el filtro de búsqueda y las propiedades a cargar
                directorySearcher.Filter = $"(sAMAccountName={username})";
                directorySearcher.PropertiesToLoad.Add("displayName");
                directorySearcher.PropertiesToLoad.Add("memberOf");

                SearchResult result = directorySearcher.FindOne();
                if (result != null)
                {
                    // Verificar si el usuario pertenece al grupo especificado
                    bool isMemerOfGroup = IsUserInGroup(result, "APP-FOTOS");

                    if (!isMemerOfGroup)
                    {
                        ViewBag.ErrorMessage = "No tiene permisos para acceder a la aplicación";
                        return false;
                    }
                    string displayName = result.Properties["displayName"][0].ToString();
                    string initials = string.IsNullOrEmpty(displayName) ? "?" : string.Concat(displayName.Split(' ').Select(n => n[0])).ToUpper();

                    // Crear los claims de autenticación
                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, username),
                            new Claim("FullName", displayName)
                        };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = false
                    };

                    // Iniciar sesión del usuario
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                      new ClaimsPrincipal(claimsIdentity),
                      authProperties).Wait();

                    // Obtener y guardar el nombre del usuario en la sesión

                    HttpContext.Session.SetString("Username", username);
                    HttpContext.Session.SetString("Name", displayName);

                    ViewBag.Initials = initials;
                    return true;
                }
                else
                {
                    ViewBag.ErrorMessage = "Usuario o contraseña incorrecta";
                    return false;
                }
            }
            catch (LdapException ldapEx)
            {
                ViewBag.ErrorMessage = $"Error en la conexion ldap:{ldapEx.Message}";
                return false;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return false;
            }
        }

        // Método para verificar si el usuario pertenece a un grupo específico
        private bool IsUserInGroup(SearchResult userResult, string groupName)
        {
            try
            {
                if (userResult.Properties["memberOf"] == null) return false;

                foreach (var group in userResult.Properties["memberOf"])
                {
                    if (group.ToString().Contains(groupName))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verificando grupos del usuario: {ex.Message}");
                return false;
            }
        }
    }
}
