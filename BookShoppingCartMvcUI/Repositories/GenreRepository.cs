using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMvcUI.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public GenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Genre> GetAllGenres()
        {
            return _context.Genres.ToList();
        }

        public Genre GetGenreById(int id)
        {
            return _context.Genres.Find(id);
        }

        public void AddGenre(Genre genre)
        {
            _context.Genres.Add(genre);
            _context.SaveChanges();
        }

        public void UpdateGenre(Genre genre)
        {
            _context.Entry(genre).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteGenre(Genre genre)
        {
            _context.Genres.Remove(genre);
            _context.SaveChanges();
        }
        public bool GenreNameExists(Genre genre)
        {
            // Check if any genre with the same name exists, excluding the current genre
            return _context.Genres.Any(g => g.GenreName == genre.GenreName && g.Id != genre.Id);
        }
    }

}
