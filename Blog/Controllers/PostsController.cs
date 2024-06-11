using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Blog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public PostsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        // GET: Posts
        public async Task<IActionResult> Index(string searchString, int? year, int? month)
        {
            // Retorna apenas se user estiver logado
            if (User.Identity.IsAuthenticated)
            {
                var posts = from p in _context.Posts
                            select p;

                if (!String.IsNullOrEmpty(searchString))
                {
                    posts = posts.Where(s => s.Description.Contains(searchString));
                }

                if (year.HasValue)
                {
                    posts = posts.Where(s => s.CreatedAt.Year == year.Value);
                }

                if (month.HasValue)
                {
                    posts = posts.Where(s => s.CreatedAt.Month == month.Value);
                }

                return View(await posts.ToListAsync());
            }
            else
            {
                // Login do user
                return RedirectToAction("Login", "Identity/Account");
            }
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);
            if (posts == null)
            {
                return NotFound();
            }

            return View(posts);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description")] Posts posts, IFormFile imageFile)
        {
            // Definir o UserId do user logado
            posts.UserId = _userManager.GetUserId(User);

            // Definir a data de criação
            posts.CreatedAt = DateTime.Now;

            // Definir o Email do user logado
            var user = await _userManager.GetUserAsync(User);
            posts.Email = user?.Email;

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Gera um nome de arquivo único 
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                    // Define o caminho onde o arquivo será salvo (pode ser um diretório dentro de wwwroot)
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                    // Salva o arquivo no disco
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    // Define o caminho relativo da imagem no disco para a propriedade Image
                    posts.Image = "/images/" + fileName;
                }

                _context.Add(posts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Adicionando debug para listar erros do ModelState
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }

            return View(posts);
        }



    // GET: Posts/Edit/5
public async Task<IActionResult> Edit(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var post = await _context.Posts.FindAsync(id);
    if (post == null)
    {
        return NotFound();
    }

    // Verificar se o usuário logado é o autor da postagem
    var userId = _userManager.GetUserId(User);
    if (post.UserId != userId)
    {
        return Forbid(); // Retornar um erro 403 (Forbidden)
    }

    return View(post);
}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Image")] Posts editedPost)
        {
            if (id != editedPost.Id)
            {
                return NotFound();
            }

            // Verificar se o usuário está autenticado
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Identity/Account");
            }

            // Verificar se o usuário é o autor do post
            var userId = _userManager.GetUserId(User);
            var originalPost = await _context.Posts.FindAsync(id);
            if (originalPost.UserId != userId)
            {
                return Forbid(); // Retornar um erro 403 (Forbidden)
            }

            // Atualizar apenas as propriedades editáveis
            originalPost.Description = editedPost.Description;
            originalPost.Image = editedPost.Image;

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostsExists(editedPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(editedPost);
        }



        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);
            if (posts == null)
            {
                return NotFound();
            }

            return View(posts);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var posts = await _context.Posts.FindAsync(id);
            if (posts != null)
            {
                _context.Posts.Remove(posts);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostsExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        // GET: Posts/MyPosts
        public async Task<IActionResult> MyPosts()
        {
            // Verifica se o user está autenticado
            if (User.Identity.IsAuthenticated)
            {
                // Obtém o ID 
                var userId = _userManager.GetUserId(User);
                // Obtém os posts 
                var userPosts = await _context.Posts.Where(p => p.UserId == userId).ToListAsync();
                return View(userPosts);
            }
            else
            {
                return RedirectToAction("Login", "Identity/Account");
            }
        }


    }
}
