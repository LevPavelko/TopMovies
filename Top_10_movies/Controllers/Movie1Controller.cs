using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Top_10_movies.Models;

namespace Top_10_movies.Controllers
{
    public class Movie1Controller : Controller
    {
        private readonly MovieContext _context;

        IWebHostEnvironment _appEnvironment;

        public Movie1Controller(MovieContext context, IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
            _context = context;
        }

        // GET: Movie1
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Movie1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movie1/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movie1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Director,Genre,Year,Description,Poster")] Movie movie, IFormFile poster)
        {
            var realise_year = movie.Year;
            if (realise_year != null)
            {
                int year = (int)realise_year;
                int currentYear = DateTime.Now.Year;

                if (year > currentYear)
                {
                    ModelState.AddModelError("", "Год не может быть больше текущего.");
                }
            }


            if (ModelState.IsValid)
            {
                if (poster != null)
                {

                    string path = "/img/" + poster.FileName;


                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await poster.CopyToAsync(fileStream);
                    }
                    movie.Poster = path;
                    _context.Add(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                
                
            }
            return View(movie);
        }

        // GET: Movie1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movie1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Director,Genre,Year,Description,Poster")] Movie movie, IFormFile uploadedFile)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }
            var realise_year = movie.Year;
            if (realise_year != null)
            {
                int year = (int)realise_year;
                int currentYear = DateTime.Now.Year;

                if (year > currentYear)
                {
                    ModelState.AddModelError("", "Год не может быть больше текущего.");
                }
            }


            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(movie);
                    if (uploadedFile != null)
                    {
                        
                        string path = "/img/" + uploadedFile.FileName; 

                        
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(fileStream); 
                        }
                        movie.Poster = path;
                        _context.Update(movie);
                        
                      
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movie1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movie1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
