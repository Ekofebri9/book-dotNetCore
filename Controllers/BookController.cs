using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using basicwebapi.Model.Param;
using basicwebapi.Model.View;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace basicwebapi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly object dbContext;

        public BookController(Data dbCcontext)
        {
            this.dbContext = dbContext;
            if (this.dbContext.Books.Count() == 0)
            {
                this.dbContext.Books.Add(new Book
                {
                    Id = 1,
                    Date = DateTime.Now,
                    Title = "harry Potter"
                });
                this.dbContext.SaveChanges();
            }
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookView>>> Get([FromQuery]GetBookParam param)
        {
            var datasource = await dbContext.Books
            .Select(book => new BookView
            {
                Id = book.Id,
                Title = book.Title,
                Date = book.Date
            }).ToListAsync();
            if (string.IsNullOrWhiteSpace(param.Title))
            {
                return datasource;
            }
            this.dbContext.SaveChanges();
            return datasource
            .Where(item => item.Title.ToLower().Contains(param.Title.ToLower()))
            .ToList();
            // return datasource
            // .Where(item => item.Title.Contains(param.Title, StringComparison.OrdinalIgnoreCase))
            // .ToList();
        }
        [HttpPost]
        public async Task<ActionResult<IEnumerable<BookView>>> Post([FromBody]BookView param)
        {
            var newId = await dbContext.Book.CountAsync() + 1;
            dbContext.Add(new Book
            {
                Title = param.Title,
                Date = param.Date,
            });
            await this.dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);

        }
        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<BookView>>> Put([FromRoute]int id, [FromBody]BookView param)
        {
            var selectBook = await dbContext.Books.SingleOrDefault(item => item.Id == id);
            if (selectBook == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            selectBook.Title = param.Title;
            selectBook.Date = param.Date;
            await dbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status200OK);
        }
    }

    internal class Book
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
    }
}