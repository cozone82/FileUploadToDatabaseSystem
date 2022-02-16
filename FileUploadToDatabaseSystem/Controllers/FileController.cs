using Microsoft.AspNetCore.Mvc;
using FileUploadToDatabaseSystem.Data;
using FileUploadToDatabaseSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using FileUploadToDatabaseSystem.Models;

namespace FileUploadToDatabaseSystem.Controllers
{
    public class FileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FileController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var fileuploadViewModel = await LoadAllFiles();
            ViewBag.Message = TempData["Message"];
            return View(fileuploadViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UploadToDatabase(List<IFormFile> files, string description)
        {
            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                var fileModel = new FileOnDatabaseModel
                {
                    CreatedOn = DateTime.Now,
                    FileType = file.ContentType,
                    Extension = extension,
                    Name = fileName,
                    Description = description
                };

                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    fileModel.Data = dataStream.ToArray();
                }
                _context.FilesOnDatabase.Add(fileModel);
                _context.SaveChanges();
            }
            TempData["Message"] = "File was successfully uploaded to the database.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DownloadFileFromDatabase(int id)
        {
            var file = await _context.FilesOnDatabase.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (file == null)
            {
                return null;
            }
            return File(file.Data, file.FileType, file.Name + file.Extension);
        }

        public async Task<IActionResult> DeleteFileFromDatabase(int id)
        {
            var file = await _context.FilesOnDatabase.Where(x =>x.Id == id).FirstOrDefaultAsync();
            _context.FilesOnDatabase.Remove(file);
            _context.SaveChanges();
            TempData["Message"] = $"Removed {file.Name + file.Extension} successfully from the database.";
            return RedirectToAction("Index");
        }

        private async Task<FileUploadViewModel> LoadAllFiles()
        {
            var viewModel = new FileUploadViewModel();
            viewModel.FilesOnDatabase = await _context.FilesOnDatabase.ToListAsync();
            return viewModel;
        }
    }
}
