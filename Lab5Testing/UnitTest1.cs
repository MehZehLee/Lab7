using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Lab5;
using Lab5.Services;

public class LibraryServiceTests
{
    private string mockData;

    public LibraryServiceTests()
    {
        // Set the test data directory to a folder within the test project.
        mockData = Path.Combine(Directory.GetCurrentDirectory(), "Data");

        
        Directory.CreateDirectory(mockData);

        //Makes sure mock data is in there for testing.
        File.WriteAllText(
            Path.Combine(mockData, "DeliberateFail.csv"),
            //Path.Combine(mockData, "Books.csv"),
            "1,Book 1,Author 1,111\n2,Book 2,Author 2,222"
        );

        File.WriteAllText(
            Path.Combine(mockData, "Users.csv"),
            "1,User 1,user1@example.com\n2,User 2,user2@example.com"
        );
    }

    /// <summary>
    /// Connects to LibraryService.cs in Lab 5.
    /// </summary>
    /// <returns>Mock Library</returns>
    private LibraryService CreateLibraryService()
    {
        var mockBooks = Path.Combine(mockData, "Books.csv");
        var mockUsers = Path.Combine(mockData, "Users.csv");

        return new LibraryService(mockBooks, mockUsers);
    }

    //Book Management
    
    /// <summary>
    /// Should add book to Books.csv.
    /// </summary>
    [Fact]
    public void AddBook()
    {
        // Arrange
        var libraryService = CreateLibraryService();
        var newBook = new Book { Title = "Book 3", Author = "Author 3", ISBN = "333" };

        // Act
        libraryService.AddBook(newBook);

        // Assert
        var books = libraryService.ReadBooks();
        Assert.Contains(books, b => b.Title == "Book 3" && b.Author == "Author 3" && b.ISBN == "333");
    }

    /// <summary>
    /// Should update a book's details.
    /// </summary>
    [Fact]
    public void EditBook()
    {
        // Arrange
        var libraryService = CreateLibraryService();
        var updatedBook = new Book { Id = 1, Title = "Updated Book 1", Author = "Updated Author", ISBN = "111" };

        // Act
        libraryService.EditBook(updatedBook);

        // Assert
        var book = libraryService.ReadBooks().FirstOrDefault(b => b.Id == 1);
        Assert.NotNull(book);
        Assert.Equal("Updated Book 1", book.Title);
        Assert.Equal("Updated Author", book.Author);
    }

    /// <summary>
    /// Should remove a book from Books.csv.
    /// </summary>
    [Fact]
    public void DeleteBook()
    {
        // Arrange
        var libraryService = CreateLibraryService();
        var bookIdToDelete = 1;

        // Act
        libraryService.DeleteBook(bookIdToDelete);

        // Assert
        var book = libraryService.ReadBooks().FirstOrDefault(b => b.Id == bookIdToDelete);
        Assert.Null(book);
    }
    
    //Book Borrowing/Returning

    /// <summary>
    /// Should show all books that are not borrowed.
    /// </summary>
    [Fact]
    public void GetAvailableBooks()
    {
        // Arrange
        var libraryService = CreateLibraryService();
        libraryService.BorrowBook(1, 1); // Borrow Book 1 by User 1

        // Act
        var availableBooks = libraryService.GetAvailableBooks();

        // Assert
        Assert.Single(availableBooks);
        Assert.Equal("Book 2", availableBooks.First().Title);
    }

    /// <summary>
    /// Should add a borrowed book to user.
    /// </summary>
    [Fact]
    public void BorrowBook()
    {
        // Arrange
        var libraryService = CreateLibraryService();
        var bookId = 1;
        var userId = 1;

        // Act
        libraryService.BorrowBook(bookId, userId);

        // Assert
        var borrowedBooks = libraryService.GetBorrowedBooks();
        Assert.True(borrowedBooks.ContainsKey(libraryService.GetUsers().First(u => u.Id == userId)));
        Assert.Contains(borrowedBooks[libraryService.GetUsers().First(u => u.Id == userId)], b => b.Id == bookId);
    }

    /// <summary>
    /// Should remove borrowed book from user.
    /// </summary>
    [Fact]
    public void ReturnBook()
    {
        // Arrange
        var libraryService = CreateLibraryService();
        var bookId = 1;
        var userId = 1;
        libraryService.BorrowBook(bookId, userId);

        // Act
        libraryService.ReturnBook(bookId, userId);

        // Assert
        var borrowedBooks = libraryService.GetBorrowedBooks();
        Assert.False(borrowedBooks.ContainsKey(libraryService.GetUsers().First(u => u.Id == userId)) &&
                     borrowedBooks[libraryService.GetUsers().First(u => u.Id == userId)].Any(b => b.Id == bookId));
    }

    //User Management

    /// <summary>
    /// Should add a user to Users.csv.
    /// </summary>
    [Fact]
    public void AddUser()
    {
        // Arrange
        var libraryService = CreateLibraryService();
        var newUser = new User { Name = "User 3", Email = "user3@example.com" };

        // Act
        libraryService.AddUser(newUser);

        // Assert
        var users = libraryService.GetUsers();
        Assert.Contains(users, u => u.Name == "User 3" && u.Email == "user3@example.com");
    }

    /// <summary>
    /// Should edit a user.
    /// </summary>
    [Fact]
    public void EditUser()
    {
        // Arrange
        var libraryService = CreateLibraryService();
        var updatedUser = new User { Id = 1, Name = "Updated User 1", Email = "updated1@example.com" };

        // Act
        libraryService.EditUser(updatedUser);

        // Assert
        var user = libraryService.GetUsers().FirstOrDefault(u => u.Id == 1);
        Assert.NotNull(user);
        Assert.Equal("Updated User 1", user.Name);
        Assert.Equal("updated1@example.com", user.Email);
    }

    /// <summary>
    /// Should remove a user from Users.csv.
    /// </summary>
    [Fact]
    public void DeleteUser()
    {
        // Arrange
        var libraryService = CreateLibraryService();
        var userIdToDelete = 1;

        // Act
        libraryService.DeleteUser(userIdToDelete);

        // Assert
        var user = libraryService.GetUsers().FirstOrDefault(u => u.Id == userIdToDelete);
        Assert.Null(user);
    }
}
