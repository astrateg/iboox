using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iBoox.Models;

namespace iBoox.Controllers
{
    public class NavController : Controller
    {
        private iBooxEntities db;

        public NavController()
        {
            db = new iBooxEntities();
        }

        public PartialViewResult TagsCloud()
        {
            //FillTagsTable();
            
            TagsListViewModel viewModel = new TagsListViewModel { 
                Tags = db.Tags.OrderBy(t => t.Title),
                TagsMaxCount = db.Tags.Max(t => (t.Count ?? 0))
            };

            return PartialView(viewModel);
        }
        
        [NonAction]
        private void FillTagsTable()
        {
            //Заполняем таблицу Tags (выполняется 1 раз, ну и если руками добавляли книги в Books)
            //**************************************************
            var tagsFromRequest = db.GetTagsList();
            string[] tagsMerge = new string[] { };
            string[] tagsTemp = new string[] { };
            IEnumerable<string> tagsResult;

            foreach (var tag in tagsFromRequest)
            {
                tagsTemp = tag.Split(new string[] { ", " }, StringSplitOptions.None);
                tagsMerge = tagsMerge.Concat(tagsTemp).ToArray<string>();
            }

            tagsResult = (
                            from t in tagsMerge
                            orderby t ascending
                            select t
                         ).Distinct().ToList();

            bool isChanged = false;
            foreach (var tag in tagsResult)
            {
                Tag newTag = new Tag();
                newTag.Title = tag;
                newTag.Count = (from t in db.Books.Where(b => b.Cover2 == 0)
                                where t.Tags.Contains(tag)
                                select t).Count();

                // Если такой тег есть, переписываем его свойство Count (только если его свойство Count изменилось - иначе ничего не делаем), 
                // иначе добавляем в базу новый тег
                Tag oldTag = db.Tags.FirstOrDefault(t => t.Title == tag);
                if (oldTag == null)
                {
                    db.Tags.AddObject(newTag);
                    isChanged = true;
                }
                else
                {
                    if (oldTag.Count != newTag.Count)
                    {
                        oldTag.Count = newTag.Count;
                        isChanged = true;
                    }
                }
            }
            if (isChanged)
                db.SaveChanges();
            //**************************************************
        }
    }
}
