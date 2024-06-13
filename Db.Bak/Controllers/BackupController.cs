using Db.Bak.Models;
using Db.Bak.Services;
using Microsoft.AspNetCore.Mvc;


public class BackupController : Controller
{
    private readonly IDbService _dbService;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private const string BackupDir = "Backups";
    private const string DatabaseName = "TestBak"; 

    public BackupController(IDbService dbService, IWebHostEnvironment hostingEnvironment)
    {
        _dbService = dbService;
        _hostingEnvironment = hostingEnvironment;
    }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(BackupList));
    }

    [HttpGet]
    public IActionResult BackupList()
    {
        var backupPath = Path.Combine(_hostingEnvironment.WebRootPath, BackupDir);
        if (!Directory.Exists(backupPath))
        {
            Directory.CreateDirectory(backupPath);
        }

        var files = Directory.GetFiles(backupPath).Select(f => new FileInfo(f)).ToList();
        var rows = files.Select(x => new DbBackupModel(x)
        {
            DownloadUrl = Url.Action(nameof(DownloadBackup), new { name = x.Name })
        }).ToList();

        return View(new GridModel<DbBackupModel>
        {
            Rows = rows,
            Total = rows.Count
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateBackup(string databaseName)
    {
        try
        {
            var backupName = $"{databaseName}_backup_{DateTime.Now:yyyyMMddHHmmss}.bak";
            var backupPath = Path.Combine(_hostingEnvironment.WebRootPath, BackupDir, backupName);

            await _dbService.BackupDatabaseAsync(databaseName, backupPath);

            TempData["Message"] = "Backup created successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("BackupList");
    }

    [HttpPost]
    public async Task<IActionResult> UploadBackup(IFormFile uploadFile)
    {
        if (uploadFile != null)
        {
            var backupPath = Path.Combine(_hostingEnvironment.WebRootPath, BackupDir, uploadFile.FileName);

            using (var stream = new FileStream(backupPath, FileMode.Create))
            {
                await uploadFile.CopyToAsync(stream);
            }

            TempData["Message"] = "Backup uploaded successfully.";
        }
        else
        {
            TempData["Error"] = "No file uploaded.";
        }

        return RedirectToAction(nameof(BackupList));
    }

    [HttpPost]
    public async Task<IActionResult> RestoreBackup(string name)
    {
        if (string.IsNullOrEmpty(name) || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new BadHttpRequestException("Invalid file name: " + name);
        }

        try
        {
            var backupPath = Path.Combine(_hostingEnvironment.WebRootPath, BackupDir, name);
            await _dbService.RestoreDatabaseAsync(DatabaseName, backupPath);

            TempData["Message"] = "Database restored successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("BackupList");
    }

    [HttpPost]
    public IActionResult DeleteBackup(string name)
    {
        var backupPath = Path.Combine(_hostingEnvironment.WebRootPath, BackupDir, name);

        if (System.IO.File.Exists(backupPath))
        {
            try
            {
                System.IO.File.Delete(backupPath);
                TempData["Message"] = "Backup deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
        }
        else
        {
            TempData["Error"] = "Backup file not found.";
        }

        return RedirectToAction("BackupList");
    }

    [HttpGet]
    public async Task<IActionResult> DownloadBackup(string name)
    {
        if (string.IsNullOrEmpty(name) || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new BadHttpRequestException("Invalid file name: " + name);
        }

        var backupPath = Path.Combine(_hostingEnvironment.WebRootPath, BackupDir, name);
        var contentType = "application/octet-stream";

        try
        {
            var fileBytes = await System.IO.File.ReadAllBytesAsync(backupPath);
            return File(fileBytes, contentType, name);
        }
        catch (IOException)
        {
            TempData["Error"] = "File is in use.";
        }

        return RedirectToAction("BackupList");
    }
}
