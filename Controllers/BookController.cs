using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using iBoox.Models;
using iBoox.Helpers;

namespace iBoox.Controllers
{
	public class BookController : Controller
	{
		private iBooxEntities db;
		public int PageSize = int.Parse(WebConfigurationManager.AppSettings["PageSize"]);

		public BookController()
		{
			db = new iBooxEntities();
		}

		public ViewResult AboutUs()
		{
			ViewBag.Themes = db.GetThemesList().ToList();
			ViewBag.Series = db.GetSeriesList().ToList();

			return View();
		}

		public ActionResult Index(string search, string seriesID, string themeID, int? authorID, int? tagID, int page = 1)
		{
			IEnumerable<Book> books = db.Books.Where(b => b.Cover2 == 0).ToList();
			IEnumerable<Theme> themes = db.GetThemesList().ToList();
			IEnumerable<Series> series = db.GetSeriesList().ToList();

			if (search != null)
			{
				books = books.Where(b => b.Title.ToLower().Contains(search.ToLower()));
				if (books.Count() > 0)
				{
					ViewBag.Title = String.Format("Вы искали: \"{0}\" ({1})", search, books.Count());
				}
				else
					ViewBag.Title = "Книги не найдены";
			}

			else if (themeID != null)
			{
				Theme theme = db.Themes.SingleOrDefault(t => t.ThemeID == themeID);
				if (theme != null)
				{
					books = books.Where(b => b.ThemeID == themeID);
					Theme selectedTheme = themes.SingleOrDefault(t => t.ThemeID == themeID);
					ViewBag.Title = String.Format("{0} ({1})", selectedTheme.ThemeFull, books.Count());
				}
				else
					return RedirectToAction("NotFound", "Error", new { message = "Тематика не найдена." });
			}

			else if (seriesID != null)
			{
				Series serie = db.Series.SingleOrDefault(s => s.SeriesID == seriesID);
				if (serie != null)
				{
					books = books.Where(b => b.SeriesID == seriesID);
					Series selectedSeries = series.SingleOrDefault(s => s.SeriesID == seriesID);
					ViewBag.Title = String.Format("Серия: \"{0}\" ({1})", selectedSeries.SeriesFull, books.Count());
				}
				else
					return RedirectToAction("NotFound", "Error", new { message = "Серия не найдена." });
			}
			
			else if (authorID != null)
			{
				Author author = db.Authors.SingleOrDefault(a => a.AuthorID == authorID);
				if (author != null)
				{
					IEnumerable<Author> authors = db.GetAuthorsList();
					Author selectedAuthor = authors.SingleOrDefault(a => a.AuthorID == authorID);
					books = books.Where(b => b.Authors.Count(a => a.AuthorID == authorID) > 0);
					ViewBag.Title = String.Format("Автор: \"{0}\" ({1})", selectedAuthor.FIO, books.Count());
				}
				else
					return RedirectToAction("NotFound", "Error", new { message = "Автор не найден." });
			}

			else if (tagID != null)
			{
				Tag tag = db.Tags.SingleOrDefault(t => t.TagID == tagID);
				if (tag != null)
				{
					books = books.Where(b => b.Tags.Contains(tag.Title));
					ViewBag.Title = String.Format("Категория: \"{0}\" ({1})", tag.Title, books.Count());
				}
				else
					return RedirectToAction("NotFound", "Error", new { message = "Категория не найдена." });
			}

			else
				ViewBag.Title = String.Format("Все книги ({0})", books.Count());

			ViewBag.Themes = themes;
			ViewBag.Series = series;

			BooksListViewModel viewModel = new BooksListViewModel
			{
				Books = books.OrderBy(b => b.Title)
							 .Skip((page - 1) * PageSize)
							 .Take(PageSize),
				PagingInfo = new PagingInfo
				{
					CurrentPage = page,
					ItemsPerPage = PageSize,
					TotalItems = books.Count()
				}
			};

			return View(viewModel);
		}

