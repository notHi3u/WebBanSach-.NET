namespace BookShoppingCartMvcUI.Repositories
{
    public interface IGenreRepository
    {
        IEnumerable<Genre> GetAllGenres();
        Genre GetGenreById(int id);
        void AddGenre(Genre genre);
        void UpdateGenre(Genre genre);
        void DeleteGenre(Genre genre);
        public bool GenreNameExists(Genre genre);
    }
}
