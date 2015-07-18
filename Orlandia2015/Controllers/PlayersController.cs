﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Orlandia2015.Models;
using Orlandia2015.Services;

namespace Orlandia2015.Controllers
{
    public class PlayersController : AsyncController
    {
        private OrlandiaDbContext db = new OrlandiaDbContext();

        // GET: Players
        public async Task<ActionResult> IndexAsync()
        {
            var players = db.Players.Include(p => p.Faction);
            return View("Index", await players.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IndexAsync(string txtPlayerName)
        {
            var players = db.Players.Include(p => p.Faction);
            ViewBag.Name = txtPlayerName;
            return View("Index", await players.Where(p => p.sName.Contains(txtPlayerName)).ToListAsync());
        }

        // GET: Players/Details/5
        public async Task<ActionResult> DetailsAsync(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var player = await db.Players.FindAsync(id);
            if (player == null)
            {
                return HttpNotFound();
            }

            var nextRank = await db.Ranks.OrderBy(r => r.iRankNumber).FirstOrDefaultAsync(r => r.uFactionID == player.uFactionID && r.iRankPoints > player.iPoints);
            if (nextRank != null)
                ViewBag.MaxRankPoints = nextRank.iRankPoints;
            else
                ViewBag.MaxRankPoints = -1;
                

            return View(player);
        }

        // GET: Players/Create
        public async Task<ActionResult> CreateAsync()
        {
            ViewBag.uFactionID = new SelectList(await db.Factions.ToListAsync(), "uFactionID", "sName");
            return View();
        }

        // POST: Players/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind(Include = "uPlayerID,uFactionID,sName")] Player player)
        {
            if (ModelState.IsValid)
            {
                player.uPlayerID = Guid.NewGuid();
                db.Players.Add(player);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.uFactionID = new SelectList(await db.Factions.ToListAsync(), "uFactionID", "sName", player.uFactionID);
            return View(player);
        }

        // GET: Players/Edit/5
        public async Task<ActionResult> EditAsync(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var player = await db.Players.FindAsync(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            ViewBag.uFactionID = new SelectList(await db.Factions.ToListAsync(), "uFactionID", "sName", player.uFactionID);
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind(Include = "uPlayerID,uFactionID,sName")] Player player)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(player).State = EntityState.Modified;
                db.Players.Attach(player);
                db.Entry(player).Property(x => x.sName).IsModified = true;
                db.Entry(player).Property(x => x.uFactionID).IsModified = true;

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.uFactionID = new SelectList(await db.Factions.ToListAsync(), "uFactionID", "sName", player.uFactionID);
            return View(player);
        }

        // GET: Players/Delete/5
        public async Task<ActionResult> DeleteAsync(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var player = await db.Players.FindAsync(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync(Guid id)
        {
            var player = await db.Players.FindAsync(id);
            db.Players.Remove(player);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> AddPointsAsync(Guid id)
        {
            var player = await db.Players.FindAsync(id);

            if (player == null)
            {
                return HttpNotFound();
            }

            return View(player);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPointsAsync(Guid uPlayerID, int iPoints)
        {
            var player = await db.Players.FindAsync(uPlayerID);

            if (player == null)
            {
                return HttpNotFound();
            }

            if (iPoints < 0)
            {
                return new HttpStatusCodeResult(400, "Invalid number of points to add");
            }

            player.iPoints += iPoints;
            db.Players.Attach(player);
            db.Entry(player).Property(p => p.iPoints).IsModified = true;

            // Check Rank
            var nextRank =
                await
                    db.Ranks.OrderBy(r => r.iRankNumber)
                        .FirstAsync(r => r.uFactionID == player.uFactionID && r.iRankPoints > player.iPoints);

            if (nextRank != null && nextRank.iRankNumber > player.Rank.iRankNumber)
            {
                player.uRankID = nextRank.uRankID;
                db.Entry(player).Property(p => p.uRankID).IsModified = true;
            }

            await db.SaveChangesAsync();

            return new RedirectResult("Index");

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
