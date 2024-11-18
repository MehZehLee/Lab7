using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lab5.Services;
using Lab5;

namespace Lab5.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly string booksFilePath;
        private readonly string usersFilePath;

        private List<Book> books = new List<Book>();
        private List<User> users = new List<User>();
        private Dictionary<User, List<Book>> borrowedBooks = new Dictionary<User, List<Book>>();

        // Default constructor for production use
        public LibraryService()
            : this("./Data/Books.csv", "./Data/Users.csv") // Call the new constructor with default paths
        {
        }

        // Constructor for testing or custom paths
        public LibraryService(string booksFilePath, string usersFilePath)
        {
            this.booksFilePath = booksFilePath;
            this.usersFilePath = usersFilePath;

            books = ReadBooks();
            users = ReadUsers();
        }

        // Methods for managing books
        public List<Book> ReadBooks()
        {
            var booksList = new List<Book>();
            try
            {
                foreach (var line in File.ReadLines(booksFilePath))
                {
                    var fields = line.Split(',');

                    if (fields.Length >= 4)
                    {
                        var book = new Book
                        {
                            Id = int.Parse(fields[0].Trim()),
                            Title = fields[1].Trim(),
                            Author = fields[2].Trim(),
                            ISBN = fields[3].Trim()
                        };

                        booksList.Add(book);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading books: {ex.Message}");
            }
            return booksList;
        }

        public void AddBook(Book book)
        {
            book.Id = books.Any() ? books.Max(b => b.Id) + 1 : 1;
            books.Add(book);
            SaveBooksToFile();
        }

        public void EditBook(Book book)
        {
            var existingBook = books.FirstOrDefault(b => b.Id == book.Id);
            if (existingBook != null)
            {
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.ISBN = book.ISBN;
                SaveBooksToFile();
            }
        }

        public void DeleteBook(int bookId)
        {
            var book = books.FirstOrDefault(b => b.Id == bookId);
            if (book != null)
            {
                books.Remove(book);
                SaveBooksToFile();
            }
        }

        public List<Book> GetAvailableBooks()
        {
            // Return the books that are not in any user's borrowed list
            return books.Where(b => !borrowedBooks.Values.SelectMany(v => v).Contains(b)).ToList();
        }


        // Methods for managing users
        public List<User> ReadUsers()
        {
            var usersList = new List<User>();
            try
            {
                foreach (var line in File.ReadLines(usersFilePath))
                {
                    var fields = line.Split(',');

                    if (fields.Length >= 3)
                    {
                        var user = new User
                        {
                            Id = int.Parse(fields[0].Trim()),
                            Name = fields[1].Trim(),
                            Email = fields[2].Trim()
                        };

                        usersList.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading users: {ex.Message}");
            }
            return usersList;
        }

        public void AddUser(User user)
        {
            user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            users.Add(user);
            SaveUsersToFile();
        }

        public void EditUser(User user)
        {
            var existingUser = users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                SaveUsersToFile();
            }
        }

        public void DeleteUser(int userId)
        {
            var user = users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                users.Remove(user);
                SaveUsersToFile();
            }
        }

        public List<User> GetUsers()
        {
            return users;
        }

        // Methods for borrowing and returning books
        public void BorrowBook(int bookId, int userId)
        {
            var user = users.FirstOrDefault(u => u.Id == userId);
            var book = books.FirstOrDefault(b => b.Id == bookId);

            if (user != null && book != null)
            {
                // Remove the book from available books
                books.Remove(book);

                // Add to the borrowed list for the user
                if (!borrowedBooks.ContainsKey(user))
                {
                    borrowedBooks[user] = new List<Book>();
                }
                borrowedBooks[user].Add(book);

                SaveBooksToFile();
                SaveUsersToFile();
            }
        }


        public void ReturnBook(int bookId, int userId)
        {
            var user = users.FirstOrDefault(u => u.Id == userId);
            if (user != null && borrowedBooks.ContainsKey(user))
            {
                var book = borrowedBooks[user].FirstOrDefault(b => b.Id == bookId);
                if (book != null)
                {
                    borrowedBooks[user].Remove(book);
                    books.Add(book);
                }
            }
        }

        public void AddBookWithoutChangingId(Book book)
        {
            // Check if the book already exists by ID to prevent duplicates
            var existingBook = books.FirstOrDefault(b => b.Id == book.Id);
            if (existingBook == null)
            {
                // Only add the book if it does not already exist
                books.Add(book);
                SaveBooksToFile();
            }
        }


        public Dictionary<User, List<Book>> GetBorrowedBooks()
        {
            return borrowedBooks;
        }

        // Helper methods to save books and users to CSV files
        private void SaveBooksToFile()
        {
            File.WriteAllLines(booksFilePath, books.Select(b => $"{b.Id},{b.Title},{b.Author},{b.ISBN}"));
        }

        private void SaveUsersToFile()
        {
            File.WriteAllLines(usersFilePath, users.Select(u => $"{u.Id},{u.Name},{u.Email}"));
        }
    }
}
