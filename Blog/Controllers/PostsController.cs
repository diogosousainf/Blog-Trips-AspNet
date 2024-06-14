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

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Likes) // Incluir os likes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }



        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description, Image")] Posts posts, IFormFile imageFile)
        {
            posts.UserId = _userManager.GetUserId(User);
            posts.CreatedAt = DateTime.Now;
            var user = await _userManager.GetUserAsync(User);
            posts.Email = user?.Email;

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        posts.Image = "/images/" + fileName;
                    }

                    _context.Add(posts);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    Console.WriteLine(ex.Message);
                }
            }

            // Se houver erros de validação, exiba os erros na view
            return View(posts);
        }



        // GET: Posts/Edit/5
        [HttpGet("Posts/Edit/{id}")]
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

            var userId = _userManager.GetUserId(User);
            if (post.UserId != userId)
            {
                return Forbid();
            }

            return View(post);
        }

        // POST: Posts/Edit/5
        [HttpPost("Posts/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Image")] Posts editedPost, IFormFile imageFile)
        {
            if (id != editedPost.Id)
            {
                return NotFound();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Identity/Account");
            }

            var userId = _userManager.GetUserId(User);
            var originalPost = await _context.Posts.FindAsync(id);
            if (originalPost.UserId != userId)
            {
                return Forbid();
            }

            originalPost.Description = editedPost.Description;

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                originalPost.Image = "/images/" + fileName;
            }

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

            var post = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
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
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                var userPosts = await _context.Posts.Where(p => p.UserId == userId).ToListAsync();
                return View(userPosts);
            }
            else
            {
                return RedirectToAction("Login", "Identity/Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(int postId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Identity/Account");
            }

            var userId = _userManager.GetUserId(User);

            // Verifica se o usuário já deu like no post
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
            {
                // Se já deu like, então remove o like (dislike)
                _context.Likes.Remove(existingLike);
            }
            else
            {
                // Se ainda não deu like, cria um novo like
                var newLike = new Like
                {
                    PostId = postId,
                    UserId = userId
                };

                // Adiciona o like ao contexto
                _context.Likes.Add(newLike);
            }

            // Salva as alterações no banco de dados
            await _context.SaveChangesAsync();

            // Atualiza a contagem de likes do post
            var post = await _context.Posts.FindAsync(postId);
            post.LikeCount = await _context.Likes.Where(l => l.PostId == postId).CountAsync();
            await _context.SaveChangesAsync();

            // Redireciona de volta para a página de posts
            return RedirectToAction(nameof(Index));
        }



    }
}


