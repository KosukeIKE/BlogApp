using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlogApp.Models;

namespace BlogApp.Controllers
{
    public class ArticlesController : Controller
    {
        private BlogContext db = new BlogContext();

        [AllowAnonymous]//誰でも確認可
        // GET: Articles
        public ActionResult Index()
        {
            return View(db.Articles.ToList());
        }
        [AllowAnonymous]

        // GET: Articles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View();
        }
        [Authorize(Roles = "Owners")]//管理者のみ確認可能
        // GET: Articles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Articles/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 をご覧ください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owners")]//管理者のみ確認可能
        public ActionResult Create([Bind(Include = "Id,Title,Body,CategoryName")] Article article)//Articlenに値をセットしている
        {
            if (ModelState.IsValid)
            {
                article.Created = DateTime.Now;
                article.Modifyed = DateTime.Now;

                var category = db.Categories
                    .Where(item => item.CategoryName.Equals(article.CategoryName))
                    .FirstOrDefault();

                if(category == null)
                {
                    category = new Category()
                    {
                        CategoryName = article.CategoryName,
                        Count = 1
                    };
                    db.Categories.Add(category);
                }else
                {
                    category.Count++;
                    db.Entry(category).State = EntityState.Modified;
                }
                article.Category = category;

                db.Articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(article);
        }
        [Authorize(Roles = "Owners")]
        // GET: Articles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            article.CategoryName = article.Category.CategoryName;
            return View(article);
        }

        // POST: Articles/Edit/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 をご覧ください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,CategoryName")] Article article)
        {
            if (ModelState.IsValid)
            {
                var dbArticle = db.Articles.Find(article.Id);

                if(article == null)
                {
                    return HttpNotFound();
                }
                dbArticle.Title = article.Title;
                dbArticle.Body = article.Body;
                dbArticle.Modifyed = DateTime.Now;
                dbArticle.CategoryName = article.CategoryName;


                var beforeCategory = dbArticle.Category;
                if(!beforeCategory.CategoryName.Equals(article.CategoryName))
                {
                    beforeCategory.Articles.Remove(dbArticle);
                    beforeCategory.Count--;
                    db.Entry(beforeCategory).State = EntityState.Modified;


                    var category = db.Categories
                        .Where(item => item.CategoryName.Equals(article.CategoryName))
                        .FirstOrDefault();
                    if(category ==null)
                    {
                        category = new Category()
                        {
                            CategoryName = article.CategoryName,
                            Count = 1
                        };
                        db.Categories.Add(category);
                    }else
                    {
                        category.Count++;
                        db.Entry(category).State = EntityState.Modified;
                    }
                    dbArticle.Category = category;
                }

                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(article);
        }
        [Authorize(Roles ="Owners")]
        // GET: Articles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Articles/Delete/5
        [Authorize(Roles = "Owners")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Article article = db.Articles.Find(id);

            Category category = article.Category;

            if(category != null)
            {
                category.Count--;
                db.Entry(category).State = EntityState.Modified;
            }

            article.Comments.Clear();


            db.Articles.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCommnet([Bind(Include ="ArticleId, Body")] Comment comment)
        {
            var article = db.Articles.Find(comment.ArticleId);
            if(article == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            comment.Create = DateTime.Now;
            comment.Article = article;

            db.Comments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("Delete", new { id = comment.ArticleId });
        }
        [HttpPost]
        [Authorize(Roles = "Owners")]
        public ActionResult DeleteComments(int? id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var comment = db.Comments.Find(id);

            if(comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
