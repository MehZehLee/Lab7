using System.Collections.Generic;
using Lab5;
using Lab5.Components;
using Lab5.Services;

namespace Lab5.Services
{
    public interface ILibraryService
    {
        // Methods for managing books
        List<Book> ReadBooks();
        void AddBook(Book book);
        void AddBookWithoutChangingId(Book book);
        void EditBook(Book book);
        void DeleteBook(int bookId);
        List<Book> GetAvailableBooks();


        // Methods for managing users
        List<User> ReadUsers();
        void AddUser(User user);
        void EditUser(User user);
        void DeleteUser(int userId);
        List<User> GetUsers();

        // Methods for borrowing and returning books
        void BorrowBook(int bookId, int userId);
        void ReturnBook(int bookId, int userId);
        Dictionary<User, List<Book>> GetBorrowedBooks();
    }
}
