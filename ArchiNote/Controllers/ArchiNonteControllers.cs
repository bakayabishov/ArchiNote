using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteApp.Domain.Core;
using NoteApp.Infrastructure.Data;


namespace ArchiNote.Controllers;

public class NoteViewModel
{
    // [Required]
    // [MaxLength(30)]
    public int Id { get; set; } 
    public string? Head { get; set; }
    public string? Body { get; set; }
    public List<FilesViewModel> Files { get; set; }
}

    public class FilesViewModel
    {
        public string? FileName { get; set; }
        public string? FileDir { get; set; }
        
    }

[ApiController]
[Route("api/[controller]")]

public class ArchiNoteController : ControllerBase
{
    NoteContext db;
    private readonly NoteRepository noteRepository;

    public ArchiNoteController(NoteContext context)
    {
        db = context;
        noteRepository = new NoteRepository(context);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Note>>> Get()
    {
        var notes = await noteRepository.GetAsync();
        var result = notes.Select(note => new NoteViewModel()
        {
            Id = note.Id,
            Head = note.Head,
            Body = note.Body,
            Files = note.Files.Select(file => new FilesViewModel()
            {
                FileDir = file.FileDir,
                FileName = file.FileName
            }).ToList()
        });
        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        var notes = await noteRepository.GetAsync();
        var result = notes.Select(note => new NoteViewModel()
        {
            Id = note.Id,
            Head = note.Head,
            Body = note.Body,
            Files = note.Files.Select(file => new FilesViewModel()
            {
                FileDir = file.FileDir,
                FileName = file.FileName
            }).ToList()
        }).Where(note => note.Id == id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Note>> PostAsync(NoteViewModel? note)
    {
        if (note == null)
        {
            return BadRequest();
        }

        var files = note.Files.Select(file => new NoteFile()
        {
            FileDir = file.FileDir,
            FileName = file.FileName
        }).ToList();
        var noteNew = new Note(id: 0, note.Head, note.Body, files);
        await noteRepository.AddAsync(noteNew);
        return Ok(note);
    }

    // PUT api/users/
    [HttpPut]
    public async Task<ActionResult<Note>> UpdateAsync(NoteViewModel? note)
    {
        var result = await db.Notes
            .FirstOrDefaultAsync(n => n.Id == note.Id);
        if (result == null)
        {
            return BadRequest();
        }
        await noteRepository.UpdateAsync(result);
        await db.SaveChangesAsync();
        return Ok();
    }

    // DELETE api/users/5
    [HttpDelete]
    public async Task<ActionResult> DeleteAsync(NoteViewModel note)
    {
        var result = await db.Notes
            .FirstOrDefaultAsync(n => n.Id == note.Id);
        if (result != null)
        {
            db.Remove(result);
            await db.SaveChangesAsync();
        }

        return Ok();
    }
}

