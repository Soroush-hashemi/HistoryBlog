﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestBlog.Services.DTOs.Posts;
using TestBlog.Services.Services.Posts;
using TestBlog.Services.Utilities;
using TestBlog.Web.Areas.Admin.Models.Posts;

namespace TestBlog.Web.Areas.Admin.Controllers
{
    public class PostController : AdminControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        public IActionResult Index(int pageId = 1, string title = "", string categorySlug = "")
        {
            var param = new PostFilterParams()
            {
                CategorySlug = categorySlug,
                PageId = pageId,
                Take = 10,
                Title = title
            };
            var model = _postService.GetPostsByFilter(param);
            return View(model);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(CreatePostViewModel createViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(createViewModel);
            }

            var result = _postService.CreatePost(new CreatePostDto()
            {
                CategoryId = createViewModel.CategoryId,
                Description = createViewModel.Description,
                ImageFile = createViewModel.ImageFile,
                Slug = createViewModel.Slug,
                SubCategoryId = createViewModel.SubCategoryId == 0 ? null : createViewModel.SubCategoryId,
                Title = createViewModel.Title,
                IsSpecial = createViewModel.IsSpecial,
                UserId = User.GetUserId()
            });

            return RedirectAndShowAlert(result, RedirectToAction("Index"));
        }

        public IActionResult Edit(int id)
        {
            var post = _postService.GetPostById(id);
            if (post == null)
                return RedirectToAction("Index");

            var model = new EditPostViewModel()
            {
                CategoryId = post.CategoryId,
                Description = post.Description,
                Slug = post.Slug,
                SubCategoryId = post.SubCategoryId,
                Title = post.Title,
                ImageFile = post.ImageFile,
                IsSpecial = post.IsSpecial
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EditPostViewModel editViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editViewModel);
            }

            var result = _postService.EditPost(new EditPostDto()
            {
                CategoryId = editViewModel.CategoryId,
                Description = editViewModel.Description,
                ImageFile = editViewModel.ImageFile,
                Slug = editViewModel.Slug,
                SubCategoryId = editViewModel.SubCategoryId == 0 ? null : editViewModel.SubCategoryId,
                Title = editViewModel.Title,
                PostId = id,
                IsSpecial = editViewModel.IsSpecial
            });
            return RedirectAndShowAlert(result, RedirectToAction("Index"));
        }

        public IActionResult Delete(int Id)
        {
            if (Id == null || Id == 0)
                OperationResult.NotFound("Error");

            var PostDelete = _postService.DeletePost(Id);
            SuccessAlert();
            return RedirectToAction("Index");
        }
    }
}
