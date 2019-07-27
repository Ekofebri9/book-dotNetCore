using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using basicwebapi.Model.Param;
using basicwebapi.Model.View;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using basicwebapi.Model;
using basicwebapi.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace basicwebapi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly Data dbContext;

        public BookController(Data dbContext)
        {
            this.dbContext = dbContext;
            if (this.dbContext.Books.Count() == 0)
            {
                for (int i = 1; i < 5; i++)
                {
                    this.dbContext.Books.Add(new Book
                    {
                        Id = i,
                        Date = DateTime.UtcNow,
                        Title = "Harry Potter eps:" + i
                    });
                    this.dbContext.SaveChanges();
                }
            }
        }
        public BookView[] BookList { get; set; }
        [HttpGet("{id}")]
        public async Task<ActionResult<BookView>> Get([FromRoute]int id)
        {
            var selectBook = await dbContext.Books
            .Where(book => book.Id == id)
            .Select(book => new BookView
            {
                Title = book.Title,
                Date = book.Date
            }).SingleOrDefaultAsync();
            if (selectBook == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            } return selectBook;


        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookView>>> Get([FromQuery]GetBookParam param)
        {
            var datasource = await dbContext.Books
            .Select(book => new BookView
            {
                //Id = book.Id, optional
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
            var newId = await dbContext.Books.CountAsync() + 1;
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
            var selectBook = await dbContext.Books.SingleOrDefaultAsync(item => item.Id == id);
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
}