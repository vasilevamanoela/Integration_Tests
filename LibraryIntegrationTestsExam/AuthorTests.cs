using IntegrationTests.Factories;
using IntegrationTests.Models;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace IntegrationTests
{
    [TestFixture]
    public class AuthorTests
    {
        private RestClient _httpClient;
        private string _id;
        private string _bookId;

        [SetUp]
        public void SetUp()
        {
            _httpClient = new RestClient();
            _httpClient.BaseUrl = new Uri("https://libraryjuly.azurewebsites.net");
            _id = CreateAuthor();
            _bookId = AssignBookToAuthor();
        }


        ////POST REQUESTS

        //POST Author
        [Test]
        public void PostAuthor_CreatedStatusCode()
        {
            var author = new Author()
            {
                FirstName = "Pesho",
                LastName = "Peshov",
                Genre = "Male"
            };

            var request = new RestRequest($"/api/authors");
            request.AddJsonBody(author.ToJson(), "application/json");

            var response = _httpClient.Post(request);

            var actualAuthor = Author.FromJson(response.Content);

            var expectedAuthor = new Author()
            {
                Name = $"{author.FirstName} {author.LastName}",
                Genre = author.Genre
            };

            Assert.AreEqual(expectedAuthor.Name, actualAuthor.Name);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        //POST Author (invalid body)
        [Test]
        public void PostAuthorWithInvalidBody_BadRequestStatusCode()
        {
            var author = new Author()
            {
                FirstName = "Gosho",
                LastName = "Goshkov",
                DateOfBirth = "invalid date",
                Genre = "Male"
            };

            var request = new RestRequest($"/api/authors");
            request.AddJsonBody(author.ToJson(), "application/json");

            var response = _httpClient.Post(request);

            var actualAuthor = Author.FromJson(response.Content);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        //POST Book for Author
        [Test]
        public void PostBookForAuthor_CreatedStatusCode()
        {
            var newBook = new Book()
            {
                Title = "Test Book",
                Description = "Nice book"
            };

            var request = new RestRequest($"/api/authors/{_id}/books");
            request.AddJsonBody(newBook.ToJson(), "application/json");

            var response = _httpClient.Post(request);

            var assignedBook = Book.FromJson(response.Content);

            var expextedBook = new Book()
            {
                Title = newBook.Title,
                Description = newBook.Description
            };

            Assert.AreEqual(expextedBook.Title, assignedBook.Title);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        //POST Book for Author (unexisting Author)
        [Test]
        public void PostBookForAuthorUnexistingAuthor_NotFoundStatusCode()
        {
            var authorId = new Guid();

            var newBook = new Book()
            {
                Title = "Test Book",
                Description = "Nice book"
            };

            var request = new RestRequest($"/api/authors/{authorId}/books");
            request.AddJsonBody(newBook.ToJson(), "application/json");

            var response = _httpClient.Post(request);

            var assignedBook = Book.FromJson(response.Content);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        //POST Author with Books
        [Test]
        public void PostAuthorWithBooks_CreatedStatusCode()
        {
            var firstBook = new Book()
            {
                Title = "Book One",
                Description = "My first book"
            };

            var secondBook = new Book()
            {
                Title = "Book Two",
                Description = "My second book"
            };

            var books = new List<Book>();
            books.Add(firstBook);
            books.Add(secondBook);

            var newAuthor = new Author()
            {
                FirstName = "Pesho",
                LastName = "Peshov",
                DateOfBirth = "1990-03-01",
                Genre = "test",
                Books = books
            };

            var request = new RestRequest("/api/authors");
            request.AddJsonBody(newAuthor.ToJson(), "application/json");

            var response = _httpClient.Post(request);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        //POST Author collection
        [Test]
        public void PostAuthorCollection_CreatedStatusCode()
        {
            var fisrtAuthor = new Author()
            {
                FirstName = "FAuthor One",
                LastName = "LAuthor One",
                DateOfBirth = "1999-01-02",
                Genre = "test"
            };

            var secondAuthor = new Author()
            {
                FirstName = "FAuthor Two",
                LastName = "LAuthor Two",
                DateOfBirth = "1991-02-01",
                Genre = "test"
            };

            var authors = new List<Author>();
            authors.Add(fisrtAuthor);
            authors.Add(secondAuthor);

            var request = new RestRequest($"/api/authorcollections");
            request.AddJsonBody(authors.ToJson(), "application/json");

            var response = _httpClient.Post(request);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        //POST Author (single, unexisting - should fail with 404)
        [Test]
        public void PostAuthorUnexisting_NotFoundStatusCode()
        {
            var author = new Author()
            {
                FirstName = "Gosho",
                LastName = "Goshov",
                DateOfBirth = "invalid date",
                Genre = "male"
            };

            var authorId = new Guid();

            var request = new RestRequest($"/api/authors/{authorId}");
            request.AddJsonBody(author.ToJson(), "application/json");

            var response = _httpClient.Post(request);

            var actualAuthor = Author.FromJson(response.Content);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        //POST Author(single, existing - should fail with 409)
        [Test]
        public void PostAuthorExisting_ConflictStatusCode()
        {
            var author = new Author()
            {
                FirstName = "Gosho",
                LastName = "Goshov",
                DateOfBirth = "invalid date",
                Genre = "male"
            };

            var request = new RestRequest($"/api/authors/{_id}");
            request.AddJsonBody(author.ToJson(), "application/json");

            var response = _httpClient.Post(request);

            var actualAuthor = Author.FromJson(response.Content);

            Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        }

        //POST Author (XML input)
        [Test]
        public void PostAuthorXMLInput_CreatedStatusCode()
        {
            var author = new Author()
            {
                FirstName = "Pesho",
                LastName = "Peshov",
                Genre = "Male"
            };

            var request = new RestRequest($"/api/authors");
            request.AddJsonBody(author.ToJson(), "application/xml");

            var response = _httpClient.Post(request);

            var actualAuthor = Author.FromJson(response.Content);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        //POST Author (XML input, XML output)
        [Test]
        public void PostAuthorXMLInputXMLOutput_InternalServerErrorStatusCode()
        {
            var author = new Author()
            {
                FirstName = "Pesho",
                LastName = "Peshov",
                Genre = "Male"
            };

            var request = new RestRequest($"/api/authors");
            request.AddHeader("Accept", "application/xml");
            request.AddJsonBody(author.ToJson(), "application/xml");

            var response = _httpClient.Post(request);

            var actualAuthor = Author.FromJson(response.Content);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }


        ////DELETE REQUESTS

        //DELETE Book for Author
        [Test]
        public void DeleteBookForAuthor_NoContentStatusCode()
        {
            var request = new RestRequest($"/api/authors/{_id}/books/{_bookId}");

            var response = _httpClient.Delete(request);

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        //DELETE Book for Author (unexisting Book)
        [Test]
        public void DeleteBookForAuthorUnexistingBook_NotFoundStatusCode()
        {
            var bookId = new Guid();

            var request = new RestRequest($"/api/authors/{_id}/books/{bookId}");

            var response = _httpClient.Delete(request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        //DELETE Book for Author (unexisting Author)
        [Test]
        public void DeleteBookForAuthorUnexistingAuthor_NotFoundStatusCode()
        {
            var authorId = new Guid();

            var request = new RestRequest($"/api/authors/{authorId}/books/{_bookId}");

            var response = _httpClient.Delete(request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        //DELETE Author 
        [Test]
        public void DeleteAuthor_NoContentStatusCode()
        {
            var request = new RestRequest($"/api/authors/{_id}");

            var response = _httpClient.Delete(request);

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }


        ////GET REQUESTS

        //GET Author (Accept: application/xml)
        [Test]
        public void GetAuthorXML_InternalServerErrorStatusCode()
        {
            var request = new RestRequest($"/api/authors/{_id}");
            request.AddHeader("Accept", "application/xml");

            var response = _httpClient.Get(request);

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        //GET Author (Accept: application/json)
        [Test]
        public void GetAuthorJson_OKStatusCode()
        {
            var request = new RestRequest($"/api/authors/{_id}");
            request.AddHeader("Accept", "application/json");
            var response = _httpClient.Get(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        //GET Book for Author (unexisting Author)
        [Test]
        public void GetBookForAuthorUnexistingAuthor_NotFoundStatusCode()
        {
            var authorId = new Guid();

            var request = new RestRequest($"/api/authors/{authorId}/books/{_bookId}");

            var response = _httpClient.Get(request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        //GET Book for Author (unexisting Book)
        [Test]
        public void GetBookForAuthorUnexistingBook_NotFoundStatusCode()
        {
            var bookId = new Guid();

            var request = new RestRequest($"/api/authors/{_id}/books/{bookId}");

            var response = _httpClient.Get(request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        //GET Book for Author 
        [Test]
        public void GetBookForAuthor_OKStatusCode()
        {
            var request = new RestRequest($"/api/authors/{_id}/books/{_bookId}");

            var response = _httpClient.Get(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        //GET Books for Author (unexisting Author)
        [Test]
        public void GetBooksForAuthorUnexistingAuthor_NotFoundStatusCode()
        {
            var authorId = new Guid();

            var request = new RestRequest($"/api/authors/{authorId}/books");

            var response = _httpClient.Get(request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        //GET Books for Author
        [Test]
        public void GetBooksForAuthor_OKStatusCode()
        {
            var request = new RestRequest($"/api/authors/{_id}/books");

            var response = _httpClient.Get(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        //GET Author (unexisting)
        [Test]
        public void GetAuthorUnexisting_NotFoundStatusCode()
        {
            var authorId = new Guid();

            var request = new RestRequest($"/api/authors/{authorId}");

            var response = _httpClient.Get(request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        //GET Author
        [Test]
        public void GetAuthor_OKStatusCode()
        {
            var request = new RestRequest($"/api/authors/{_id}");

            var response = _httpClient.Get(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        //GET Authors
        [Test]
        public void GetAuthors_OKStatusCode()
        {
            var request = new RestRequest($"/api/authors");

            var response = _httpClient.Get(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }


        ////METHODS

        //Method create author => returns author ID
        public string CreateAuthor()
        {
            var newAuthor = AuthorFactory.CreateAuthor();

            var request = new RestRequest($"/api/authors");
            request.AddJsonBody(newAuthor.ToJson(), "application/json");

            var response = _httpClient.Post(request);

            var createdAuthor = Author.FromJson(response.Content);

            return createdAuthor.Id.ToString();
        }

        //Method assign book to author => returns assignedBook ID
        public string AssignBookToAuthor()
        {
            var newBook = BookFactory.CreateBook();

            var request = new RestRequest($"/api/authors/{_id}/books");
            request.AddJsonBody(newBook.ToJson(), "application/json");

            var response = _httpClient.Post(request);

            var assignedBook = Book.FromJson(response.Content);

            return assignedBook.Id.ToString();
        }
    }
}
