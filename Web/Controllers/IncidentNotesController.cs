using DataLibrary;
using Microsoft.AspNetCore.Mvc;
using ViewModels.Incident;
using Microsoft.EntityFrameworkCore;

[Route("IncidentNotes")]
public class IncidentNotesController : Controller
{
    private readonly IIncidentNoteService _noteService;
    private readonly ApplicationDbContext _db;

    public IncidentNotesController(IIncidentNoteService noteService, ApplicationDbContext db)
    {
        _noteService = noteService;
        _db = db;
    }
    [HttpGet("GetNotesModal")]
    public async Task<IActionResult> GetNotesModal(long incidentId)
    {
        var viewModel = await _noteService.GetIncidentNotesModal(incidentId);

        if (viewModel == null)
            return NotFound();

        return PartialView("_IncidentNotesModal", viewModel);
    }

    [HttpPost("AddNote")]
    public async Task<IActionResult> AddNote(long incidentId, string author, string noteType, string content, IFormFile? file)
    {
        string? fileUrl = null;

        if (file != null && file.Length > 0)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Storage", "uploads", "notes");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            fileUrl = $"/Storage/uploads/notes/{fileName}";
        }

        await _noteService.AddNote(incidentId, author, noteType, content, fileUrl);

        // 🔄 Fetch updated notes list
        var updatedNotes = await _noteService.GetNotesByIncidentId(incidentId);

        // ✅ Return only the communication log partial
        return PartialView("_IncidentNotesLog", updatedNotes);
    }

}