		public ActionResult ViewBook(int id)
		{
			Book book = db.Books.FirstOrDefault(b => b.BookID == id);
			if (book == null)
			{
				return RedirectToAction("NotFound", "Error", new { message = "Книга не найдена." });
			}

			Book secondBook = db.Books.Where(b => b.Title == book.Title).FirstOrDefault(b => b.ISBN != book.ISBN);
			if (secondBook == null)
				secondBook = book;

			BookViewModel viewModel = new BookViewModel
			{
				Book = book,
				SecondISBN = secondBook.ISBN,
				SecondCover = secondBook.Cover.CoverFull
			};

			ViewBag.Themes = db.GetThemesList().ToList();
			ViewBag.Series = db.GetSeriesList().ToList();

			return View(viewModel);
		}

		public JsonResult Vote(string widget_id, string clicked_on)
		{
			int id = 0;
			int.TryParse(widget_id, out id);

			Book book = db.Books.FirstOrDefault(b => b.BookID == id);
			if (book != null)
			{
				var selectedMark = Request.Cookies["rate_" + widget_id];
				if (selectedMark != null)
				{
					return Json(null);
				}

				string starNumber = clicked_on.Substring(5, 1); // clicked_on содержит строку типа "star_3 ..." - нам нужна цифра, т.е. 5-й символ в строке
				int rate;
				bool canParse = int.TryParse(starNumber, out rate);
				if (canParse)
				{
					book.Votes++;
					book.TotalRating += rate;

					// Сохраняем оценку в лог-таблицу BookVotes (в каждой таблице должен быть Primary Key!!!)
					BookVote vote = new BookVote
					{
						BookID = book.BookID,
						AddedVote = rate,
						AddedBy = User.Identity.Name,
						AddedByIP = Request.UserHostAddress,
						AddedDate = DateTime.Now
					};
					db.BookVotes.AddObject(vote);

					db.SaveChanges();

					var result = new
					{
						mark = starNumber,
						widget_id = widget_id,
						number_votes = book.Votes,
						total_points = book.TotalRating,
						dec_avg = book.RatingDecAvg.ToString(), // обязательно преобразовываем в строку, иначе во вьюхе получим '3.7' вместо '3,7'
						whole_avg = book.RatingIntAvg
					};

					return Json(result);
				}
			}

			return Json(null);
		}

		[HttpPost]
		public ActionResult CreateComment(int bookId, string body)
		{
			Book book = db.Books.FirstOrDefault(b => b.BookID == bookId);

			if (book == null) {
				return RedirectToAction("NotFound", "Error", new { message = "Книга не найдена." });
			}

			BookComment comment = new BookComment
			{
				AddedBy = User.Identity.Name,
				AddedByIP = Request.UserHostAddress,
				AddedDate = DateTime.Now,
				Body = body.AntiRudeFilter()
			};
			
			book.BookComments.Add(comment);
			db.SaveChanges();

			return Json(new
			{
				commentId = comment.BookCommentID,
				name = comment.AddedBy,
				body = comment.Body
			});
		}

		[HttpPost]
		public ActionResult RemoveComment(string id)
		{
			int commentId;
			bool canParse = int.TryParse(id.Replace("remove-comment-",""), out commentId);
			BookComment comment = db.BookComments.SingleOrDefault(c => c.BookCommentID == commentId);

			if (comment == null)
				return RedirectToAction("NotFound", "Error", new { message = "Комментарий не найден." });

			db.BookComments.DeleteObject(comment);
			db.SaveChanges();

			return Json(new { commentId = commentId });
		}

		[HttpPost]
		public ActionResult EditComment(string id, string body)
		{
			int commentId;
			bool canParse = int.TryParse(id.Replace("edit-comment-", ""), out commentId);
			BookComment comment = db.BookComments.SingleOrDefault(c => c.BookCommentID == commentId);

			if (comment == null)
				return RedirectToAction("NotFound", "Error", new { message = "Комментарий не найден." });

			comment.Body = body.AntiRudeFilter();

			db.SaveChanges();

			return Json(new
			{
				commentId = comment.BookCommentID,
				body = comment.Body
			});
		}
	}
}
