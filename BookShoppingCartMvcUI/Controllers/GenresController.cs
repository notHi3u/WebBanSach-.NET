using BookShoppingCartMvcUI.Models;
using BookShoppingCartMvcUI.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebBanSach.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GenresController : Controller
    {
        private readonly IGenreRepository _genreRepository;

        public GenresController(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public IActionResult Index()
        {
            IEnumerable<Genre> genres = _genreRepository.GetAllGenres();
            return View(genres);
        }

        public IActionResult Details(int id)
        {
            Genre genre = _genreRepository.GetGenreById(id);

            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(GenreViewModel genreViewModel)
        {
            // Check if the genre name already exists
            Genre genre = new Genre
            {
                Id = 0,
                GenreName = genreViewModel.GenreName
            };

            if (_genreRepository.GenreNameExists(genre))
            {
                ModelState.AddModelError(nameof(genreViewModel.GenreName), "Genre name must be unique.");
            }

            if (ModelState.IsValid)
            {
                // Map from GenreViewModel to Genre
                genre = new Genre
                {
                    Id = 0,
                    GenreName = genreViewModel.GenreName
                };

                // Add the mapped genre to the repository
                _genreRepository.AddGenre(genre);

                return RedirectToAction("Index");
            }

            // If ModelState is not valid, redisplay the form
            return View(genreViewModel);
        }

        public IActionResult Edit(int id)
        {
            Genre genre = _genreRepository.GetGenreById(id);

            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Genre genre)
        {
            if (_genreRepository.GenreNameExists(genre))
            {
                ModelState.AddModelError(nameof(Genre.GenreName), "Genre name must be unique.");
            }
            if (ModelState.IsValid)
            {
                _genreRepository.UpdateGenre(genre);
                return RedirectToAction("Index");
            }

            // If ModelState is not valid, redisplay the form
            return View(genre);
        }

        public IActionResult Delete(int id)
        {
            Genre genre = _genreRepository.GetGenreById(id);

            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Genre genre = _genreRepository.GetGenreById(id);

            if (genre == null)
            {
                return NotFound();
            }

            _genreRepository.DeleteGenre(genre);
            return RedirectToAction("Index");
        }

    }


}
