using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using slice_web.Models;

namespace slice_web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Core では Server.MapPath が使えないことの対応
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upload()
        {
            return View();
        }

        // 数キロバイトのファイルしかアップロードできない！！！
        // 大容量ファイルをアップロードするには･･･
        //   メモ➡https://worklog.be/archives/3332
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            string result = "";

            if (file != null && file.Length > 0)
            {
                // アップロードされたファイル名を取得。ブラウザが IE 
                // の場合 postedFile.FileName はクライアント側でのフ
                // ルパスになることがあるので Path.GetFileName を使う
                string filename =
                              Path.GetFileName(file.FileName);

                // アプリケーションルートの物理パスを取得。Core では
                // Server.MapPath は使えないので以下のようにする
                string contentRootPath =
                                _hostingEnvironment.ContentRootPath;
                string filePath = $@"C:\Users\takas\Documents\{filename}"; /*contentRootPath + "\\" +
                                  "UploadedFiles\\" + filename;*/

                using (var stream =
                            new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                result = filename + " (" + file.ContentType +
                         ") - " + file.Length +
                         " bytes アップロード完了";
            }
            else
            {
                result = "ファイルアップロードに失敗しました";
            }

            // Core では Request.IsAjaxRequest() は使えない
            if (Request.Headers["X-Requested-With"] ==
                                                "XMLHttpRequest")
            {
                return Content(result);
            }
            else
            {
                ViewBag.Result = result;
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
