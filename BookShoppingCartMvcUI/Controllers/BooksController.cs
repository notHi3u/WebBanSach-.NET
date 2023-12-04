using Microsoft.AspNetCore.Mvc;
using System.Linq;
using BookShoppingCartMvcUI.Models;
using Microsoft.EntityFrameworkCore;
using BookShoppingCartMvcUI.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;

public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public BooksController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        var books = _context.Books.Include(b => b.Genre).ToList();
        return View(books);
    }

    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = _context.Books.Include(b => b.Genre).FirstOrDefault(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }


    public IActionResult Create()
    {
        ViewBag.Genres = GetGenreSelectList();
        return View();
    }

    [HttpPost]
    public IActionResult Create(BookUploadModel book)
    {
        // Find the genre by name
        var genre = _context.Genres.FirstOrDefault(g => book.GenreName == g.GenreName );

        if (genre == null)
        {
            // Handle the case where the selected genre does not exist
            ModelState.AddModelError("GenreName", "Selected genre does not exist.");
            ViewBag.Genres = GetGenreSelectList(); // Use the helper method
            return View(book);
        }
        if (ModelState.IsValid)
        {
            

            // Handle file upload for the book cover
            string fileName = UploadFile(book);

            // Create a new Book entity with the uploaded file name
            var newBook = new Book
            {
                BookName = book.BookName,
                AuthorName = book.AuthorName,
                Price = book.Price,
                Description = book.Description,
                Image = fileName,
                GenreId = genre.Id,
                // Populate other properties as needed
            };

            _context.Books.Add(newBook);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Genres = GetGenreSelectList(); // Use the helper method
        return View(book);
    }

    // Helper method to get the genre names as IEnumerable<SelectListItem>
    private IEnumerable<SelectListItem> GetGenreSelectList()
    {
        return _context.Genres
            .Select(g => new SelectListItem
            {
                Value = g.GenreName,
                Text = g.GenreName
            })
            .ToList();
    }


    private string UploadFile(BookUploadModel book)
    {
        string fileName = null;

        if (book.Image != null)
        {
            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            fileName = Guid.NewGuid().ToString() + "-" + book.Image.FileName;
            string filePath = Path.Combine(uploadDir, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                book.Image.CopyTo(fileStream);
            }
        }

        return fileName;
    }


    [HttpGet]
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = _context.Books.Include(b => b.Genre).FirstOrDefault(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        // Map Book to BookUploadModel
        var bookUploadModel = MapBookToBookEditModel(book);

        ViewBag.Genres = GetGenreSelectList();
        return View(bookUploadModel);
    }

    [HttpPost]
    public IActionResult Edit(BookEditModel book)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Genres = GetGenreSelectList();
            return View(book);
        }

        // Find the genre by name
        var genre = _context.Genres.FirstOrDefault(g => book.GenreName == g.GenreName);

        if (genre == null)
        {
            ModelState.AddModelError("GenreName", "Selected genre does not exist.");
            ViewBag.Genres = GetGenreSelectList();
            return View(book);
        }

        // Update book properties
        var existingBook = _context.Books.Find(book.Id);
        if (existingBook == null)
        {
            return NotFound();
        }

        if (book.Image != null)
        {
            DeleteImage(existingBook.Image);

            // Handle file upload for the new book cover
            string fileName = UploadFile(book);

            existingBook.Image = fileName;
        }

        existingBook.BookName = book.BookName;
        existingBook.AuthorName = book.AuthorName;
        existingBook.Price = book.Price;
        existingBook.Description = book.Description;
        existingBook.GenreId = genre.Id;
        existingBook.Genre = genre;

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    private void DeleteImage(string fileName)
    {
        if (fileName != null)
        {
            // Construct the full file path
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

            // Check if the file exists and delete it
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }

    private BookEditModel MapBookToBookEditModel(Book book)
    {
        return new BookEditModel
        {
            Id = book.Id,
            BookName = book.BookName,
            AuthorName = book.AuthorName,
            Price = book.Price,
            Description = book.Description,
            GenreId = book.GenreId,
            GenreName = book.Genre?.GenreName, // Assuming you want to map the genre name as well
            Imagefile = book.Image
        };
    }

    private string UploadFile(BookEditModel book)
    {
        string fileName = null;

        if (book.Image != null)
        {
            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            fileName = Guid.NewGuid().ToString() + "-" + book.Image.FileName;
            string filePath = Path.Combine(uploadDir, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                book.Image.CopyTo(fileStream);
            }
        }

        return fileName;
    }


    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = _context.Books.Include(b => b.Genre).FirstOrDefault(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var book = _context.Books.Find(id);

        if (book == null)
        {
            return NotFound();
        }

        // Remove the image file from wwwroot/images/ if it exists
        if (!string.IsNullOrEmpty(book.Image))
        {
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", book.Image);

            // Check if the file exists before attempting to delete
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        _context.Books.Remove(book);
        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }


}
