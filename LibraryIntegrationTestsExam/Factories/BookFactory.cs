using IntegrationTests.Models;

namespace IntegrationTests.Factories
{
    public static class BookFactory
    {
        public static Book CreateBook()
        {
            return new Book
            {
                Title = "Test Title", 
                Description = "Test Discription"
            };
        }
    }
}
